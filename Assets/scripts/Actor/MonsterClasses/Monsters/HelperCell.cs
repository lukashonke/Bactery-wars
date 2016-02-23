using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.AI;
using Assets.scripts.Skills;
using Assets.scripts.Skills.ActiveSkills;
using Assets.scripts.Skills.Base;
using Assets.scripts.Skills.SkillEffects;

namespace Assets.scripts.Actor.MonsterClasses.Monsters
{
    public class HelperCell : MonsterTemplate
    {
		public HelperCell()
        {
            MaxHp = 5;
            MaxMp = 50;
            MaxSpeed = 5;

            IsAggressive = true;
            AggressionRange = 20;
            RambleAround = false;
        }

        protected override void AddSkillsToTemplate()
        {
            TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.SkillTestProjectile));
        }

	    public override void InitSkillsOnMonster(SkillSet set, ActiveSkill meleeSkill, int level)
	    {
			SkillTestProjectile skill = set.GetSkill(SkillId.SkillTestProjectile) as SkillTestProjectile;
		    skill.range = 8;
		    skill.castTime = 1f;
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
            return MonsterId.HelperCell;
        }
    }

	public class NonaggressiveHelperCell : MonsterTemplate
	{
		public NonaggressiveHelperCell()
		{
			MaxHp = 5;
			MaxMp = 50;
			MaxSpeed = 5;

			IsAggressive = false;
			AggressionRange = 20;
			RambleAround = true;
		}

		protected override void AddSkillsToTemplate()
		{
			TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.SkillTestProjectile));
		}

		public override void InitMonsterStats(Monster m, int level)
		{
			m.UpdateMaxHp(m.Status.MaxHp + 10 * (level-1));
		}

		public override void InitSkillsOnMonster(SkillSet set, ActiveSkill meleeSkill, int level)
		{
			SkillTestProjectile skill = set.GetSkill(SkillId.SkillTestProjectile) as SkillTestProjectile;
			skill.range = 12;
			skill.castTime = 1f;
			skill.reuse = 0.5f;
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
			return MonsterId.NonaggressiveHelperCell;
		}
	}

	public class FloatingHelperCell : MonsterTemplate
	{
		public FloatingHelperCell()
		{
			MaxHp = 10;
			MaxMp = 50;
			MaxSpeed = 10;

			IsAggressive = true;
			AggressionRange = 10;
			RambleAround = true;
		}

		protected override void AddSkillsToTemplate()
		{
			TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.SkillTestProjectile));
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
			//ai.IsAggressive = false;
			ai.evadeInterval = -1;
			ai.evadeChance = -1;

			ai.floatInterval = 0.5f;
			ai.floatChance = 100;
			ai.floatSpeed = 4;
			ai.floatRange = 10;

			ai.shootWhileMoving = true;
			return ai;
		}

		public override GroupTemplate GetGroupTemplate()
		{
			return null;
		}

		public override MonsterId GetMonsterId()
		{
			return MonsterId.FloatingHelperCell;
		}
	}

	public class DementCell : MonsterTemplate
	{
		public DementCell()
		{
			MaxHp = 5;
			MaxMp = 50;
			MaxSpeed = 20;

			IsAggressive = false;
			AggressionRange = 20;
			RambleAround = true;
			RambleAroundMaxDist = 20;
		}

		protected override void AddSkillsToTemplate()
		{
			TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.CollisionDamageAttack));
		}

		public override void InitSkillsOnMonster(SkillSet set, ActiveSkill meleeSkill, int level)
		{
			CollisionDamageAttack skill = set.GetSkill(SkillId.CollisionDamageAttack) as CollisionDamageAttack;
			skill.AddAdditionalEffect(new EffectKillSelf());
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

	public class PassiveHelperCell : MonsterTemplate
	{
		public PassiveHelperCell()
		{
			MaxHp = 10;
			MaxMp = 50;
			MaxSpeed = 10;

			IsAggressive = true;
			AggressionRange = 15;
			RambleAround = true;
		}

		protected override void AddSkillsToTemplate()
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
			MaxHp = 50;
			MaxMp = 50;
			MaxSpeed = 4;

			IsAggressive = false;
			AggressionRange = 30;
			RambleAround = true;
		}

		protected override void AddSkillsToTemplate()
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
			MaxHp = 50;
			MaxMp = 50;
			MaxSpeed = 4;

			IsAggressive = false;
			AggressionRange = 30;
			RambleAround = true;
		}

		protected override void AddSkillsToTemplate()
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
