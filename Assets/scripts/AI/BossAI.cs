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
	public class TankSpreadshooterAI : MonsterAI
	{
		public float spreadshootInterval = 5f;
		public float jumpInterval = 1f;

		public float jumpAtPlayerMinInterval = 2f;
		public float jumpAtPlayerMaxInterval = 5f;

		private float nextJumpAtPlayerInterval;

		protected ActiveSkill jump = null;
		protected ActiveSkill shot = null;

		public TankSpreadshooterAI(Character o) : base(o)
		{
			UseTimers();
		}

		public override void AnalyzeSkills()
		{
			jump = (ActiveSkill)GetSkillWithTrait(SkillTraits.Jump);
			shot = (ActiveSkill)GetSkillWithTrait(SkillTraits.Damage);

			nextJumpAtPlayerInterval = Random.Range(jumpAtPlayerMinInterval, jumpAtPlayerMinInterval);
		}

		protected override void AttackTarget(Character target)
		{
			SetMainTarget(target);

			bool isCasting = Owner.GetData().IsCasting;
			bool forcedVelocity = Owner.GetData().forcedVelocity;

			// already doing something
			if (isCasting || forcedVelocity)
				return;

			if (Owner.GetData().Target == null || Owner.GetData().Target.Equals(target.GetData().GetBody()))
				Owner.GetData().Target = target.GetData().GetBody();

			Vector3 ownerPos = Owner.GetData().GetBody().transform.position;
			Vector3 targetPos = target.GetData().GetBody().transform.position;
			float dist = Vector3.Distance(ownerPos, targetPos);

			if (currentAction == null && jump != null && GetTimer("jump", jumpInterval))
			{
				Vector3 pos;

				if (GetTimer("jump_at_player", nextJumpAtPlayerInterval))
				{
					pos = targetPos;

					SetTimer("jump_at_player");

					nextJumpAtPlayerInterval = Random.Range(jumpAtPlayerMinInterval, jumpAtPlayerMinInterval);
				}
				else
				{
					Owner.GetData().Target = null;
					pos = Utils.GenerateRandomPositionAround(targetPos, 15f, 2f);
				}

				Debug.DrawLine(ownerPos, pos, Color.magenta, 2f);

				float range = Vector3.Distance(ownerPos, pos);

				jump.range = (int) range;

				if (StartAction(CastSkill(pos, jump, dist, true, false, 0f, 0f), 0.5f))
				{
					SetTimer("jump");
					return;
				}
			}

			if (shot != null && GetTimer("shoot", spreadshootInterval))
			{
				if (StartAction(CastSkill(null, shot, dist, true, false, 0f, 0f), 0.5f))
				{
					SetTimer("shoot");
					return;
				}
			}

			Vector3 nextTarget = Utils.GenerateRandomPositionAround(ownerPos, 5f);
			StartAction(MoveAction(nextTarget, false, 3), 1f);
		}
	}
}
