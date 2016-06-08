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
	public class Explode : ActiveSkill
	{
		protected GameObject effect;

		public string effectName = "Explosion";

		public float area = 5f;

		public Explode()
		{
			castTime = 0.5f;
			coolDown = 0;
			reuse = 8f;

			baseDamage = 10;

			range = 15;

			//movementAbortsSkill = true;
			//updateFrequency = 0.01f;

			requireConfirm = true;
			AvailableToPlayer = true;
			RequiredSlotLevel = 2;
		}

		public override SkillId GetSkillId()
		{
			return SkillId.Explode;
		}

		public override string GetVisibleName()
		{
			return "Explode";
		}

		public override Skill Instantiate()
		{
			return new Explode();
		}

		public override string GetDescription()
		{
			return "Explodes around player.";
		}

		public override SkillEffect[] CreateEffects(int param)
		{
			return new SkillEffect[] { new EffectDamage(baseDamage, 0), };
		}

		public override void InitTraits()
		{
			AnalyzeEffectsForTrais();
		}

		public override void OnBeingConfirmed()
		{
			if (confirmObject == null)
			{
				confirmObject = GetPlayerData().CreateSkillResource("SkillTemplate", "circletargetting", true, GetPlayerData().GetShootingPosition().transform.position);
				confirmObject.transform.localScale = new Vector3(0.2f * area, 0.2f * area);
			}
		}

		public override bool OnCastStart()
		{
			if(castTime > 0)
				CreateCastingEffect(true, GetName());
			return true;
		}

		public override void OnLaunch()
		{
			DeleteCastingEffect();

			GameObject explosion = CreateParticleEffect(effectName, false, GetOwnerData().GetBody().transform.position);
			explosion.GetComponent<ParticleSystem>().Play();
			Object.Destroy(explosion, 2f);

			foreach (Collider2D c in Physics2D.OverlapCircleAll(GetOwnerData().GetBody().transform.position, area))
			{
				Character targetCh = c.gameObject.GetChar();
				if (targetCh == null || c.gameObject.Equals(GetOwnerData().GetBody()))
					continue;

				if (!Owner.CanAttack(targetCh))
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
