using System;
using Assets.scripts.Actor.PlayerClasses.Base;
using Assets.scripts.Skills;
using Assets.scripts.Skills.Base;

namespace Assets.scripts.Actor.PlayerClasses
{
	public class DefaultPlayerClass : ClassTemplate
	{
		public DefaultPlayerClass()
		{
			MaxHp = 50;
			MaxMp = 50;
			MaxSpeed = 10;
		}

		protected override void AddSkillsToTemplate()
		{
			TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.SkillTestProjectile)); // the projectile test skill
			TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.SkillTestProjectileTriple)); // the projectile test skill triple
			TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.SkillTestProjectileAllAround)); // aura
			TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.JumpShort)); // jump
			TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.ChainSkill)); // chain skill
			TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.SkillAreaExplode)); // bomb skill
			TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.MissileProjectile)); // missile projectile
			TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.ChainedProjectile)); // chained projectile

			SetMeleeAttackSkill((ActiveSkill) SkillTable.Instance.GetSkill(SkillId.MeleeAttack));
		}

		public override ClassId GetClassId()
		{
			return ClassId.Default;
		}
	}
}
