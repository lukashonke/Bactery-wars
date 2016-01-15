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
        private float lastEvadeTime;

        // how often does he think about evading; -1 to disable
	    public float evadeInterval = -1;

        // -1 to make it always 100% 
        public float evadeChance = -1;

		public RangedMonsterAI(Character o) : base(o)
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
			bool isMeleeAttacking = Owner.GetData().IsMeleeAttacking();
			float dist = Utils.DistancePwr(target.GetData().transform.position, Owner.GetData().transform.position);
			int hpPercentage = (int)((GetStatus().Hp / (float)GetStatus().MaxHp) * 100);

			// already doing something
			if (isCasting)
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

                    Debug.DrawLine(pos, Owner.GetData().transform.position, Color.blue, 4f);

		            if (StartAction(MoveAction(pos, true), 3f))
		            {
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
				StartAction(CastSkill(target, topSkill, dist, false, true, 0, 0), 10f);
			else if(HasMeleeSkill())
				Owner.GetData().MeleeInterract(target.GetData().GetBody(), true);
		}
	}
}
