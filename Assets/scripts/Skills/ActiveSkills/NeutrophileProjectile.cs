using Assets.scripts.Base;
using Assets.scripts.Mono;
using Assets.scripts.Skills.Base;
using Assets.scripts.Skills.SkillEffects;
using UnityEngine;

namespace Assets.scripts.Skills.ActiveSkills
{
	public class NeutrophileProjectile : ActiveSkill
	{
		private GameObject activeProjectile;

		public NeutrophileProjectile()
		{
			castTime = 0f;
			reuse = 2f;
			coolDown = 0;
			requireConfirm = true;
			baseDamage = 10;

			range = 10;
		}

		public override SkillId GetSkillId()
		{
			return SkillId.NeutrophileProjectile;
		}

		public override string GetVisibleName()
		{
			return "Neutrophile Projectile";
		}

		public override Skill Instantiate()
		{
			return new NeutrophileProjectile();
		}

		public override SkillEffect[] CreateEffects()
		{
			return new SkillEffect[] {new EffectDamage(baseDamage, 10)};
		}

		public override void InitTraits()
		{
			AddTrait(SkillTraits.Damage);
		}

		public override bool OnCastStart()
		{
			RotatePlayerTowardsMouse();

			CreateCastingEffect(true, "SkillTemplate");

			return true;
		}

		public override void OnLaunch()
		{
			DeleteCastingEffect();

			activeProjectile = CreateSkillProjectile("projectile_00", true);

			if (activeProjectile != null)
			{
				Rigidbody2D rb = activeProjectile.GetComponent<Rigidbody2D>();
				rb.velocity = (GetOwnerData().GetForwardVector(0) * 17);

				//Debug.DrawRay(GetOwnerData().GetShootingPosition().transform.position, rb.velocity, Color.green, 5f);

				Object.Destroy(activeProjectile, 5f);
			}
		}

		public override void OnFinish()
		{

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
