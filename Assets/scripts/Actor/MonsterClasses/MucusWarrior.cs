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
			MaxHp = 30;
			MaxMp = 50;
			MaxSpeed = 12;

			IsAggressive = false;
			AggressionRange = 15;
		}

		public override void InitSkillsOnMonster(SkillSet set, ActiveSkill meleeSkill)
		{
			meleeSkill.baseDamage = 5;
			meleeSkill.reuse = 2f;
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
