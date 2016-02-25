using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.AI;
using Assets.scripts.Skills;
using Assets.scripts.Skills.ActiveSkills;
using Assets.scripts.Skills.Base;

namespace Assets.scripts.Actor.MonsterClasses.Monsters
{
    public class FourDiagShooterCell : MonsterTemplate
    {
		public FourDiagShooterCell()
        {
            MaxHp = 40;
            MaxMp = 50;
            MaxSpeed = 6;

            IsAggressive = true;
            AggressionRange = 10;
            RambleAround = true;
        }

        protected override void AddSkillsToTemplate()
        {
            TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.SkillTestProjectileAllAround));
        }

	    public override void InitSkillsOnMonster(SkillSet set, ActiveSkill meleeSkill, int level)
	    {
			SkillTestProjectileAllAround skill = set.GetSkill(SkillId.SkillTestProjectileAllAround) as SkillTestProjectileAllAround;

		    skill.projectileCount = 4;
		    skill.range = 13;
		    skill.reuse = 3f;
		    skill.castTime = 0.5f;
		    skill.force = 30;
	    }

	    public override MonsterAI CreateAI(Character ch)
        {
			RangedMonsterAI ai = new RangedMonsterAI(ch);
		    ai.evadeChance = 50;
		    ai.evadeInterval = 1f;
            return ai;
        }

        public override GroupTemplate GetGroupTemplate()
        {
            return null;
        }

        public override MonsterId GetMonsterId()
        {
            return MonsterId.FourDiagShooterCell;
        }
    }
}
