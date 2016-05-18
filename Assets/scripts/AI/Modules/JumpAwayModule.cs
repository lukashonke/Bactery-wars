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
	public class JumpAwayModule : AIAttackModule
	{
		public JumpAwayModule(MonsterAI ai)
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

			Vector3 runDirection = (thisPos - targetPos).normalized;
			Vector3 nextTarget;

			nextTarget = Utils.GeneratePerpendicularPositionAround(thisPos, (thisPos + (runDirection)), 0f, 2f);

			ActiveSkill jump = (ActiveSkill)ai.GetSkillWithTrait(SkillTraits.Jump);

			if (jump.CanUse() && !ai.Owner.GetData().forcedVelocity)
			{
				if (ai.StartAction(ai.CastSkill(nextTarget, jump, distSqr, true, false, 0f, 0f), 0.5f))
				{
					return true;
				}
			}

			return false;
		}
	}
}
