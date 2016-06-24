// Copyright (c) 2015, Lukas Honke
// ========================
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Skills;
using Assets.scripts.Skills.Base;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace Assets.scripts.AI
{
	public class BouncingMonsterAI : MonsterAI
	{
		public BouncingMonsterAI(Character o) : base(o)
		{
			alwaysActive = true;
			o.GetData().cancelMovementTargetOnCollision = true;
		}

		protected override void AttackTarget(Character target)
		{
			if (State != AIState.ACTIVE)
				SetAIState(AIState.ACTIVE);
		}

		protected override void ThinkActive()
		{
			//if (GetTemplate().RambleAround)
			{
				if (Owner.GetData().HasTargetToMoveTo && Owner.GetData().HasZeroVelocity())
				{
					Owner.GetData().BreakMovement(true);
				}

				if (!Owner.GetData().HasTargetToMoveTo)
				{
					SetIsWalking(false);
					bool found = false;

					int limit = 5;
					while (!found)
					{
						int dist = GetTemplate().RambleAroundMaxDist;
						Vector3 randomPos = Utils.GenerateRandomPositionAround(homeLocation, dist, 1);

						//TODO region check
						//TODO add object collision check on target around, to check if he can fit

						bool collides = false;
						foreach (RaycastHit2D r2d in Physics2D.RaycastAll(Owner.GetData().GetBody().transform.position, randomPos - Owner.GetData().GetBody().transform.position, Vector3.Distance(Owner.GetData().GetBody().transform.position, randomPos)))
						{
							if (r2d.collider == null)
								continue;

							if (r2d.collider.gameObject.Equals(Owner.GetData().GetBody()))
								continue;

							collides = true;
						}

						if (collides)
						{
							//Debug.DrawRay(Owner.GetData().GetBody().transform.position, randomPos - Owner.GetData().GetBody().transform.position, Color.blue, 5);
						}
						else
						{
							found = true;

							MoveTo(randomPos);
							//Debug.DrawRay(Owner.GetData().GetBody().transform.position, randomPos - Owner.GetData().GetBody().transform.position, Color.green, 5);
						}

						limit--;
						if (limit <= 0)
							break;
					}
				}
			}
		}
	}
}
