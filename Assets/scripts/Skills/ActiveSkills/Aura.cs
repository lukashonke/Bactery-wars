using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.AI;
using Assets.scripts.Skills.Base;
using Assets.scripts.Skills.SkillEffects;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Assets.scripts.Skills.ActiveSkills
{
	public class Aura : ActiveSkill
	{
		protected GameObject effect;

		public string effectName = "Explosion";

		public float area = 10f;

		public Aura()
		{
			castTime = 2f;
			coolDown = 0;
			reuse = 1f;

			baseDamage = 1;

			range = 15;

			//movementAbortsSkill = true;
			//updateFrequency = 0.01f;

			requireConfirm = false;
		}

		public override SkillId GetSkillId()
		{
			return SkillId.Aura;
		}

		public override string GetVisibleName()
		{
			return "Aura";
		}

		public override Skill Instantiate()
		{
			return new Aura();
		}

		public override string GetDescription()
		{
			return "Aura sill around player.";
		}

		public override SkillEffect[] CreateEffects(int param)
		{
			return new SkillEffect[] { new EffectDamage(10, 0),  };
		}

		public override void InitTraits()
		{
			AnalyzeEffectsForTrais();
		}

		public override bool OnCastStart()
		{
			CreateCastingEffect(true, GetName());
			return true;
		}

		public override void OnLaunch()
		{
			DeleteCastingEffect();

			GameObject explosion = CreateParticleEffect(effectName, false, GetOwnerData().GetBody().transform.position);
			explosion.GetComponent<ParticleSystem>().Play();
			Object.Destroy(explosion, 2f);

			foreach (Collider2D c in Physics2D.OverlapCircleAll(GetOwnerData().GetBody().transform.position, 10))
			{
				Character targetCh = c.gameObject.GetChar();
				if (targetCh == null || c.gameObject.Equals(GetOwnerData().GetBody()))
					continue;

				if (Offensive() && !Owner.CanAttack(targetCh))
					continue;

				if (!Offensive() && Owner.CanAttack(targetCh))
					continue;

				if (targetCh.IsInteractable())
					continue;

				ApplyEffects(Owner, c.gameObject);
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
			if(effect != null)
				DeleteParticleEffect(effect);
		}

		public override void MonoUpdate(GameObject gameObject, bool fixedUpdate)
		{
		}

		public virtual bool Offensive()
		{
			return true;
		}

		public override bool CanMove()
		{
			return !IsActive();
		}

		public override bool CanRotate()
		{
			return !IsActive();
		}
	}
}
