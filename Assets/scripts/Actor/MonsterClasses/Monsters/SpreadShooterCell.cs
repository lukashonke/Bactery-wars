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
    public class FourDiagShooterCell : MonsterTemplate
    {
		public FourDiagShooterCell()
        {
			Name = "Cell";
			MaxHp = 40;
            MaxMp = 50;
            MaxSpeed = 6;

            IsAggressive = true;
            AggressionRange = 10;
            RambleAround = true;
			XpReward = 2;
		}

	    public override void AddSkillsToTemplate()
        {
            TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.ProjectileAllAround));
        }

	    public override void InitSkillsOnMonster(SkillSet set, ActiveSkill meleeSkill, int level)
	    {
			ProjectileAllAround skill = set.GetSkill(SkillId.ProjectileAllAround) as ProjectileAllAround;

			if (skill != null)
			{
				skill.projectileCount = 4;
				skill.range = 13;
				skill.reuse = 3f;
				skill.castTime = 0.5f;
				skill.force = 30;
			}
	    }

	    public override MonsterAI CreateAI(Character ch)
        {
			RangedMonsterAI ai = new RangedMonsterAI(ch);
		    ai.AddAttackModule(new EvadeModule(ai));
		    ai.GetAttackModule<EvadeModule>().chance = 50;
			ai.GetAttackModule<EvadeModule>().interval = 2f;
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
