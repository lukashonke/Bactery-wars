using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Skills.Base;
using UnityEngine;

namespace Assets.scripts.Skills.SkillEffects
{
	public class EffectSkillReuse : EffectStatus
	{
		private float multiplier;
		private float fixedValue;
		private SkillTraits traitToAffect;
		//private SkillId[] idsToAffect;

		public EffectSkillReuse(float mul, float value, float duration, SkillTraits toAffect/*, params SkillId[] idsToAffect*/) : base(duration)
		{
			this.multiplier = mul;
			this.fixedValue = value;
			this.traitToAffect = toAffect;

			isOffensive = false;
		}

		public EffectSkillReuse(float mul, float duration)
			: base(duration)
		{
			this.multiplier = mul;
			this.fixedValue = -1;
			this.traitToAffect = SkillTraits.None;

			isOffensive = false;
		}

		protected override void ApplyEffect()
		{
		}

		protected override void RemoveEffect()
		{
		}

		public override void ModifySkillReuse(ActiveSkill sk, ref float reuse)
		{
			if (traitToAffect == SkillTraits.None || sk.HasTrait(traitToAffect))
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

		public override SkillTraits[] GetTraits()
		{
			return new SkillTraits[] { SkillTraits.BuffDamage, };
		}
	}
}
