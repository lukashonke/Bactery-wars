using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.scripts.Skills
{
	public abstract class PassiveSkill : Skill
	{
		public PassiveSkill(string name, int id) : base(name, id)
		{

		}

		public abstract void ApplyEffect();

		public sealed override void SkillAdded()
		{
			ApplyEffect();
		}

		public override bool CanUse()
		{
			return false;
		}

		public override void SetCooldownTimer()
		{
		}

		public override void Start()
		{
		}

		public override void End()
		{
		}

		public override void AbortCast()
		{
		}

		public override bool IsBeingCasted()
		{
			return false;
		}
	}
}
