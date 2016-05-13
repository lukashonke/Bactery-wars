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
	/// set interval to >0 to activate this module
	/// </summary>
	public class RunAwayModule : AIAttackModule
	{
		public float runRange = 8f;
		public float runRangeRandomAdd = 2;

		public float maxRunDistance = 15f;

		public bool panic = false;
		public bool stopWhenTooFar = true;

		public RunAwayModule(MonsterAI ai)
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

			if (Utils.DistanceSqr(thisPos, targetPos) >= (maxRunDistance*maxRunDistance))
			{
				if (stopWhenTooFar)
					return true;
				return false;
			}
			else
			{
				Vector3 runDirection = (thisPos - targetPos).normalized;

				Vector3 nextTarget;

				if (panic)
					nextTarget = Utils.GeneratePerpendicularPositionAround(thisPos, thisPos + (runDirection * runRange), 3f, 7f);
				else
					nextTarget = Utils.GeneratePerpendicularPositionAround(thisPos, (thisPos + (runDirection * (runRange + UnityEngine.Random.Range(-runRangeRandomAdd, runRangeRandomAdd)))), 0f, 2f);

				Debug.DrawLine(thisPos, nextTarget, Color.cyan, 1f);
				if (ai.StartAction(ai.MoveAction(nextTarget, false), 1f))
					return true;

				return false;
			}
		}
	}
}
