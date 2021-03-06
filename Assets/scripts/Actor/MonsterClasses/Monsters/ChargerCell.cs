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
    public class ChargerCell : MonsterTemplate
    {
		public ChargerCell()
        {
			Name = "Cell";
			MaxHp = 20;
            MaxMp = 50;
            MaxSpeed = 1;

            IsAggressive = true;
            AggressionRange = 20;
            RambleAround = true;
			XpReward = 3;
        }

	    public override void AddSkillsToTemplate()
        {
            TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.CollisionDamageAttack));
			TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.JumpShort));
        }

	    public override void InitSkillsOnMonster(SkillSet set, ActiveSkill meleeSkill, int level)
	    {
		    JumpShort skill = set.GetSkill(SkillId.JumpShort) as JumpShort;

			if (skill != null)
			{
				skill.jumpSpeed = 30;
				skill.range = 20;
				skill.reuse = 5f;
				skill.castTime = 1f;
			}

			CollisionDamageAttack skill2 = set.GetSkill(SkillId.CollisionDamageAttack) as CollisionDamageAttack;

			if (skill2 != null)
			{
				skill2.pushForce = 100;
			}

	    }

	    public override MonsterAI CreateAI(Character ch)
        {
			MeleeMonsterAI ai = new MeleeMonsterAI(ch);
		    ai.AddAttackModule(new JumpSkillModule(ai));
            return ai;
        }

        public override GroupTemplate GetGroupTemplate()
        {
            return null;
        }

        public override MonsterId GetMonsterId()
        {
            return MonsterId.ChargerCell;
        }
    }
}
