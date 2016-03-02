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
	public class RangedMonsterAI : MonsterAI
	{
        // how often does he think about evading; -1 to disable
	    public float evadeInterval = -1;

        // -1 to make it always 100% 
        public float evadeChance = -1;
        private float lastEvadeTime;

		// float around target
		public float floatChance = -1;
		public float floatInterval = -1;
		public float floatSpeed = 4;
		public int floatRange = 5;
		public float lastFloatTime;

		public bool shootWhileMoving = false;

		public RangedMonsterAI(Character o) : base(o)
		{
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
			bool isMeleeAttacking = Owner.GetData().IsMeleeAttacking();
			float dist = Utils.GetDistPwr(Owner.GetData().GetBody(), target.GetData().GetBody());
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

            // try spawn skills
			List<Skill> spawnSkills = GetAllSkillsWithTrait(SkillTraits.SpawnMinion);
			foreach (Skill s in spawnSkills)
			{
				if (s.CanUse())
					StartAction(CastSkill(null, (ActiveSkill)s, 0, true, false, 0, 0), 0.5f);
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

			if (dist < 10*10 && floatInterval > -1 && lastFloatTime + floatInterval < Time.time)
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
			int topDamage = 0;
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
				StartAction(CastSkill(target, topSkill, dist, false, true, 0, 0), 10f, false, shootWhileMoving);
			else if(HasMeleeSkill())
				Owner.GetData().MeleeInterract(target.GetData().GetBody(), true);
		}
	}
}
