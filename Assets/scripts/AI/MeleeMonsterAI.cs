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
	public class MeleeMonsterAI : MonsterAI
	{
		public int dodgeRate = 0;

		public MeleeMonsterAI(Character o) : base(o)
		{
		}

		protected override void AttackTarget(Character target)
		{
			SetMainTarget(target);

			bool isCasting = Owner.GetData().IsCasting;
			bool isMeleeAttacking = Owner.GetData().IsMeleeAttacking();

			// already doing something
			if (isCasting || currentAction != null)
				return;

			List<Skill> spawnSkills = GetAllSkillsWithTrait(SkillTraits.SpawnMinion);
			foreach (Skill s in spawnSkills)
			{
				if (s.CanUse())
					StartAction(CastSkill(null, (ActiveSkill)s, 0, true, false, 0, 0), 0.5f);
			}

			if (Owner.GetData().Target == null || Owner.GetData().Target.Equals(target.GetData().GetBody()))
				Owner.GetData().Target = target.GetData().GetBody();

			Vector3 ownerPos = Owner.GetData().GetBody().transform.position;
			Vector3 targetPos = target.GetData().GetBody().transform.position;
			float dist = Vector3.Distance(ownerPos, targetPos);

			ActiveSkill jump = (ActiveSkill)GetSkillWithTrait(SkillTraits.Jump);
			if (jump != null && jump.CanUse())
			{
				if (StartAction(CastSkill(target, jump, dist, true, false, 0f, 0f), 0.5f))
					return;
			}

			if (dodgeRate > 0 && dist > 3)
			{
				if (Random.Range(0, 100) < dodgeRate)
				{
					Vector3 nextTarget;
					nextTarget = Utils.GenerateRandomPositionOnCircle(targetPos, dist/2);
					Debug.DrawRay(ownerPos, nextTarget, Color.cyan, 1f);

					StartAction(MoveAction(nextTarget, false), 1f);
				}
				else
				{
					StartAction(MoveAction(targetPos, false), 1f);
				}
			}
			else
			{
				if (Owner.MeleeSkill != null)
					Owner.GetData().MeleeInterract(target.GetData().GetBody(), true);
				else
					MoveTo(target);
			}
		}
	}
}
