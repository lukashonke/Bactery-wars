using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Skills;
using Assets.scripts.Skills.Base;

namespace Assets.scripts.AI.Modules
{
	public class LongRangeDamageModule : AIAttackModule
	{
		// -1 to not check
		public float minRange = -1f;

		public LongRangeDamageModule(MonsterAI ai, float minRange=-1f) : base(ai)
		{
			this.minRange = minRange;
		}

		public override void Init()
		{
			available = ai.GetAllSkillsWithTrait(SkillTraits.LongRange, SkillTraits.Damage).Count > 0;
		}

		public override bool Trigger(Character target, float distSqr)
		{
			if (distSqr >= (minRange*minRange))
			{
				ActiveSkill dmg = (ActiveSkill)ai.GetSkillWithTrait(SkillTraits.Damage, SkillTraits.LongRange);
				if (dmg != null && dmg.CanUse() && !ai.Owner.GetData().forcedVelocity)
				{
					if (ai.StartAction(ai.CastSkill(target, dmg, distSqr, true, false, 0f, 0f), 0.5f))
					{
						return true;
					}
				}
			}

			return false;
		}
	}
}
