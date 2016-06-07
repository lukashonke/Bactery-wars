using System.Runtime.Remoting.Messaging;
using Assets.scripts.Base;
using Assets.scripts.Mono;
using Assets.scripts.Skills.Base;
using Assets.scripts.Skills.SkillEffects;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.scripts.Skills.ActiveSkills
{
	public class ChargeSkill : ActiveSkill
	{
		private GameObject activeProjectile;

		private readonly float reuseVal = 0.5f;

		public float rangeBoost = 1f;
		public int nullReuseChance = 0;

		public float power = 1.5f;

		private bool active = false;

		public ChargeSkill()
		{
			castTime = 0f;
			reuse = 5f;
			coolDown = 0;
			requireConfirm = false;
			canBeCastSimultaneously = true;
			baseDamage = 10;
			resetMoveTarget = false;

			AvailableToPlayer = true;
		}

		public override SkillId GetSkillId()
		{
			return SkillId.ChargeSkill;
		}

		public override string GetVisibleName()
		{
			return "Charge Skill";
		}

		public override Skill Instantiate()
		{
			return new ChargeSkill();
		}

		public override string GetDescription()
		{
			return "Your next skill will have +50% damage.";
		}

		public override string GetBaseInfo()
		{
			return "Reuse " + reuse + " sec";
		}

		public override void NotifyAnotherSkillCastStart(Skill sk)
		{
			if (active && sk is ActiveSkill)
			{
				active = false;

				// boost skills power
				((ActiveSkill)sk).TempBoostDamage(power);

				// remove particle system
				DeleteParticleEffect(particleSystem);
			}
		}

		public override SkillEffect[] CreateEffects(int param)
		{
			//SkillEffect[] effects = new SkillEffect[1];
			//effects[0] = new EffectSkillDamage(2.0f, duration);
			//return effects;
			return null;
		}

		public override void InitTraits()
		{
			AddTrait(SkillTraits.BuffDamage);
		}

		public override bool OnCastStart()
		{
			return true;
		}

		public override void OnLaunch()
		{
			particleSystem = CreateParticleEffect("ActiveEffect", true);
			StartParticleEffect(particleSystem);

			ApplyEffects(Owner, Owner.GetData().gameObject);
			active = true;
			//DeleteParticleEffect(particleSystem);
		}

		public override void OnFinish()
		{
			if (nullReuseChance > 0)
			{
				if (Random.Range(0, 100) < nullReuseChance)
				{
					this.LastUsed = 0;
					GetOwnerData().SetSkillReuseTimer(this, true);
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
