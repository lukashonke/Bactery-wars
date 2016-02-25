using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Skills.Base;
using UnityEngine;

namespace Assets.scripts.Skills.SkillEffects
{
	public class EffectSkillRange : EffectStatus
	{
		private float multiplier;
		private SkillTraits traitToAffect;
		//private SkillId[] idsToAffect;

		public EffectSkillRange(float multiplier, float duration, SkillTraits toAffect/*, params SkillId[] idsToAffect*/) : base(duration)
		{
			this.multiplier = multiplier;
			this.traitToAffect = toAffect;

			isOffensive = false;
		}

		protected override void ApplyEffect()
		{
		}

		protected override void RemoveEffect()
		{
		}

		public override void ModifySkillRange(ActiveSkill sk, ref int range)
		{
			if (sk.HasTrait(traitToAffect))
			{
				range = (int) (range * multiplier);
			}
		}
	}
}
