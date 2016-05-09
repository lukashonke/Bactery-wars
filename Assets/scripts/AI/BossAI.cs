using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Skills;
using Assets.scripts.Skills.ActiveSkills;
using Assets.scripts.Skills.Base;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace Assets.scripts.AI
{
	public class SwarmerBossAI : MonsterAI
	{
		public float spawnMinInterval = 4f;
		public float spawnMaxInterval = 5f;

		private float nextSpawnInterval;

		public int maxMonstersSpawned = 20;

		protected ActiveSkill spawnSkill = null;

		private List<Monster> spawned = new List<Monster>(); 

		public SwarmerBossAI(Character o)
			: base(o)
		{
			UseTimers();
		}

		public override void AnalyzeSkills()
		{
			spawnSkill = (ActiveSkill)GetSkillWithTrait(SkillTraits.SpawnMinion);

			nextSpawnInterval = Random.Range(spawnMinInterval, spawnMaxInterval);
		}

		protected override void ThinkActive()
		{
			base.ThinkActive();

			bool isCasting = Owner.GetData().IsCasting;
			if (isCasting)
				return;

			DoSpawn();
		}

		protected override void AttackTarget(Character target)
		{
			SetMainTarget(target);

			bool isCasting = Owner.GetData().IsCasting;

			// already doing something
			if (isCasting)
				return;

			if (Owner.GetData().Target == null || Owner.GetData().Target.Equals(target.GetData().GetBody()))
				Owner.GetData().Target = target.GetData().GetBody();

			Vector3 ownerPos = Owner.GetData().GetBody().transform.position;

			DoSpawn();

			Vector3 nextTarget = Utils.GenerateRandomPositionAround(ownerPos, 5f);
			StartAction(MoveAction(nextTarget, false), 1f);
		}

		private void DoSpawn()
		{
			int count = 0;

			for(int i = spawned.Count-1; i>=0; i--)
			{
				Monster m = spawned[i];

				if (m.Status.IsDead)
				{
					spawned.RemoveAt(i);
					continue;
				}

				count ++;
			}

			if (count < 20 && spawnSkill != null && GetTimer("spawn", nextSpawnInterval))
			{
				if (StartAction(CastSkill(null, spawnSkill, 0, true, false, 0f, 0f), 0.5f))
				{
					SetTimer("spawn");

					if (spawnSkill is SwarmerSpawnSkill)
					{
						Monster m = ((SwarmerSpawnSkill)spawnSkill).lastSpawned;
						m.AI.AddAggro(GameSystem.Instance.CurrentPlayer, 10000);

						spawned.Add(m);

						((SwarmerSpawnSkill)spawnSkill).lastSpawned = null;
					}
					return;
				}
			}
		}
	}

	public class TankSpreadshooterConfusedAI : MonsterAI
	{
		public float spreadshootInterval = 5f;
		public float jumpInterval = 4f;

		public float jumpAtPlayerMinInterval = 4f;
		public float jumpAtPlayerMaxInterval = 5f;

		private float nextJumpAtPlayerInterval;

		protected ActiveSkill jump = null;
		protected ActiveSkill shot = null;

		public TankSpreadshooterConfusedAI(Character o) : base(o)
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

			bool canSee = Utils.CanSee(Owner.GetData().GetBody(), GetMainTarget().GetData().GetBody());

			if (Owner.GetData().Target == null || Owner.GetData().Target.Equals(target.GetData().GetBody()))
				Owner.GetData().Target = target.GetData().GetBody();

			Vector3 ownerPos = Owner.GetData().GetBody().transform.position;
			Vector3 targetPos = target.GetData().GetBody().transform.position;
			float distSqr = Utils.DistanceSqr(ownerPos, targetPos);

			if (currentAction == null && canSee && jump != null && GetTimer("jump", jumpInterval))
			{
				Vector3 pos = targetPos;

				SetTimer("jump_at_player");

				float range = Vector3.Distance(ownerPos, pos);

				jump.range = (int)range;

				if (StartAction(CastSkill(pos, jump, distSqr, true, false, 0f, 0f), 0.5f))
				{
					SetTimer("jump");
					return;
				}
			}

			if (shot != null && GetTimer("shoot", spreadshootInterval))
			{
				if (StartAction(CastSkill(null, shot, distSqr, true, false, 0f, 0f), 0.5f))
				{
					SetTimer("shoot");
					return;
				}
			}

			if (canSee)
			{
				Vector3 nextTarget = Utils.GenerateRandomPositionAround(ownerPos, 5f);
				StartAction(MoveAction(nextTarget, false, 3), 1f);
				Debug.Log("can see");
			}
			else
			{
				Owner.GetData().MeleeInterract(target.GetData().GetBody(), true);
				Debug.Log("CANNOT see");
			}
		}
	}

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
			float distSqr = Utils.DistanceSqr(ownerPos, targetPos);

			if (currentAction == null && jump != null && GetTimer("jump", jumpInterval))
			{
				Vector3 pos;

				if (GetTimer("jump_at_player", nextJumpAtPlayerInterval))
				{
					pos = targetPos;

					SetTimer("jump_at_player");

					nextJumpAtPlayerInterval = Random.Range(jumpAtPlayerMinInterval, jumpAtPlayerMaxInterval);
				}
				else
				{
					Owner.GetData().Target = null;
					pos = Utils.GenerateRandomPositionAround(targetPos, 15f, 2f);
				}

				Debug.DrawLine(ownerPos, pos, Color.magenta, 2f);

				float range = Vector3.Distance(ownerPos, pos);

				jump.range = (int) range;

				if (StartAction(CastSkill(pos, jump, distSqr, true, false, 0f, 0f), 0.5f))
				{
					SetTimer("jump");
					return;
				}
			}

			if (shot != null && GetTimer("shoot", spreadshootInterval))
			{
				if (StartAction(CastSkill(null, shot, distSqr, true, false, 0f, 0f), 0.5f))
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
