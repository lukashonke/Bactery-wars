// Copyright (c) 2015, Lukas Honke
// ========================
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Skills;
using Assets.scripts.Skills.Base;
using UnityEngine;
using Random = System.Random;

namespace Assets.scripts.AI.Modules
{
	public class SelfBuffSkillModule : AIAttackModule
	{
		public int minHpPercent = -1;

		private List<Skill> buffSkills; 

		public SelfBuffSkillModule(MonsterAI ai, int minHpPercent=-1) : base(ai)
		{
			this.minHpPercent = minHpPercent;
		}

		public override void Init()
		{
			buffSkills = ai.GetAllSkillsWithTrait(SkillTraits.BuffDamage, SkillTraits.BuffDefense);
			canTrigger = buffSkills.Count > 0;
		}

		public override bool Trigger(Character target, float distSqr)
		{
			if (minHpPercent < 0 || GetHpPercentage() <= minHpPercent)
			{
				foreach (Skill s in buffSkills)
				{
					if (s.CanUse())
					{
						ai.StartAction(ai.CastSkill(ai.Owner, (ActiveSkill)s, 0, true, false, 0, 0), 0.5f);
						return true;
					}
				}
			}

			return false;
		}
	}
}
