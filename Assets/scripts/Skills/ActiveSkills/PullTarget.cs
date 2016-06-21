// Copyright (c) 2015, Lukas Honke
// ========================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Skills.Base;
using Assets.scripts.Skills.SkillEffects;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.scripts.Skills.ActiveSkills
{
	public class PullTarget : ActiveSkill
	{
		private GameObject targettedPlayer;
		private GameObject activeProjectile;

		public PullTarget()
		{
			castTime = 0f;
			reuse = 5;
			coolDown = 0;
			baseDamage = 15;

			range = 24;

			triggersOwnerCollision = true;
			requireConfirm = true;
			AvailableToPlayer = true;
		}

		public override SkillId GetSkillId()
		{
			return SkillId.PullTarget;
		}

		public override string GetVisibleName()
		{
			return "Pull Target";
		}

		public override string GetDescription()
		{
			return "Click on a monster to pull it towards the player.";
		}

		public override Skill Instantiate()
		{
			return new PullTarget();
		}

		public override SkillEffect[] CreateEffects(int param)
		{
			return new SkillEffect[] { new EffectPull(100),  };
		}

		public override void InitTraits()
		{
			AddTrait(SkillTraits.Damage);
			AddTrait(SkillTraits.Missile);
		}

		public override void OnBeingConfirmed()
		{
			StartPlayerTargetting(); // TODO nakreslit kolecko ktery se hodi tam kde se klikne a odtamtud to vybere target automaticky pokud neni zadnej jinej vybranej
		}

		public override bool OnCastStart()
		{
			GameObject target = GetTarget();

			if (target == null)
				target = initTarget;

			if (target == null || Utils.DistanceSqr(target.transform.position, GetOwnerData().GetBody().transform.position) > (range*range))
			{
				Owner.Message("Select a target.");
				AbortCast();
				return false;
			}

			targettedPlayer = target;

			RotatePlayerTowardsMouse();

			if(castTime > 0)
				CreateCastingEffect(true);

			return true;
		}

		private GameObject targetPullEffect;

		public override void OnLaunch()
		{
			DeleteCastingEffect();

			particleSystem = CreateParticleEffect("SkillUseEffect", true);
			StartParticleEffect(particleSystem);
			DeleteParticleEffect(particleSystem, 5f);

			if (targettedPlayer != null)
			{
				Character ch = targettedPlayer.GetChar();
				if (ch != null && !Owner.CanAttack(ch))
					return;

				targetPullEffect = CreateParticleEffectOnTarget(targettedPlayer, "TargetHitEffect");
				StartParticleEffect(targetPullEffect);
				DeleteParticleEffect(targetPullEffect, 2f);

				ApplyEffects(Owner, targettedPlayer);
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

		public override void MonoCollisionEnter(GameObject gameObject, Collision2D coll)
		{
			if (targettedPlayer != null && coll.gameObject.Equals(targettedPlayer))
			{
				PauseParticleEffect(targetPullEffect);
			}
		}

		public override void MonoTriggerEnter(GameObject gameObject, Collider2D coll)
		{
		}

		public override bool CanMove()
		{
			return !IsBeingCasted();
		}

		public override bool CanRotate()
		{
			return !IsBeingCasted();
		}
	}
}
