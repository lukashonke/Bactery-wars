using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.AI;
using Assets.scripts.AI.Modules;
using Assets.scripts.Mono.ObjectData;
using Assets.scripts.Skills;
using Assets.scripts.Skills.ActiveSkills;
using Assets.scripts.Skills.Base;
using Assets.scripts.Skills.SkillEffects;
using UnityEngine;

namespace Assets.scripts.Actor.MonsterClasses.Monsters
{
    public class BasicCell : MonsterTemplate
    {
		public BasicCell()
        {
			Name = "Neutrophyle Patrol";
			MaxHp = 5;
            MaxMp = 50;
            MaxSpeed = 5;

			HpLevelScale = 5;

			IsAggressive = true;
            AggressionRange = 20;
            RambleAround = false;
			XpReward = 2;
        }

	    public override void AddSkillsToTemplate()
        {
            TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.Projectile));
        }

		public override void InitMonsterStats(Monster m, int level)
		{
			base.InitMonsterStats(m, level);
		}

		public override void InitSkillsOnMonster(SkillSet set, ActiveSkill meleeSkill, int level)
	    {
			Projectile skill = set.GetSkill(SkillId.Projectile) as Projectile;

			if (skill != null)
			{
				skill.range = 8;
				skill.castTime = 1f;
			}
	    }

	    public override MonsterAI CreateAI(Character ch)
        {
            MonsterAI ai = new RangedMonsterAI(ch);
            return ai;
        }

        public override GroupTemplate GetGroupTemplate()
        {
            return null;
        }

        public override MonsterId GetMonsterId()
        {
            return MonsterId.BasicCell;
        }
    }

	public class CustomCell : MonsterTemplate
	{
		public CustomCell()
		{
			Name = "Blank Cell";
			MaxHp = 20;
			MaxMp = 50;
			MaxSpeed = 5;

			HpLevelScale = 5;

			IsAggressive = false;
			AggressionRange = 20;
			RambleAround = false;
			XpReward = 2;
		}

		public override void InitMonsterStats(Monster m, int level)
		{
			base.InitMonsterStats(m, level);
		}
		
		public override void InitSkillsOnMonster(SkillSet set, ActiveSkill meleeSkill, int level)
		{
		}

		public override void AddSkillsToTemplate()
		{
		}

		public override MonsterAI CreateAI(Character ch)
		{
			MonsterAI ai = new MeleeMonsterAI(ch);
			return ai;
		}

		public override GroupTemplate GetGroupTemplate()
		{
			return null;
		}

		public override MonsterId GetMonsterId()
		{
			return MonsterId.CustomCell;
		}
	}

	public class BasicMeleeCell : MonsterTemplate
	{
		public BasicMeleeCell()
		{
			Name = "Melee Cell";
			MaxHp = 5;
			MaxMp = 50;
			MaxSpeed = 5;

			HpLevelScale = 5;

			IsAggressive = true;
			AggressionRange = 20;
			RambleAround = false;
			XpReward = 2;
		}

		public override void InitMonsterStats(Monster m, int level)
		{
			base.InitMonsterStats(m, level);
		}

		public override void InitSkillsOnMonster(SkillSet set, ActiveSkill meleeSkill, int level)
		{
			MeleeAttack skill = meleeSkill as MeleeAttack;

			if (skill != null)
			{
				skill.castTime = 1f;
			}
		}

		public override void AddSkillsToTemplate()
		{
			SetMeleeAttackSkill((ActiveSkill)SkillTable.Instance.GetSkill(SkillId.MeleeAttack));
		}

		public override MonsterAI CreateAI(Character ch)
		{
			MonsterAI ai = new MeleeMonsterAI(ch);
			return ai;
		}

		public override GroupTemplate GetGroupTemplate()
		{
			return null;
		}

		public override MonsterId GetMonsterId()
		{
			return MonsterId.BasicMeleeCell;
		}
	}

	public class NonaggressiveHelperCell : MonsterTemplate
	{
		public NonaggressiveHelperCell()
		{
			Name = "Cell";
			MaxHp = 5;
			MaxMp = 50;
			MaxSpeed = 5;

			IsAggressive = false;
			AggressionRange = 20;
			RambleAround = true;
			XpReward = 1;
		}

		public override void AddSkillsToTemplate()
		{
			TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.Projectile));
		}

		public override void InitMonsterStats(Monster m, int level)
		{
			base.InitMonsterStats(m, level);
			m.UpdateMaxHp(m.Status.MaxHp + 10 * (level-1));
		}

		public override void InitSkillsOnMonster(SkillSet set, ActiveSkill meleeSkill, int level)
		{
			Projectile skill = set.GetSkill(SkillId.Projectile) as Projectile;

			if (skill != null)
			{
				skill.range = 12;
				skill.castTime = 1f;
				skill.reuse = 0.5f;
			}
		}

		public override MonsterAI CreateAI(Character ch)
		{
			MonsterAI ai = new RangedMonsterAI(ch);
			return ai;
		}

		public override GroupTemplate GetGroupTemplate()
		{
			return null;
		}

		public override MonsterId GetMonsterId()
		{
			return MonsterId.NonaggressiveBasicCell;
		}
	}

	public class FloatingHelperCell : MonsterTemplate
	{
		public FloatingHelperCell()
		{
			Name = "Cell";
			MaxHp = 10;
			MaxMp = 50;
			MaxSpeed = 10;

			IsAggressive = true;
			AggressionRange = 10;
			RambleAround = true;
			XpReward = 3;
		}

		public override void AddSkillsToTemplate()
		{
			TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.Projectile));
		}

		public override void InitSkillsOnMonster(SkillSet set, ActiveSkill meleeSkill, int level)
		{
			/*SkillTestProjectile skill = set.GetSkill(SkillId.SkillTestProjectile) as SkillTestProjectile;
			skill.pushbackForce = 50;
			skill.range = 10;*/
		}

		public override MonsterAI CreateAI(Character ch)
		{
			RangedMonsterAI ai = new RangedMonsterAI(ch);
			ai.AddAttackModule(new FloatModule(ai));

			ai.GetAttackModule<FloatModule>().interval = 0.5f;
			ai.GetAttackModule<FloatModule>().chance = 100;
			ai.GetAttackModule<FloatModule>().floatSpeed = 5;
			ai.GetAttackModule<FloatModule>().floatRange = 10;

			ai.GetAttackModule<DamageSkillModule>().shootWhileMoving = true;
			return ai;
		}

		public override GroupTemplate GetGroupTemplate()
		{
			return null;
		}

		public override MonsterId GetMonsterId()
		{
			return MonsterId.FloatingBasicCell;
		}
	}

	public class DementCell : MonsterTemplate
	{
		public DementCell()
		{
			Name = "Cell";
			MaxHp = 5;
			MaxMp = 50;
			MaxSpeed = 20;

			IsAggressive = false;
			AggressionRange = 20;
			RambleAround = true;
			RambleAroundMaxDist = 20;
			XpReward = 1;
		}

		public override void AddSkillsToTemplate()
		{
			TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.CollisionDamageAttack));
		}

		public override void InitSkillsOnMonster(SkillSet set, ActiveSkill meleeSkill, int level)
		{
			CollisionDamageAttack skill = set.GetSkill(SkillId.CollisionDamageAttack) as CollisionDamageAttack;

			if (skill != null)
			{
				skill.AddAdditionalEffect(new EffectKillSelf());
			}
		}

		public override MonsterAI CreateAI(Character ch)
		{
			MonsterAI ai = new BouncingAI(ch);
			//ai.IsAggressive = false;
			return ai;
		}

		public override GroupTemplate GetGroupTemplate()
		{
			return null;
		}

		public override MonsterId GetMonsterId()
		{
			return MonsterId.DementCell;
		}
	}

	public class BigPassiveFloatingCell : MonsterTemplate
	{
		public BigPassiveFloatingCell()
		{
			Name = "Cell";
			MaxHp = 50;
			MaxMp = 50;
			MaxSpeed = 10;

			IsAggressive = false;
			AggressionRange = 20;
			RambleAround = true;
			RambleAroundMaxDist = 15;
			XpReward = 1;
		}

		public override void AddSkillsToTemplate()
		{
			TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.CollisionDamageAttack));
		}

		public override void InitSkillsOnMonster(SkillSet set, ActiveSkill meleeSkill, int level)
		{
			CollisionDamageAttack skill = set.GetSkill(SkillId.CollisionDamageAttack) as CollisionDamageAttack;

			if (skill != null)
			{

			}
			//skill.AddAdditionalEffect(new EffectKillSelf());
		}

		public override void InitAppearanceData(Monster m, EnemyData data)
		{
			Vector3 current = data.GetBody().transform.localScale;

			data.GetBody().transform.localScale = new Vector3(current.x*(1.1f*(m.Level)), current.y*(1.1f*(m.Level)));
		}

		public override MonsterAI CreateAI(Character ch)
		{
			BouncingAI ai = new BouncingAI(ch);
			//ai.IsAggressive = false;
			return ai;
		}

		public override GroupTemplate GetGroupTemplate()
		{
			return null;
		}

		public override MonsterId GetMonsterId()
		{
			return MonsterId.BigPassiveFloatingCell;
		}
	}

	public class BigPassiveCell : MonsterTemplate
	{
		public BigPassiveCell()
		{
			Name = "Cell";
			MaxHp = 50;
			MaxMp = 50;
			MaxSpeed = 10;

			IsAggressive = false;
			AggressionRange = 20;
			RambleAround = false;
			RambleAroundMaxDist = 15;
			XpReward = 1;
		}

		public override void AddSkillsToTemplate()
		{
			TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.CollisionDamageAttack));
		}

		public override void InitSkillsOnMonster(SkillSet set, ActiveSkill meleeSkill, int level)
		{
			CollisionDamageAttack skill = set.GetSkill(SkillId.CollisionDamageAttack) as CollisionDamageAttack;

			if (skill != null)
			{

			}
			//skill.AddAdditionalEffect(new EffectKillSelf());
		}

		public override void InitAppearanceData(Monster m, EnemyData data)
		{
			Vector3 current = data.GetBody().transform.localScale;

			data.GetBody().transform.localScale = new Vector3(current.x * (1.1f * (m.Level)), current.y * (1.1f * (m.Level)));
		}

		public override MonsterAI CreateAI(Character ch)
		{
			BouncingAI ai = new BouncingAI(ch);
			//ai.IsAggressive = false;
			return ai;
		}

		public override GroupTemplate GetGroupTemplate()
		{
			return null;
		}

		public override MonsterId GetMonsterId()
		{
			return MonsterId.BigPassiveCell;
		}
	}

	public class PassiveHelperCell : MonsterTemplate
	{
		public PassiveHelperCell()
		{
			Name = "Cell";
			MaxHp = 10;
			MaxMp = 50;
			MaxSpeed = 10;

			IsAggressive = true;
			AggressionRange = 15;
			RambleAround = true;
			XpReward = 1;
		}

		public override void AddSkillsToTemplate()
		{
		}

		public override MonsterAI CreateAI(Character ch)
		{
			MonsterAI ai = new CoverMonsterAI(ch);
			//MonsterAI ai = new IdleMonsterAI(ch);
			return ai;
		}

		public override GroupTemplate GetGroupTemplate()
		{
			return null;
		}

		public override MonsterId GetMonsterId()
		{
			return MonsterId.PassiveHelperCell;
		}
	}

	public class ObstacleCell : MonsterTemplate
	{
		public ObstacleCell()
		{
			Name = "Cell";
			MaxHp = 50;
			MaxMp = 50;
			MaxSpeed = 4;

			IsAggressive = false;
			AggressionRange = 30;
			RambleAround = true;
			XpReward = 3;
		}

		public override void AddSkillsToTemplate()
		{
		}

		public override MonsterAI CreateAI(Character ch)
		{
			MonsterAI ai = new CoverMonsterAI(ch);
			return ai;
		}

		public override GroupTemplate GetGroupTemplate()
		{
			return null;
		}

		public override MonsterId GetMonsterId()
		{
			return MonsterId.ObstacleCell;
		}
	}

	public class IdleObstacleCell : MonsterTemplate
	{
		public IdleObstacleCell()
		{
			Name = "Cell";
			MaxHp = 500;
			MaxMp = 50;
			MaxSpeed = 4;

			IsAggressive = false;
			AggressionRange = 30;
			RambleAround = true;
			XpReward = 3;
		}

		public override void AddSkillsToTemplate()
		{
		}

		public override MonsterAI CreateAI(Character ch)
		{
			MonsterAI ai = new IdleMonsterAI(ch);
			return ai;
		}

		public override GroupTemplate GetGroupTemplate()
		{
			return null;
		}

		public override MonsterId GetMonsterId()
		{
			return MonsterId.IdleObstacleCell;
		}
	}
}
