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
	public class EffectSlow : EffectStatus
	{
		private int value;
		private float mul;

		private float temp;

		public string effectName;

		public EffectSlow(int value, float duration) : base(duration)
		{
			this.value = value;
			mul = 1f;
		}

		public EffectSlow(float mul, float duration)
			: base(duration)
		{
			value = 0;
			this.mul = mul;
		}

		protected override void ApplyEffect()
		{
			if (target == null)
				return;

			source.OnAttack(target);

			if(value > 0)
				target.SetMoveSpeed(target.Status.MoveSpeed - value);

			temp = target.Status.MoveSpeed*mul;
			if(temp > 0)
				target.SetMoveSpeed(target.Status.MoveSpeed - temp);

			Debug.Log("current " + target.Status.MoveSpeed + ", temp " + temp + " value " + value);

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
					GameObject effect = ((ActiveSkill)SourceSkillObject).CreateParticleEffectOnTarget(target.GetData().GetBody(), "SkillTemplate", "Slow");
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

			if (value > 0)
				target.SetMoveSpeed(target.Status.MoveSpeed + value);

			if(temp > 0)
				target.SetMoveSpeed(target.Status.MoveSpeed + temp);

			Debug.Log("after (mul " + temp + ", mul " + mul + " remove: " + target.Status.MoveSpeed );
		}

		public override SkillTraits[] GetTraits()
		{
			return new SkillTraits[] { SkillTraits.Immobilize, };
		}
	}
}
