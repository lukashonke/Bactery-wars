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
	public class RambleAroundModule : AIAttackModule
	{
		public float range = 5f;
		public RambleAroundModule(MonsterAI ai) : base(ai)
		{
		}

		public override void Init()
		{
			canTrigger = true;
		}

		public override bool Trigger(Character target, float distSqr)
		{
			Vector3 ownerPos = ai.Owner.GetData().GetBody().transform.position;
			Vector3 nextTarget = Utils.GenerateRandomPositionAround(ownerPos, range);

			if (ai.StartAction(ai.MoveAction(nextTarget, false), 1f))
				return true;

			return false;
		}
	}
}
