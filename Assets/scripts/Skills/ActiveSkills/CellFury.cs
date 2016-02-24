using System.Runtime.Remoting.Messaging;
using Assets.scripts.Base;
using Assets.scripts.Mono;
using Assets.scripts.Skills.Base;
using Assets.scripts.Skills.SkillEffects;
using UnityEngine;

namespace Assets.scripts.Skills.ActiveSkills
{
	public class CellFury : ActiveSkill
	{
		private GameObject activeProjectile;

		private readonly float reuseVal = 0.5f;

		public float rangeBoost = 1f;
		public int nullReuseChance = 0;

		public int duration = 5;

		public CellFury()
		{
			castTime = 0f;
			reuse = 15f;
			coolDown = 0;
			requireConfirm = false;
			canBeCastSimultaneously = true;
			baseDamage = 10;
			resetMoveTarget = false;
		}

		public override SkillId GetSkillId()
		{
			return SkillId.CellFury;
		}

		public override string GetVisibleName()
		{
			return "Cell Fury";
		}

		public override Skill Instantiate()
		{
			return new CellFury();
		}

		public override string GetDescription()
		{
			return "Greatly decreases cooldown of autoattack.";
		}

		public override SkillEffect[] CreateEffects(int param)
		{
			int count = 1;
			if (rangeBoost > 1 || rangeBoost < 1)
				count++;

			SkillEffect[] effects = new SkillEffect[count];
			int index = 0;

			effects[index++] = new EffectMeleeReuse(1, 0.2f, duration, SkillTraits.Melee);
			if(rangeBoost > 1 || rangeBoost < 1)
				effects[index++] = new EffectSkillRange(rangeBoost, duration, SkillTraits.Melee);

			return effects;
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
			DeleteParticleEffect(particleSystem, duration);
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
