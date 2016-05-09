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

		public float teleportInterval = -1;

		// ==== SELF BUFF SKILL CAST INTERVAL ====
		// -1 to make it always cast if the skill is available
		public float selfBuffCastInterval = -1;
		// chance to cast self buff (-1 to make it 100%)
		public float selfBuffCastChance = -1;
		// min percent HP to cast self buff (-1 to disable)
		public float minHpPercentToCastSelfBuff = -1;

		private bool hasSpawnSkills = false;
		private bool hasJumpSkill = false;
		private bool hasLongRangeDmgSkill = false;
		private bool hasTeleportSkill = false;
		private bool hasSelfBuffSkills = false;

		private float lastSelfBuffCastTime;
		private float lastTeleportTime;

		public MeleeMonsterAI(Character o) : base(o)
		{
		}

		public override void AnalyzeSkills()
		{
			hasSpawnSkills = GetAllSkillsWithTrait(SkillTraits.SpawnMinion).Count > 0;
			hasJumpSkill = GetSkillWithTrait(SkillTraits.Jump) != null;
			hasLongRangeDmgSkill = GetSkillWithTrait(SkillTraits.Damage, SkillTraits.LongRange) != null;
			hasTeleportSkill = GetSkillWithTrait(SkillTraits.Teleport) != null;
			hasSelfBuffSkills = GetAllSkillsWithTrait(SkillTraits.BuffDamage, SkillTraits.BuffDefense).Count > 0;
		}

		protected override void AttackTarget(Character target)
		{
			SetMainTarget(target);

			bool isCasting = Owner.GetData().IsCasting;
			bool isMeleeAttacking = Owner.GetData().IsMeleeAttacking();
			bool forcedVelocity = Owner.GetData().forcedVelocity;
			int hpPercentage = (int)((GetStatus().Hp / (float)GetStatus().MaxHp) * 100);

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

			if (hasSelfBuffSkills && (minHpPercentToCastSelfBuff < 0 || hpPercentage <= minHpPercentToCastSelfBuff))
			{
				if (selfBuffCastInterval < 0 || lastSelfBuffCastTime + selfBuffCastInterval < Time.time)
				{
					if (selfBuffCastChance < 0 || Random.Range(1, 100) < selfBuffCastChance)
					{
						List<Skill> buffSkills = GetAllSkillsWithTrait(SkillTraits.BuffDamage, SkillTraits.BuffDefense);
						foreach (Skill s in buffSkills)
						{
							if (s.CanUse())
								StartAction(CastSkill(Owner, (ActiveSkill)s, 0, true, false, 0, 0), 0.5f);
						}
					}
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
				if (teleportInterval < 0 || lastTeleportTime + teleportInterval < Time.time)
				{
					ActiveSkill teleport = (ActiveSkill)GetSkillWithTrait(SkillTraits.Teleport);
					// if range too high, teleport
					if (teleport.CanUse() && !Owner.GetData().forcedVelocity && distSqr >= (teleport.range * teleport.range) / 3f && distSqr <= (teleport.range * teleport.range))
					{
						if (StartAction(CastSkill(target, teleport, distSqr, true, false, 0f, 0f), 0.5f))
							return;
					}
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
