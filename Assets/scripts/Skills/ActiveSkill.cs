using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.scripts.Skills
{
	public abstract class ActiveSkill : Skill
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
		public abstract void OnLaunch();
		public abstract void UpdateLaunched();
		public abstract void OnFinish();
		public abstract void OnSkillEnd();

		/// can the player move while casting?
		public abstract bool CanMove();

		/// can the player rotate while casting?
		public abstract bool CanRotate();

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
			OnSkillEnd();

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
				OnSkillEnd();
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
	}
}
