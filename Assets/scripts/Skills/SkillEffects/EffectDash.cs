using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Skills.Base;

namespace Assets.scripts.Skills.SkillEffects
{
	public class EffectDash : EffectStatus
	{
		private int value;
		private float mul;

		private float temp;

		public EffectDash(int value, float duration) : base(duration)
		{
			this.value = value;
			mul = 0f;
		}

		public EffectDash(float mul, float duration)
			: base(duration)
		{
			value = 0;
			this.mul = mul;
		}

		protected override void ApplyEffect()
		{
			if (target == null)
				return;

			if(value > 0)
				target.SetMoveSpeed(target.Status.MoveSpeed + value);

			temp = target.Status.MoveSpeed*mul;

			if(temp > 0)
				target.SetMoveSpeed(target.Status.MoveSpeed + temp);
		}

		protected override void RemoveEffect()
		{
			if (target == null)
				return;

			if (value > 0)
				target.SetMoveSpeed(target.Status.MoveSpeed - value);

			if(temp > 0)
				target.SetMoveSpeed(target.Status.MoveSpeed - temp);
		}

		public override SkillTraits[] GetTraits()
		{
			return new SkillTraits[] { SkillTraits.BuffDefense, };
		}
	}
}
