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
			MaxHp = 70;
			MaxMp = 50;
			MaxSpeed = 10;
			Shield = 1.0f;
			CriticalRate = 0;
			CriticalDamageMul = 2f;
		}

		protected override void AddSkillsToTemplate()
		{
			TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.SneezeShot)); // the projectile test skill
			TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.Dodge));
			TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.CellFury));
			TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.MucusWarrior));
			TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.RhinoBeam));

			SetMeleeAttackSkill((ActiveSkill)SkillTable.Instance.GetSkill(SkillId.MeleeAttack));
		}

		public override ClassId GetClassId()
		{
			return ClassId.CommonCold;
		}
	}
}
