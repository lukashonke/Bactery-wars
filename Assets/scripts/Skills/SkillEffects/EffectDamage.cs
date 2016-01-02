using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Mono;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.scripts.Skills.SkillEffects
{
	public class EffectDamage : SkillEffect
	{
		protected int Dmg { get; set; }
		protected int RandomOffset { get; set; }

		public EffectDamage(int damage, int randomOffset)
		{
			Dmg = damage;
			RandomOffset = randomOffset;
		}

		public override void ApplyEffect(Character source, GameObject target)
		{
			AbstractData data = target.GetComponentInParent<AbstractData>();

			if (data == null)
				return;

			Character targetCh = data.GetOwner();

			if (targetCh == null)
				return;

			if (source.CanAttack(targetCh))
			{
				if (RandomOffset > 0)
					targetCh.ReceiveDamage(source, Dmg + Random.Range(-RandomOffset, RandomOffset));
				else
					targetCh.ReceiveDamage(source, Dmg);
			}
		}
	}
}
