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

		public override SkillEffect[] CreateEffects()
		{
			return new SkillEffect[] {new EffectMeleeReuse(1, 0.3f, duration, SkillTraits.Melee), };
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
			Debug.Log("on launch");
			particleSystem = CreateParticleEffect("ActiveEffect", true);
			StartParticleEffect(particleSystem);

			ApplyEffects(Owner, Owner.GetData().gameObject);
			DeleteParticleEffect(particleSystem, duration);
		}

		public override void OnFinish()
		{
			
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
