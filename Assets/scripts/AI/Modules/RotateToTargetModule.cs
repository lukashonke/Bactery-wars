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
	public class RotateToTargetModule : AIAttackModule
	{
		public RotateToTargetModule(MonsterAI ai)
			: base(ai)
		{
		}

		public override void Init()
		{
			canTrigger = ai.GetTemplate().TargetRotationSpeed > 0;
		}

		public override bool Trigger(Character target, float distSqr)
		{
			//if(ai.GetTemplate().TargetRotationSpeed  > 0)
				ai.Owner.GetData().Rotate(target.GetData().GetBody().transform.position, ai.GetTemplate().TargetRotationSpeed);
			//else
			//	ai.RotateToTarget(target);
			return true;
		}
	}
}
