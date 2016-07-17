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
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Assets.scripts.Skills.SkillEffects
{
	public class EffectReproduceOnDie : EffectStatus
	{
		protected float radius;

		public bool attackAll = false;

		private int attackTeam = -1;
		private Character attackerTarget;
		private Character attacker;

		public EffectReproduceOnDie(int duration, int radius) : base(duration)
		{
			this.radius = radius;
			this.attackAll = false;
		}

		public EffectReproduceOnDie(int duration, int radius, bool attackAll)
			: base(duration)
		{
			this.radius = radius;
			this.attackAll = attackAll;
		}

		private void DoReproduce()
		{
			if (attackerTarget == null)
				return;

			Character targetCh;

			Collider2D[] colls = Physics2D.OverlapCircleAll(attackerTarget.GetData().transform.position, radius);

			// explosion effect
			GameObject explosion = ((ActiveSkill)SourceSkillObject).CreateParticleEffect("PoisonExplosion", false, attackerTarget.GetData().GetBody().transform.position);
			explosion.GetComponent<ParticleSystem>().Play();
			Object.Destroy(explosion, 2f);

			// give damages
			foreach (Collider2D col in colls)
			{
				if (col != null && col.gameObject != null)
				{
					targetCh = col.gameObject.GetChar();

					if (targetCh == null)
					{
					}
					else
					{
						if (attackTeam == targetCh.Team || attackAll)
						{
							SourceSkillObject.ApplyEffects(attacker, col.gameObject);
						}
					}
				}
			}
		}

		public override void ApplyEffect(Character source, GameObject target)
		{
			Character ch = target.GetChar();

			if (ch != null)
			{
				this.attacker = source;

				this.attackerTarget = ch;
				this.attackTeam = ch.Team;
			}

			base.ApplyEffect(source, target);
		}

		public override void Update()
		{
		}

		public override void OnRemove()
		{
		}

		protected override void ApplyEffect()
		{
			
		}

		protected override void RemoveEffect()
		{
			
		}

		public override void OnCharDie()
		{
			DoReproduce();
		}

		public override SkillTraits[] GetTraits()
		{
			return new SkillTraits[] { SkillTraits.Explode, };
		}
	}
}
