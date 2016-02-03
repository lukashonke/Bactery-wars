﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Mono;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.scripts.Skills.SkillEffects
{
	public class EffectAuraDamage : SkillEffect
	{
		protected int Dmg { get; set; }
		protected int RandomOffset { get; set; }
		protected float radius;

		public EffectAuraDamage(int damage, int randomOffset, float radius)
		{
			this.radius = radius;
			Dmg = damage;
			RandomOffset = randomOffset;
		}

		public override void ApplyEffect(Character source, GameObject target)
		{
			Character targetCh;

			Collider2D[] colls = Physics2D.OverlapCircleAll(source.GetData().transform.position, radius);

			foreach (Collider2D col in colls)
			{
				if (col != null && col.gameObject != null)
				{
					targetCh = col.gameObject.GetChar();

					if (targetCh == null)
					{
						Destroyable d = target.GetComponent<Destroyable>();

						if (d != null && source.CanAttack(d))
						{
							int damage = source.CalculateDamage(Dmg + Random.Range(-RandomOffset, RandomOffset), null, true);
							d.ReceiveDamage(source, damage);
						}
					}
					else
					{
						if (source.CanAttack(targetCh))
						{
							int damage = source.CalculateDamage(Dmg + Random.Range(-RandomOffset, RandomOffset), targetCh, true);

							source.OnAttack(targetCh);

							targetCh.ReceiveDamage(source, damage);
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
	}
}