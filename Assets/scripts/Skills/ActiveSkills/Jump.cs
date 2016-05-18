﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.AI;
using Assets.scripts.Skills.Base;
using Assets.scripts.Skills.SkillEffects;
using UnityEngine;

namespace Assets.scripts.Skills.ActiveSkills
{
	public class Jump : ActiveSkill
	{
		public int jumpSpeed = 100;

		public Jump()
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
			return SkillId.Jump;
		}

		public override string GetVisibleName()
		{
			return "Jump";
		}

		public override Skill Instantiate()
		{
			return new Jump();
		}

		public override SkillEffect[] CreateEffects(int param)
		{
			return null;
		}

		public override void InitTraits()
		{
			AddTrait(SkillTraits.Jump);
			AnalyzeEffectsForTrais();
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
				GetOwnerData().JumpForward(mouseDirection, range, jumpSpeed);
			else
			{
				if (initTarget != null)
				{
					Vector3 pos = Utils.GetDirectionVector(Owner.GetData().GetBody().transform.position, initTarget.transform.position)*-1;
					GetOwnerData().JumpForward(pos, range, jumpSpeed);
				}
				else
				{
					GetOwnerData().JumpForward(GetOwnerData().GetForwardVector(), range, jumpSpeed);
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