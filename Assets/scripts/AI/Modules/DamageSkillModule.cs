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
		// by default, AI will shoot skill if the distance between player and monster is <= 60% of skill's range
		// you tweak the % value by settings this variable: 0.2 will make the AI shoot when distance is <= 60+20%
		public float boostShootRange = 0f;

		public bool shootWhileMoving;

		private List<Skill> skills;

		public DamageSkillModule(MonsterAI ai) : base(ai)
		{
			shootWhileMoving = false;
		}

		public override void Init()
		{
			skills = ai.GetAllSkillsWithTrait(SkillTraits.Damage);
			canTrigger = skills.Count > 0;
		}

		public override bool Trigger(Character target, float distSqr)
		{
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
				ai.StartAction(ai.CastSkill(target, topSkill, distSqr, false, true, boostShootRange), topSkill.GetSkillActiveDuration()*2, false, shootWhileMoving);
				return true;
			}

			return false;
		}
	}
}
