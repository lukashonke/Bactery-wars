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
	public class EffectExplodeOnDie : EffectStatus
	{
		protected int Dmg { get; set; }
		protected float radius;

		public bool attackAll = false;

		private int attackTeam = -1;
		private Character source;

		public EffectExplodeOnDie(int duration, int damage, int radius) : base(duration)
		{
			this.radius = radius;
			Dmg = damage;
			this.attackAll = false;
		}

		public EffectExplodeOnDie(int duration, int damage, int radius, bool attackAll) : base(duration)
		{
			this.radius = radius;
			Dmg = damage;
			this.attackAll = attackAll;
		}

		private void DoExplode()
		{
			if (source == null)
				return;

			Character targetCh;

			Collider2D[] colls = Physics2D.OverlapCircleAll(source.GetData().transform.position, radius);

			// explosion effect
			GameObject explosion = ((ActiveSkill)SourceSkillObject).CreateParticleEffect("PoisonExplosion", false, source.GetData().GetBody().transform.position);
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
						Destroyable d = col.gameObject.GetComponent<Destroyable>();

						if (d != null && attackAll) // TODO fix
						{
							bool crit;
							int damage = source.CalculateDamage(Dmg, null, SourceSkillObject, true, out crit);
							d.ReceiveDamage(source, damage);
						}
					}
					else
					{
						if (attackTeam == targetCh.Team || attackAll)
						{
							bool crit;
							int damage = source.CalculateDamage(Dmg, targetCh, SourceSkillObject, true, out crit);

							source.OnAttack(targetCh);

							targetCh.ReceiveDamage(source, damage, SourceSkill, crit);
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
				this.source = ch;
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
			DoExplode();
		}

		public override SkillTraits[] GetTraits()
		{
			return new SkillTraits[] { SkillTraits.Explode, };
		}
	}
}
