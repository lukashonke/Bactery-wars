using System;
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
		public SummonerMonsterAI(Character o)
			: base(o)
		{
			UseTimers();
		}

		public override void CreateModules()
		{
			AddAttackModule(new SpawnMinionModule(this));
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

			int hpPercentage = (int)((GetStatus().Hp / (float)GetStatus().MaxHp) * 100);

			if (Owner.GetData().Target == null || Owner.GetData().Target.Equals(target.GetData().GetBody()))
				Owner.GetData().Target = target.GetData().GetBody();

			if (LaunchAttackModule(target, distSqr, hpPercentage))
				return;
		}
	}
}
