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
	/// 
	/// </summary>
	public class JumpRandomDirectionModule : AIAttackModule
	{
		public bool aroundTarget = true;
		public float minDistanceAroundTarget = 5f;
		public float maxDistanceAroundTarget = 10f;

		public JumpRandomDirectionModule(MonsterAI ai)
			: base(ai)
		{
		}

		public override void Init()
		{
			canTrigger = ai.GetAllSkillsWithTrait(SkillTraits.Jump).Count > 0;
		}

		public override bool Trigger(Character target, float distSqr)
		{
			Vector3 thisPos = ai.Owner.GetData().transform.position;
			Vector3 targetPos = target.GetData().GetBody().transform.position;

			Vector3 nextTarget;

			int limit = 5;
			while (--limit > 0)
			{
				if (aroundTarget)
				{
					nextTarget = Utils.GenerateRandomPositionAround(thisPos, targetPos, maxDistanceAroundTarget, minDistanceAroundTarget);
				}
				else
				{
					nextTarget = Utils.GenerateRandomPositionAround(thisPos, thisPos, maxDistanceAroundTarget, minDistanceAroundTarget);
				}

				if (Utils.CanSee(thisPos, targetPos))
				{
					ActiveSkill jump = (ActiveSkill)ai.GetSkillWithTrait(SkillTraits.Jump);

					if (jump.CanUse() && !ai.Owner.GetData().forcedVelocity)
					{
						if (ai.StartAction(ai.CastSkill(nextTarget, jump, distSqr, true, false, 0f, 0f), 0.5f))
						{
							return true;
						}
					}
				}
			}
			
			return false;
		}
	}
}
