using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.scripts.Skills.SkillEffects
{
	public class EffectSlow : EffectDebuff
	{
		private int value;

		public EffectSlow(int value, float duration) : base(duration)
		{
			this.value = value;
		}

		protected override void ApplyEffect()
		{
			if (target == null)
				return;

			target.SetMoveSpeed(target.Status.MoveSpeed - value);
		}

		protected override void RemoveEffect()
		{
			if (target == null)
				return;

			target.SetMoveSpeed(target.Status.MoveSpeed + value);
		}
	}
}
