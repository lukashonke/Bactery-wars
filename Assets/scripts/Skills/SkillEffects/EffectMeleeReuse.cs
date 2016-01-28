using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Skills.Base;
using UnityEngine;

namespace Assets.scripts.Skills.SkillEffects
{
	public class EffectMeleeReuse : EffectStatus
	{
		private float multiplier;
		private float fixedValue;
		private SkillTraits traitToAffect;
		//private SkillId[] idsToAffect;

		public EffectMeleeReuse(float multiplier, float fixedValue, float duration, SkillTraits toAffect/*, params SkillId[] idsToAffect*/) : base(duration)
		{
			this.multiplier = multiplier;
			this.fixedValue = fixedValue;
			this.traitToAffect = toAffect;

			isOffensive = false;
		}

		protected override void ApplyEffect()
		{
			Debug.Log("adding");
		}

		protected override void RemoveEffect()
		{
			Debug.Log("removing");
		}

		public override void ModifySkillReuse(ActiveSkill sk, ref float reuse)
		{
			if (sk.HasTrait(traitToAffect))
			{
				reuse *= multiplier;

				if (fixedValue > -1)
				{
					reuse = fixedValue;
				}
			}
		}

		public override void ModifySkillCasttime(ActiveSkill sk, ref float reuse)
		{
			/*if (sk.HasTrait(traitToAffect))
			{
				reuse *= multiplier;

				if (fixedValue > -1)
				{
					reuse = fixedValue;
				}
			}*/
		}
	}
}
