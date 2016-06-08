// Copyright (c) 2015, Lukas Honke
// ========================
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
	/// <summary>
	/// simply uses SpawnSkill without setting minions aggro or something
	/// </summary>
	public class SpawnSkillModule : AIAttackModule
	{
		private List<Skill> spawnSkills;
		 
		public SpawnSkillModule(MonsterAI ai) : base(ai)
		{
		}

		public override void Init()
		{
			spawnSkills = ai.GetAllSkillsWithTrait(SkillTraits.SpawnMinion);
			canTrigger = spawnSkills.Count > 0;
		}

		public override bool Trigger(Character target, float distSqr)
		{
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
