using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Skills.Base;
using Assets.scripts.Skills.SkillEffects;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.scripts.Skills.ActiveSkills
{
	public class MissileProjectile : ActiveSkill
	{
		private GameObject targettedPlayer;
		private GameObject activeProjectile;

		public MissileProjectile(string name, int id) : base(name, id)
		{
			castTime = 1f;
			reuse = 0;
			coolDown = 0;
			baseDamage = 15;

			requireConfirm = true;
		}

		public override Skill Instantiate()
		{
			return new MissileProjectile(Name, Id);
		}

		public override SkillEffect[] CreateEffects()
		{
			return new SkillEffect[] { new EffectDamage(baseDamage, 2) };
		}

		public override void InitTraits()
		{
			AddTrait(SkillTraits.Damage);
			AddTrait(SkillTraits.Missile);
		}

		public override void OnBeingConfirmed()
		{
			StartPlayerTargetting();
		}

		public override bool OnCastStart()
		{
			GameObject target = GetTarget();
			if (target == null)
			{
				AbortCast();
				return false;
			}

			targettedPlayer = target;

			RotatePlayerTowardsMouse();
			CreateCastingEffect(true);

			return true;
		}

		public override void OnLaunch()
		{
			DeleteCastingEffect();

			if (targettedPlayer != null)
			{
				activeProjectile = CreateSkillProjectile("projectile_blacktest_i00", true);

				if (activeProjectile != null)
				{
					Rigidbody2D rb = activeProjectile.GetComponent<Rigidbody2D>();

					Vector3 direction = GetDirectionVector(GetOwnerData().GetBody().transform.position, targettedPlayer.transform.position);
					rb.velocity = (direction * 10);

					Object.Destroy(activeProjectile, 15f);
				}
			}
		}

		private Vector3 GetDirectionVector(Vector3 pos, Vector3 targetPos)
		{
			Vector3 direction = Utils.GetDirectionVector(targetPos, pos).normalized;

			if (Vector3.Distance(pos, targetPos) > 1)
			{
				direction = Utils.RotateDirectionVector(direction, UnityEngine.Random.Range(-30, 30));
			}

			return direction;
		}

		public override void UpdateLaunched()
		{
		}

		public override void OnAbort()
		{
		}

		public override void OnFinish()
		{
		}

		public override void MonoUpdate(GameObject gameObject)
		{
			if (activeProjectile == null)
				return;

			// updates: 2/sec
			if (System.Environment.TickCount%50 == 0)
			{
				Rigidbody2D rb = activeProjectile.GetComponent<Rigidbody2D>();

				rb.velocity = (GetDirectionVector(gameObject.transform.position, targettedPlayer.transform.position) * 10);
			}
		}

		public override void MonoTriggerEnter(GameObject gameObject, Collider2D other)
		{
			if (other.gameObject.Equals(GetOwnerData().GetBody()))
				return;

			ApplyEffects(Owner, other.gameObject);
			DestroyProjectile(gameObject);
		}

		public override bool CanMove()
		{
			return !IsBeingCasted();
		}

		public override bool CanRotate()
		{
			return !IsBeingCasted();
		}
	}
}
