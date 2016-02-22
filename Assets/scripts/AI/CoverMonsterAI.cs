using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
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

		protected override void AttackTarget(Character target)
		{
			SetMainTarget(target);

			Character closestAlly = null;

			if (HasMaster())
			{
				closestAlly = GetMaster();
			}
			else if (protectingTarget != null)
			{
				closestAlly = protectingTarget;
			}
			
			if (currentAction != null || Owner.Status.IsStunned())
				return;

			Vector3 targetPos;
			if (closestAlly == null)
			{
				targetPos = target.GetData().transform.position;
			}
			else
			{
				Character masterTarget = closestAlly.AI.GetMainTarget();

				if (masterTarget == null)
					return;

				Vector3 masterTargetPos = masterTarget.GetData().GetBody().transform.position;
				targetPos = Vector3.Lerp(closestAlly.GetData().transform.position, masterTargetPos, 0.5f);
			}

			MoveTo(targetPos, false);
		}
	}
}
