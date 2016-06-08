// Copyright (c) 2015, Lukas Honke
// ========================
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.AI.Modules;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Assets.scripts.AI
{
	public class CoverMonsterAI : MonsterAI
	{
		public Character protectingTarget;

		public CoverMonsterAI(Character o) : base(o)
		{
		}

		public override void CreateModules()
		{
			AddAttackModule(new CoverAllyModule(this));
			AddAttackModule(new AutoattackModule(this));
		}

		protected override void AttackTarget(Character target)
		{
			SetMainTarget(target);

			if (currentAction != null || Owner.Status.IsStunned())
				return;

			Vector3 ownerPos = Owner.GetData().GetBody().transform.position;
			Vector3 targetPos = target.GetData().GetBody().transform.position;
			float distSqr = Utils.DistanceSqr(ownerPos, targetPos);
			int hpPercentage = (int)((GetStatus().Hp / (float)GetStatus().MaxHp) * 100);

			if (LaunchAttackModule(target, distSqr, hpPercentage))
				return;

			// default action
			MoveTo(targetPos, false);
		}
	}
}
