using Assets.scripts.Base;
using Assets.scripts.Mono;
using Assets.scripts.Skills.Base;
using Assets.scripts.Skills.SkillEffects;
using UnityEngine;

namespace Assets.scripts.Skills.ActiveSkills
{
	public class PushbackProjectile : ActiveSkill
	{
		private GameObject activeProjectile;
		public int pushbackForce = 25;

        public PushbackProjectile()
		{
			castTime = 1f;
			reuse = 5f;
			coolDown = 0;
			requireConfirm = true;
			baseDamage = 10;

			range = 25;
		}

		public override SkillId GetSkillId()
		{
            return SkillId.PushbackProjectile;
		}

		public override string GetVisibleName()
		{
            return "Pushback Projectile";
		}

		public override Skill Instantiate()
		{
            return new PushbackProjectile();
		}

		public override SkillEffect[] CreateEffects(int param)
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
				rb.velocity = (GetOwnerData().GetForwardVector(0) * 27);

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
            Vector3 pushDir = GetOwnerData().GetForwardVector(0).normalized * -1;
			GetOwnerData().AddPhysicsPush(pushDir * pushbackForce, ForceMode2D.Impulse, null);
			//GetOwnerData().gameObject.GetComponent<Rigidbody2D>().AddForce(pushDir * pushbackForce, ForceMode2D.Impulse);
		}

		public override void MonoDestroy(GameObject gameObject)
		{
		}

		public override void MonoTriggerEnter(GameObject gameObject, Collider2D coll)
		{
			if (coll.gameObject.Equals(GetOwnerData().GetBody()))
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
