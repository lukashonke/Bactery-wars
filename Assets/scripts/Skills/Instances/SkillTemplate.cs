using UnityEngine;

namespace Assets.scripts.Skills.Instances
{
	public class SkillTemplate : ActiveSkill
	{
		public SkillTemplate(string name, int id) : base(name, id)
		{
			
		}

		public override Skill Instantiate()
		{
			return new SkillTemplate(Name, Id);
		}

		public override bool OnCastStart()
		{
			return true;
		}

		public override void OnLaunch()
		{
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
			return true;
		}

		public override bool CanRotate()
		{
			return true;
		}
	}
}
