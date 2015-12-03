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
		public Character Target { get; set; }

		public MonsterAI(Character o) : base(o)
		{
			AutoAttackRange = 5;
		}

		public override void Think()
		{
			switch (State)
			{
				case AIState.IDLE:
					Debug.Log("idle");
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

					break;
				case AIState.ACTIVE:
					Debug.Log("act");
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
									Target = ch;
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

					break;
				case AIState.ATTACKING:
					Debug.Log("attack");
					if (Target == null || Target.Status.IsDead)
					{
						SetAIState(AIState.ACTIVE);
						return;
					}

					ThinkAttack();

					break;
			}
		}

		private void ThinkAttack()
		{
			if (Vector3.Distance(Owner.GetData().GetBody().transform.position, Target.GetData().GetBody().transform.position) >
			    AutoAttackRange*2)
			{
				SetAIState(AIState.ACTIVE);
				Target = null;
				return;
			}

			Owner.GetData().SetMovementTarget(Target.GetData().GetBody().transform.position);
			Owner.GetData().HasTargetToMoveTo = true;
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
