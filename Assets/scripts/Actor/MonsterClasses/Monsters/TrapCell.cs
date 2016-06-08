// Copyright (c) 2015, Lukas Honke
// ========================
using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.AI;
using Assets.scripts.Skills;
using Assets.scripts.Skills.ActiveSkills;
using Assets.scripts.Skills.Base;
using Assets.scripts.Skills.SkillEffects;

namespace Assets.scripts.Actor.MonsterClasses.Monsters
{
    class TrapCell : MonsterTemplate
    {
		public TrapCell()
        {
			Name = "Trap";
			MaxHp = 1;
            MaxMp = 50;
            MaxSpeed = 0;

            IsAggressive = false;
            AggressionRange = 10;
            RambleAround = false;
			XpReward = 1;
		}

	    public override void AddSkillsToTemplate()
        {
            TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.CollisionDamageAttack));
        }

		public override void InitSkillsOnMonster(SkillSet set, ActiveSkill meleeSkill, int level)
		{
			CollisionDamageAttack skill = set.GetSkill(SkillId.CollisionDamageAttack) as CollisionDamageAttack;

			if (skill != null)
			{
				skill.DisableOriginalEffects();
				skill.AddAdditionalEffect(new EffectKillSelf());
				skill.AddAdditionalEffect(new EffectAuraDamage(20, 5, 10f));
			}
		}

	    public override MonsterAI CreateAI(Character ch)
        {
			ImmobileMonsterAI ai = new ImmobileMonsterAI(ch);
            return ai;
        }

        public override GroupTemplate GetGroupTemplate()
        {
            return null;
        }

        public override MonsterId GetMonsterId()
        {
            return MonsterId.TrapCell;
        }
    }
}
