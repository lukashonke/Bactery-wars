using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.AI;
using Assets.scripts.Skills;
using Assets.scripts.Skills.ActiveSkills;
using Assets.scripts.Skills.Base;

namespace Assets.scripts.Actor.MonsterClasses.Monsters
{
    public class SlowerCell : MonsterTemplate
    {
        public SlowerCell()
        {
            MaxHp = 20;
            MaxMp = 50;
            MaxSpeed = 10;

            IsAggressive = true;
            AggressionRange = 10;
            RambleAround = true;
			XpReward = 2;
		}

        protected override void AddSkillsToTemplate()
        {
            TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.SlowBeam));
        }

		public override void InitSkillsOnMonster(SkillSet set, ActiveSkill meleeSkill, int level)
		{
			SlowBeam skill = set.GetSkill(SkillId.SlowBeam) as SlowBeam;
			//skill.AddAdditionalEffect(new EffectKillSelf());
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
            return MonsterId.SlowerCell;
        }
    }
}
