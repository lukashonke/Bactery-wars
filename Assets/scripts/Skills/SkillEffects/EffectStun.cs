// Copyright (c) 2015, Lukas Honke
// ========================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Skills.Base;
using UnityEngine;

namespace Assets.scripts.Skills.SkillEffects
{
	public class EffectStun : EffectStatus
	{
		private float temp;

		public string effectName = null;

		public EffectStun(float duration) : base(duration)
		{
		}

		protected override void ApplyEffect()
		{
			if (target == null)
				return;

			source.OnAttack(target);

			target.Status.Stunned = true;

			if (SourceSkillObject != null && SourceSkillObject is ActiveSkill)
			{
				if (effectName != null)
				{
					GameObject effect = ((ActiveSkill)SourceSkillObject).CreateParticleEffectOnTarget(target.GetData().GetBody(), effectName);
					if (effect == null)
						return;

					((ActiveSkill)SourceSkillObject).StartParticleEffect(effect);
					((ActiveSkill)SourceSkillObject).DeleteParticleEffect(effect, duration);
				}
				else
				{
					GameObject effect = ((ActiveSkill)SourceSkillObject).CreateParticleEffectOnTarget(target.GetData().GetBody(), "SkillTemplate", "Stun");
					if (effect == null)
						return;

					((ActiveSkill)SourceSkillObject).StartParticleEffect(effect);
					((ActiveSkill)SourceSkillObject).DeleteParticleEffect(effect, duration);
				}
			}
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
