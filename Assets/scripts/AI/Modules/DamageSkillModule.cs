using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Skills;
using Assets.scripts.Skills.Base;

namespace Assets.scripts.AI.Modules
{
	public class DamageSkillModule : AIAttackModule
	{
		public bool shootWhileMoving;

		public DamageSkillModule(MonsterAI ai) : base(ai)
		{
			shootWhileMoving = false;
		}

		public override void Init()
		{
			available = ai.GetAllSkillsWithTrait(SkillTraits.Damage).Count > 0;
		}

		public override bool Trigger(Character target, float distSqr)
		{
			List<Skill> skills = ai.GetAllSkillsWithTrait(SkillTraits.Damage); //TODO put into cache

			// 1. get the most damage skill and cast if it is available
			int topDamage = -1;
			ActiveSkill topSkill = null;

			foreach (Skill skill in skills)
			{
				ActiveSkill sk = (ActiveSkill)skill;
				if (sk.CanUse() && sk.GetTotalDamageOutput() > topDamage)
				{
					topDamage = sk.GetTotalDamageOutput();
					topSkill = sk;
				}
			}

			if (topSkill != null)
			{
				ai.StartAction(ai.CastSkill(target, topSkill, distSqr, false, true), 10f, false, shootWhileMoving);
				return true;
			}

			return false;
		}
	}
}
