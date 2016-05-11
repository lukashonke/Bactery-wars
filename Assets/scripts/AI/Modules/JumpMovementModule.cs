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
	public class JumpMovementModule : AIAttackModule
	{
		public float chanceEveryTick = 100;

		public float minRange = 5f;

		public JumpMovementModule(MonsterAI ai, float chanceEveryTick=0) : base(ai)
		{
			this.chanceEveryTick = chanceEveryTick;
		}

		public override void Init()
		{
			canTrigger = ai.GetAllSkillsWithTrait(SkillTraits.Jump).Count > 0;
		}

		public override bool Trigger(Character target, float distSqr)
		{
			if (chanceEveryTick > 0)
			{
				ActiveSkill jump = (ActiveSkill)ai.GetSkillWithTrait(SkillTraits.Jump);

				if (jump != null && jump.CanUse() && !ai.Owner.GetData().forcedVelocity)
				{
					Vector3 ownerPos = ai.Owner.GetData().GetBody().transform.position;
					Vector3 targetPos = target.GetData().GetBody().transform.position;

					if (UnityEngine.Random.Range(0, 100) < chanceEveryTick)
					{
						if (distSqr > (minRange*minRange))
						{
							Vector3 nextTarget = Utils.GeneratePerpendicularPositionAround(ownerPos, targetPos, 2, 6);
							//Debug.DrawLine(ownerPos, nextTarget, Color.cyan, 1f);

							//if (ai.StartAction(ai.CastSkill(target, jump, distSqr, true, false, 0f, 0f), 0.5f))
							if (ai.StartAction(ai.CastSkill(nextTarget, jump, distSqr, true, false, 0f, 0f), 0.5f))
								return true;
						}
						else
						{
							if (ai.StartAction(ai.CastSkill(targetPos, jump, distSqr, true, false, 0f, 0f), 0.5f))
								return true;
						}
					}
				}
			}

			return false;
		}
	}
}
