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

		private bool hasSpawnSkills = false;
		private bool hasJumpSkill = false;
		private bool hasLongRangeDmgSkill = false;
		private bool hasTeleportSkill = false;

		public MeleeMonsterAI(Character o) : base(o)
		{
		}

		public override void AnalyzeSkills()
		{
			hasSpawnSkills = GetAllSkillsWithTrait(SkillTraits.SpawnMinion).Count > 0;
			hasJumpSkill = GetSkillWithTrait(SkillTraits.Jump) != null;
			hasLongRangeDmgSkill = GetSkillWithTrait(SkillTraits.Damage, SkillTraits.LongRange) != null;
			hasTeleportSkill = GetSkillWithTrait(SkillTraits.Teleport) != null;
		}

		protected override void AttackTarget(Character target)
		{
			SetMainTarget(target);

			bool isCasting = Owner.GetData().IsCasting;
			bool isMeleeAttacking = Owner.GetData().IsMeleeAttacking();
			bool forcedVelocity = Owner.GetData().forcedVelocity;

			// already doing something
			if (isCasting || forcedVelocity || currentAction != null || Owner.Status.IsStunned())
				return;

			if (hasSpawnSkills)
			{
				List<Skill> spawnSkills = GetAllSkillsWithTrait(SkillTraits.SpawnMinion);
				foreach (Skill s in spawnSkills)
				{
					if (s.CanUse())
						StartAction(CastSkill(null, (ActiveSkill)s, 0, true, false, 0, 0), 0.5f);
				}
			}

			if (Owner.GetData().Target == null || Owner.GetData().Target.Equals(target.GetData().GetBody()))
				Owner.GetData().Target = target.GetData().GetBody();

			Vector3 ownerPos = Owner.GetData().GetBody().transform.position;
			Vector3 targetPos = target.GetData().GetBody().transform.position;
			float distSqr = Utils.DistanceSqr(ownerPos, targetPos);

			if (hasJumpSkill)
			{
				ActiveSkill jump = (ActiveSkill)GetSkillWithTrait(SkillTraits.Jump);
				if (jump != null && jump.CanUse() && !Owner.GetData().forcedVelocity)
				{
					if (StartAction(CastSkill(target, jump, distSqr, true, false, 0f, 0f), 0.5f))
						return;
				}
			}

			if (hasTeleportSkill)
			{
				ActiveSkill teleport = (ActiveSkill)GetSkillWithTrait(SkillTraits.Teleport);
				// if range too high, teleport
				if (teleport.CanUse() && !Owner.GetData().forcedVelocity && distSqr >= (teleport.range*teleport.range)/3f && distSqr <= (teleport.range*teleport.range))
				{
					if (StartAction(CastSkill(target, teleport, distSqr, true, false, 0f, 0f), 0.5f))
						return;
				}
			}

			if (hasLongRangeDmgSkill)
			{
				ActiveSkill dmg = (ActiveSkill)GetSkillWithTrait(SkillTraits.Damage, SkillTraits.LongRange);
				if (dmg != null && dmg.CanUse() && !Owner.GetData().forcedVelocity)
				{
					if (StartAction(CastSkill(target, dmg, distSqr, true, false, 0f, 0f), 0.5f))
						return;
				}
			}

			if (dodgeRate > 0 && distSqr > 3*3)
			{
				if (Random.Range(0, 100) < dodgeRate)
				{
					Vector3 nextTarget;
					nextTarget = Utils.GenerateRandomPositionOnCircle(targetPos, (Mathf.Sqrt(distSqr)/2f));
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
