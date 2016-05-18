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
	/// <summary>
	/// 
	/// </summary>
	public class KeepDistanceModule : AIAttackModule
	{
		public float distance = 10f;
		public float tolerance = 3f;

		public KeepDistanceModule(MonsterAI ai)
			: base(ai)
		{
		}

		public override void Init()
		{
			canTrigger = true;
		}

		public override bool Trigger(Character target, float distSqr)
		{
			Vector3 thisPos = ai.Owner.GetData().transform.position;
			Vector3 targetPos = target.GetData().GetBody().transform.position;
			float currentDistSqr = Utils.DistanceSqr(thisPos, targetPos);
			float distToKeep = distance*distance;

			// too far - dont trigger
			if (currentDistSqr > distToKeep)
				return false;

			float delta = distToKeep - currentDistSqr;

			if (delta > tolerance * tolerance)
			{
				float distToRun = Mathf.Sqrt(delta);

				Vector3 runDirection = (thisPos - targetPos).normalized;
				Vector3 nextTarget;

				nextTarget = Utils.GeneratePerpendicularPositionAround(thisPos, (thisPos + (runDirection * distToRun)), 0f, 1f);

				if (ai.StartAction(ai.MoveAction(nextTarget, false), 1f))
					return true;

				return false;
			}

			return false;
		}
	}
}
