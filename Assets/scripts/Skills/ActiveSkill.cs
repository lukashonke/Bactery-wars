using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.AI;
using Assets.scripts.Base;
using Assets.scripts.Mono;
using Assets.scripts.Skills.Base;
using Assets.scripts.Skills.SkillEffects;
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
		protected float rangeCheckFrequency;

		public float castTime;
		public float coolDown;
		public float reuse;
		public int range;
		public int baseDamage;

		/// how often (in seconds) is the damage dealth - eg. 250dmg/sec will be 0.25f; if it is one time damage, leave it at 0
		public float baseDamageFrequency;

		/// if the skill requires confirmation before casting (second click)
		protected bool requireConfirm;
		public bool breaksMouseMovement;

		protected GameObject initTarget;

		/// not used currently
		public bool MovementBreaksConfirmation { get; protected set; }

		/// the time when the skill was last used
		protected int LastUsed { get; private set; }

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
			requireConfirm = false;
			MovementBreaksConfirmation = true;
			breaksMouseMovement = true;

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
		public abstract void MonoUpdate(GameObject gameObject);

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
				UpdateDirectionArrowScale(range, confirmObject);
			}

			//UpdateParticleSystemRange(range, confirmObject.GetComponent<ParticleSystem>());
			UpdateMouseDirection(confirmObject.transform);
			RotateArrowToMouseDirection(confirmObject, 0);
		}

		protected void RotateArrowToMouseDirection(GameObject o, int plusAngle)
		{
			Quaternion newRotation = Quaternion.LookRotation(-mouseDirection, Vector3.forward);
			newRotation.z = newRotation.z + plusAngle;
			newRotation.x = 0;
			newRotation.y = 0;
			confirmObject.transform.rotation = newRotation;  //Utils.GetRotationToDirectionVector(mouseDirection);
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

		public virtual void MonoTriggerEnter(GameObject gameObject, Collider2D other) { }
		public virtual void MonoTriggerExit(GameObject gameObject, Collider2D other) { }
		public virtual void MonoTriggerStay(GameObject gameObject, Collider2D other) { }
		public virtual void OnAfterEnd() { }

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

			if (range > 10)
				AddTrait(SkillTraits.LongRange);
		}

		/// called when the skill is added to the player (useful mostly for passive skills to active effects)
		public override void SkillAdded()
		{

		}

		public override bool CanUse()
		{
			if (Owner == null)
			{
				Debug.LogError("Error: skill ID " + Enum.GetName(typeof(SkillId), GetSkillId()) + " nema nastavenyho majitele skillu - nelze ho castit");
				return false;
			}

			if (active)
			{
				Debug.Log("skill already in use");
				return false;
			}

			if (reuse > 0)
			{
				int time = Environment.TickCount;

				// the reuse time has passed
				if (LastUsed + (reuse * 1000) < time)
				{
					return true;
				}
			}
			else
				return true;

			Debug.Log("the skill is still being reused");
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
			LastUsed = Environment.TickCount;
		}

		public void Start(Vector3 inputPosition)
		{
			mouseDirection = inputPosition - GetOwnerData().GetBody().transform.position;
			Start();
		}

		public void Start(GameObject target)
		{
			initTarget = target;
			Start();
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

			// start the reuse timer
			SetReuseTimer();

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

			Owner.Status.ActiveSkills.Remove(this);

			Owner.NotifyCastingModeChange();

			UpdateTask = null;
			Task = null;

			OnAfterEnd();
		}

		public override void AbortCast()
		{
			if (state == SkillState.SKILL_CONFIRMING)
			{
				GetPlayerData().ActiveConfirmationSkill = null;
				CancelConfirmation();
				state = SkillState.SKILL_IDLE;
				return;
			}

			Owner.StopTask(Task);

			End();
		}

		protected virtual IEnumerator SkillTask()
		{
			state = SkillState.SKILL_CASTING;

			CancelConfirmation();

			// pri spusteni skillu zacni kouzlit
			if (!OnCastStart())
			{
				OnAbort();
				yield break; //TODO test!
			}

			float castTime = this.castTime;

			// TODO apply debuffs, etc here

			if (castTime > 0)
			{
				yield return new WaitForSeconds(castTime);
			}

			// nastavit stav - active
			state = SkillState.SKILL_ACTIVE;

			OnLaunch();

			float coolDown = this.coolDown;

			// TODO apply cooldown modifying stuff here

			if (coolDown > 0)
			{
				yield return new WaitForSeconds(coolDown);
			}

			// nastavit stav - idle
			state = SkillState.SKILL_IDLE;

			OnFinish();

			End();

			yield return null;
		}

		protected virtual IEnumerator StartUpdateTask()
		{
			while (active)
			{
				yield return new WaitForSeconds(updateFrequency);

				if (GetOwnerData() != null && GetOwnerData().GetBody() != null)
				UpdateLaunched();
			}

			yield return null;
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

		/// <summary>
		/// Makes the gameobject send Start() and Update() methods to this class to MonoUpdate and MonoStart
		/// </summary>
		protected void AddMonoReceiver(GameObject obj)
		{
			UpdateSender us = obj.GetComponent<UpdateSender>();

			if (us == null)
			{
				Debug.LogError("a projectile doesnt have UpdateSender " + GetName() + "; adding it automatically");
				obj.AddComponent<UpdateSender>().target = this;
			}
			else
				us.target = this;
		}

		/// <summary>
		/// Loads a prefab template from resources folder (does not instantiate it)
		/// </summary>
		protected GameObject LoadSkillResource(string particleObjectName)
		{
			GameObject o = GetOwnerData().LoadResource("skill", GetName(), particleObjectName);

			return o;
		}

		/// <summary>
		/// Instantiates GameObject and optionally makes it a child (moves with the player)
		/// </summary>
		protected GameObject CreateSkillObject(string particleObjectName, bool makeChild, bool addMonoReceiver)
		{
			GameObject o = GetOwnerData().CreateSkillResource(GetName(), particleObjectName, makeChild, GetOwnerData().GetBody().transform.position);

			if (addMonoReceiver)
				AddMonoReceiver(o);

			return o;
		}

		/// <summary>
		/// Instantiates GameObject into defined position and optionally makes it a child (moves with the player)
		/// </summary>
		protected GameObject CreateSkillObject(string particleObjectName, bool makeChild, bool addMonoReceiver, Vector3 spawnPosition)
		{
			GameObject o = GetOwnerData().CreateSkillResource(GetName(), particleObjectName, makeChild, spawnPosition);

			if (addMonoReceiver)
				AddMonoReceiver(o);

			return o;
		}

		/// <summary>
		/// Creates an object in skill's folder and places it into Shooting position
		/// </summary>
		protected GameObject CreateSkillProjectile(string projectileObjectName, bool addMonoReceiver)
		{
			GameObject o = GetOwnerData().CreateSkillResource(GetName(), projectileObjectName, false, GetOwnerData().GetShootingPosition().transform.position);

			if (addMonoReceiver)
				AddMonoReceiver(o);

			AddRangeCheck(o);

			return o;
		}

		protected GameObject CreateSkillProjectile(string folderName, string projectileObjectName, bool addMonoReceiver)
		{
			GameObject o = GetOwnerData().CreateSkillResource(folderName, projectileObjectName, false, GetOwnerData().GetShootingPosition().transform.position);

			if (addMonoReceiver)
				AddMonoReceiver(o);

			AddRangeCheck(o);

			return o;
		}

		protected GameObject CreateSkillProjectile(string projectileObjectName, bool addMonoReceiver, Transform spawnPosition)
		{
			GameObject o = GetOwnerData().CreateSkillResource(GetName(), projectileObjectName, false, spawnPosition.position);

			if (addMonoReceiver)
				AddMonoReceiver(o);

			AddRangeCheck(o);

			return o;
		}

		protected GameObject CreateSkillProjectile(string folderName, string projectileObjectName, bool addMonoReceiver, Transform spawnPosition)
		{
			GameObject o = GetOwnerData().CreateSkillResource(folderName, projectileObjectName, false, spawnPosition.position);

			if (addMonoReceiver)
				AddMonoReceiver(o);

			AddRangeCheck(o);

			return o;
		}

		/// <summary>
		/// Clones a prefab object which contains Particle Effect, adds it to the player and returns it.
		/// The particle effect prefab must be within skill's folder in Resources/prefabs/skill 
		/// </summary>
		/// <param name="folderName">the folder in Resources/prefabs/skill to look into</param>
		/// <param name="particleObjectName">name of the .prefab object</param>
		/// <param name="makeChild">The particle effect position will move with player</param>
		protected GameObject CreateParticleEffect(string particleObjectName, bool makeChild)
		{
			GameObject o = GetOwnerData().CreateSkillResource(GetName(), particleObjectName, makeChild, GetOwnerData().GetParticleSystemObject().transform.position);
			return o;
		}

		protected GameObject CreateParticleEffect(string particleObjectName, bool makeChild, Vector3 spawnPosition)
		{
			GameObject o = GetOwnerData().CreateSkillResource(GetName(), particleObjectName, makeChild, spawnPosition);
			return o;
		}

		protected GameObject CreateParticleEffect(string folderName, string particleObjectName, bool makeChild)
		{
			GameObject o = GetOwnerData().CreateSkillResource(folderName, particleObjectName, makeChild, GetOwnerData().GetParticleSystemObject().transform.position);
			return o;
		}

		protected GameObject CreateParticleEffect(string folderName, string particleObjectName, bool makeChild, Vector3 spawnPosition)
		{
			GameObject o = GetOwnerData().CreateSkillResource(folderName, particleObjectName, makeChild, spawnPosition);
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
		protected void StartParticleEffect(GameObject obj)
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
		protected void PauseParticleEffect(GameObject obj)
		{
			try
			{
				ParticleSystem ps = obj.GetComponent<ParticleSystem>();

				if (ps.enableEmission)
					ps.enableEmission = false;
			}
			catch (Exception e)
			{
				Debug.LogError("couldnt pause particle effect" + GetName());
			}
		}

		/// <summary>
		/// Deletes the particle effect (change is permanent! - cannot be restarted, need to call CreateParticleEffect() again
		/// </summary>
		protected void DeleteParticleEffect(GameObject obj)
		{
			Object.Destroy(obj);
		}

		/// <summary>
		/// After delay, deletes the particle effect (change is permanent - cannot be restarted, need to call CreateParticleEffect() again
		/// </summary>
		protected void DeleteParticleEffect(GameObject obj, float delay)
		{
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

		protected void StartPlayerTargetting()
		{
			GetPlayerData().TargettingActive = true;
		}

		protected void StopPlayerTargetting()
		{
			//GetPlayerData().TargettingActive = false;
			GetPlayerData().HighlightTarget(GetPlayerData().Target, false);
		}

		public GameObject GetTarget()
		{
			return GetOwnerData().Target;
		}

		protected void DestroyProjectile(GameObject proj)
		{
			ProjectileBlackTestData pd = proj.GetComponent<ProjectileBlackTestData>();
			if(pd != null)
				pd.collapse();
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
	}
}
