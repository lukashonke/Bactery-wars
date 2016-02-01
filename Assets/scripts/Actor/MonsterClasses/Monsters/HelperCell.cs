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

        public override MonsterAI CreateAI(Character ch)
        {
            MonsterAI ai = new RangedMonsterAI(ch);
            //ai.IsAggressive = false;
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
			return MonsterId.PassiveHelperCell;
		}
	}
}
