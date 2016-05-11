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
	/// important parameters: evadeInterval, floatInterval, shootWhileMoving
	/// other parameters: evadeChance, floatChance
	/// 
	/// can have any number of Damage skills
	/// can have SpawnMinion skill
	/// can have Self-Support skills
	/// </summary>
	public class RangedMonsterAI : MonsterAI
	{
		public RangedMonsterAI(Character o) : base(o)
		{
		}

		public override void CreateModules()
		{
			AddAttackModule(new SpawnSkillModule(this));
			AddAttackModule(new SelfBuffSkillModule(this));

			AddAttackModule(new EvadeModule(this));
			AddAttackModule(new FloatModule(this));

			// set to 0 unless Monster Template changes it
			AddAttackModule(new EvasiveMovementModule(this, 0));
			AddAttackModule(new JumpMovementModule(this, 0));
			AddAttackModule(new DamageSkillModule(this));
			AddAttackModule(new AutoattackModule(this));
		}

		public override void AnalyzeSkills()
		{
		}

		protected virtual bool IsLowHp(int hpPercent)
		{
			return (30 + UnityEngine.Random.Range(-10, 10) >= hpPercent);
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

			foreach (AIAttackModule module in attackModules)
			{
				if (module.Launch(target, distSqr))
				{
					return;
				}
			}

			//else stand idle


			/*if (IsLowHp(hpPercentage))
			{
				StartAction(RunAway(target, 5f, 20), 1f);
				return;
			}*/
		}
	}
}
