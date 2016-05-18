using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Skills.Base;

namespace Assets.scripts.Skills.SkillEffects
{
	public class EffectStun : EffectStatus
	{
		private float temp;

		public EffectStun(float duration) : base(duration)
		{
		}

		protected override void ApplyEffect()
		{
			if (target == null)
				return;

			source.OnAttack(target);

			target.Status.Stunned = true;
		}

		protected override void RemoveEffect()
		{
			if (target == null)
				return;

			target.Status.Stunned = false;
		}

		public override SkillTraits[] GetTraits()
		{
			return new SkillTraits[] { SkillTraits.Immobilize, };
		}
	}
}
