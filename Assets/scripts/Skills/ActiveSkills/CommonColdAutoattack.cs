using Assets.scripts.Actor;
using Assets.scripts.Base;
using Assets.scripts.Mono;
using Assets.scripts.Skills.Base;
using Assets.scripts.Skills.SkillEffects;
using UnityEngine;

namespace Assets.scripts.Skills.ActiveSkills
{
	public class CommonColdAutoattack : ActiveSkill
	{
		private GameObject target;

		private GameObject activeProjectile;

		public int projectileForce = 30;

		public CommonColdAutoattack()
		{
			castTime = 0;
			reuse = 1f;
			coolDown = 0f;
			requireConfirm = false;
			baseDamage = 10;

			range = 10;
		}

		public override SkillId GetSkillId()
		{
			return SkillId.CommonColdAutoattack;
		}

		public override string GetVisibleName()
		{
			return "Common Cold Melee";
		}

		public override Skill Instantiate()
		{
			return new CommonColdAutoattack();
		}

		public override SkillEffect[] CreateEffects()
		{
			return new SkillEffect[] {new EffectDamage(baseDamage, 5)};
		}

		public override void InitTraits()
		{
			AddTrait(SkillTraits.Damage);
			AddTrait(SkillTraits.Melee);
		}

		public override bool OnCastStart()
		{
			if (initTarget != null)
			{
				target = initTarget;
				RotatePlayerTowardsTarget(initTarget);
			}
			else
			{
				RotatePlayerTowardsMouse();
			}

			if(castTime > 0)
				CreateCastingEffect(true, "SkillTemplate");

			return true;
		}

		public override void OnLaunch()
		{
			DeleteCastingEffect();

			if (target != null)
			{
				RotatePlayerTowardsTarget(target);
			}
			else
			{
				RotatePlayerTowardsMouse();
			}

			GameObject activeProjectile;

			activeProjectile = CreateSkillProjectile("projectile_00", true);
			if (activeProjectile != null)
			{
				Rigidbody2D rb = activeProjectile.GetComponent<Rigidbody2D>();
				rb.velocity = (GetOwnerData().GetForwardVector() * projectileForce);

				Object.Destroy(activeProjectile, 5f);
			}
		}

		public override void OnFinish()
		{

		}

		public override void OnAfterEnd()
		{
			if (castTime > 0 && target != null)
			{
				// continue with next attack
				if (GetOwnerData().RepeatingMeleeAttack)
					GetOwnerData().MeleeInterract(target, true);
			}
		}

		public override void OnAterReuse()
		{
			if (target != null)
			{
				// continue with next attack
				if (GetOwnerData().RepeatingMeleeAttack)
					GetOwnerData().MeleeInterract(target, true);
			}
		}

		public override void OnMove()
		{
			 AbortCast();
		}

		public override void MonoUpdate(GameObject gameObject)
		{
		}

		public override void MonoStart(GameObject gameObject)
		{
			
		}

		public override void MonoDestroy(GameObject gameObject)
		{
		}

		public override void MonoTriggerEnter(GameObject gameObject, Collider2D other)
		{
			if (other.gameObject.Equals(GetOwnerData().GetBody()))
				return;

			Character ch = other.gameObject.GetChar();
			if (ch == null)
			{
				Destroyable d = other.gameObject.GetComponent<Destroyable>();
				if (d != null && !Owner.CanAttack(d))
					return;
			}
			else if (!Owner.CanAttack(ch))
				return;

			// the only possible collisions are the projectile with target
			ApplyEffects(Owner, other.gameObject);
			DestroyProjectile(gameObject);
		}

		public override void MonoTriggerExit(GameObject gameObject, Collider2D other)
		{
			
		}

		public override void MonoTriggerStay(GameObject gameObject, Collider2D other)
		{
			
		}

		public override void MonoCollisionEnter(GameObject gameObject, Collision2D coll)
		{
			
		}

		public override void MonoCollisionExit(GameObject gameObject, Collision2D coll)
		{
		}

		public override void MonoCollisionStay(GameObject gameObject, Collision2D coll)
		{
		}

		public override void UpdateLaunched()
		{

		}

		public override void OnAbort()
		{
		}

		public override bool CanMove()
		{
			return !IsActive() && !IsBeingCasted();
		}

		public override bool CanRotate()
		{
			return CanMove();
		}
	}
}
