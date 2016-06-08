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
using Random = System.Random;

namespace Assets.scripts.AI.Modules
{
	/// <summary>
	/// if the distance is correct, the owner of this module will move in an "evasive way"
	/// </summary>
	public class EvasiveMovementModule : AIAttackModule
	{
		// if this chance fails, he will move directly at target
		public float chanceToEvade = 75;

		public float minRange = 3f;

		public EvasiveMovementModule(MonsterAI ai) : base(ai)
		{
		}

		public override void Init()
		{
			canTrigger = ai.GetStatus().MoveSpeed > 0;
		}

		public override bool Trigger(Character target, float distSqr)
		{
			if (chanceToEvade > 0 && distSqr > (minRange*minRange))
			{
				Vector3 ownerPos = ai.Owner.GetData().GetBody().transform.position;
				Vector3 targetPos = target.GetData().GetBody().transform.position;

				if (UnityEngine.Random.Range(0, 100) < chanceToEvade)
				{
					Vector3 nextTarget = Utils.GeneratePerpendicularPositionAround(ownerPos, targetPos, (Mathf.Sqrt(distSqr) / 2f), (Mathf.Sqrt(distSqr) / 2f));
					//Vector3 nextTarget = Utils.GenerateRandomPositionOnCircle(targetPos, (Mathf.Sqrt(distSqr) / 2f));

					Debug.DrawLine(ownerPos, nextTarget, Color.cyan, 1f);

					if(ai.StartAction(ai.MoveAction(nextTarget, false), 1f))
						return true;
				}
				else
				{
					if(ai.StartAction(ai.MoveAction(targetPos, false), 1f))
						return true;
				}
			}

			return false;
		}
	}
}
