using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.AI;
using Assets.scripts.AI.Modules;
using Assets.scripts.Skills;
using Assets.scripts.Skills.ActiveSkills;
using Assets.scripts.Skills.Base;

namespace Assets.scripts.Actor.MonsterClasses.Monsters
{
    public class JumpCell : MonsterTemplate
    {
		public JumpCell()
        {
			Name = "Decursio Cell";
			MaxHp = 30;
            MaxMp = 50;
            MaxSpeed = 0;

            IsAggressive = true;
            AggressionRange = 10;
            RambleAround = true;
			XpReward = 3;
		}

	    public override void AddSkillsToTemplate()
        {
            TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.CollisionDamageAttack));
			TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.JumpShort));
        }

	    private const int jumpRange = 15;

	    public override void InitSkillsOnMonster(SkillSet set, ActiveSkill meleeSkill, int level)
	    {
		    JumpShort skill = set.GetSkill(SkillId.JumpShort) as JumpShort;

			if (skill != null)
			{
				skill.jumpSpeed = 75;
				skill.range = jumpRange;
				skill.reuse = 4f;
				skill.castTime = 1f;
			}
	    }

	    public override MonsterAI CreateAI(Character ch)
        {
			MeleeMonsterAI ai = new MeleeMonsterAI(ch);
		    ai.AddAttackModule(new JumpMovementModule(ai));
			ai.AddAttackModule(new JumpSkillModule(ai));

		    ai.GetAttackModule<JumpMovementModule>().minRange = jumpRange;
			ai.GetAttackModule<JumpMovementModule>().chanceEveryTick = 100;
            return ai;
        }

        public override GroupTemplate GetGroupTemplate()
        {
            return null;
        }

        public override MonsterId GetMonsterId()
        {
            return MonsterId.JumpCell;
        }
    }
}
