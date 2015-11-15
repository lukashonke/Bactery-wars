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
		public const int SKILL_CASTING = 1;
		public const int SKILL_ACTIVE = 2;

		protected bool active;
		protected Coroutine Task { get; set; }
		protected Coroutine UpdateTask { get; set; }
		protected int state;

		// how often should UpdateLaunched be called (in seconds)
		protected float updateFrequency;

		protected float castTime;
		protected float coolDown;
		protected float reuse;

		protected int LastUsed { get; private set; }

		public ActiveSkill(string name, int id) : base(name, id)
		{
			active = false;
			state = 0;
			castTime = 5;
			coolDown = 5;
			reuse = 5;
			updateFrequency = 0.1f;
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
		/// skill is being casted (for example: no projectile was shot yet)
		/// </summary>
		public override bool IsBeingCasted()
		{
			return state == SKILL_CASTING;
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
			Owner.StopTask(Task);

			End();
		}

		protected virtual IEnumerator SkillTask()
		{
			Debug.Log("[Casting]");
			state = SKILL_CASTING;

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

			Debug.Log("[Active]");

			// nastavit stav - active
			state = SKILL_ACTIVE;

			OnLaunch();

			float coolDown = this.coolDown;

			// TODO apply cooldown modifying stuff here

			if (coolDown > 0)
			{
				yield return new WaitForSeconds(coolDown);
			}

			Debug.Log("[Idle]");

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
	}
}
