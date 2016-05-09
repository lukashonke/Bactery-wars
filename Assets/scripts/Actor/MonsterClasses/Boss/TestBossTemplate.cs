using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.Actor.Status;
using Assets.scripts.AI;
using Assets.scripts.Skills;
using Assets.scripts.Skills.ActiveSkills;
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

		public override void AddSkillsToTemplate()
		{
			TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.ProjectileAllAround));
			TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.CollisionDamageAttack));
		}

		public override void InitSkillsOnMonster(SkillSet set, ActiveSkill meleeSkill, int level)
		{
			ProjectileAllAround sk = set.GetSkill(SkillId.ProjectileAllAround) as ProjectileAllAround;

			sk.baseDamage = 15;
			sk.projectileCount = 12;
			sk.range = 20;
			sk.castTime = 0;
			sk.reuse = 6f;

			CollisionDamageAttack sk2 = set.GetSkill(SkillId.CollisionDamageAttack) as CollisionDamageAttack;

			sk2.baseDamage = 20;
			sk2.pushForce = 100;
			sk2.reuse = 1f;
		}

		public override MonsterAI CreateAI(Character ch)
		{
			TankSpreadshooterAI a = new TankSpreadshooterAI(ch);
		    return a;
		}

		public override void InitMonsterStats(Monster m, int level)
		{
			base.InitMonsterStats(m, level);
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
