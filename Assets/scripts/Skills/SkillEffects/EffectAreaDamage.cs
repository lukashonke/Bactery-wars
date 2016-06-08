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
	public class EffectAreaDamage : SkillEffect
	{
		protected int Dmg { get; set; }
		protected int RandomOffset { get; set; }
		protected float radius;

		public EffectAreaDamage(int damage, int randomOffset, float radius)
		{
			this.radius = radius;
			Dmg = damage;
			RandomOffset = randomOffset;
		}

		public override void ApplyEffect(Character source, GameObject target)
		{
			Vector3 targetPos = target.transform.position;

			Collider2D[] colls = Physics2D.OverlapCircleAll(targetPos, radius);

			foreach (Collider2D col in colls)
			{
				if (col != null && col.gameObject != null)
				{
					Character targetCh = col.gameObject.GetChar();

					if (targetCh == null)
					{
						Destroyable d = col.gameObject.GetComponent<Destroyable>();

						if (d != null && source.CanAttack(d))
						{
							bool crit;
							int damage = source.CalculateDamage(Dmg + Random.Range(-RandomOffset, RandomOffset), null, SourceSkillObject, true, out crit);
							d.ReceiveDamage(source, damage);
						}
					}
					else
					{
						if (source.CanAttack(targetCh))
						{
							bool crit;
							int damage = source.CalculateDamage(Dmg + Random.Range(-RandomOffset, RandomOffset), targetCh, SourceSkillObject, true, out crit);

							source.OnAttack(targetCh);

							targetCh.ReceiveDamage(source, damage, SourceSkill, crit);
						}
					}
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
			return new SkillTraits[] { SkillTraits.AreaDamage, };
		}
	}
}
