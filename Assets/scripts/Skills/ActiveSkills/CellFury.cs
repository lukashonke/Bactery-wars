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

		public CellFury()
		{
			castTime = 0f;
			reuse = 1;
			coolDown = 0;
			requireConfirm = false;
			baseDamage = 10;
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
			return new SkillEffect[] {new EffectMeleeReuse(1, 0.5f, 10, SkillTraits.Melee), };
		}

		public override void InitTraits()
		{
			AddTrait(SkillTraits.BuffDamage);
		}

		public override bool OnCastStart()
		{
			//CreateCastingEffect(true, "SkillTemplate");

			return true;
		}

		public override void OnLaunch()
		{
			//DeleteCastingEffect();

			ApplyEffects(Owner, Owner.GetData().gameObject);
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

		public override void MonoTriggerEnter(GameObject gameObject, Collider2D other)
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
