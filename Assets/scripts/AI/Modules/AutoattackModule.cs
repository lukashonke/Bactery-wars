// Copyright (c) 2015, Lukas Honke
// ========================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Skills;
using Assets.scripts.Skills.Base;

namespace Assets.scripts.AI.Modules
{
	public class AutoattackModule : AIAttackModule
	{
		public AutoattackModule(MonsterAI ai) : base(ai)
		{
		}

		public override void Init()
		{
			canTrigger = ai.HasMeleeSkill();
		}

		public override bool Trigger(Character target, float distSqr)
		{
			ai.Owner.GetData().MeleeInterract(target.GetData().GetBody(), true);
			return true;
		}
	}
}
