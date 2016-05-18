using System.Runtime.Remoting.Messaging;
using Assets.scripts.Base;
using Assets.scripts.Mono;
using Assets.scripts.Skills.Base;
using Assets.scripts.Skills.SkillEffects;
using UnityEngine;

namespace Assets.scripts.Skills.ActiveSkills
{
	public class Selfbuff : ActiveSkill
	{
		public int duration = 5;

		public string effectName;

		public Selfbuff()
		{
			castTime = 0f;
			reuse = 15f;
			coolDown = 0;
			requireConfirm = false;
			canBeCastSimultaneously = true;
			baseDamage = 10;
			resetMoveTarget = false;

			effectName = "Electric";
		}

		public override SkillId GetSkillId()
		{
			return SkillId.Selfbuff;
		}

		public override string GetVisibleName()
		{
			return "Selfbuff";
		}

		public override Skill Instantiate()
		{
			return new Selfbuff();
		}

		public override string GetDescription()
		{
			return "Selfbuff";
		}

		public override SkillEffect[] CreateEffects(int param)
		{
			return new SkillEffect[] {};
		}

		public override void InitTraits()
		{
			AnalyzeEffectsForTrais();
		}

		public override bool OnCastStart()
		{
			return true;
		}

		public override void OnLaunch()
		{
			particleSystem = CreateParticleEffect(effectName, true);
			StartParticleEffect(particleSystem);

			ApplyEffects(Owner, Owner.GetData().gameObject);
			DeleteParticleEffect(particleSystem, duration);
		}

		public override void OnFinish()
		{
			/*if (nullReuseChance > 0)
			{
				if (Random.Range(0, 100) < nullReuseChance)
				{
					this.LastUsed = 0;
					GetOwnerData().SetSkillReuseTimer(this, true);
				}
			}*/
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
