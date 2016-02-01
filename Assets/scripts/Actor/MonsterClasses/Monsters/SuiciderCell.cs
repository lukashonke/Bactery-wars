using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.AI;
using Assets.scripts.Skills;
using Assets.scripts.Skills.ActiveSkills;
using Assets.scripts.Skills.Base;
using Assets.scripts.Skills.SkillEffects;

namespace Assets.scripts.Actor.MonsterClasses.Monsters
{
    public class SuiciderCell : MonsterTemplate
    {
		public SuiciderCell()
        {
            MaxHp = 25;
            MaxMp = 50;
            MaxSpeed = 6;

            IsAggressive = true;
            AggressionRange = 10;
            RambleAround = true;
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
			MeleeMonsterAI ai = new MeleeMonsterAI(ch);
            return ai;
        }

        public override GroupTemplate GetGroupTemplate()
        {
            return null;
        }

        public override MonsterId GetMonsterId()
        {
            return MonsterId.SuiciderCell;
        }
    }
}
