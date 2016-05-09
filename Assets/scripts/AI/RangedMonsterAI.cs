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
		// ==== EVADING PARAMS ====
        // how often (in seconds) does he think about evading; -1 to disable
	    public float evadeInterval = -1;
        // -1 to make it always 100% 
        public float evadeChance = -1;

		// ==== FLOATING AROUND TARGET - circular move ====
		// how often does he consider floating
		public float floatInterval = -1;
		// chance to float  (-1 to make it 100%)
		public float floatChance = -1;
		// move speed when floating
		public float floatSpeed = 4;
		// how far to float
		public int floatRange = 5;
		// if the distance is higher than this, the char will not float but move directly towards player
		public int maxDistToFloat = 10;

		// ==== CAN SHOT WHILE MOVING (false will stop char when shooting) ====
		public bool shootWhileMoving = false;

		// ==== SELF BUFF SKILL CAST INTERVAL ====
		// -1 to make it always cast if the skill is available
		public float selfBuffCastInterval = -1;
		// chance to cast self buff (-1 to make it 100%)
		public float selfBuffCastChance = -1;
		// min percent HP to cast self buff (-1 to disable)
		public float minHpPercentToCastSelfBuff = -1;



        private float lastEvadeTime;
		private float lastFloatTime;
		private float lastSelfBuffCastTime;

		private bool hasSpawnSkills = false;
		private bool hasSelfBuffSkills = false;

		public RangedMonsterAI(Character o) : base(o)
		{
		}

		public override void AnalyzeSkills()
		{
			hasSpawnSkills = GetAllSkillsWithTrait(SkillTraits.SpawnMinion).Count > 0;
			hasSelfBuffSkills = GetAllSkillsWithTrait(SkillTraits.BuffDamage, SkillTraits.BuffDefense).Count > 0;
		}

		protected virtual bool IsLowHp(int hpPercent)
		{
			return (30 + UnityEngine.Random.Range(-10, 10) >= hpPercent);
		}

		private int lastAngle = 0;

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

			/*if (IsLowHp(hpPercentage))
			{
				StartAction(RunAway(target, 5f, 20), 1f);
				return;
			}*/

			if (hasSpawnSkills)
			{
				// try spawn skills
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

            if (evadeInterval > -1 && lastEvadeTime + evadeInterval < Time.time)
		    {
		        if (evadeChance < 0 || Random.Range(1, 100) < evadeChance)
		        {
		            Vector3 pos = Utils.GenerateRandomPositionAround(Owner.GetData().GetBody().transform.position, 4f, 3f);

		            if (StartAction(MoveAction(pos, true), 3f))
		            {
						Debug.DrawLine(pos, Owner.GetData().transform.position, Color.blue, 4f);
                        lastEvadeTime = Time.time;
                        return;
		            }
		        }
		    }

			if (distSqr < Math.Pow(maxDistToFloat, 2) && floatInterval > -1 && lastFloatTime + floatInterval < Time.time)
			{
				if (floatChance < 0 || Random.Range(1, 100) < floatChance)
				{
					// circulate target
					Vector3 pos = Utils.GenerateRandomPositionOnCircle(target.GetData().GetBody().transform.position, floatRange, lastAngle);

					if (StartAction(MoveAction(pos, true, floatSpeed), 2f, false))
					{
						lastAngle = lastAngle + Random.Range(-2, 2);
						Debug.DrawLine(pos, Owner.GetData().transform.position, Color.green, 4f);
						lastEvadeTime = Time.time;
						return;
					}
				}
			}

			List<Skill> skills = GetAllSkillsWithTrait(SkillTraits.Damage);

			// 1. get the most damage skill and cast if it is available
			int topDamage = -1;
			ActiveSkill topSkill = null;

			foreach (Skill skill in skills)
			{
				ActiveSkill sk = (ActiveSkill) skill;
				if (sk.CanUse() && sk.GetTotalDamageOutput() > topDamage)
				{
					topDamage = sk.GetTotalDamageOutput();
					topSkill = sk;
				}
			}

			//if(topSkill != null)
			//Debug.Log(topSkill.GetName());

			if (topSkill != null)
			{
				StartAction(CastSkill(target, topSkill, distSqr, false, true), 10f, false, shootWhileMoving);
			}
			else if(HasMeleeSkill())
				Owner.GetData().MeleeInterract(target.GetData().GetBody(), true);
		}
	}
}
