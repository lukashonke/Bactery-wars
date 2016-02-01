using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.Actor.Status;
using Assets.scripts.AI;
using Assets.scripts.Skills.Base;
using UnityEngine;

namespace Assets.scripts.Actor.MonsterClasses.Boss
{
	public class TestBossTemplate : BossTemplate
	{
		public TestBossTemplate()
		{
			MaxHp = 200;
			MaxMp = 50;
			MaxSpeed = 10;

			IsAggressive = false;
			AggressionRange = 10;
			RambleAround = false;
			AlertsAllies = true;
		}

		protected override void AddSkillsToTemplate()
		{
			TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.NeutrophileProjectile));
			TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.CollisionDamageAttack));
		}

		public override MonsterAI CreateAI(Character ch)
		{
			MeleeMonsterAI a = new MeleeMonsterAI(ch);
		    return a;
		}

		public override void InitMonsterStats(Monster m, MonsterStatus status, int level)
		{
			base.InitMonsterStats(m, status, level);
		}

		public override GroupTemplate GetGroupTemplate()
		{
			return null;
		}

		public override MonsterId GetMonsterId()
		{
			return MonsterId.TestBoss;
		}
	}
}
