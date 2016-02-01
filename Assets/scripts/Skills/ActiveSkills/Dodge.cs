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
	public class Dodge : ActiveSkill
	{
		public Dodge()
		{
			castTime = 0f;
			reuse = 1f;
			coolDown = 0f;
			requireConfirm = true;
			breaksMouseMovement = false;
			resetMoveTarget = false;

			range = 20;
		}

		public override SkillId GetSkillId()
		{
			return SkillId.Dodge;
		}

		public override string GetVisibleName()
		{
			return "Cold Dodge";
		}

		public override Skill Instantiate()
		{
			return new Dodge();
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
				GetOwnerData().JumpForward(mouseDirection, range, 50);
			else
			{
				GetOwnerData().JumpForward(GetOwnerData().GetForwardVector(), range, 50);
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
