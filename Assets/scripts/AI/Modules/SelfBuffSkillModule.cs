﻿using System.Collections.Generic;
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

		public SelfBuffSkillModule(MonsterAI ai) : base(ai)
		{
		}

		public override void Init()
		{
			available = ai.GetAllSkillsWithTrait(SkillTraits.BuffDamage, SkillTraits.BuffDefense).Count > 0;
		}

		public override bool Trigger(Character target, float distSqr)
		{
			if (minHpPercent < 0 || GetHpPercentage() <= minHpPercent)
			{
				List<Skill> buffSkills = ai.GetAllSkillsWithTrait(SkillTraits.BuffDamage, SkillTraits.BuffDefense);
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
