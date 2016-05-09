﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.AI.Modules;
using Assets.scripts.Skills;
using Assets.scripts.Skills.ActiveSkills;
using Assets.scripts.Skills.Base;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace Assets.scripts.AI
{
	public class SummonerMonsterAI : MonsterAI
	{
		private List<Monster> spawned = new List<Monster>();

		public delegate void Callback();

		public SummonerMonsterAI(Character o)
			: base(o)
		{
			UseTimers();
		}

		public override void CreateModules()
		{
			AddAttackModule(new SpawnMinionModule(this));
			//AddAttackModule(new RambleAroundModule(this));
		}

		public override void AnalyzeSkills()
		{
		}

		/*protected override void ThinkActive()
		{
			base.ThinkActive();

			bool isCasting = Owner.GetData().IsCasting;
			if (isCasting)
				return;

			DoSpawn();
		}*/

		protected override void AttackTarget(Character target)
		{
			SetMainTarget(target);

			bool isCasting = Owner.GetData().IsCasting;
			float distSqr = Utils.DistanceSqr(Owner.GetData().GetBody().transform.position, target.GetData().GetBody().transform.position);

			// already doing something
			if (isCasting || Owner.Status.IsStunned())
				return;

			if (Owner.GetData().Target == null || Owner.GetData().Target.Equals(target.GetData().GetBody()))
				Owner.GetData().Target = target.GetData().GetBody();

			foreach (AIAttackModule module in attackModules)
			{
				if (module.Launch(target, distSqr))
				{
					return;
				}
			}

			Vector3 ownerPos = Owner.GetData().GetBody().transform.position;
			Vector3 nextTarget = Utils.GenerateRandomPositionAround(ownerPos, 5f);
			StartAction(MoveAction(nextTarget, false), 1f);
		}
	}
}
