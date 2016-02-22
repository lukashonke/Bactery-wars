using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.Actor.Status;
using Assets.scripts.AI;
using Assets.scripts.Skills;
using Assets.scripts.Skills.ActiveSkills;
using Assets.scripts.Skills.Base;
using UnityEngine;

namespace Assets.scripts.Actor.MonsterClasses.Boss
{
	public class TankSpreadshooter : BossTemplate
	{
		public TankSpreadshooter()
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
			TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.SkillTestProjectileAllAround));
			TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.CollisionDamageAttack));
			TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.JumpShort));
		}

		public override void InitSkillsOnMonster(SkillSet set, ActiveSkill meleeSkill, int level)
		{
			SkillTestProjectileAllAround sk = set.GetSkill(SkillId.SkillTestProjectileAllAround) as SkillTestProjectileAllAround;

			sk.baseDamage = 1;
			sk.projectileCount = 12;
			sk.range = 20;
			sk.castTime = 0;
			sk.reuse = 0f;

			CollisionDamageAttack sk2 = set.GetSkill(SkillId.CollisionDamageAttack) as CollisionDamageAttack;

			sk2.baseDamage = 1;
			sk2.pushForce = 100;
			sk2.reuse = 1f;

			JumpShort sk3 = set.GetSkill(SkillId.JumpShort) as JumpShort;

			sk3.jumpSpeed = 45;
			sk3.range = 7;
			sk3.reuse = 0f;
		}

		public override MonsterAI CreateAI(Character ch)
		{
			TankSpreadshooterAI a = new TankSpreadshooterAI(ch);

			a.spreadshootInterval = 5f;
			a.jumpInterval = 1f;

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
			return MonsterId.TankSpreadshooter;
		}
	}
}
