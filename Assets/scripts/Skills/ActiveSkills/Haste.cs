using System.Runtime.Remoting.Messaging;
using Assets.scripts.Base;
using Assets.scripts.Mono;
using Assets.scripts.Skills.Base;
using Assets.scripts.Skills.SkillEffects;
using UnityEngine;

namespace Assets.scripts.Skills.ActiveSkills
{
	public class Haste : ActiveSkill
	{
		public float rangeBoost = 1f;
		public int nullReuseChance = 0;

		public int count = 5;

		public Haste()
		{
			castTime = 0f;
			reuse = 15f;
			coolDown = 0;
			requireConfirm = false;
			canBeCastSimultaneously = true;
			baseDamage = 10;
			resetMoveTarget = false;

			AvailableToPlayer = true;
		}

		public override SkillId GetSkillId()
		{
			return SkillId.Haste;
		}

		public override string GetVisibleName()
		{
			return "Haste";
		}

		public override Skill Instantiate()
		{
			return new Haste();
		}

		public override string GetDescription()
		{
			return "The next " + count + " autoattacks will have null reuse and cooldown.";
		}

		public override string GetBaseInfo()
		{
			return "Reuse " + reuse + " sec";
		}

		public override void NotifyEffectRemoved(SkillEffect eff)
		{
			if (eff is EffectSkillReuse)
			{
				DeleteParticleEffect(particleSystem);
			}
		}

		public override SkillEffect[] CreateEffects(int param)
		{
			SkillEffect[] effects = new SkillEffect[1];
			effects[0] = new EffectSkillReuse(1, 0.1f, -1, SkillTraits.Melee).SetCountUses(count);
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
