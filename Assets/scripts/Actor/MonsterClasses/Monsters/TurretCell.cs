// Copyright (c) 2015, Lukas Honke
// ========================
using System.Runtime.InteropServices;
using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.AI;
using Assets.scripts.AI.Modules;
using Assets.scripts.Skills;
using Assets.scripts.Skills.ActiveSkills;
using Assets.scripts.Skills.Base;

namespace Assets.scripts.Actor.MonsterClasses.Monsters
{
	public class TurretCell : MonsterTemplate
	{
		public TurretCell()
		{
			Name = "Cell";
			MaxHp = 20;
			MaxMp = 50;
			MaxSpeed = 0;

			IsAggressive = true;
			AggressionRange = 15;
			RambleAround = false;
			AlertsAllies = false;
			XpReward = 3;
		}

		public override void AddSkillsToTemplate()
		{
			TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.Projectile));
		}

		public override void InitSkillsOnMonster(SkillSet set, ActiveSkill meleeSkill, int level)
		{
			Projectile sk = set.GetSkill(SkillId.Projectile) as Projectile;

			if (sk != null)
			{
				sk.castTime = 0.5f;
				sk.range = AggressionRange;
			}
		}

		public override void InitMonsterStats(Monster m, int level)
		{
			base.InitMonsterStats(m, level);

			if (level == 2)
				m.UpdateMaxHp(m.Status.MaxHp + 10);
			else
			{
				m.UpdateMaxHp(m.Status.MaxHp + 10 * (level - 1));
			}
		}

		public override MonsterAI CreateAI(Character ch)
		{
			ImmobileMonsterAI a = new ImmobileMonsterAI(ch);
			a.AddPriorityAttackModule(new WeakAggroModule(a));
			a.GetAttackModule<WeakAggroModule>().enabled = true;
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
			XpReward = 5;
		}

		public override void AddSkillsToTemplate()
		{
			TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.MissileProjectile));
		}

		public override void InitSkillsOnMonster(SkillSet set, ActiveSkill meleeSkill, int level)
		{
			MissileProjectile sk = set.GetSkill(SkillId.MissileProjectile) as MissileProjectile;

			if (sk != null)
			{
				sk.range = AggressionRange*2;
			}
		}

		public override MonsterAI CreateAI(Character ch)
		{
			ImmobileMonsterAI a = new ImmobileMonsterAI(ch);
			a.AddPriorityAttackModule(new WeakAggroModule(a));
			a.GetAttackModule<WeakAggroModule>().enabled = true;
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
