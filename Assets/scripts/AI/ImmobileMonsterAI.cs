// Copyright (c) 2015, Lukas Honke
// ========================
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.AI.Modules;
using Assets.scripts.Mono.ObjectData;
using Assets.scripts.Skills;
using Assets.scripts.Skills.Base;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace Assets.scripts.AI
{
	public class ImmobileMonsterAI : MonsterAI
	{
		public ImmobileMonsterAI(Character o) : base(o)
		{
			if (o.GetData() != null && o.GetData() is EnemyData)
			{
				((EnemyData)o.GetData()).SetMass(200);
			}
		}

		public override void CreateModules()
		{
			AddAttackModule(new DamageSkillModule(this));
			AddAttackModule(new AutoattackModule(this));
			AddAttackModule(new RotateToTargetModule(this));

			GetAttackModule<DamageSkillModule>().boostShootRange = 0.2f;
		}

		protected override void AttackTarget(Character target)
		{
			SetMainTarget(target);

			bool isCasting = Owner.GetData().IsCasting;
			bool isMeleeAttacking = Owner.GetData().IsMeleeAttacking();
			float distSqr = Utils.DistanceSqr(target.GetData().transform.position, Owner.GetData().transform.position);
			int hpPercentage = (int)((GetStatus().Hp / (float)GetStatus().MaxHp) * 100);

			// already doing something
			if (isCasting || Owner.Status.IsStunned())
				return;

			if (Owner.GetData().Target == null || Owner.GetData().Target.Equals(target.GetData().GetBody()))
				Owner.GetData().Target = target.GetData().GetBody();

			if (LaunchAttackModule(target, distSqr, hpPercentage))
				return;
		}
	}
}
