﻿using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.AI;
using Assets.scripts.Skills;
using Assets.scripts.Skills.ActiveSkills;
using Assets.scripts.Skills.Base;

namespace Assets.scripts.Actor.MonsterClasses.Monsters
{
	public class TurretCell : MonsterTemplate
	{
		public TurretCell()
		{
			MaxHp = 20;
			MaxMp = 50;
			MaxSpeed = 0;

			IsAggressive = true;
			AggressionRange = 15;
			RambleAround = false;
			AlertsAllies = false;
		}

		protected override void AddSkillsToTemplate()
		{
			TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.SkillTestProjectile));
		}

		public override void InitSkillsOnMonster(SkillSet set, ActiveSkill meleeSkill, int level)
		{
			SkillTestProjectile sk = set.GetSkill(SkillId.SkillTestProjectile) as SkillTestProjectile;

			sk.castTime = 0.5f;
			sk.range = AggressionRange;
		}

		public override MonsterAI CreateAI(Character ch)
		{
			ImmobileMonsterAI a = new ImmobileMonsterAI(ch);
			a.loseInterestWhenOuttaRange = true;
			return a;
		}

		public override GroupTemplate GetGroupTemplate()
		{
			return null;
		}

		public override MonsterId GetMonsterId()
		{
			return MonsterId.TurretCell;
		}
	}

	public class MissileTurretCell : MonsterTemplate
	{
		public MissileTurretCell()
		{
			MaxHp = 50;
			MaxMp = 50;
			MaxSpeed = 0;

			IsAggressive = true;
			AggressionRange = 15;
			RambleAround = false;
			AlertsAllies = false;
		}

		protected override void AddSkillsToTemplate()
		{
			TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.MissileProjectile));
		}

		public override void InitSkillsOnMonster(SkillSet set, ActiveSkill meleeSkill, int level)
		{
			MissileProjectile sk = set.GetSkill(SkillId.MissileProjectile) as MissileProjectile;

			sk.range = AggressionRange*2;
		}

		public override MonsterAI CreateAI(Character ch)
		{
			ImmobileMonsterAI a = new ImmobileMonsterAI(ch);
			a.loseInterestWhenOuttaRange = true;
		    return a;
		}

		public override GroupTemplate GetGroupTemplate()
		{
			return null;
		}

		public override MonsterId GetMonsterId()
		{
			return MonsterId.MissileTurretCell;
		}
	}
}
