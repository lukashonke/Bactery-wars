using Assets.scripts.Actor;
using Assets.scripts.Base;
using Assets.scripts.Mono;
using Assets.scripts.Skills.Base;
using Assets.scripts.Skills.SkillEffects;
using UnityEngine;

namespace Assets.scripts.Skills.ActiveSkills
{
	public class ProjectileStrong : ActiveSkill
	{
		private GameObject activeProjectile;

		public int force;

		public ProjectileStrong()
		{
			castTime = 0f;
			reuse = 1;
			coolDown = 0;
			requireConfirm = true;
			baseDamage = 20;

			range = 5;

			force = 10;
		}

		public override SkillId GetSkillId()
		{
			return SkillId.Projectile;
		}

		public override string GetVisibleName()
		{
			return "Test Projectile";
		}

		public override Skill Instantiate()
		{
			return new ProjectileStrong();
		}

		public override SkillEffect[] CreateEffects(int param)
		{
			return new SkillEffect[] {new EffectDamage(baseDamage, 2)};
		}

		public override void InitTraits()
		{
			AddTrait(SkillTraits.Damage);
		}

		public override bool OnCastStart()
		{
			RotatePlayerTowardsMouse();

			CreateCastingEffect(true, GetName());

			return true;
		}

		public override void OnLaunch()
		{
			DeleteCastingEffect();

			activeProjectile = CreateSkillProjectile("projectile_blacktest_i00", true);

			if (activeProjectile != null)
			{
				Rigidbody2D rb = activeProjectile.GetComponent<Rigidbody2D>();
				rb.velocity = (GetOwnerData().GetForwardVector(0) * force);

				//Debug.DrawRay(GetOwnerData().GetShootingPosition().transform.position, rb.velocity, Color.green, 5f);

				Object.Destroy(activeProjectile, 5f);
			}
		}

		public override void OnFinish()
		{

		}

		public override void MonoUpdate(GameObject gameObject, bool fixedUpdate)
		{
		}

		public override void MonoStart(GameObject gameObject)
		{
			
		}

		public override void MonoDestroy(GameObject gameObject)
		{
		}

		public override void MonoTriggerEnter(GameObject gameObject, Collider2D coll)
		{
			if (coll.gameObject.Equals(GetOwnerData().GetBody()))
				return;

			Character ch = coll.gameObject.GetChar();
			if (ch == null)
			{
				Destroyable d = coll.gameObject.GetComponent<Destroyable>();
				if (d != null && !Owner.CanAttack(d))
					return;
			}
			else if (!Owner.CanAttack(ch))
				return;

			// the only possible collisions are the projectile with target
			ApplyEffects(Owner, coll.gameObject);
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
			if (IsBeingCasted())
				return false;
			return true;
		}

		public override bool CanRotate()
		{
			return CanMove();
		}
	}
}
