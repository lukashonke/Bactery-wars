using System;
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
	/// <summary>
	/// </summary>
	public class BlankMonsterAI : MonsterAI
	{
		public BlankMonsterAI(Character o) : base(o)
		{
		}

		public override void CreateModules()
		{
		}

		public override void AnalyzeSkills()
		{
		}

		protected override void AttackTarget(Character target)
		{
			SetMainTarget(target);

			bool isCasting = Owner.GetData().IsCasting;
			//bool isMeleeAttacking = Owner.GetData().IsMeleeAttacking();
			//float distSqr = Utils.DistanceObjectsSqr(Owner.GetData().GetBody(), target.GetData().GetBody());
			float distSqr = Utils.DistanceSqr(Owner.GetData().GetBody().transform.position, target.GetData().GetBody().transform.position);
			int hpPercentage = (int)((GetStatus().Hp / (float)GetStatus().MaxHp) * 100);

			// already doing something
			if (isCasting || Owner.Status.IsStunned())
				return;

			if (Owner.GetData().Target == null || Owner.GetData().Target.Equals(target.GetData().GetBody()))
				Owner.GetData().Target = target.GetData().GetBody();

			if (LaunchAttackModule(target, distSqr, hpPercentage))
				return;

			MoveTo(target);
		}
	}
}
