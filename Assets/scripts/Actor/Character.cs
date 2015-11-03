using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor.Status;
using Assets.scripts.Base;
using Assets.scripts.Skills;
using Assets.scripts.Skills.Base;
using UnityEngine;

namespace Assets.scripts.Actor
{
	public abstract class Character : Entity
	{
		public CharStatus Status { get; private set; }

		public SkillSet Skills { get; set; }

		protected Character(string name) : base(name)
		{
			Init();
		}

		private void Init()
		{
			Status = InitStatus();
			Skills = InitSkillSet();
		}

		protected abstract CharStatus InitStatus();
		protected abstract SkillSet InitSkillSet();

		public void CastSkill(Skill skill)
		{
			// skill is passive - cant cast it
			if (skill is PassiveSkill)
				return;

			// reuse check
			if (!skill.CanUse())
			{
				Debug.Log("skill cannot be used again yet");
				return;
			}

			skill.SetCooldownTimer();

			skill.Start();
		}

		public void BreakCasting()
		{
			if (!Status.IsCasting())
				return;

			Skill sk;
			for(int i = 0; i < Status.ActiveSkills.Count; i++)
			{
				sk = Status.ActiveSkills[i];

				if (sk.IsBeingCasted())
					sk.AbortCast();
			}

			Debug.Log("break done");
		}

		public virtual Coroutine StartTask(IEnumerator skillTask)
		{
			return GameSystem.Instance.StartTask(skillTask);
		}

		public virtual void StopTask(Coroutine c)
		{
			GameSystem.Instance.StopTask(c);
		}

		public virtual void StopTask(IEnumerator t)
		{
			GameSystem.Instance.StopTask(t);
		}
	}
}
