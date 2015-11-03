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

		public override void OnCastStart()
		{
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
	}
}
