// Copyright (c) 2015, Lukas Honke
// ========================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.AI;
using Assets.scripts.Skills.Base;
using Assets.scripts.Skills.SkillEffects;
using UnityEngine;

namespace Assets.scripts.Skills.ActiveSkills
{
	public class JumpShort : ActiveSkill
	{
		public int jumpSpeed = 100;

		public JumpShort()
		{
			castTime = 0f;
			reuse = 1.0f;
			coolDown = 0f;
			requireConfirm = true;
			breaksMouseMovement = false;
			resetMoveTarget = false;

			range = 10;
		}

		public override SkillId GetSkillId()
		{
			return SkillId.JumpShort;
		}

		public override string GetVisibleName()
		{
			return "Jump Short";
		}

		public override Skill Instantiate()
		{
			return new JumpShort();
		}

		public override SkillEffect[] CreateEffects(int param)
		{
			return null;
		}

		public override void InitTraits()
		{
			AddTrait(SkillTraits.Jump);
		}

		public override bool OnCastStart()
		{
			if (castTime > 0)
				CreateCastingEffect(true, "SkillTemplate");

			return true;
		}

		public override void OnLaunch()
		{
			DeleteCastingEffect();

			if (GetOwnerData().GetOwner().AI is PlayerAI)
				GetOwnerData().JumpForward(mouseDirection, GetRange(), jumpSpeed);
			else
			{
				if (initTarget != null)
				{
					Vector3 pos = Utils.GetDirectionVector(Owner.GetData().GetBody().transform.position, initTarget.transform.position)*-1;
					GetOwnerData().JumpForward(pos, GetRange(), jumpSpeed);
				}
				else
				{
					GetOwnerData().JumpForward(GetOwnerData().GetForwardVector(), GetRange(), jumpSpeed);
				}
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
			return !IsActive(); // cant move unless the jump is finished
		}

		public override bool CanRotate()
		{
			return CanMove();
		}
	}
}
