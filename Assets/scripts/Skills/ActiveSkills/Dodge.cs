using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.AI;
using Assets.scripts.Skills.Base;
using Assets.scripts.Skills.SkillEffects;
using UnityEngine;

namespace Assets.scripts.Skills.ActiveSkills
{
	public class Dodge : ActiveSkill
	{
		public int jumpSpeed = 50;

		public int hitEnemyDamage = 0;

		public int firstEnemyHitDamage = 0;
		private bool firstEnemyHit;

		public bool penetrateThroughTargets = false;

		public Dodge()
		{
			castTime = 0f;
			reuse = 2f;
			coolDown = 0.5f;
			requireConfirm = true;
			breaksMouseMovement = false;
			resetMoveTarget = false;
			triggersOwnerCollision = true;

			range = 10;
		}

		public override SkillId GetSkillId()
		{
			return SkillId.Dodge;
		}

		public override string GetVisibleName()
		{
			return "Cold Dodge";
		}

		public override Skill Instantiate()
		{
			return new Dodge();
		}

		public override SkillEffect[] CreateEffects()
		{
			int count = 1;

			if (hitEnemyDamage > 0)
				count ++;

			SkillEffect[] effects = new SkillEffect[count];
			effects[0] = new EffectPushaway(500);

			if (hitEnemyDamage > 0)
				effects[1] = new EffectDamage(hitEnemyDamage);

			return effects;
		}

		public override void InitTraits()
		{
			AddTrait(SkillTraits.Jump);
		}

		public override bool OnCastStart()
		{
			return true;
		}

		public override void OnLaunch()
		{
			if (penetrateThroughTargets)
			{
				Owner.GetData().GetBody().GetComponent<Collider2D>().isTrigger = true;
			}

			firstEnemyHit = false;

			if (GetOwnerData().GetOwner().AI is PlayerAI)
				GetOwnerData().JumpForward(mouseDirection, range, jumpSpeed);
			else
			{
				GetOwnerData().JumpForward(GetOwnerData().GetForwardVector(), range, jumpSpeed);
			}
		}

		public override void UpdateLaunched()
		{
		}

		public override void OnAbort()
		{ 
		}

		public override void OnFinish()
		{
			if (penetrateThroughTargets)
			{
				Owner.GetData().GetBody().GetComponent<Collider2D>().isTrigger = false;
			}
		}

		public override void MonoUpdate(GameObject gameObject)
		{
		}

		public override void MonoTriggerEnter(GameObject gameObject, Collider2D coll)
		{
			if (!IsActive())
				return;

			if (coll != null && coll.gameObject != null && gameObject != null)
			{
				if (coll.gameObject.GetChar() != null && GetOwnerData().GetOwner().CanAttack(coll.gameObject.GetChar()))
				{
					if (!firstEnemyHit)
					{
						ApplyEffect(Owner, coll.gameObject, new EffectDamage(firstEnemyHitDamage));
						firstEnemyHit = true;
					}

					ApplyEffects(Owner, coll.gameObject);
				}
			}
		}

		public override void MonoCollisionEnter(GameObject gameObject, Collision2D coll)
		{
			if (!IsActive())
				return;

			if (coll != null && coll.gameObject != null && gameObject != null)
			{
				if (coll.gameObject.GetChar() != null && GetOwnerData().GetOwner().CanAttack(coll.gameObject.GetChar()))
				{
					if (!firstEnemyHit)
					{
						ApplyEffect(Owner, coll.gameObject, new EffectDamage(firstEnemyHitDamage));
						firstEnemyHit = true;
					}

					ApplyEffects(Owner, coll.gameObject);
				}
			}
		}

		public override bool CanMove()
		{
			return true; // cant move unless the jump is finished
		}

		public override bool CanRotate()
		{
			return true;
		}
	}
}
