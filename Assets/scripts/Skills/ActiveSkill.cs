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

		public abstract bool OnCastStart();
		public abstract void OnLaunch();
		public abstract void UpdateLaunched();
		public abstract void OnFinish();
		public abstract void OnSkillEnd();
		public abstract bool CanMove();
		public abstract bool CanRotate();

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

			int time = Environment.TickCount;

			// the reuse time has passed
			if (LastUsed + reuse < time)
			{
				return true;
			}

			Debug.Log("the skill is still being reused");
			// not yet
			return false;
		}

		public override bool IsBeingCasted()
		{
			return active;
		}

		public override void SetCooldownTimer()
		{
			LastUsed = Environment.TickCount;
		}

		public override void Start()
		{
			active = true;

            Owner.Status.ActiveSkills.Add(this);

			Task = Owner.StartTask(SkillTask());
			UpdateTask = Owner.StartTask(StartUpdateTask());
		}

		public override void End()
		{
			active = false;

            Owner.Status.ActiveSkills.Remove(this);

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

			yield return new WaitForSeconds(castTime);

			Debug.Log("[Active]");

			// nastavit stav - active
			state = SKILL_ACTIVE;

			OnLaunch();

			float coolDown = this.coolDown;

			// TODO apply cooldown modifying stuff here

			yield return new WaitForSeconds(coolDown);

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
