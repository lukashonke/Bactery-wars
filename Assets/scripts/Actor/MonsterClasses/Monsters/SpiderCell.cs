// Copyright (c) 2015, Lukas Honke
// ========================
using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.AI;
using Assets.scripts.AI.Modules;
using Assets.scripts.Skills;
using Assets.scripts.Skills.Base;

namespace Assets.scripts.Actor.MonsterClasses.Monsters
{
    public class SpiderCell : MonsterTemplate
    {
		public SpiderCell()
        {
			Name = "Cell";
			MaxHp = 10;
            MaxMp = 50;
            MaxSpeed = 8;

            IsAggressive = true;
            AggressionRange = 10;
            RambleAround = true;
			XpReward = 2;
		}

	    public override void AddSkillsToTemplate()
        {
            TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.CollisionDamageAttack));
			//TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.JumpShort));
        }

	    public override MonsterAI CreateAI(Character ch)
        {
			MeleeMonsterAI ai = new MeleeMonsterAI(ch);
		    ai.AddPriorityAttackModule(new EvasiveMovementModule(ai));
		    ai.GetAttackModule<EvasiveMovementModule>().chanceToEvade = 75;
	        //ai.dodgeRate = 75;
            return ai;
        }

        public override GroupTemplate GetGroupTemplate()
        {
            return null;
        }

        public override MonsterId GetMonsterId()
        {
            return MonsterId.SpiderCell;
        }
    }
}
