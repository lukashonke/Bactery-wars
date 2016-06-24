// Copyright (c) 2015, Lukas Honke
// ========================
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
	public class DieEffect : ActiveSkill
	{
		public string effectName = "Explosion";

		public float area = 10f;

		public bool damageFriendly = false;

		public DieEffect()
		{
			castTime = 0;
			coolDown = 0;
			reuse = 8f;

			baseDamage = 10;

			range = 15;

			AvailableToPlayer = false;
		}

		public override SkillId GetSkillId()
		{
			return SkillId.DieEffect;
		}

		public override string GetVisibleName()
		{
			return "DieEffect";
		}

		public override Skill Instantiate()
		{
			return new DieEffect();
		}

		public override string GetDescription()
		{
			return "Deals effect to characters around, triggered on dead.";
		}

		public override SkillEffect[] CreateEffects(int param)
		{
			return new SkillEffect[] {  };
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
			DeleteCastingEffect();
		}

		public override void NotifyCharacterDied()
		{
			GameObject explosion = CreateParticleEffect(effectName, false, GetOwnerData().GetBody().transform.position);
			explosion.GetComponent<ParticleSystem>().Play();
			Object.Destroy(explosion, 2f);

			foreach (Collider2D c in Physics2D.OverlapCircleAll(GetOwnerData().GetBody().transform.position, area))
			{
				Character targetCh = c.gameObject.GetChar();
				if (targetCh == null || c.gameObject.Equals(GetOwnerData().GetBody()))
					continue;

				if (!Owner.CanAttack(targetCh) && !damageFriendly)
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
		}

		public override void MonoUpdate(GameObject gameObject, bool fixedUpdate)
		{
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
