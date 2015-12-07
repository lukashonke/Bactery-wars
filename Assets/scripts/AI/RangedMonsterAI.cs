using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Skills;
using Assets.scripts.Skills.Base;

namespace Assets.scripts.AI
{
	public class RangedMonsterAI : MonsterAI
	{
		public RangedMonsterAI(Character o) : base(o)
		{
		}

		protected override void AttackTarget(Character target)
		{
			SetMainTarget(target);

			bool isCasting = Owner.GetData().IsCasting;
			bool isMeleeAttacking = Owner.GetData().IsMeleeAttacking();
			float dist = Utils.DistancePwr(target.GetData().transform.position, Owner.GetData().transform.position);

			// already doing something
			if (isCasting || currentAction != null)
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
			else
				Owner.GetData().MeleeAttack(target.GetData().GetBody(), true);
		}
	}
}
