﻿using System;
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
	public class MonsterAI : AbstractAI
	{
		public Dictionary<Character, int> Aggro;

		public readonly int SHORT_RANGE = 6;
		public readonly int LONG_RANGE = 10;

		public bool IsAggressive { get; set; }
		public int AggressionRange { get; set; }

		public MonsterAI(Character o) : base(o)
		{
			Aggro = new Dictionary<Character, int>();

			IsAggressive = true;
			AggressionRange = 5;
		}

		public override void Think()
		{
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
			SetMainTarget(target);

			bool isCasting = Owner.GetData().IsCasting;
			bool isMelee = Owner.GetData().RepeatingMeleeAttack;

			// already doing something
			if (isCasting || currentAction != null)
			{
				return;
			}

			if(Owner.GetData().Target == null || Owner.GetData().Target.Equals(target.GetData().GetBody()))
			{
				Owner.GetData().Target = target.GetData().GetBody();
			}

			float dist = Utils.DistancePwr(target.GetData().transform.position, Owner.GetData().transform.position);
			bool isImmobilized = Owner.GetData().CanMove();
			int hpPercentage = (int) ((GetStatus().Hp/(float)GetStatus().MaxHp)*100);
			int targetHpPercentage = (int)((target.Status.Hp / (float)target.Status.MaxHp) * 100);
			int targetHp = target.Status.Hp;

			if (IsLowHp(target))
			{
				
			}

			ActiveSkill sk = (ActiveSkill) GetSkillWithTrait(SkillTraits.Damage);

			if (sk != null && sk.CanUse())
			{
				StartAction(CastSkill(target, sk, dist, true, 0f, 0f), 5f);
			}
			else		
				Owner.GetData().MeleeAttack(target.GetData().GetBody(), false);
		}

		private bool IsLowHp(Character target)
		{
			return false;
		}

		protected IEnumerator CastSkill(Character target, ActiveSkill sk, float dist, bool moveTowardsIfRequired, float skillRangeAdd, float randomSkilLRangeAdd)
		{
			if (sk.range != 0)
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
