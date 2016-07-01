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

namespace Assets.scripts.AI.Modules
{
	public class DamageSkillModule : AIAttackModule
	{
		// by default, AI will shoot skill if the distance between player and monster is <= 60% of skill's range
		// you tweak the % value by settings this variable: 0.2 will make the AI shoot when distance is <= 60+20%
		public float boostShootRange = 0f;

		// triggers this module only if the the range between this character and range is <= skill's range
		public bool onlyIfWithinRange;
		public bool shootWhileMoving;

		// force skill to cast in this module
		public string skill;

		private List<Skill> skills;

		public DamageSkillModule(MonsterAI ai) : base(ai)
		{
			shootWhileMoving = false;
			onlyIfWithinRange = false;
			skill = null;
		}

		public override void Init()
		{
			skills = ai.GetAllSkillsWithTrait(SkillTraits.Damage);
			canTrigger = skills.Count > 0;
		}

		public override bool Trigger(Character target, float distSqr)
		{
			int topDamage = -1;
			ActiveSkill topSkill = null;

			// forced skill name to use
			if (skill != null)
			{
				foreach (Skill skk in skills)
				{
					ActiveSkill sk = (ActiveSkill)skk;

					if (sk.GetName().Equals(skill, StringComparison.InvariantCultureIgnoreCase))
					{
						if (sk.CanUse() && RangeCheck(sk, distSqr))
						{
							topSkill = sk;
						}

						break;
					}
				}
			}
			// 1. get the most damage skill and cast if it is available
			else
			{
				foreach (Skill skk in skills)
				{
					ActiveSkill sk = (ActiveSkill)skk;

					if (sk.CanUse() && RangeCheck(sk, distSqr) && sk.GetTotalDamageOutput() > topDamage)
					{
						topDamage = sk.GetTotalDamageOutput();
						topSkill = sk;
					}
				}
			}
			
			if (topSkill != null)
			{
				bool rangeCheck = true;
				// range check is already perfomed by this AI
				if (onlyIfWithinRange)
					rangeCheck = false;

				ai.StartAction(ai.CastSkill(target, topSkill, distSqr, !rangeCheck, true, boostShootRange), topSkill.GetSkillActiveDuration() * 2, false, shootWhileMoving);

				return true;
			}

			return false;
		}

		private bool RangeCheck(ActiveSkill skill, float distSqr)
		{
			// ignore range
			if (!onlyIfWithinRange)
				return true;

			// 0 range means infinite range
			if (skill.range == 0 || distSqr <= skill.range * skill.range)
				return true;
			return false;
		}
	}
}
