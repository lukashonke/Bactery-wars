using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.AI;
using Assets.scripts.Skills;
using Assets.scripts.Skills.Base;

namespace Assets.scripts.Actor.MonsterClasses
{
	public class NeutrophylePatrol : MonsterTemplate
	{
		public NeutrophylePatrol()
		{
			MaxHp = 100;
			MaxMp = 50;
			MaxSpeed = 10;

			IsAggressive = false;
			AggressionRange = 10;
			RambleAround = true;
		}

		protected override void AddSkillsToTemplate()
		{
			TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.NeutrophileProjectile));
		}

		public override MonsterAI CreateAI(Character ch)
		{
			RangedMonsterAI a = new RangedMonsterAI(ch);
		    a.evadeInterval = 2f;
		    return a;
		}

		public override GroupTemplate GetGroupTemplate()
		{
			return null;
		}

		public override MonsterId GetMonsterId()
		{
			return MonsterId.Neutrophyle_Patrol;
		}
	}
}
