// Copyright (c) 2015, Lukas Honke
// ========================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Skills.Base;

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

			target.ForcedStatus = "Silenced!";

			source.OnAttack(target);

			target.SetCanCastSkills(false);
		}

		protected override void RemoveEffect()
		{
			if (target == null)
				return;

			target.SetCanCastSkills(true);
		}

		public override SkillTraits[] GetTraits()
		{
			return new SkillTraits[] { SkillTraits.Immobilize, };
		}
	}
}
