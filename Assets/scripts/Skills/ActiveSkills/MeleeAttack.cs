using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Skills.Base;
using Assets.scripts.Skills.SkillEffects;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.scripts.Skills.ActiveSkills
{
	public class MeleeAttack : ActiveSkill
	{
		private GameObject meleeCasting, meleeHit, meleeExplosion;
		private GameObject target;

		private float meleeMaxRangeAdd = 1f;

		//TODO melee utok nebere damage od hrace
		public MeleeAttack()
		{
			castTime = 0.25f;
			coolDown = 0f;
			reuse = 1f;
			updateFrequency = 0.1f;
			baseDamage = 10;
			resetMoveTarget = false; 

			range = 4;
		}

		public override SkillId GetSkillId()
		{
			return SkillId.MeleeAttack;
		}

		public override string GetVisibleName()
		{
			return "Melee attack";
		}

		public override Skill Instantiate()
		{
			return new MeleeAttack();
		}

		public override SkillEffect[] CreateEffects(int param)
		{
			return new SkillEffect[] { new EffectDamage(baseDamage, 0) };
		}

		public override void InitTraits()
		{
			AddTrait(SkillTraits.Damage);
			AddTrait(SkillTraits.Melee);
		}

		public override bool OnCastStart()
		{
			if (initTarget == null)
				return false;

			target = initTarget;

			Character chTarget = GetCharacterFromObject(initTarget);

			if (chTarget == null || chTarget.Status.IsDead)
				return false;

			RotatePlayerTowardsTarget(initTarget);

			if (Vector3.Distance(GetOwnerData().GetBody().transform.position, initTarget.transform.position) < range)
			{
				meleeCasting = CreateParticleEffect("Melee Preparation", true, GetOwnerData().GetBody().transform.position);
				StartParticleEffect(meleeCasting);
				GetOwnerData().StartMeleeAnimation(castTime);
				return true;
			}

			// dont melee, too far
			return false;
		}

		public override void OnLaunch()
		{
			GetOwnerData().StopMeleeAnimation();

			meleeHit = CreateParticleEffect("Melee Launch", true, GetOwnerData().GetBody().transform.position);
			StartParticleEffect(meleeHit);
		}

		public override void OnFinish()
		{
			if (meleeCasting != null)
				DeleteParticleEffect(meleeCasting);

			if(meleeHit != null)
				DeleteParticleEffect(meleeHit, 1.0f);

			if (initTarget != null && !Owner.Status.IsDead)
			{
				// TODO check angle!!! 
				if (Vector3.Distance(GetOwnerData().GetBody().transform.position, initTarget.transform.position) < range + meleeMaxRangeAdd)
				{
					//RotatePlayerTowardsTarget(initTarget);
					ApplyEffects(Owner, initTarget);
				}
				else
				{
					Debug.Log("too far");
				}
			}
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
		}

		public override void OnAfterEnd()
		{
			if (reuse == 0)
			{
				// continue with next attack
				if (GetOwnerData().RepeatingMeleeAttack)
					GetOwnerData().MeleeInterract(target, true);
			}
		}

		public override void OnAterReuse()
		{
			// continue with next attack
			if (GetOwnerData().RepeatingMeleeAttack)
				GetOwnerData().MeleeInterract(target, true);
		}

		public override void UpdateLaunched()
		{
			if (target == null)
				return;

			if (Vector3.Distance(GetOwnerData().GetBody().transform.position, target.transform.position) > range+meleeMaxRangeAdd)
			{
				if(meleeCasting != null)
					DeleteParticleEffect(meleeCasting);

				if(meleeHit != null)
					DeleteParticleEffect(meleeHit);
			}
		}

		public override void OnAbort()
		{
			GetOwnerData().AbortMeleeAttacking();
		}

		public override bool CanMove()
		{
			return true;
		}

		public override bool CanRotate()
		{
			return true;
		}
	}
}
