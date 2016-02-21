using Assets.scripts.Base;
using Assets.scripts.Mono;
using Assets.scripts.Skills.Base;
using Assets.scripts.Skills.SkillEffects;
using UnityEngine;

namespace Assets.scripts.Skills.ActiveSkills
{
	public class CollisionDamageAttack : ActiveSkill
	{
		private GameObject target;

		public int pushForce = 200;

		public CollisionDamageAttack()
		{
			castTime = 0;
			reuse = 3f;
			coolDown = 0f;
			requireConfirm = false;
			baseDamage = 10;
			triggersOwnerCollision = true;

			range = 10;
		}

		public override SkillId GetSkillId()
		{
			return SkillId.CollisionDamageAttack;
		}

		public override string GetVisibleName()
		{
			return "Collision Damage Attack";
		}

		public override Skill Instantiate()
		{
			return new CollisionDamageAttack();
		}

		public override SkillEffect[] CreateEffects(int param)
		{
			return new SkillEffect[] { new EffectDamage(baseDamage, 0), new EffectPushaway(pushForce), };
		}

		public override void InitTraits()
		{
		}

		public override bool OnCastStart()
		{
			return true;
		}

		public override void OnLaunch()
		{
			if (initTarget == null)
				return;

			GameObject o = CreateParticleEffect("Explosion", false);
			StartParticleEffect(o);
			Object.Destroy(o, 2f);

			ApplyEffects(GetOwnerData().GetOwner(), initTarget);
		}

		public override void OnFinish()
		{
		}

		public override void OnAfterEnd()
		{
		}

		public override void OnAterReuse()
		{
		}

		public override void OnMove()
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
			if (gameObject != null && gameObject.Equals(GetOwnerData().GetBody()))
			{
				if (coll.gameObject.GetChar() != null && GetOwnerData().GetOwner().CanAttack(coll.gameObject.GetChar()))
				{
					if (CanUse())
					{
						Start(coll.gameObject);
					}
				}
			}
		}

		public override void MonoTriggerExit(GameObject gameObject, Collider2D other)
		{
			
		}

		public override void MonoTriggerStay(GameObject gameObject, Collider2D other)
		{
			
		}

		public override void MonoCollisionEnter(GameObject gameObject, Collision2D coll)
		{
			if (coll != null && coll.gameObject != null && gameObject != null && gameObject.Equals(GetOwnerData().GetBody()))
			{
				if (coll.gameObject.GetChar() != null && GetOwnerData().GetOwner().CanAttack(coll.gameObject.GetChar()))
				{
					if (CanUse())
					{
						Start(coll.gameObject);
					}
				}
			}
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
			return true;
		}

		public override bool CanRotate()
		{
			return CanMove();
		}
	}
}
