// Copyright (c) 2015, Lukas Honke
// ========================
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.AI.Modules;
using Assets.scripts.Skills;
using Assets.scripts.Skills.Base;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace Assets.scripts.AI
{
	//TODO modularise
	public class SupportMonsterAI : MonsterAI
	{
		public SupportMonsterAI(Character o) : base(o)
		{
		}

		public override void CreateModules()
		{
			//AddAttackModule(new AutoattackModule(this));
		}

		protected override void AttackTarget(Character target)
		{
			//SetMainTarget(target);

			bool isCasting = Owner.GetData().IsCasting;
			bool isMeleeAttacking = Owner.GetData().IsMeleeAttacking();
			//float distSqr = Utils.DistanceObjectsSqr(Owner.GetData().GetBody(), target.GetData().GetBody());
			float distSqr = Utils.DistanceSqr(Owner.GetData().GetBody().transform.position, target.GetData().GetBody().transform.position);
			int hpPercentage = (int)((GetStatus().Hp / (float)GetStatus().MaxHp) * 100);

			if (isCasting)
			{
				ActiveSkill castedSkill = null;

				foreach (ActiveSkill sk in Owner.Status.ActiveSkills)
				{
					if (sk.HasTrait(SkillTraits.BuffDamage) || sk.HasTrait(SkillTraits.BuffDefense))
					{
						castedSkill = sk;
						break;
					}
				}

				if (castedSkill != null)
				{
					GameObject initTarget = castedSkill.InitTarget;
					if (initTarget != null)
					{
						float dist = Utils.DistanceSqr(Owner.GetData().GetBody().transform.position, initTarget.transform.position);

						if (dist > Math.Pow(castedSkill.range, 2))
						{
							castedSkill.AbortCast();
						}
					}
				}
			}

			// already doing something
			if (isCasting || Owner.Status.IsStunned())
				return;

			if (Owner.GetData().Target == null || Owner.GetData().Target.Equals(target.GetData().GetBody()))
				Owner.GetData().Target = target.GetData().GetBody();

			Collider2D[] colls = Physics2D.OverlapCircleAll(Owner.GetData().transform.position, AggressionRange*3);
			List<Character> targets = new List<Character>();

			foreach (Collider2D coll in colls)
			{
				if (coll.gameObject == null) continue;

				Character ch = Utils.GetCharacter(coll.gameObject);

				if (ch != null)
				{
					if (!Owner.CanAttack(ch) && !Owner.Equals(ch) && !ch.IsInteractable())
					{
						targets.Add(ch);
					}
				}
			}

			Character supportTarget = SelectSupportTarget(targets);

			if (supportTarget != null)
			{
				// try spawn skills
				List<Skill> supportSkills = GetAllSkillsWithTraitOr(SkillTraits.BuffDefense, SkillTraits.BuffDamage);
				foreach (Skill s in supportSkills)
				{
					if (s.CanUse())
					{
						Owner.GetData().Target = supportTarget.GetData().GetBody();

						if (StartAction(CastSkill(supportTarget, (ActiveSkill)s, Utils.DistanceSqr(supportTarget.GetData().transform.position, Owner.GetData().GetBody().transform.position), false, true, 0, 0), 0.5f))
							return;
					}
				}

				//TODO add buffdamage skills support
			}

			if (LaunchAttackModule(target, distSqr, hpPercentage))
				return;
		}

		private Character SelectSupportTarget(List<Character> targets)
		{
			CompareByHpRatio comp = new CompareByHpRatio();
			targets.Sort(comp);

			if(targets.Count > 0)
				return targets[0];
			return null;
		}
	}

	public class CompareByHpRatio : Comparer<Character>
	{
		public override int Compare(Character x, Character y)
		{
			float ratioX = x.Status.Hp / (float)x.Status.MaxHp;
			float ratioY = y.Status.Hp / (float)y.Status.MaxHp;

			return ratioX.CompareTo(ratioY);
		}
	}
}
