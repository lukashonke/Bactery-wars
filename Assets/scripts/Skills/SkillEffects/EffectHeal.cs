using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Skills.Base;
using UnityEngine;

namespace Assets.scripts.Skills.SkillEffects
{
	public class EffectHeal : SkillEffect
	{
		private int value;
		private float mul;

		private float temp;

		public EffectHeal(int value)
		{
			this.value = value;
			mul = 0f;
		}

		public EffectHeal(float mul)
		{
			value = 0;
			this.mul = mul;
		}

		public override void ApplyEffect(Character source, GameObject target)
		{
			if (target == null)
				return;

			Character targetCh = Utils.GetCharacter(target);

			if (targetCh != null && !source.Equals(targetCh) /*&& !targetCh.CanAttack(source)*/)
			{
				if (value > 0)
					targetCh.ReceiveHeal(Source, value);

				int amount = (int)(targetCh.Status.MaxHp * mul);
				if (amount > 0)
					targetCh.ReceiveHeal(Source, amount);
			}
		}

		public override void Update()
		{
		}

		public override void OnRemove()
		{
		}

		public override SkillTraits[] GetTraits()
		{
			return new SkillTraits[] { SkillTraits.Heal, };
		}
	}
}
