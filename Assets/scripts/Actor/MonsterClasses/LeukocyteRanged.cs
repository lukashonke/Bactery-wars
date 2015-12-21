using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.AI;
using Assets.scripts.Skills;
using Assets.scripts.Skills.Base;

namespace Assets.scripts.Actor.MonsterClasses
{
	public class LeukocyteRanged : MonsterTemplate
	{
		public LeukocyteRanged(MonsterId id)
			: base(id)
		{
			MaxHp = 10;
			MaxMp = 50;
			MaxSpeed = 10;
		}

		protected override void AddSkills()
		{
			// no skills
			SetMeleeAttackSkill((ActiveSkill) SkillTable.Instance.GetSkill(10));

			TemplateSkills.Add(SkillTable.Instance.GetSkill(2)); // the projectile test skill
		}

		public override MonsterAI CreateAI(Character ch)
		{
			return new RangedMonsterAI(ch);
		}
	}
}
