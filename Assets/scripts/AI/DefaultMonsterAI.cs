using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Skills;
using Assets.scripts.Skills.Base;
using UnityEngine;
using Random = System.Random;

namespace Assets.scripts.AI
{
	public class DefaultMonsterAI : MonsterAI
	{
		protected readonly int SHORT_RANGE = 6;
		protected readonly int LONG_RANGE = 10;
		protected int MELEE_ATTACK_RATE = 50;
		protected int LOW_HP_DETERMINER = 50;

		public DefaultMonsterAI(Character o) : base(o)
		{

		}

		protected virtual bool IsLowHp(int hpPercent)
		{
			return (LOW_HP_DETERMINER + UnityEngine.Random.Range(-10, 10) >= hpPercent);
		}

		protected override void AttackTarget(Character target)
		{
			SetMainTarget(target);

			bool isCasting = Owner.GetData().IsCasting;
			bool isMeleeAttacking = Owner.GetData().IsMeleeAttacking();

			// already doing something
			if (isCasting || currentAction != null)
			{
				return;
			}

			if (Owner.GetData().Target == null || Owner.GetData().Target.Equals(target.GetData().GetBody()))
			{
				Owner.GetData().Target = target.GetData().GetBody();
			}

			float dist = Utils.DistancePwr(target.GetData().transform.position, Owner.GetData().transform.position);
			bool isImmobilized = Owner.GetData().CanMove();
			int hpPercentage = (int)((GetStatus().Hp / (float)GetStatus().MaxHp) * 100);
			int targetHpPercentage = (int)((target.Status.Hp / (float)target.Status.MaxHp) * 100);
			int targetHp = target.Status.Hp;

			//TODO cast this skill here if in range
			ActiveSkill sk = (ActiveSkill)GetSkillWithTrait(SkillTraits.Damage);

			if (IsLowHp(hpPercentage) && !isMeleeAttacking && dist < 5 * 5)
			{
				StartAction(RunAway(target, 5f, 20), 5f);
				return;
			}

			if (dist > 5 * 5 && UnityEngine.Random.Range(0, 100) < 20)
			{
				ActiveSkill jump = (ActiveSkill)GetSkillWithTrait(SkillTraits.Jump);
				if (jump != null && jump.CanUse())
				{
					StartAction(CastSkill(target, jump, dist, true, false, 0f, 0f), 1f);
					return;
				}
			}

			if (sk != null && sk.CanUse())
			{
				StartAction(CastSkill(target, sk, dist, false, true, 0f, 0f), 5f);
			}
			else
				Owner.GetData().MeleeInterract(target.GetData().GetBody(), false);
		}
	}
}
