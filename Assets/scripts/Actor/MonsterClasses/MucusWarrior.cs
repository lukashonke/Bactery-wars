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
	public class MucusWarrior : MonsterTemplate
	{
		public MucusWarrior()
		{
			MaxHp = 20;
			MaxMp = 50;
			MaxSpeed = 10;

			IsAggressive = false;
			AggressionRange = 15;
		}

		protected override void AddSkillsToTemplate()
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
			return MonsterId.MucusWarrior;
		}
	}
}
