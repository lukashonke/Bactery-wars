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
		public Dictionary<Character, int> Aggro;

		public bool IsAggressive { get; set; }
		public int AggressionRange { get; set; }

		public MonsterAI(Character o) : base(o)
		{
			Aggro = new Dictionary<Character, int>();

			IsAggressive = true;
			AggressionRange = 2;
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

				if (Aggro.Any())
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
			if (!Aggro.ContainsKey(ch))
			{
				Aggro.Add(ch, points);
			}
			else
			{
				int newP = 0;
				Aggro.TryGetValue(ch, out newP);
				newP += points;
				Aggro[ch] = newP;
			}
		}

		public void RemoveAggro(Character ch)
		{
			RemoveAggro(ch, 0);
		}

		public override void RemoveAggro(Character ch, int points)
		{
			if (Aggro.ContainsKey(ch))
			{
				if (points == 0)
					Aggro.Remove(ch);
				else
				{
					int newP = 0;
					Aggro.TryGetValue(ch, out newP);
					newP -= points;

					if (newP <= 0)
						Aggro.Remove(ch);
					else
						Aggro[ch] = newP;
				}
			}
		}

		private Character SelectMostAggroTarget()
		{
			if (Aggro.Count == 0)
				return null;

			int top = 0;
			Character topCh = null;
			foreach (KeyValuePair<Character, int> e in Aggro)
			{
				if (e.Value > top)
				{
					top = e.Value;
					topCh = e.Key;
				}
			}

			return topCh;
		}

		private void AttackTarget(Character target)
		{
			//just melee attack for now 
			Owner.GetData().MeleeAttack(target.GetData().GetBody());
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
			ThinkInterval = 0.2f;
		}
	}
}
