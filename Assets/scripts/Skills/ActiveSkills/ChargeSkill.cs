// Copyright (c) 2015, Lukas Honke
// ========================
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

		public int uses = 1;
		private int currentUses = 0;

		public int resetSkillSlotId = -1;
		public int resetSkillMaxReuse = 10;

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
			return "Your next skill will have +50% damage and +50% range (doesn't affect effects, such as Poison or Slow).";
		}

		public override string GetBaseInfo()
		{
			return "Reuse " + reuse + " sec";
		}

		public override void NotifyAnotherSkillCastStart(Skill sk)
		{
			if (active && sk is ActiveSkill)
			{
				currentUses --;

				// boost skills power
				((ActiveSkill)sk).TempBoostDamage(power);
				((ActiveSkill)sk).TempBoostRange(power);

				if (currentUses == 0)
				{
					active = false;

					// remove particle system
					DeleteParticleEffect(particleSystem);

					if(Owner.ActiveEffects.Count > 0)
					{
						foreach (SkillEffect ef in Owner.ActiveEffects.ToArray())
						{
							if (ef.SourceSkill == GetSkillId())
							{
								Owner.RemoveEffect(ef);
							}
						}
					}
				}
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
			currentUses = uses;

			particleSystem = CreateParticleEffect("ActiveEffect", true);
			StartParticleEffect(particleSystem);

			Owner.IncreaseRangeTillSkillLaunched(power);

			ApplyEffects(Owner, Owner.GetData().gameObject);
			active = true;

			if (resetSkillSlotId > -1)
			{
				Skill sk = Owner.Skills.GetSkill(resetSkillSlotId);

				if (sk != null && sk is ActiveSkill)
				{
					ActiveSkill ask = sk as ActiveSkill;

					if (ask.reuse <= resetSkillMaxReuse)
					{
						ask.LastUsed = -10000;
						GetOwnerData().SetSkillReuseTimer(ask, true);
					}
				}
			}
			//DeleteParticleEffect(particleSystem);
		}

		public override void OnFinish()
		{
			if (nullReuseChance > 0)
			{
				if (Random.Range(0, 100) < nullReuseChance)
				{
					this.LastUsed = -10000;
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
