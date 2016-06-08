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
	public class WeakAggroModule : AIAttackModule
	{
		public WeakAggroModule(MonsterAI ai) : base(ai)
		{
		}

		public override void Init()
		{
			canTrigger = true;
		}

		public override bool Trigger(Character target, float distSqr)
		{
			if (distSqr > ai.AggressionRange*ai.AggressionRange)
			{
				ai.RemoveAggro(target);
			}

			return false;
		}
	}
}
