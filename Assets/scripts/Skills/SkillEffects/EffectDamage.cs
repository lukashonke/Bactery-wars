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
			Character targetCh = Utils.GetCharacter(target);

			if (targetCh == null)
				return;

			if (source.CanAttack(targetCh))
			{
				int damage = source.CalculateDamage(Dmg + Random.Range(-RandomOffset, RandomOffset), targetCh, true);

				targetCh.ReceiveDamage(source, damage);
			}
		}

		public override void Update()
		{
		}

		public override void OnRemove()
		{
		}
	}
}
