// Copyright (c) 2015, Lukas Honke
// ========================
using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.AI;
using Assets.scripts.Skills;
using Assets.scripts.Skills.ActiveSkills;
using Assets.scripts.Skills.Base;

namespace Assets.scripts.Actor.MonsterClasses.Monsters
{
	public class SniperCell : MonsterTemplate
	{
		public SniperCell()
		{
			Name = "Cell";
			MaxHp = 20;
			MaxMp = 50;
			MaxSpeed = 10;

			IsAggressive = true;
			AggressionRange = 30;
			RambleAround = true;
			XpReward = 2;
		}

		public override void AddSkillsToTemplate()
		{
			TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.PushbackProjectile));
		}

		public override void InitSkillsOnMonster(SkillSet set, ActiveSkill meleeSkill, int level)
		{
			PushbackProjectile sk = set.GetSkill(SkillId.PushbackProjectile) as PushbackProjectile;

			if (sk != null)
			{
				sk.range = 0; // infinite range
			}
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
			return MonsterId.SniperCell;
		}
	}
}
