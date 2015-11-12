using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.scripts.Skills.ActiveSkills
{
	public class JumpShort : ActiveSkill
	{
		public JumpShort(string name, int id) : base(name, id)
		{
			castTime = 0f;
			reuse = 1.0f;
			coolDown = 0f;
		}

		public override Skill Instantiate()
		{
			return new JumpShort(Name, Id);
		}

		public override bool OnCastStart()
		{
			return true;
		}

		public override void OnLaunch()
		{
			GetOwnerData().JumpForward(10, 100);
		}

		public override void UpdateLaunched()
		{
		}

		public override void OnFinish()
		{
		}

		public override void OnSkillEnd()
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
