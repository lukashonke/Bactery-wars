﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.AI;
using Assets.scripts.Skills;
using Assets.scripts.Skills.Base;

namespace Assets.scripts.Actor.MonsterClasses
{
	public class LeukocyteMelee : MonsterTemplate
	{
		public LeukocyteMelee()
		{
			MaxHp = 20;
			MaxMp = 50;
			MaxSpeed = 10;
		}

		protected override void AddSkills()
		{
			// no skills
			SetMeleeAttackSkill((ActiveSkill) SkillTable.Instance.GetSkill(SkillId.MeleeAttack));
		}

		public override MonsterAI CreateAI(Character ch)
		{
			return new MeleeMonsterAI(ch);
		}

		public override GroupTemplate GetGroupTemplate()
		{
			return null;
		}

		public override MonsterId GetMonsterId()
		{
			return MonsterId.Leukocyte_melee;
		}
	}
}
