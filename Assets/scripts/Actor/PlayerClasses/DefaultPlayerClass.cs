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
			MaxHp = 50;
			MaxMp = 50;
			MaxSpeed = 10;
		}

		protected override void AddSkills()
		{
			TemplateSkills.Add(SkillTable.Instance.GetSkill(2)); // the projectile test skill
			TemplateSkills.Add(SkillTable.Instance.GetSkill(3)); // the projectile test skill triple
			TemplateSkills.Add(SkillTable.Instance.GetSkill(4)); // aura
			TemplateSkills.Add(SkillTable.Instance.GetSkill(5)); // jump
			TemplateSkills.Add(SkillTable.Instance.GetSkill(6)); // chain skill
			TemplateSkills.Add(SkillTable.Instance.GetSkill(7)); // bomb skill
			TemplateSkills.Add(SkillTable.Instance.GetSkill(8)); // missile projectile
			TemplateSkills.Add(SkillTable.Instance.GetSkill(9)); // chained projectile

			SetMeleeAttackSkill((ActiveSkill) SkillTable.Instance.GetSkill(10));
		}
	}
}
