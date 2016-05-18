using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.AI;
using Assets.scripts.Skills;
using Assets.scripts.Skills.Base;

namespace Assets.scripts.Actor.MonsterClasses.Monsters
{
    public class LymfocyteRanged : MonsterTemplate
    {
        public LymfocyteRanged()
        {
			Name = "Cell";
			MaxHp = 20;
            MaxMp = 50;
            MaxSpeed = 10;

            IsAggressive = true;
            AggressionRange = 10;
            RambleAround = true;
			XpReward = 2;
		}

	    public override void AddSkillsToTemplate()
        {
            TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.PushbackProjectile));
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
            return MonsterId.Lymfocyte_ranged;
        }
    }

	public class LymfocyteMelee : MonsterTemplate
	{
        public LymfocyteMelee()
		{
			Name = "Cell";
			MaxHp = 20;
			MaxMp = 50;
			MaxSpeed = 10;

			IsAggressive = true;
			AggressionRange = 5;
			RambleAround = true;
			XpReward = 2;
		}

		public override void AddSkillsToTemplate()
		{
			SetMeleeAttackSkill((ActiveSkill) SkillTable.Instance.GetSkill(SkillId.MeleeAttack));
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
			return MonsterId.Lymfocyte_melee;
		}
	}

	public class DurableMeleeCell : MonsterTemplate
	{
		public DurableMeleeCell()
		{
			Name = "Cell";
			MaxHp = 50;
			MaxMp = 50;
			MaxSpeed = 10;

			IsAggressive = true;
			AggressionRange = 12;
			RambleAround = false;
			XpReward = 5;
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
			return MonsterId.DurableMeleeCell;
		}
	}

	public class SwarmerMeleeCell : MonsterTemplate
	{
		public SwarmerMeleeCell()
		{
			Name = "Cell";
			MaxHp = 40;
			MaxMp = 50;
			MaxSpeed = 10;

			IsAggressive = true;
			AggressionRange = 12;
			RambleAround = false;
			XpReward = 0;
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
			return MonsterId.SwarmerMeleeCell;
		}
	}
}
