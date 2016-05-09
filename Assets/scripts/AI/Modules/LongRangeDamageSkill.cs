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
		public LongRangeDamageModule(MonsterAI ai) : base(ai)
		{
		}

		public override void Init()
		{
			available = ai.GetAllSkillsWithTrait(SkillTraits.LongRange, SkillTraits.Damage).Count > 0;
		}

		public override bool Trigger(Character target, float distSqr)
		{
			ActiveSkill dmg = (ActiveSkill)ai.GetSkillWithTrait(SkillTraits.Damage, SkillTraits.LongRange);
			if (dmg != null && dmg.CanUse() && !ai.Owner.GetData().forcedVelocity)
			{
				if (ai.StartAction(ai.CastSkill(target, dmg, distSqr, true, false, 0f, 0f), 0.5f))
				{
					return true;
				}
			}

			return false;
		}
	}
}
