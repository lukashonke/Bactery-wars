using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Skills;
using Assets.scripts.Skills.Base;
using UnityEngine;

namespace Assets.scripts.AI.Modules
{
	public class SpawnSkillModule : AIAttackModule
	{
		public SpawnSkillModule(MonsterAI ai) : base(ai)
		{
		}

		public override void Init()
		{
			available = ai.GetAllSkillsWithTrait(SkillTraits.SpawnMinion).Count > 0;
		}

		public override bool Trigger(Character target, float distSqr)
		{
			List<Skill> spawnSkills = ai.GetAllSkillsWithTrait(SkillTraits.SpawnMinion);
			foreach (Skill s in spawnSkills)
			{
				if (s.CanUse())
				{
					ai.StartAction(ai.CastSkill(null, (ActiveSkill)s, 0, true, false, 0, 0), 0.5f);
					return true;
				}
			}

			return false;
		}
	}
}
