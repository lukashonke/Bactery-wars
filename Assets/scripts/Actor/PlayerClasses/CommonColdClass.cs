using System;
using Assets.scripts.Actor.PlayerClasses.Base;
using Assets.scripts.Skills;
using Assets.scripts.Skills.Base;

namespace Assets.scripts.Actor.PlayerClasses
{
	public class CommonColdClass : ClassTemplate
	{
		public CommonColdClass()
		{
			MaxHp = 100;
			MaxMp = 50;
			MaxSpeed = 30;
			Shield = 1.0f;
			CriticalRate = 0;
			CriticalDamageMul = 2f;
		}

		protected override void AddSkillsToTemplate()
		{
			TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.PoisonShot)); // the projectile test skill
			//TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.Aura)); // the projectile test skill

			//TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.Dodge));
			//TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.ColdPush));
			//TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.CellFury));
			//TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.RhinoBeam));

			SetMeleeAttackSkill((ActiveSkill)SkillTable.Instance.GetSkill(SkillId.CellJump));
		}

		public override ClassId GetClassId()
		{
			return ClassId.CommonCold;
		}
	}
}
