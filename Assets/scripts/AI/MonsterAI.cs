using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Mono;
using UnityEngine;

namespace Assets.scripts.AI
{
	public class MonsterAI : AbstractAI
	{
		public float AutoAttackRange { get; set; }

		public MonsterAI(Character o) : base(o)
		{
			AutoAttackRange = 5;
		}

		public override void Think()
		{
			Debug.Log(State.ToString());

			switch (State)
			{
				case AIState.IDLE:
					ThinkIdle();

					break;
				case AIState.ACTIVE:
					ThinkActive();

					break;
				case AIState.ATTACKING:
					if (GetMainTarget() == null || GetMainTarget().Status.IsDead)
					{
						SetAIState(AIState.ACTIVE);
						return;
					}

					ThinkAttack();

					break;
			}
		}

		private void ThinkIdle()
		{
			if (Owner.Knownlist.KnownObjects.Count > 0)
			{
				foreach (GameObject o in Owner.Knownlist.KnownObjects)
				{
					if (o == null || o.Equals(Owner.GetData().GetBody())) continue;

					Character ch = Utils.GetCharacter(o);

					if (ch != null && Owner.CanAutoAttack(ch)) // replace with isAttackable method
					{
						SetAIState(AIState.ACTIVE);
						break;
					}
				}
			}
		}

		private void ThinkAttack()
		{
			// target is too far (> attackdistance*2) - abandone attacking
			if (Vector3.Distance(Owner.GetData().GetBody().transform.position, GetMainTarget().GetData().GetBody().transform.position) > AutoAttackRange * 2)
			{
				SetAIState(AIState.ACTIVE);
				RemoveMainTarget();
				return;
			}

			Debug.DrawRay(Owner.GetData().GetBody().transform.position, GetMainTarget().GetData().GetBody().transform.position, Color.blue);

			AttackTarget(GetMainTarget());
		}

		private void AttackTarget(Character target)
		{
			Owner.GetData().MeleeAttack(GetMainTarget().GetData().GetBody());
			//Owner.GetData().SetMovementTarget(GetMainTarget().GetData().GetBody().transform.position);
			//Owner.GetData().HasTargetToMoveTo = true;
		}

		private void ThinkActive()
		{
			bool stillActive = false;
			if (Owner.Knownlist.KnownObjects.Count > 0)
			{
				foreach (GameObject o in Owner.Knownlist.KnownObjects)
				{
					if (o == null) continue;

					Character ch = Utils.GetCharacter(o);

					if (ch != null && Owner.CanAutoAttack(ch))
					{
						stillActive = true;

						// find target that can be attacked
						if (Vector3.Distance(Owner.GetData().GetBody().transform.position, ch.GetData().GetBody().transform.position) < AutoAttackRange)
						{
							SetMainTarget(ch);
							SetAIState(AIState.ATTACKING);
						}

						break;
					}
				}
			}

			if (!stillActive)
			{
				SetAIState(AIState.IDLE);
			}
		}

		public override void OnSwitchIdle()
		{
			ThinkInterval = 3f;
		}

		public override void OnSwitchActive()
		{
			ThinkInterval = 0.5f;
		}

		public override void OnSwitchAttacking()
		{
			
		}
	}
}
