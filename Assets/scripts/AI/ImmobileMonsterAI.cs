using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Skills;
using Assets.scripts.Skills.Base;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace Assets.scripts.AI
{
	public class ImmobileMonsterAI : MonsterAI
	{
		public bool loseInterestWhenOuttaRange = false;

		public ImmobileMonsterAI(Character o) : base(o)
		{
		}

		protected virtual bool IsLowHp(int hpPercent)
		{
			return (30 + UnityEngine.Random.Range(-10, 10) >= hpPercent);
		}

		protected override void AttackTarget(Character target)
		{
			SetMainTarget(target);

			bool isCasting = Owner.GetData().IsCasting;
			bool isMeleeAttacking = Owner.GetData().IsMeleeAttacking();
			float dist = Utils.DistancePwr(target.GetData().transform.position, Owner.GetData().transform.position);
			int hpPercentage = (int)((GetStatus().Hp / (float)GetStatus().MaxHp) * 100);

			if (loseInterestWhenOuttaRange && dist > AggressionRange*AggressionRange)
			{
				RemoveAggro(target);
				return;
			}

			// already doing something
			if (isCasting)
				return;

			if (Owner.GetData().Target == null || Owner.GetData().Target.Equals(target.GetData().GetBody()))
				Owner.GetData().Target = target.GetData().GetBody();

			List<Skill> skills = GetAllSkillsWithTrait(SkillTraits.Damage);

			// 1. get the most damage skill and cast if it is available
			int topDamage = 0;
			ActiveSkill topSkill = null;

			foreach (Skill skill in skills)
			{
				ActiveSkill sk = (ActiveSkill) skill;
				if (sk.CanUse() && sk.GetTotalDamageOutput() > topDamage)
				{
					topDamage = sk.GetTotalDamageOutput();
					topSkill = sk;
				}
			}

			if (topSkill != null)
				StartAction(CastSkill(target, topSkill, dist, false, true, 0, 0), 10f);
		}
	}
}
