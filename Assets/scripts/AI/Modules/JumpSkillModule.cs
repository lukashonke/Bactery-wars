using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Skills;
using Assets.scripts.Skills.Base;

namespace Assets.scripts.AI.Modules
{
	public class JumpSkillModule : AIAttackModule
	{
		// -1 to not check ranges
		public float minRangeToJump = -1;
		public float maxRangeToJump = -1;

		public JumpSkillModule(MonsterAI ai) : base(ai)
		{
		}

		public override void Init()
		{
			canTrigger = ai.GetAllSkillsWithTrait(SkillTraits.Jump).Count > 0;
		}

		public override bool Trigger(Character target, float distSqr)
		{
			if (minRangeToJump < 0 || distSqr >= (minRangeToJump*minRangeToJump))
			{
				if (maxRangeToJump < 0 || distSqr <= (maxRangeToJump*maxRangeToJump))
				{
					ActiveSkill jump = (ActiveSkill)ai.GetSkillWithTrait(SkillTraits.Jump);

					if (jump != null && jump.CanUse() && !ai.Owner.GetData().forcedVelocity)
					{
						if (ai.StartAction(ai.CastSkill(target, jump, distSqr, true, false, 0f, 0f), 0.5f))
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
