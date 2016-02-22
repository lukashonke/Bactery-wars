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
		public int Dmg { get; set; }
		public int RandomOffset { get; set; }

		public EffectDamage(int damage, int randomOffset=0)
		{
			Dmg = damage;
			RandomOffset = randomOffset;
		}

		public override void ApplyEffect(Character source, GameObject target)
		{
			Character targetCh = Utils.GetCharacter(target);

			if (targetCh == null) // target may be a Destroyable object
			{
				Destroyable d = target.GetComponent<Destroyable>();

				if (d != null && source.CanAttack(d))
				{
					int damage = source.CalculateDamage(Dmg + Random.Range(-RandomOffset, RandomOffset), null, true);
					d.ReceiveDamage(source, damage);
				}
			}
			else // target may be a character
			{
				if (source.CanAttack(targetCh))
				{
					int damage = source.CalculateDamage(Dmg + Random.Range(-RandomOffset, RandomOffset), targetCh, true);

					source.OnAttack(targetCh);

					targetCh.ReceiveDamage(source, damage, SourceSkill);
				}
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
