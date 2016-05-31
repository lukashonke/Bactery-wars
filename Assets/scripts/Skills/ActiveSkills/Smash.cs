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
	public class Smash : ActiveSkill
	{
		public string effectName = "Smash";

		private GameObject meleeHit, meleeExplosion;

		public bool checkAngleToo = true;

		private float meleeMaxRangeAdd = 1f;

		public int angle = 60;

		public Smash()
		{
			castTime = 0f;
			coolDown = 0f;
			reuse = 1.5f;
			updateFrequency = 0.1f;
			baseDamage = 10;
			resetMoveTarget = false; 

			range = 4;
			AvailableToPlayerAsAutoattack = true;
		}

		public override SkillId GetSkillId()
		{
			return SkillId.Smash;
		}

		public override string GetVisibleName()
		{
			return "Smash";
		}

		public override string GetDescription()
		{
			return "A generic smash skill";
		}

		public override Skill Instantiate()
		{
			return new Smash();
		}

		public override SkillEffect[] CreateEffects(int param)
		{
			return new SkillEffect[] { new EffectDamage(baseDamage, 2) };
		}

		public override void InitTraits()
		{
			AddTrait(SkillTraits.Damage);
			AddTrait(SkillTraits.Melee);
		}

		public override bool OnCastStart()
		{
			RotatePlayerTowardsMouse();

			if(castTime > 0)
				CreateCastingEffect(true, GetName());

			return true;
		}

		public override void OnLaunch()
		{
			DeleteCastingEffect();

			meleeHit = CreateParticleEffect(effectName, true, GetOwnerData().GetBody().transform.position);

			if (GetPlayerData() != null)
			{
				UpdateMouseDirection(meleeHit.transform);

				meleeHit.transform.rotation = Utils.GetRotationToMouse(meleeHit.transform);
			}
			else if (initTarget != null)
			{
				meleeHit.transform.rotation = Utils.GetRotationToTarget(meleeHit.transform, initTarget);
			}

			StartParticleEffect(meleeHit);
		}

		public override void OnFinish()
		{
			if(meleeHit != null)
				DeleteParticleEffect(meleeHit, 1.0f);

			RaycastHit2D[] hits = Utils.CastBoxInDirection(Owner.GetData().GetBody(), GetOwnerData().GetForwardVector(), range, range);

			foreach (RaycastHit2D h in hits)
			{
				Character ch = h.transform.gameObject.GetChar();
				if (ch == null || !Owner.CanAttack(ch))
					continue;

				if (Utils.IsInCone(Owner.GetData().GetBody(), GetOwnerData().GetForwardVector(), h.transform.gameObject, angle, range))
					ApplyEffects(Owner, h.transform.gameObject);
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

		public override void OnAfterEnd()
		{
		}

		public override void OnAterReuse()
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
