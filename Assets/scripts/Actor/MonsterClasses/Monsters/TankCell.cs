﻿// Copyright (c) 2015, Lukas Honke
// ========================
using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.AI;
using Assets.scripts.Skills;
using Assets.scripts.Skills.ActiveSkills;
using Assets.scripts.Skills.Base;

namespace Assets.scripts.Actor.MonsterClasses.Monsters
{
	public class TankCell : MonsterTemplate
	{
		public TankCell()
		{
			Name = "Cell";
			MaxHp = 65;
			MaxMp = 50;
			MaxSpeed = 7;

			IsAggressive = true;
			AggressionRange = 20;
			RambleAround = false;
			AlertsAllies = false;
			XpReward = 5;
		}

		public override void AddSkillsToTemplate()
		{
			TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.CollisionDamageAttack));
		}

		public override void InitSkillsOnMonster(SkillSet set, ActiveSkill meleeSkill, int level)
		{
			CollisionDamageAttack sk = set.GetSkill(SkillId.CollisionDamageAttack) as CollisionDamageAttack;

			if (sk != null)
			{
				sk.baseDamage = 20;
				sk.pushForce = 100;
				sk.reuse = 1.5f;
			}
		}

		public override MonsterAI CreateAI(Character ch)
		{
			MeleeMonsterAI a = new MeleeMonsterAI(ch);
		    return a;
		}

		public override GroupTemplate GetGroupTemplate()
		{
			return null;
		}

		public override MonsterId GetMonsterId()
		{
			return MonsterId.TankCell;
		}
	}

	public class SmallTankCell : MonsterTemplate
	{
		public SmallTankCell()
		{
			MaxHp = 55;
			MaxMp = 50;
			MaxSpeed = 7;

			IsAggressive = true;
			AggressionRange = 20;
			RambleAround = false;
			AlertsAllies = false;
			XpReward = 3;
		}

		public override void AddSkillsToTemplate()
		{
			TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.CollisionDamageAttack));
		}

		public override void InitSkillsOnMonster(SkillSet set, ActiveSkill meleeSkill, int level)
		{
			CollisionDamageAttack sk = set.GetSkill(SkillId.CollisionDamageAttack) as CollisionDamageAttack;

			if (sk != null)
			{
				sk.baseDamage = 20;
				sk.pushForce = 100;
				sk.reuse = 1.5f;
			}
		}

		public override MonsterAI CreateAI(Character ch)
		{
			MeleeMonsterAI a = new MeleeMonsterAI(ch);
			return a;
		}

		public override GroupTemplate GetGroupTemplate()
		{
			return null;
		}

		public override MonsterId GetMonsterId()
		{
			return MonsterId.SmallTankCell;
		}
	}

	public class ArmoredCell : MonsterTemplate
	{
		public ArmoredCell()
		{
			MaxHp = 20;
			MaxMp = 50;
			MaxSpeed = 5;

			IsAggressive = true;
			AggressionRange = 20;
			RambleAround = false;
			AlertsAllies = false;
			XpReward = 4;
		}

		public override void AddSkillsToTemplate()
		{
			TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.CollisionDamageAttack));
			//TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.SkillTestProjectile));
		}

		public override void InitSkillsOnMonster(SkillSet set, ActiveSkill meleeSkill, int level)
		{
			CollisionDamageAttack sk = set.GetSkill(SkillId.CollisionDamageAttack) as CollisionDamageAttack;

			if (sk != null)
			{
				sk.baseDamage = 20;
				sk.pushForce = 100;
				sk.reuse = 1.5f;
			}

			//SkillTestProjectile sk2 = set.GetSkill(SkillId.SkillTestProjectile) as SkillTestProjectile;

			//sk2.reuse = 3f;
		}

		public override MonsterAI CreateAI(Character ch)
		{
			MeleeMonsterAI a = new MeleeMonsterAI(ch);
			return a;
		}

		public override GroupTemplate GetGroupTemplate()
		{
			return null;
		}

		public override MonsterId GetMonsterId()
		{
			return MonsterId.ArmoredCell;
		}
	}
}
