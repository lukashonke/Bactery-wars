using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.scripts.Skills.SkillEffects
{
	public class EffectSilence : EffectStatus
	{
		public EffectSilence(float duration)
			: base(duration)
		{
		}

		protected override void ApplyEffect()
		{
			if (target == null)
				return;

			source.OnAttack(target);

			target.SetCanCastSkills(false);
		}

		protected override void RemoveEffect()
		{
			if (target == null)
				return;

			target.SetCanCastSkills(true);
		}
	}
}
