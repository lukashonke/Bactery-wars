using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Skills.Base;
using UnityEngine;

namespace Assets.scripts.Skills.SkillEffects
{
	public class EffectCritRate : EffectStatus
	{
		private float multiplier;

		public EffectCritRate(float mul, float duration)
			: base(duration)
		{
			this.multiplier = mul;

			isOffensive = false;
		}

		protected override void ApplyEffect()
		{
		}

		protected override void RemoveEffect()
		{
		}

		public override void ModifyCritRate(ref int critRate)
		{
			critRate = (int)(critRate * multiplier);
		}
	}
}
