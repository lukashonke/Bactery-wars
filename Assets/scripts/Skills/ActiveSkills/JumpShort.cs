using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Skills.SkillEffects;
using UnityEngine;

namespace Assets.scripts.Skills.ActiveSkills
{
	public class JumpShort : ActiveSkill
	{
		public JumpShort(string name, int id) : base(name, id)
		{
			castTime = 0f;
			reuse = 1.0f;
			coolDown = 0f;
			requireConfirm = true;
			MovementBreaksConfirmation = true;
		}

		public override Skill Instantiate()
		{
			return new JumpShort(Name, Id);
		}

		public override SkillEffect[] CreateEffects()
		{
			return null;
		}

		public override bool OnCastStart()
		{
			return true;
		}

		public override void OnLaunch()
		{
			GetOwnerData().JumpForward(mouseDirection, 4, 100);
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
