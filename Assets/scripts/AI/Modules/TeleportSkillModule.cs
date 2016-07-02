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
	public class TeleportSkillModule : AIAttackModule
	{
		// -1 to not check ranges
		public float minRangeToTeleport = -1;
		public float maxRangeToTeleport = -1;

		public bool checkTeleportSkillRange = true;

		public TeleportSkillModule(MonsterAI ai) : base(ai)
		{
		}

		public override void Init()
		{
			canTrigger = ai.GetAllSkillsWithTrait(SkillTraits.Teleport).Count > 0;
		}

		public override bool Trigger(Character target, float distSqr)
		{
			if (minRangeToTeleport < 0 || distSqr >= (minRangeToTeleport*minRangeToTeleport))
			{
				if (maxRangeToTeleport < 0 || distSqr <= (maxRangeToTeleport*maxRangeToTeleport))
				{
					ActiveSkill teleport = (ActiveSkill)ai.GetSkillWithTrait(SkillTraits.Teleport);
					// if range too high, teleport
					if (teleport.CanUse() && !ai.Owner.GetData().forcedVelocity && (!checkTeleportSkillRange || (distSqr >= ((Math.Pow(teleport.GetRange(), 2)) / 4f) && distSqr <= (Math.Pow(teleport.GetRange(), 2)))))
					{
						if (ai.StartAction(ai.CastSkill(target, teleport, distSqr, true, false, 0f, 0f), 0.5f))
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
