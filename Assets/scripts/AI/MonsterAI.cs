using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Mono;
using Assets.scripts.Skills;
using Assets.scripts.Skills.Base;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.scripts.AI
{
	public abstract class MonsterAI : AbstractAI
	{
		protected Dictionary<Character, int> aggro;

		public bool IsAggressive { get; set; }
		public int AggressionRange { get; set; }

		protected MonsterAI(Character o) : base(o)
		{
			aggro = new Dictionary<Character, int>();

			IsAggressive = true;
			AggressionRange = 5;
		}

		public override void Think()
		{
			if (Owner == null)
				return;

			if (GetStatus().IsDead)
			{
				if (State == AIState.IDLE)
					SetAIState(AIState.IDLE);
			}

			switch (State)
			{
				case AIState.IDLE:
					ThinkIdle();

					break;
				case AIState.ACTIVE:
					ThinkActive();

					break;
				case AIState.ATTACKING:
					ThinkAttack();

					break;
			}
		}

		private void ThinkIdle()
		{
			if (GetStatus().IsDead)
				return;

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

		private void ThinkActive()
		{
			bool stillActive = false;
			if (Owner.Knownlist.KnownObjects.Count > 0)
			{
				foreach (GameObject o in Owner.Knownlist.KnownObjects)
				{
					if (o == null) continue;

					Character ch = Utils.GetCharacter(o);

					if (IsAggressive && ch != null && Owner.CanAutoAttack(ch))
					{
						stillActive = true;

						// find target that can be attacked
						if (Vector3.Distance(Owner.GetData().GetBody().transform.position, ch.GetData().GetBody().transform.position) < AggressionRange)
						{
							AddAggro(ch, 1);
						}


						break;
					}
				}

				if (aggro.Any())
					SetAIState(AIState.ATTACKING);
			}

			if (!stillActive)
			{
				SetAIState(AIState.IDLE);
			}
		}

		private void ThinkAttack()
		{
			Character possibleTarget = SelectMostAggroTarget();

			// no target this monster hates - go back to active
			if (possibleTarget == null)
			{
				SetAIState(AIState.ACTIVE);
				return;
			}

			if (possibleTarget.Status.IsDead)
			{
				RemoveAggro(possibleTarget);
				return;
			}

			// target is too far (> attackdistance*2) - abandone attacking
			if (Vector3.Distance(Owner.GetData().GetBody().transform.position, possibleTarget.GetData().GetBody().transform.position) > AggressionRange * 2)
			{
				RemoveAggro(possibleTarget, 1);
			}

			AttackTarget(possibleTarget);
		}

		public override void AddAggro(Character ch, int points)
		{
			if (!aggro.ContainsKey(ch))
			{
				aggro.Add(ch, points);
			}
			else
			{
				int newP = 0;
				aggro.TryGetValue(ch, out newP);
				newP += points;
				aggro[ch] = newP;
			}
		}

		public void RemoveAggro(Character ch)
		{
			RemoveAggro(ch, 0);
		}

		public override void RemoveAggro(Character ch, int points)
		{
			if (aggro.ContainsKey(ch))
			{
				if (points == 0)
					aggro.Remove(ch);
				else
				{
					int newP = 0;
					aggro.TryGetValue(ch, out newP);
					newP -= points;

					if (newP <= 0)
						aggro.Remove(ch);
					else
						aggro[ch] = newP;
				}
			}
		}

		protected virtual Character SelectMostAggroTarget()
		{
			if (aggro.Count == 0)
				return null;

			int top = 0;
			Character topCh = null;
			foreach (KeyValuePair<Character, int> e in aggro)
			{
				if (e.Value > top)
				{
					top = e.Value;
					topCh = e.Key;
				}
			}

			return topCh;
		}

		protected abstract void AttackTarget(Character target);

		protected virtual IEnumerator RunAway(Character target, float distance, int randomAngleAdd)
		{
			Vector3 dirVector = -Utils.GetDirectionVector(target.GetData().GetBody().transform.position, Owner.GetData().transform.position).normalized * distance;

			int randomDir = Random.Range(-randomAngleAdd, randomAngleAdd);
			Vector3 nv = Quaternion.Euler(new Vector3(0, 0, randomDir)) * dirVector;

			/*RaycastHit2D hit = Physics2D.Linecast(Owner.GetData().GetBody().transform.position, nv);

			if (hit != null && hit.transform != null)
			{
				currentAction = null;
				yield break;
			}*/
			
			MoveTo(nv);

			yield return null;

			while (Owner.GetData().HasTargetToMoveTo)
			{
				yield return null;
			}

			currentAction = null;
		}

		protected virtual IEnumerator CastSkill(Character target, ActiveSkill sk, float dist, bool noRangeCheck, bool moveTowardsIfRequired, float skillRangeAdd, float randomSkilLRangeAdd)
		{
			if (!noRangeCheck && sk.range != 0)
			{
				while ((sk.range + skillRangeAdd + Random.Range(-randomSkilLRangeAdd, randomSkilLRangeAdd)) < dist)
				{
					dist = Vector3.Distance(target.GetData().transform.position, Owner.GetData().transform.position);

					if (moveTowardsIfRequired)
					{
						MoveTo(target);
						yield return null;
					}
					else // too far, cant move closer - break the action
					{
						currentAction = null;
						yield break;
					}
				}
			}

			Owner.GetData().BreakMovement(true);
			RotateToTarget(target);
			Owner.CastSkill(sk);
			currentAction = null;
		}

		protected override void OnSwitchIdle()
		{
			ThinkInterval = 3f;
		}

		protected override void OnSwitchActive()
		{
			ThinkInterval = 0.5f;
		}

		protected override void OnSwitchAttacking()
		{
			ThinkInterval = 0.2f;
		}
	}
}
