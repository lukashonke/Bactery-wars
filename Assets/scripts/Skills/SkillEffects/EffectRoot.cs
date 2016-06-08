// Copyright (c) 2015, Lukas Honke
// ========================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Skills.Base;

namespace Assets.scripts.Skills.SkillEffects
{
	public class EffectRoot : EffectStatus
	{
		private float temp;

		public EffectRoot(float duration) : base(duration)
		{
		}

		protected override void ApplyEffect()
		{
			if (target == null)
				return;

			source.OnAttack(target);

			temp = target.Status.MoveSpeed;
			target.SetMoveSpeed(0);
		}

		protected override void RemoveEffect()
		{
			if (target == null)
				return;

			target.SetMoveSpeed(temp);
		}

		public override SkillTraits[] GetTraits()
		{
			return new SkillTraits[] { SkillTraits.Immobilize, };
		}
	}
}
