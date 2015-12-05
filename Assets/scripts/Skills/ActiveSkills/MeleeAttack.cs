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
		private GameObject meleeEffect;

		public MeleeAttack(string name, int id) : base(name, id)
		{
			castTime = 1.0f;
			coolDown = 0f;
			reuse = 0;
			updateFrequency = 0.1f;

			range = 4;
		}

		public override Skill Instantiate()
		{
			return new MeleeAttack(Name, Id);
		}

		public override SkillEffect[] CreateEffects()
		{
			return new SkillEffect[] { new EffectDamage(10, 0) };
		}

		public override void InitTraits()
		{
			AddTrait(SkillTraits.Damage);
		}

		public override bool OnCastStart()
		{
			if (initTarget == null)
				return false;

			Character chTarget = GetCharacterFromObject(initTarget);

			if (chTarget == null || chTarget.Status.IsDead)
				return false;

			RotatePlayerTowardsTarget(initTarget);

			if (Vector3.Distance(GetOwnerData().GetBody().transform.position, initTarget.transform.position) < range)
			{
				meleeEffect = CreateParticleEffect("Melee2", true, GetOwnerData().GetBody().transform.position);
				StartParticleEffect(meleeEffect);
				GetOwnerData().StartMeleeAnimation(castTime);
				return true;
			}

			// dont melee, too far
			return false;
		}

		public override void OnLaunch()
		{
			GetOwnerData().StopMeleeAnimation();
		}

		public override void OnFinish()
		{
			if (meleeEffect != null)
				DeleteParticleEffect(meleeEffect);

			if (initTarget != null)
			{
				if (Vector3.Distance(GetOwnerData().GetBody().transform.position, initTarget.transform.position) < range)
				{
					ApplyEffects(Owner, initTarget);

					if (GetOwnerData().IsMeleeAttacking) // continue with next attack
						GetOwnerData().MeleeAttack(initTarget);
				}
				else
				{
					Debug.Log("too far");
				}
			}
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
		}

		public override void UpdateLaunched()
		{
			if (Vector3.Distance(GetOwnerData().GetBody().transform.position, initTarget.transform.position) > range)
			{
				if(meleeEffect != null)
					DeleteParticleEffect(meleeEffect);
			}
		}

		public override void OnAbort()
		{
			GetOwnerData().AbortMeleeAttacking();
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
