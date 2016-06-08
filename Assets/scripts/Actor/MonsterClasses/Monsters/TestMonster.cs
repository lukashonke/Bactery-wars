// Copyright (c) 2015, Lukas Honke
// ========================
using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.AI;
using Assets.scripts.Skills;
using Assets.scripts.Skills.Base;

namespace Assets.scripts.Actor.MonsterClasses.Monsters
{
	public class TestMonster : MonsterTemplate
	{
		public TestMonster()
		{
			MaxHp = 20;
			MaxMp = 50;
			MaxSpeed = 10;
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
			return new GroupTemplate().Add(MonsterId.TestMonster, 3);
		}

		public override MonsterId GetMonsterId()
		{
			return MonsterId.TestMonster;
		}
	}
}
