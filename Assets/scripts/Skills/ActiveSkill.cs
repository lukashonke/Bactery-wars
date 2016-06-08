using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.AI;
using Assets.scripts.Base;
using Assets.scripts.Mono;
using Assets.scripts.Mono.ObjectData;
using Assets.scripts.Skills.Base;
using Assets.scripts.Skills.SkillEffects;
using Assets.scripts.Upgrade;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.scripts.Skills
{
	public abstract class ActiveSkill : Skill, IMonoReceiver
	{
		public enum SkillState
		{
			SKILL_IDLE,
			SKILL_CONFIRMING,
			SKILL_CASTING,
			SKILL_ACTIVE,
		}

		protected bool active;
		protected Coroutine Task { get; set; }
		protected Coroutine UpdateTask { get; set; }
		protected Coroutine RangeCheckTask { get; set; }
		protected SkillState state;

		/// how often should UpdateLaunched() be called (in seconds)
		protected float updateFrequency;

		/// pri kratkem range je vhodne snizit tuto frekvenci pro presnejsi vypocet, defaultne to je nastaveno na 0.2f (5x/s)
		public float rangeCheckFrequency;

		public float castTime;
		public float coolDown;
		public float reuse;
		public int range;
		public int baseDamage;
		public bool canBeCastSimultaneously;
		public bool resetMoveTarget;
		public bool movementAbortsSkill;
		public bool triggersOwnerCollision;
		public SkillId skillToBeRechargedOnThisUse;

		/// how often (in seconds) is the damage dealth - eg. 250dmg/sec will be 0.25f; if it is one time damage, leave it at 0
		public float baseDamageFrequency;

		/// if the skill requires confirmation before casting (second click)
		protected bool requireConfirm;
		public bool breaksMouseMovement;

		protected GameObject initTarget;
		protected Vector3 fixedTarget;
		public GameObject InitTarget { get { return initTarget; } }

		/// not used currently
		public bool MovementBreaksConfirmation { get; protected set; }

		/// the time when the skill was last used
		public float LastUsed { get; set; }


		public int maxConsecutiveCharges;
		public int currentConsecutiveCharges;
		public float consecutiveTimelimit;
		public float tempDamageBoost;

		// custom image for projectile
		public string image;

		private float firstUsedCharge;
		private bool isWaitingForConsecutiveCharges;
		private Coroutine startReuseTask;

		/// represents the vector that usually points from the player_object to the mouse direction
		protected Vector3 mouseDirection;

		/// represents usually the arrow object that shows the direction the skill will be fired to
		protected GameObject confirmObject;

		/// represents the main particleSystem for 
		protected GameObject particleSystem;

		protected Dictionary<GameObject, Vector3> RangeChecks;

		public ActiveSkill()
		{
			active = false;
			state = 0;
			castTime = 5;
			coolDown = 5;
			reuse = 5;
			updateFrequency = 0.1f;
			rangeCheckFrequency = 0.1f;
			canBeCastSimultaneously = false;
			requireConfirm = false;
			MovementBreaksConfirmation = true;
			breaksMouseMovement = true;
			resetMoveTarget = true;
			triggersOwnerCollision = false;
			maxConsecutiveCharges = 1;
			consecutiveTimelimit = 3f;
			skillToBeRechargedOnThisUse = 0;
			LastUsed = -1000f;
			tempDamageBoost = -1;

			image = null;

			RangeChecks = new Dictionary<GameObject, Vector3>(); 
		}

		/// returning false will make the skill not start
		public abstract bool OnCastStart();

		/// called when casting is done
		public abstract void OnLaunch();

		/// periodically called after casting is done
		public abstract void UpdateLaunched();

		/// called when cooldown runs out (skill ends)
		public abstract void OnAbort();

		/// called when cooldown rns out (skill ends)
		public abstract void OnFinish();

		/// sent from GameObjects created using this skill
		public abstract void MonoUpdate(GameObject gameObject, bool fixedUpdate);

		/// can the player move while casting?
		public abstract bool CanMove();

		/// can the player rotate while casting?
		public abstract bool CanRotate();

		// overridable
		public virtual void OnBeingConfirmed()
		{
			if (confirmObject == null)
			{
				confirmObject = GetPlayerData().CreateSkillResource("SkillTemplate", "directionarrow", true, GetPlayerData().GetShootingPosition().transform.position);
				UpdateDirectionArrowScale(range > 0 ? range : 5, confirmObject);
			}

			//UpdateParticleSystemRange(range, confirmObject.GetComponent<ParticleSystem>());
			UpdateMouseDirection(confirmObject.transform);
			RotateArrowToMouseDirection(confirmObject, 0);
		}

		protected void RotateArrowToMouseDirection(GameObject o, int plusAngle)
		{
			Vector3 dir = mouseDirection;
			if (plusAngle > 0)
			{
				dir = Quaternion.Euler(new Vector3(0, 0, plusAngle)) * dir;
			}

			Quaternion newRotation = Quaternion.LookRotation(-dir, Vector3.forward);
			newRotation.z = newRotation.z;
			newRotation.x = 0;
			newRotation.y = 0;

			o.transform.rotation = newRotation;  //Utils.GetRotationToDirectionVector(mouseDirection);
		}

		protected void UpdateDirectionArrowScale(int range, GameObject o)
		{
			if (range > 10)
				range = 10;

			o.transform.localScale = new Vector3(0.24f, 0.145f*range, 0);
		}

		protected void UpdateParticleSystemRange(int range, ParticleSystem ps)
		{
			// 5 dist = 1
			ps.startLifetime = 1;
			ps.startSpeed = (range);
			Debug.DrawRay(GetOwnerData().shootingPosition.transform.position, new Vector3(5, 0, 0), Color.red, 10f);
		}

		public virtual void CancelConfirmation()
		{
			if (confirmObject != null)
				Object.Destroy(confirmObject);

			if(GetPlayerData() != null && GetPlayerData().TargettingActive)
				StopPlayerTargetting();
		}

		public virtual void MonoStart(GameObject gameObject) { }
		public virtual void MonoDestroy(GameObject gameObject) { }

		public virtual void MonoCollisionEnter(GameObject gameObject, Collision2D coll) { }
		public virtual void MonoCollisionExit(GameObject gameObject, Collision2D coll) { }
		public virtual void MonoCollisionStay(GameObject gameObject, Collision2D coll) { }

		public virtual void MonoTriggerEnter(GameObject gameObject, Collider2D coll) { }
		public virtual void MonoTriggerExit(GameObject gameObject, Collider2D other) { }
		public virtual void MonoTriggerStay(GameObject gameObject, Collider2D other) { }
		public virtual void OnAfterEnd() { }
		public virtual void OnAterReuse() { }

		protected override void InitDynamicTraits()
		{
			if (castTime <= 1f)
				AddTrait(SkillTraits.ShortCastingTime);

			if (castTime >= 3)
				AddTrait(SkillTraits.LongCastingTime);

			if (reuse <= 1f)
				AddTrait(SkillTraits.ShortReuse);

			if (reuse >= 3)
				AddTrait(SkillTraits.LongReuse);

			if (range < 5)
				AddTrait(SkillTraits.ShortRange);

			if (range >= 5)
				AddTrait(SkillTraits.LongRange);
		}

		public void OnCollision(bool triggerOnly, Collision2D collision, Collider2D collider)
		{
			if (triggersOwnerCollision)
			{
				if (!triggerOnly)
				{
					MonoCollisionEnter(GetOwnerData().GetBody(), collision);
				}
				else
				{
					MonoTriggerEnter(GetOwnerData().GetBody(), collider);
				}
			}
		}

		/// called when the skill is added to the player (useful mostly for passive skills to active effects)
		public override void SkillAdded()
		{

		}

		public override bool CanUse()
		{
			if (IsLocked)
				return false;

			if (Owner == null)
			{
				Debug.LogError("Error: skill ID " + Enum.GetName(typeof(SkillId), GetSkillId()) + " nema nastavenyho majitele skillu - nelze ho castit");
				return false;
			}

			if (active)
			{
				//Debug.Log("skill already in use");
				return false;
			}

			if (isWaitingForConsecutiveCharges)
			{
				float time = Time.time;

				if (firstUsedCharge + consecutiveTimelimit < time && currentConsecutiveCharges < maxConsecutiveCharges)
				{
					return true;
				}
			}

			if (reuse > 0)
			{
				float time = Time.time;

				// the reuse time has passed
				if (LastUsed + (GetReuse(true)) < time)
				{
					return true;
				}
			}
			else
				return true;

			//Debug.Log("the skill is still being reused");
			// not yet
			return false;
		}

		/// <summary>
		/// skil isActive() during: player started to cast skill (skill.Start() was called) until --> Skill.End() was called
		/// </summary>
		public override bool IsActive()
		{
			return active;
		}

		/// <summary>
		/// skill is being casted (no projectile was shot yet)
		/// </summary>
		public override bool IsBeingCasted()
		{
			return state == SkillState.SKILL_CASTING;
		}

		/// <summary>
		/// skill is waiting for player to confirm the launch of the skill 
		/// </summary>
		public override bool IsBeingConfirmed()
		{
			return state == SkillState.SKILL_CONFIRMING;
		}

		/// <summary>
		/// resets the reuse timer so that the owner of the skill needs to wait before using the skill again
		/// </summary>
		public override void SetReuseTimer()
		{
			isWaitingForConsecutiveCharges = false;
			LastUsed = Time.time;
			GetOwnerData().SetSkillReuseTimer(this);
		}

		public void Start(Vector3 inputPosition)
		{
			initTarget = null;
			mouseDirection = inputPosition - GetOwnerData().GetBody().transform.position;
			fixedTarget = inputPosition;
			Start();
		}

		public void Start(GameObject target)
		{
			initTarget = target;
			Start();
		}

		// only player can call this method!
		public bool DoAutoattack()
		{
			if (GetPlayerData().autoAttackTargetting && state == SkillState.SKILL_IDLE)
			{
				// the player has another skill waiting to be confirmed -> set this skil back to idle
				if (GetPlayerData().ActiveConfirmationSkill != null &&
				    GetPlayerData().ActiveConfirmationSkill.state == SkillState.SKILL_CONFIRMING)
				{
					GetPlayerData().ActiveConfirmationSkill.AbortCast();
				}

				GetPlayerData().ActiveConfirmationSkill = this;
				state = SkillState.SKILL_CONFIRMING;
				return false;
			}

			Owner.CastSkill(this);
			return true;
		}

		public override void Start()
		{
			bool start = false;
			bool isPlayer = GetPlayerData() != null && GetPlayerData().GetOwner().AI is PlayerAI;
			bool keyboardMovement = isPlayer && GetPlayerData().keyboardMovementAllowed;

			// works only for Players (not needed for other characters)
			if (requireConfirm && isPlayer && !keyboardMovement)
			{
				// switch this skill from idle to being confirmed
				if (state == SkillState.SKILL_IDLE)
				{
					// the player has another skill waiting to be confirmed -> set this skil back to idle
					if (GetPlayerData().ActiveConfirmationSkill != null &&
						GetPlayerData().ActiveConfirmationSkill.state == SkillState.SKILL_CONFIRMING)
					{
						GetPlayerData().ActiveConfirmationSkill.AbortCast();
					}

					GetPlayerData().ActiveConfirmationSkill = this;
					state = SkillState.SKILL_CONFIRMING;
					return;
				}

				if (state == SkillState.SKILL_CONFIRMING && GetPlayerData().ActiveConfirmationSkill.Equals(this))
				{
					start = true;
					GetPlayerData().ActiveConfirmationSkill = null;
				}
			}
			else
			{
				start = true;

				if (isPlayer)
				{
					if (GetPlayerData().ActiveConfirmationSkill != null)
						GetPlayerData().ActiveConfirmationSkill.AbortCast();
				}
			}

			if (!start)
				return;

			if (Owner.Status.IsStunned())
				return;

			if (resetMoveTarget && GetPlayerData() != null && GetPlayerData().castingBreaksMovement)
			{
				GetOwnerData().HasTargetToMoveTo = false;
			}

			// start the reuse timer
			if (maxConsecutiveCharges <= 1)
				SetReuseTimer();
			else
			{
				//TODO move this to when skill ends?
				if (isWaitingForConsecutiveCharges) // after consecutive charge fired
				{
					currentConsecutiveCharges++;

					if (currentConsecutiveCharges >= maxConsecutiveCharges)
					{
						if (startReuseTask != null)
							Owner.StopTask(startReuseTask);

						SetReuseTimer();
					}
				}
				else // after first charge fired
				{
					firstUsedCharge = Time.time;
					isWaitingForConsecutiveCharges = true;
					currentConsecutiveCharges = 1;
					startReuseTask = Owner.StartTask(ScheduleStartReuse());
				}
			}

			if (tempDamageBoost > 0)
			{
				tempDamageBoost = -1;
			}

			int count = Owner.Skills.Skills.Count;
			for (int i = 0; i < count; i++)
			{
				Skill sk = Owner.Skills.Skills[i];
				sk.NotifyAnotherSkillCastStart(this);
			}

			active = true;

			Owner.Status.ActiveSkills.Add(this);

			Owner.NotifyCastingModeChange();

			Task = Owner.StartTask(SkillTask());
			UpdateTask = Owner.StartTask(StartUpdateTask());

			if (range > 0)
				RangeCheckTask = Owner.StartTask(RangeTaskCheck());
		}

		public override void End()
		{
			active = false;

			initTarget = null;

			Owner.Status.ActiveSkills.Remove(this);

			Owner.NotifyCastingModeChange();

			UpdateTask = null;
			Task = null;

			state = SkillState.SKILL_IDLE;

			int count = Owner.Skills.Skills.Count;
			for (int i = 0; i < count; i++)
			{
				Skill sk = Owner.Skills.Skills[i];
				sk.NotifyAnotherSkillCastEnd(this);
			}

			OnAfterEnd();

			if (GetReuse(false) > 0)
				Owner.StartTask(NotifyReuseEnd());
		}

		public override string GetBaseInfo()
		{
			return "Reuse " + reuse + " sec";
		}

		private IEnumerator NotifyReuseEnd()
		{
			yield return new WaitForSeconds(GetReuse(false));
			OnAterReuse();
		}

		private IEnumerator ScheduleStartReuse()
		{
			yield return new WaitForSeconds(consecutiveTimelimit);
			SetReuseTimer();
		}

		public override void AbortCast()
		{
			if (IsActive())
			{
				OnFinish();
			}

			//if (IsBeingCasted())
			{
				DeleteCastingEffect();

				if (state == SkillState.SKILL_CONFIRMING)
				{
					GetPlayerData().ActiveConfirmationSkill = null;
					CancelConfirmation();
					state = SkillState.SKILL_IDLE;
					return;
				}

				if(Task != null)
					Owner.StopTask(Task);

				End();
			}
		}

		protected virtual IEnumerator SkillTask()
		{
			state = SkillState.SKILL_CASTING;

			CancelConfirmation();

			// pri spusteni skillu zacni kouzlit
			if (!OnCastStart())
			{
				OnAbort();
				yield break;
			}

			float castTime = GetCastTime();

			if (castTime > 0)
			{
				yield return new WaitForSeconds(castTime);
			}

			if (GetOwnerData() == null)
				yield break;

			// nastavit stav - active
			state = SkillState.SKILL_ACTIVE;

			OnLaunch();

            //TODO if player dies he finishes casting anyway

			float coolDown = GetCooldownTime();

			// TODO apply cooldown modifying stuff here

			if (coolDown > 0)
			{
				yield return new WaitForSeconds(coolDown);
			}

			if (GetOwnerData() == null)
				yield break;

			// nastavit stav - idle
			state = SkillState.SKILL_IDLE;

			RechargeNext();

			OnFinish();

			End();

			yield return null;
		}

		protected virtual IEnumerator StartUpdateTask()
		{
			while (active || ReceiveUpdateMethod())
			{
				yield return new WaitForSeconds(updateFrequency);

				if (GetOwnerData() != null && GetOwnerData().GetBody() != null)
					UpdateLaunched();
			}

			yield return null;
		}

		protected virtual bool ReceiveUpdateMethod()
		{
			return false;
		}

		protected IEnumerator RangeTaskCheck()
		{
			while (RangeChecks.Any() || active)
			{
				CheckRanges();
				yield return new WaitForSeconds(rangeCheckFrequency);
			}

			yield return null;
		}

		private void RechargeNext()
		{
			if (skillToBeRechargedOnThisUse != 0)
			{
				ActiveSkill sk = Owner.Skills.GetSkill(skillToBeRechargedOnThisUse) as ActiveSkill;
				sk.LastUsed = 0;
				GetOwnerData().SetSkillReuseTimer(sk, true);
			}
		}

		/// <summary>
		/// Makes the gameobject send Start() and Update() methods to this class to MonoUpdate and MonoStart
		/// </summary>
		protected void AddMonoReceiver(GameObject obj)
		{
			UpdateSender us = obj.GetComponent<UpdateSender>();

			if (us == null)
			{
				Debug.LogWarning("a projectile doesnt have UpdateSender " + GetName() + "; adding it automatically");
				obj.AddComponent<UpdateSender>().target = this;
			}
			else
				us.target = this;
		}

		/// <summary>
		/// Loads a prefab template from resources folder (does not instantiate it)
		/// </summary>
		protected GameObject LoadSkillResource(string objectName)
		{
			GameObject o = GetOwnerData().LoadResource("skill", GetName(), objectName);

			return o;
		}

		/// <summary>
		/// Instantiates GameObject and optionally makes it a child (moves with the player)
		/// </summary>
		protected GameObject CreateSkillObject(string objectName, bool makeChild, bool addMonoReceiver)
		{
			GameObject o = GetOwnerData().CreateSkillResource(GetName(), objectName, makeChild, GetOwnerData().GetBody().transform.position);

			if (addMonoReceiver)
				AddMonoReceiver(o);

			return o;
		}

		/// <summary>
		/// Instantiates GameObject into defined position and optionally makes it a child (moves with the player)
		/// </summary>
		protected GameObject CreateSkillObject(string objectName, bool makeChild, bool addMonoReceiver, Vector3 spawnPosition)
		{
			GameObject o = GetOwnerData().CreateSkillResource(GetName(), objectName, makeChild, spawnPosition);

			if (addMonoReceiver)
				AddMonoReceiver(o);

			return o;
		}

		/// <summary>
		/// Creates an object in skill's folder and places it into Shooting position
		/// </summary>
		protected GameObject CreateSkillProjectile(string projectileObjectName, bool addMonoReceiver)
		{
			GameObject o;

			if (image != null)
				o = GetOwnerData().CreateSkillResource("CustomSkill", image, false, GetOwnerData().GetShootingPosition().transform.position);
			else
				o = GetOwnerData().CreateSkillResource(GetName(), projectileObjectName, false, GetOwnerData().GetShootingPosition().transform.position);

			if (addMonoReceiver)
				AddMonoReceiver(o);

			AddRangeCheck(o);
			ProjectileShotAnimation();

			return o;
		}

		protected GameObject CreateSkillProjectile(string folderName, string projectileObjectName, bool addMonoReceiver)
		{
			GameObject o;

			if (image != null)
				o = GetOwnerData().CreateSkillResource("CustomSkill", image, false, GetOwnerData().GetShootingPosition().transform.position);
			else
				o = GetOwnerData().CreateSkillResource(folderName, projectileObjectName, false, GetOwnerData().GetShootingPosition().transform.position);

			if (addMonoReceiver)
				AddMonoReceiver(o);

			AddRangeCheck(o);
			ProjectileShotAnimation();

			return o;
		}

		protected GameObject CreateSkillProjectile(string projectileObjectName, bool addMonoReceiver, Transform spawnPosition)
		{
			GameObject o;

			if (image != null)
				o = GetOwnerData().CreateSkillResource("CustomSkill", image, false, spawnPosition.position);
			else
				o = GetOwnerData().CreateSkillResource(GetName(), projectileObjectName, false, spawnPosition.position);

			if (addMonoReceiver)
				AddMonoReceiver(o);

			AddRangeCheck(o);
			ProjectileShotAnimation();

			return o;
		}

		protected GameObject CreateSkillProjectile(string folderName, string projectileObjectName, bool addMonoReceiver, Transform spawnPosition)
		{
			GameObject o;

			if (image != null)
				o = GetOwnerData().CreateSkillResource("CustomSkill", image, false, spawnPosition.position);
			else
				o = GetOwnerData().CreateSkillResource(folderName, projectileObjectName, false, spawnPosition.position);

			if (addMonoReceiver)
				AddMonoReceiver(o);

			AddRangeCheck(o);
			ProjectileShotAnimation();

			return o;
		}

		/// <summary>
		/// Clones a prefab object which contains Particle Effect, adds it to the player and returns it.
		/// The particle effect prefab must be within skill's folder in Resources/prefabs/skill 
		/// </summary>
		/// <param name="folderName">the folder in Resources/prefabs/skill to look into</param>
		/// <param name="particleObjectName">name of the .prefab object</param>
		/// <param name="makeChild">The particle effect position will move with player</param>
		public GameObject CreateParticleEffect(string particleObjectName, bool makeChild)
		{
			GameObject o = GetOwnerData().CreateSkillResource(GetName(), particleObjectName, makeChild, GetOwnerData().GetParticleSystemObject().transform.position);
			return o;
		}

		public GameObject CreateParticleEffect(string particleObjectName, bool makeChild, Vector3 spawnPosition)
		{
			GameObject o = GetOwnerData().CreateSkillResource(GetName(), particleObjectName, makeChild, spawnPosition);
			return o;
		}

		public GameObject CreateParticleEffect(string folderName, string particleObjectName, bool makeChild)
		{
			GameObject o = GetOwnerData().CreateSkillResource(folderName, particleObjectName, makeChild, GetOwnerData().GetParticleSystemObject().transform.position);
			return o;
		}

		public GameObject CreateParticleEffect(string folderName, string particleObjectName, bool makeChild, Vector3 spawnPosition)
		{
			GameObject o = GetOwnerData().CreateSkillResource(folderName, particleObjectName, makeChild, spawnPosition);
			return o;
		}

		public GameObject CreateParticleEffectOnTarget(GameObject target, string particleObjectName)
		{
			GameObject o = GetOwnerData().CreateSkillResource(GetName(), particleObjectName, false, target.transform.position);
			o.transform.parent = target.transform;
			return o;
		}

		public GameObject CreateParticleEffectOnTarget(GameObject target, string folderName, string particleObjectName)
		{
			GameObject o = GetOwnerData().CreateSkillResource(folderName, particleObjectName, false, target.transform.position);
			o.transform.parent = target.transform;
			return o;
		}

		/// <summary>
		/// Updates the mouseDirection from GetOwnerData().Body
		/// </summary>
		protected void UpdateMouseDirection()
		{
			mouseDirection = Utils.GetDirectionVectorToMousePos(GetOwnerData().GetBody().transform);
		}

		/// <summary>
		/// Updates the mouseDirection from the provided transform
		/// </summary>
		/// <param name="from"></param>
		protected void UpdateMouseDirection(Transform from)
		{
			mouseDirection = Utils.GetDirectionVectorToMousePos(from);
		}

		/// <summary>
		/// Rotates the player data towards mouse
		/// Does anything only if owner is a player
		/// </summary>
		protected void RotatePlayerTowardsMouse()
		{
			if (GetPlayerData() != null && GetPlayerData().GetOwner().AI is PlayerAI)
				GetOwnerData().SetRotation(Camera.main.ScreenToWorldPoint(Input.mousePosition), true);
		}

		protected void RotatePlayerTowardsTarget(GameObject target)
		{
			if (GetOwnerData() != null)
				GetOwnerData().SetRotation(target.transform.position, true);
		}

		/// <summary>
		/// Starts or unpauses obj's particle system
		/// </summary>
		public void StartParticleEffect(GameObject obj)
		{
			try
			{
				ParticleSystem ps = obj.GetComponent<ParticleSystem>();
				if (ps.isPlaying == false)
					ps.Play();

				if (!ps.enableEmission)
					ps.enableEmission = true;
			}
			catch (Exception e)
			{
				Debug.LogError("couldnt play particle effect " + GetName());
			}
		}

		/// <summary>
		/// Pauses the emmission of the particles 
		/// </summary>
		public void PauseParticleEffect(GameObject obj)
		{
			try
			{
				ParticleSystem ps = obj.GetComponent<ParticleSystem>();

				if (ps.enableEmission)
					ps.enableEmission = false;
			}
			catch (Exception e)
			{
				//Debug.LogError("couldnt pause particle effect" + GetName());
			}
		}

		/// <summary>
		/// Deletes the particle effect (change is permanent! - cannot be restarted, need to call CreateParticleEffect() again
		/// </summary>
		public void DeleteParticleEffect(GameObject obj)
		{
			Object.Destroy(obj);
		}

		/// <summary>
		/// After delay, deletes the particle effect (change is permanent - cannot be restarted, need to call CreateParticleEffect() again
		/// </summary>
		public void DeleteParticleEffect(GameObject obj, float delay)
		{
			if(obj != null)
			Object.Destroy(obj, delay);
		}

		/// <summary>
		/// turns off colliders and visible graphics for this object 
		/// </summary>
		protected void DisableProjectile(GameObject o)
		{
			o.GetComponent<SpriteRenderer>().enabled = false;
			o.GetComponent<Collider2D>().enabled = false;
		}

		/// <summary>
		/// Instantiates and optionally starts effect CastingEffect.prefab in the skill's folder
		/// </summary>
		protected void CreateCastingEffect(bool start)
		{
			particleSystem = CreateParticleEffect("CastingEffect", true);
			if(start)
				StartParticleEffect(particleSystem);
		}

		/// <summary>
		/// Instantiates and optionally starts effect CastingEffect.prefab in the [folderName] folder
		/// </summary>
		protected void CreateCastingEffect(bool start, string folderName)
		{
			particleSystem = CreateParticleEffect(folderName, "CastingEffect", true);
			if (start)
				StartParticleEffect(particleSystem);
		}

		/// <summary>
		/// Stops the current casting effect 
		/// </summary>
		protected void DeleteCastingEffect()
		{
			if (particleSystem != null)
			{
				DeleteParticleEffect(particleSystem);
			}
		}

		protected void StartPlayerTargetting(bool onlyEnemies=true)
		{
			GetPlayerData().TargettingActive = true;
			GetPlayerData().SkillTargetting = true;
			GetPlayerData().SkillTargettingEnemiesOnly = onlyEnemies;
			GetPlayerData().SkillTargettingRange = range;
		}

		protected void StopPlayerTargetting()
		{
			//GetPlayerData().TargettingActive = false;
			GetPlayerData().SkillTargetting = false;
			GetPlayerData().SkillTargettingRange = -1;
			GetPlayerData().HighlightTarget(GetPlayerData().Target, false);
		}

		public GameObject GetTarget()
		{
			return GetOwnerData().Target;
		}

		public void ProjectileShotAnimation()
		{
			try
			{
				GameObject obj = CreateParticleEffect(GetName(), "ProjectileShot", false, GetOwnerData().GetShootingPosition().transform.position);

				if (obj != null)
				{
					StartParticleEffect(obj);
					DeleteParticleEffect(obj, 1f);
				}
			}
			catch (Exception)
			{
				/*GameObject obj = CreateParticleEffect("SkillTemplate", "ProjectileShot", false, proj.transform.position);

				if (obj != null)
				{
					StartParticleEffect(obj);
					DeleteParticleEffect(obj, 1f);
				}*/
			}
		}

		public void ProjectileHitAnimation(GameObject proj)
		{
			try
			{
				GameObject obj = CreateParticleEffect(GetName(), "ProjectileHit", false, proj.transform.position);

				if (obj != null)
				{
					StartParticleEffect(obj);
					DeleteParticleEffect(obj, 1f);
				}
			}
			catch (Exception)
			{
				GameObject obj = CreateParticleEffect("SkillTemplate", "ProjectileHit", false, proj.transform.position);

				if (obj != null)
				{
					StartParticleEffect(obj);
					DeleteParticleEffect(obj, 1f);
				}
			}
		}

		protected void DestroyProjectile(GameObject proj, float delay=1f)
		{
			if (proj == null)
				return;

			ProjectileHitAnimation(proj);
			
			/*ProjectileBlackTestData pd = proj.GetComponent<ProjectileBlackTestData>();
			if(pd != null)
				pd.collapse();
			else*/

			if (delay > 0)
			{
				proj.GetComponent<SpriteRenderer>().enabled = false;
				proj.GetComponent<Collider2D>().enabled = false;
				proj.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
				PauseParticleEffect(proj);
				Object.Destroy(proj, delay);
			}
			else
				Object.Destroy(proj);
		}

		protected void AddRangeCheck(GameObject o)
		{
			if(range > 0)
				RangeChecks.Add(o, o.transform.position);
		}

		protected void CheckRanges()
		{
			int range = GetUpgradableRange();

			List<GameObject> temp = new List<GameObject>(RangeChecks.Keys);
			foreach (GameObject proj in temp)
			{
				Vector3 init = RangeChecks[proj];
				bool far = CheckRange(proj, init, range);

				if (far)
				{
					DestroyProjectile(proj);
					RangeChecks.Remove(proj);
					continue;
				}
			}
		}

		/// <summary>
		/// returns true if it is out of range
		/// pracuje s hodnotami nadruhou
		/// </summary>
		/// <param name="o"></param>
		/// <param name="init"></param>
		/// <param name="range"></param>
		/// <returns></returns>
		private bool CheckRange(GameObject o, Vector3 init, int range)
		{
			if (o != null && o.activeInHierarchy)
			{
				Vector3 rangeVector = init - o.transform.position;
				return rangeVector.sqrMagnitude >= range*range;
			}

			return false;
		}

		private int totalDamageOutput = -1;

		/// <summary>
		/// returns the max ammount of damage this skill does during its lifetime
		/// </summary>
		public int GetTotalDamageOutput()
		{
			if (totalDamageOutput > -1)
				return totalDamageOutput;

            if (baseDamageFrequency > 0)
			{
				int damage = 0;
				damage += (int) ((1/baseDamageFrequency)*coolDown*damage);
				totalDamageOutput = damage;
			}
			else
			{
				int damage = baseDamage;
				totalDamageOutput = damage;
			}

			return totalDamageOutput;
		}

		public virtual void OnMove()
		{
			
		}

		public float GetReuse(bool skillBeingCast)
		{
			float reuse = this.reuse;

			foreach (EquippableItem u in Owner.Inventory.ActiveUpgrades)
			{
				u.ModifySkillReuse(this, ref reuse);
			}

			foreach (SkillEffect ef in Owner.ActiveEffects)
			{
				ef.ModifySkillReuse(this, ref reuse, skillBeingCast);
			}
			return reuse;
		}

		public float GetCooldownTime()
		{
			float coolDown = this.coolDown;

			foreach (EquippableItem u in Owner.Inventory.ActiveUpgrades)
			{
				u.ModifySkillCooldown(this, ref coolDown);
			}

			foreach (SkillEffect ef in Owner.ActiveEffects)
			{
				ef.ModifySkillCooldown(this, ref coolDown);
			}

			return coolDown;
		}

		public float GetCastTime()
		{
			float casttime = this.castTime;

			foreach (EquippableItem u in Owner.Inventory.ActiveUpgrades)
			{
				u.ModifySkillCasttime(this, ref casttime);
			}

			foreach (SkillEffect ef in Owner.ActiveEffects)
			{
				ef.ModifySkillCasttime(this, ref casttime);
			}

			return casttime;
		}

		public int GetUpgradableRange()
		{
			int newRange = this.range;

			foreach (SkillEffect ef in Owner.ActiveEffects)
			{
				ef.ModifySkillRange(this, ref newRange);
			}

			return newRange;
		}

		public void SetRange(int r)
		{
			this.range = r;
		}

		/// <summary>
		/// boost skill damage for one use
		/// </summary>
		/// <param name="val">the skill damage multiplier</param>
		public void TempBoostDamage(float val)
		{
			tempDamageBoost = val;
		}

		public float GetBoostDamage()
		{
			return tempDamageBoost;
		}

		protected int CalcAngleForProjectile(int index, int totalProjectiles, int angleAdd)
		{
			int temp = totalProjectiles * angleAdd - angleAdd;
			temp = -temp / 2;
			return temp + index * angleAdd;
		}

		public float GetSkillActiveDuration()
		{
			return castTime + coolDown;
		}

		public void AnalyzeEffectsForTrais()
		{
			SkillEffect[] efs = CreateEffects(0);

			if (efs != null && !originalEffectsDisabled)
			{
				foreach (SkillEffect eff in efs)
				{
					foreach (SkillTraits t in eff.GetTraits())
					{
						if (!HasTrait(t))
							AddTrait(t);
					}
				}
			}

			if (additionalEffects != null)
			{
				foreach (SkillEffect eff in additionalEffects)
				{
					foreach (SkillTraits t in eff.GetTraits())
					{
						if (!HasTrait(t))
							AddTrait(t);
					}
				}
			}
		}

		public float GetProjectileLifetime(float speed)
		{
			return range/(speed) + 0.5f;
		}

		//TODO finish this for other params too
	}
}
