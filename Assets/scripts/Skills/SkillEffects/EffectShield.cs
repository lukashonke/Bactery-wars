// Copyright (c) 2015, Lukas Honke
// ========================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Skills.Base;

namespace Assets.scripts.Skills.SkillEffects
{
	public class EffectShield : EffectStatus
	{
		private float value;

		private float temp;

		public EffectShield(float value, float duration) : base(duration)
		{
			this.value = value;
		}

		protected override void ApplyEffect()
		{
			if (target == null)
				return;

			source.OnAttack(target);

			if(value > 0 || value < 0)
				target.SetShield(target.Status.Shield + value);
		}

		protected override void RemoveEffect()
		{
			if (target == null)
				return;

			if (value > 0 || value < 0)
				target.SetShield(target.Status.Shield - value);
		}

		public override SkillTraits[] GetTraits()
		{
			return new SkillTraits[] { SkillTraits.BuffDefense, };
		}
	}
}
