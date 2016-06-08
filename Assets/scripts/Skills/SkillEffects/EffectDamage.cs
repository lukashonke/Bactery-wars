// Copyright (c) 2015, Lukas Honke
// ========================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Mono;
using Assets.scripts.Skills.Base;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.scripts.Skills.SkillEffects
{
	public class EffectDamage : SkillEffect
	{
		public int Dmg { get; set; }
		public int RandomOffset { get; set; }

		public int CriticalRate { get; set; }

		public EffectDamage(int damage, int randomOffset, int criticalRate)
		{
			Dmg = damage;
			RandomOffset = randomOffset;
			CriticalRate = criticalRate;
		}

		public EffectDamage(int damage, int randomOffset)
		{
			Dmg = damage;
			RandomOffset = randomOffset;
			CriticalRate = 0;
		}

		public EffectDamage(int damage)
		{
			Dmg = damage;
			RandomOffset = 0;
			CriticalRate = 0;
		}

		public override void ApplyEffect(Character source, GameObject target)
		{
			Character targetCh = Utils.GetCharacter(target);

			if (targetCh == null) // target may be a Destroyable object
			{
				Destroyable d = target.GetComponent<Destroyable>();

				if (d != null && source.CanAttack(d))
				{
					bool crit;
					int damage = source.CalculateDamage(Dmg + Random.Range(-RandomOffset, RandomOffset), null, SourceSkillObject, true, out crit, CriticalRate);
					d.ReceiveDamage(source, damage);
				}
			}
			else // target may be a character
			{
				if (source.CanAttack(targetCh))
				{
					bool crit;
					int damage = source.CalculateDamage(Dmg + Random.Range(-RandomOffset, RandomOffset), targetCh, SourceSkillObject, true, out crit, CriticalRate);

					source.OnAttack(targetCh);

					targetCh.ReceiveDamage(source, damage, SourceSkill, crit);
				}
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
			return new SkillTraits[] { SkillTraits.Damage, };
		}
	}
}
