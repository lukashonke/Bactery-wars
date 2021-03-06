﻿// Copyright (c) 2015, Lukas Honke
// ========================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Skills;
using Assets.scripts.Skills.Base;
using UnityEngine;
using Random = System.Random;

namespace Assets.scripts.AI.Modules
{
	/// <summary>
	/// 
	/// </summary>
	public class JumpMovementModule : AIAttackModule
	{
		// true if the player will jump directly at player when distance is <minRange
		public bool jumpAtEnemy = true;

		// pokud je bliz nez tato vzdalenost, pouzije tento pohyb aby se dostal bliz
		public float minRangeToJump = 5f;

		public JumpMovementModule(MonsterAI ai) : base(ai)
		{
		}

		public override void Init()
		{
			canTrigger = ai.GetAllSkillsWithTrait(SkillTraits.Jump).Count > 0;
		}

		public override bool Trigger(Character target, float distSqr)
		{
			ActiveSkill jump = (ActiveSkill)ai.GetSkillWithTrait(SkillTraits.Jump);

			if (jump != null && jump.CanUse() && !ai.Owner.GetData().forcedVelocity)
			{
				Vector3 ownerPos = ai.Owner.GetData().GetBody().transform.position;
				Vector3 targetPos = target.GetData().GetBody().transform.position;

				if (distSqr > (minRangeToJump*minRangeToJump))
				{
					Vector3 nextTarget = Utils.GeneratePerpendicularPositionAround(ownerPos, targetPos, 2, 6);
					//Debug.DrawLine(ownerPos, nextTarget, Color.cyan, 1f);

					//if (ai.StartAction(ai.CastSkill(target, jump, distSqr, true, false, 0f, 0f), 0.5f))
					if (ai.StartAction(ai.CastSkill(nextTarget, jump, distSqr, true, false, 0f, 0f), 0.5f))
						return true;
				}
				else if(jumpAtEnemy)
				{
					if (ai.StartAction(ai.CastSkill(targetPos, jump, distSqr, true, false, 0f, 0f), 0.5f))
						return true;
				}
			}

			return false;
		}
	}
}
