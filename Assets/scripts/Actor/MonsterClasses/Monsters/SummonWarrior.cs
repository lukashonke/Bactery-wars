// Copyright (c) 2015, Lukas Honke
// ========================
using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.AI;
using Assets.scripts.Skills;
using Assets.scripts.Skills.Base;

namespace Assets.scripts.Actor.MonsterClasses.Monsters
{
	public class SummonWarrior : MonsterTemplate
	{
		public SummonWarrior()
		{
			MaxHp = 65;
			MaxMp = 50;
			MaxSpeed = 18;

			IsAggressive = true;
			AggressionRange = 15;
		}

		public override void InitSkillsOnMonster(SkillSet set, ActiveSkill meleeSkill, int level)
		{
			if (meleeSkill != null)
			{
				meleeSkill.baseDamage = 5;
				meleeSkill.reuse = 1f;
			}
		}

		public override void AddSkillsToTemplate()
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
			return MonsterId.SummonWarrior;
		}
	}
}
