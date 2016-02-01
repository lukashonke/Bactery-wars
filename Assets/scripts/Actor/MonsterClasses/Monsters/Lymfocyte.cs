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
            MaxHp = 30;
            MaxMp = 50;
            MaxSpeed = 10;

            IsAggressive = false;
            AggressionRange = 30;
            RambleAround = true;
        }

        protected override void AddSkillsToTemplate()
        {
            TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.PushbackProjectile));
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
            return MonsterId.Lymfocyte_ranged;
        }
    }

	public class LymfocyteMelee : MonsterTemplate
	{
        public LymfocyteMelee()
		{
			MaxHp = 40;
			MaxMp = 50;
			MaxSpeed = 10;

			IsAggressive = false;
			AggressionRange = 10;
			RambleAround = true;
		}

		protected override void AddSkillsToTemplate()
		{
			// no skills, only melee
			SetMeleeAttackSkill((ActiveSkill) SkillTable.Instance.GetSkill(SkillId.MeleeAttack));
		}

		public override MonsterAI CreateAI(Character ch)
		{
			MonsterAI ai = new MeleeMonsterAI(ch);
		    //ai.IsAggressive = false; //TODO use this instead of template.IsAggressive
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
}
