using System;
using Assets.scripts.Actor.PlayerClasses.Base;
using Assets.scripts.Skills;
using Assets.scripts.Skills.Base;

namespace Assets.scripts.Actor.PlayerClasses
{
	public class DefaultPlayerClass : ClassTemplate
	{
		public DefaultPlayerClass() : base(ClassId.Default)
		{

		}

		protected override void AddSkills()
		{
			TemplateSkills.Add(SkillTable.Instance.GetSkill(2)); // the projectile test skill
			TemplateSkills.Add(SkillTable.Instance.GetSkill(3)); // the projectile test skill triple
			TemplateSkills.Add(SkillTable.Instance.GetSkill(4)); // aura
		}
	}
}
