﻿// Copyright (c) 2015, Lukas Honke
// ========================
using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.AI;
using Assets.scripts.AI.Modules;
using Assets.scripts.Skills;
using Assets.scripts.Skills.ActiveSkills;
using Assets.scripts.Skills.Base;

namespace Assets.scripts.Actor.MonsterClasses.Monsters
{
    public class RogueCell : MonsterTemplate
    {
        public RogueCell()
        {
			Name = "Cell";
			MaxHp = 20;
            MaxMp = 50;
            MaxSpeed = 7;

            IsAggressive = true;
            AggressionRange = 10;
            RambleAround = true;
			XpReward = 2;
		}

	    public override void AddSkillsToTemplate()
        {
			SetMeleeAttackSkill((ActiveSkill)SkillTable.Instance.GetSkill(SkillId.MeleeAttack));
			TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.Teleport));
        }

		public override void InitSkillsOnMonster(SkillSet set, ActiveSkill meleeSkill, int level)
		{
			Teleport skill = set.GetSkill(SkillId.Teleport) as Teleport;

			if (skill != null)
			{
				skill.range = 15;
				skill.castTime = 0.5f;
			}

			if (meleeSkill != null)
			{
				((MeleeAttack) meleeSkill).castTime = 0.5f;
				((MeleeAttack) meleeSkill).range = 3;
			}
		}

		public override MonsterAI CreateAI(Character ch)
        {
            MonsterAI ai = new MeleeMonsterAI(ch);
			ai.AddPriorityAttackModule(new TeleportSkillModule(ai));
            return ai;
        }

        public override GroupTemplate GetGroupTemplate()
        {
            return null;
        }

        public override MonsterId GetMonsterId()
        {
            return MonsterId.RogueCell;
        }
    }
}
