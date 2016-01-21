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

		public override SkillEffect[] CreateEffects()
		{
			return null;
		}

		public override void InitTraits()
		{
			AddTrait(SkillTraits.Jump);
		}

		public override bool OnCastStart()
		{
			return true;
		}

		public override void OnLaunch()
		{
			if (GetOwnerData().GetOwner().AI is PlayerAI)
				GetOwnerData().JumpForward(mouseDirection, range, 100);
			else
			{
				GetOwnerData().JumpForward(GetOwnerData().GetForwardVector(), range, 100);
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

		public override void MonoUpdate(GameObject gameObject)
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
