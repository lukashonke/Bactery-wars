using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Base;
using Assets.scripts.Mono;
using Assets.scripts.Skills.SkillEffects;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.scripts.Skills
{
	public abstract class ActiveSkill : Skill, IMonoReceiver
	{
		public const int SKILL_IDLE = 0;
		public const int SKILL_CONFIRMING = 1;
		public const int SKILL_CASTING = 2;
		public const int SKILL_ACTIVE = 3;

		protected bool active;
		protected Coroutine Task { get; set; }
		protected Coroutine UpdateTask { get; set; }
		protected int state;

		/// <summary>how often should UpdateLaunched() be called (in seconds)</summary>
		protected float updateFrequency;

		protected float castTime;
		protected float coolDown;
		protected float reuse;

		/// <summary>if the skill requires confirmation before casting (second click)</summary>
		protected bool requireConfirm;
		public bool MovementBreaksConfirmation { get; protected set; }

		protected int LastUsed { get; private set; }

		protected Vector3 mouseDirection;
		protected GameObject confirmObject;

		public ActiveSkill(string name, int id) : base(name, id)
		{
			active = false;
			state = 0;
			castTime = 5;
			coolDown = 5;
			reuse = 5;
			updateFrequency = 0.1f;
			requireConfirm = false;
			MovementBreaksConfirmation = false;
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
				confirmObject = GetPlayerData().CreateSkillResource("TemplateSkill", "directionarrow", true, GetPlayerData().GetShootingPosition().transform.position);

			UpdateMouseDirection(confirmObject.transform);
			confirmObject.transform.rotation = Utils.GetRotationToDirectionVector(mouseDirection);
		}

		public virtual void CancelConfirmation()
		{
			if (confirmObject != null)
				Object.Destroy(confirmObject);
		}

		public virtual void MonoStart(GameObject gameObject) { }
		public virtual void MonoDestroy(GameObject gameObject) { }

		public virtual void MonoCollisionEnter(GameObject gameObject, Collision2D coll) { }
		public virtual void MonoCollisionExit(GameObject gameObject, Collision2D coll) { }
		public virtual void MonoCollisionStay(GameObject gameObject, Collision2D coll) { }

		public virtual void MonoTriggerEnter(GameObject gameObject, Collider2D other) { }
		public virtual void MonoTriggerExit(GameObject gameObject, Collider2D other) { }
		public virtual void MonoTriggerStay(GameObject gameObject, Collider2D other) { }

		/// called when the skill is added to the player (useful mostly for passive skills to active effects)
		public override void SkillAdded()
		{

		}

		public override bool CanUse()
		{
			if (Owner == null)
			{
				Debug.LogError("Error: skill ID " + Id + " nema nastavenyho majitele skillu - nelze ho castit");
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
				if (LastUsed + (reuse*1000) < time)
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
			return state == SKILL_CASTING;
		}

		/// <summary>
		/// skill is waiting for player to confirm the launch of the skill 
		/// </summary>
		public override bool IsBeingConfirmed()
		{
			return state == SKILL_CONFIRMING;
		}

		/// <summary>
		/// resets the reuse timer so that the owner of the skill needs to wait before using the skill again
		/// </summary>
		public override void SetReuseTimer()
		{
			LastUsed = Environment.TickCount;
		}

		public override void Start()
		{
			bool start = false;
			bool isPlayer = GetPlayerData() != null;

			// works only for Players (not needed for other characters)
			if (requireConfirm && isPlayer)
			{
				// switch this skill from idle to being confirmed
				if (state == SKILL_IDLE)
				{
					// the player has another skill waiting to be confirmed -> set this skil back to idle
					if (GetPlayerData().ActiveConfirmationSkill != null &&
					    GetPlayerData().ActiveConfirmationSkill.state == SKILL_CONFIRMING)
					{
						GetPlayerData().ActiveConfirmationSkill.AbortCast();
					}

					GetPlayerData().ActiveConfirmationSkill = this;
					state = SKILL_CONFIRMING;
					return;
				}

				if (state == SKILL_CONFIRMING && GetPlayerData().ActiveConfirmationSkill.Equals(this))
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

			active = true;

            Owner.Status.ActiveSkills.Add(this);

			Owner.NotifyCastingModeChange();

			Task = Owner.StartTask(SkillTask());
			UpdateTask = Owner.StartTask(StartUpdateTask());
		}

		public override void End()
		{
			active = false;

            Owner.Status.ActiveSkills.Remove(this);

			Owner.NotifyCastingModeChange();

			UpdateTask = null;
			Task = null;
		}

		public override void AbortCast()
		{
			if (state == SKILL_CONFIRMING)
			{
				GetPlayerData().ActiveConfirmationSkill = null;
				CancelConfirmation();
				state = SKILL_IDLE;
				return;
			}

			Owner.StopTask(Task);

			End();
		}

		protected virtual IEnumerator SkillTask()
		{
			state = SKILL_CASTING;

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
			state = SKILL_ACTIVE;

			OnLaunch();

			float coolDown = this.coolDown;

			// TODO apply cooldown modifying stuff here

			if (coolDown > 0)
			{
				yield return new WaitForSeconds(coolDown);
			}

			// nastavit stav - idle
			state = SKILL_IDLE;

			OnFinish();

			End();

			yield return null;
		}

		protected virtual IEnumerator StartUpdateTask()
		{
			while (active)
			{
				yield return new WaitForSeconds(updateFrequency);
				UpdateLaunched();
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
				Debug.LogError("a projectile doesnt have UpdateSender " + Name + "; adding it automatically");
				obj.AddComponent<UpdateSender>().target = this;
			}
			else
				us.target = this;
		}

		/// <summary>
		/// Clones a prefab object which contains Particle Effect, adds it to the player and returns it.
		/// The particle effect prefab must be within skill's folder in Resources/prefabs/skill 
		/// </summary>
		/// <param name="folderName">the folder in Resources/prefabs/skill to look into</param>
		/// <param name="particleObjectName">name of the .prefab object</param>
		/// <param name="makeChild">The particle effect position will move with player</param>
		protected GameObject CreateParticleEffect(string folderName, string particleObjectName, bool makeChild)
		{
			GameObject o = GetOwnerData().CreateSkillParticleEffect(folderName, particleObjectName, makeChild);

			if (o == null)
				throw new NullReferenceException("effect " + folderName + ", " + particleObjectName + " not found");

			return o;
		}

		/// <summary>
		/// Starts or unpauses obj's particle system
		/// </summary>
		protected void StartParticleEffect (GameObject obj)
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
				Debug.LogError("couldnt play particle effect " + Name);
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
				Debug.LogError("couldnt pause particle effect" + Name);
			}
		}

		/// <summary>
		/// Deletes the particle effect (change is permanent - cannot be restarted, need to call CreateParticleEffect() again
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

		protected void UpdateMouseDirection(Transform from)
		{
			mouseDirection = Utils.GetDirectionVectorToMousePos(from);
		}

		protected void RotatePlayerTowardsMouse()
		{
			GetPlayerData().SetRotation(Camera.main.ScreenToWorldPoint(Input.mousePosition), true);
		}
	}
}
