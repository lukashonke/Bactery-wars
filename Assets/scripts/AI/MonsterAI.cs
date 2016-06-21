// Copyright (c) 2015, Lukas Honke
// ========================
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Actor.MonsterClasses;
using Assets.scripts.AI.Modules;
using Assets.scripts.Mono;
using Assets.scripts.Mono.ObjectData;
using Assets.scripts.Skills;
using Assets.scripts.Skills.Base;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.scripts.AI
{
	public abstract class MonsterAI : AbstractAI
	{
		protected List<AIAttackModule> attackModules;
		protected AIAttackModule currentPrioridyModule;
		private float currentPrioridyModuleTime;
		private float attackStartedTime;

		protected Dictionary<Character, int> aggro;

		public bool IsAggressive { get; set; }
		public int AggressionRange { get; set; }

		private readonly int rambleInterval = 2;
		private float lastRambleTime;
		private bool isWalking;

		private Dictionary<string, float> timers;

		private bool useTimers;

		protected MonsterAI(Character o) : base(o)
		{
			attackModules = new List<AIAttackModule>();
			aggro = new Dictionary<Character, int>();
			currentPrioridyModule = null;

			attackStartedTime = 0;

			useTimers = false;

			IsAggressive = GetTemplate().IsAggressive;
			AggressionRange = GetTemplate().AggressionRange;

			CreateModules();
		}

		/// <summary>
		/// modules added earlier have higher priority !!!
		/// </summary>
		/// <returns></returns>
		public AIAttackModule AddAttackModule(AIAttackModule mod)
		{
			attackModules.Add(mod);
			return mod;
		}

		public AIAttackModule AddPriorityAttackModule(AIAttackModule mod, bool atStart=true)
		{
			if(atStart)
				attackModules.Insert(0, mod);
			else
				attackModules.Add(mod);

			return mod;
		}

		public AIAttackModule GetAttackModule(Type type)
		{
			foreach (AIAttackModule mod in attackModules)
			{
				if (mod.GetType() == type)
					return mod;
			}
			return null;
		}

		public T GetAttackModule<T>() where T : AIAttackModule
		{
			foreach (AIAttackModule mod in attackModules)
			{
				if (mod is T)
					return mod as T;
			}
			return null;
		}

		public T GetAttackModule<T>(int id) where T : AIAttackModule
		{
			foreach (AIAttackModule mod in attackModules)
			{
				if (mod is T && mod.id == id)
					return mod as T;
			}
			return null;
		}

		public AIAttackModule GetAttackModule(int id)
		{
			foreach (AIAttackModule mod in attackModules)
			{
				if (mod.id == id)
					return mod;
			}
			return null;
		}

		public AIAttackModule GetAttackModule(string typeName)
		{
			foreach (AIAttackModule mod in attackModules)
			{
				if (mod.GetType().Name.Equals(typeName, StringComparison.InvariantCultureIgnoreCase))
					return mod;
			}
			return null;
		}

		public AIAttackModule GetAttackModule(string typeName, int id)
		{
			foreach (AIAttackModule mod in attackModules)
			{
				if (mod.GetType().Name.Equals(typeName, StringComparison.InvariantCultureIgnoreCase) && mod.id == id)
					return mod;
			}
			return null;
		}

		public void UseTimers()
		{
			useTimers = true;
			timers = new Dictionary<string, float>();
		}

		public void SetTimer(string name)
		{
			if (!useTimers)
				return;

			if (timers.ContainsKey(name))
			{
				timers[name] = Time.time;
			}
			else
			{
				timers.Add(name, Time.time);
			}
		}

		public bool GetTimer(string name, float minTimeToPass)
		{
			if (!useTimers)
				return false;

			float time;
			bool found = timers.TryGetValue(name, out time);

			if (!found)
				return true;
			return time + minTimeToPass <= Time.time;
		}

		public override void InitModules()
		{
			foreach (AIAttackModule module in attackModules)
			{
				module.Init();
			}
		}

		public override void Think()
		{
			if (Owner == null || Owner.GetData() == null)
				return;

			if (GetStatus().IsDead)
			{
				if (State != AIState.IDLE)
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

			//TODO once in 10 seconds add wall check
		}

		public Character GetMaster()
		{
			return ((Monster) Owner).GetMaster();
		}

		public bool HasMaster()
		{
			return ((Monster) Owner).HasMaster() && GetMaster() != null && GetMaster().Status.IsDead == false;
		}

		private void ThinkIdle()
		{
			if (GetStatus().IsDead)
				return;

			if (alwaysActive && !GetStatus().IsDead)
			{
				SetAIState(AIState.ACTIVE);
				return;
			}

			// pokud ma mastera, musi byt vzdy v aktivnim stavu (pro sledovani pohybu mastera, atd.)
			if (HasMaster())
			{
				SetAIState(AIState.ACTIVE);
				return;
			}

			if (aggro.Any())
			{
				SetAIState(AIState.ATTACKING);
				return;
			}

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

			/*if (State == AIState.IDLE && !isWalking)
			{
				SetIsWalking(true);
			}*/

			if (ReturnHomeIfNeeded())
				return;

			TryRambleAround();
		}

		protected virtual void ThinkActive()
		{
			bool stillActive = false;

			if (Owner.Knownlist.KnownObjects.Count > 0)
			{
				// muze si sam vybirat cile pouze pokud nema mastera
				//if (!HasMaster())
				{
					foreach (GameObject o in Owner.Knownlist.KnownObjects)
					{
						if (o == null) continue;

						Character ch = Utils.GetCharacter(o);
						if (ch == null)
							continue;

						stillActive = true;

						if (IsAggressive && Owner.CanAutoAttack(ch))
						{
							// find target that can be attacked
							if (Vector3.Distance(Owner.GetData().GetBody().transform.position, ch.GetData().GetBody().transform.position) < AggressionRange)
							{
								AddAggro(ch, 1);
								//NotifyNearbyAggroAdded(ch);
							}

							break;
						}
					}
				}

				// pokud master utoci, summon zacne utocit take (v attack metode se vybere cil mastera)
				if (HasMaster())
				{
					if (CheckIfTooFarFromMaster())
					{
						TryFollowLeader();
						return;
					}

					if (GetMaster().AI.State == AIState.ATTACKING)
					{
						AddAggro(GetMaster().AI.GetMainTarget(), 1);
					}					
				}

				if (aggro.Any())
				{
					SetAIState(AIState.ATTACKING);
					return;
				}
			}

			// vzdy aktivni pokud ma mastera
			if (!stillActive && GetMaster() != null)
				stillActive = true;

			if (!stillActive && !alwaysActive)
			{
				SetAIState(AIState.IDLE);
				return;
			}

			if (State == AIState.ACTIVE)
			{
				if (TryFollowLeader())
					return;

				if (ReturnHomeIfNeeded())
					return;

				if (TryRambleAround())
					return;
			}
		}

		private float lastNotify;
		public float notifyInterval = 1;

		private void NotifyNearbyAggroAdded(Character target)
		{
			if (!GetTemplate().AlertsAllies)
				return;

			if (lastNotify + notifyInterval < Time.time)
			{
				lastNotify = Time.time;
				Collider2D[] colliders = Physics2D.OverlapCircleAll(Owner.GetData().GetBody().transform.position, AggressionRange);

				foreach (Collider2D c in colliders)
				{
					if (c.gameObject == null)
						continue;

					AbstractData d = c.gameObject.GetData();

					if (d == null)
						continue;

					if (d.GetOwner() is Monster)
					{
						Monster monster = (Monster)d.GetOwner();

						if (monster.AI.State != AIState.ATTACKING)
						{
							monster.AI.AddAggro(target, 1);
							continue;
						}
					}
				}
			}
		}

		private void NotifySummonsAboutAttack(Character target)
		{
			/*if (Owner.HasSummons())
			{
				foreach (Monster summon in Owner.summons)
				{
					MonsterAI ai = summon.AI as MonsterAI;
				}
			}*/
		}

		private bool CheckIfTooFarFromMaster()
		{
			if (HasMaster())
			{
				float dist = Utils.DistanceSqr(Owner.Data.GetBody().transform.position, GetMaster().GetData().GetBody().transform.position);
				int distToFollow = ((EnemyData)Owner.GetData()).distanceToFollowLeader;

				if (dist > distToFollow*distToFollow + 30*30)
				{
					return true;
				}
			}
			return false;
		}

		private int tempAggroCounter = 0;

		protected virtual void ThinkAttack()
		{
			SetIsWalking(false);

			Character possibleTarget = SelectMostAggroTarget();

			if (CheckIfTooFarFromMaster())
			{
				SetAIState(AIState.ACTIVE);
				return;
			}

			// no target this monster hates - go back to active
			if (possibleTarget == null)
			{
				SetAIState(AIState.ACTIVE);
				return;
			}

			if (possibleTarget.Status.IsDead || possibleTarget.GetData() == null)
			{
				RemoveAggro(possibleTarget);
				return;
			}

			//TODO make sure ma tohle byt volano jen jednou ?
			NotifyNearbyAggroAdded(possibleTarget);
			NotifySummonsAboutAttack(possibleTarget);

			tempAggroCounter++;

			// target is too far (> attackdistance*2) - abandone attacking
			if (tempAggroCounter >= 3 && Vector3.Distance(Owner.GetData().GetBody().transform.position, possibleTarget.GetData().GetBody().transform.position) > AggressionRange * 2)
			{
				RemoveAggro(possibleTarget, 1);
				tempAggroCounter = 0;
			}

			AttackTarget(possibleTarget);
		}

		protected bool TryFollowLeader()
		{
			if (Owner.GetData().HasTargetToMoveTo)
			{
				if (HasMaster())
				{
					bool retargetMove = false;

					Vector3 leaderPos = GetMaster().Data.GetBody().transform.position;
					Vector3 moveDestination = Owner.GetData().GetMovementTarget();

					int distToFollow = ((EnemyData)Owner.GetData()).distanceToFollowLeader;

					if (Utils.DistanceSqr(moveDestination, leaderPos) > (distToFollow*distToFollow))
					{
						retargetMove = true;
					}

					if (!retargetMove)
						return false;
				}
			}

			// ma mastera - charakter ktery tento objekt vlastni
			if (HasMaster())
			{
				Character master = GetMaster();

				if (master.Data.GetBody() != null)
				{
					Vector3 leaderPos = master.Data.GetBody().transform.position;
					int distToFollow = ((EnemyData)Owner.GetData()).distanceToFollowLeader;

					if (Utils.DistanceSqr(Owner.Data.GetBody().transform.position, leaderPos) > (distToFollow * distToFollow))
					{
						Vector3 rnd = Owner.Data.GetBody().transform.position;

						try
						{
							rnd = Utils.GenerateFixedPositionAroundObject(master.GetData().gameObject, distToFollow - 1, -90);
							rnd.z = 0;
						}
						catch (Exception)
						{
						}

						SetIsWalking(false);
						MoveTo(rnd);

						//Debug.DrawRay(Owner.GetData().GetBody().transform.position, leaderPos - Owner.GetData().GetBody().transform.position, Color.blue, 5);

						return true;
					}
				}
			} // ma skupinu - nasleduje vudce ale udrzuje si jinak samostatnost
			else if (GetGroupLeader() != null && !IsGroupLeader)
			{
				Character leader = GetGroupLeader();

				if (leader.Data.GetBody() != null)
				{
					Vector3 leaderPos = leader.Data.GetBody().transform.position;

					int distToFollow = ((EnemyData)Owner.GetData()).distanceToFollowLeader;

					if (Utils.DistanceSqr(Owner.Data.GetBody().transform.position, leaderPos) > distToFollow * distToFollow)
					{
						Vector3 rnd = Utils.GenerateRandomPositionAround(leaderPos, distToFollow - 1);
						rnd.z = 0;

						SetIsWalking(false);
						MoveTo(rnd);

						return true;
					}
				}
			}

			return false;
		}

		protected bool ReturnHomeIfNeeded()
		{
			if (HasMaster())
				return false;

			if (!Owner.GetData().HasTargetToMoveTo && (!IsInGroup() || IsGroupLeader))
			{
				if (Utils.DistanceSqr(homeLocation, Owner.GetData().GetBody().transform.position) > GetTemplate().RambleAroundMaxDist * GetTemplate().RambleAroundMaxDist)
				{
					SetIsWalking(false);
					MoveTo(homeLocation);
					return true;
				}
			}

			return false;
		}

		protected bool TryRambleAround()
		{
			if (HasMaster())
				return false;

			if (GetTemplate().RambleAround && !Owner.GetData().HasTargetToMoveTo)
			{
				if (lastRambleTime + rambleInterval < Time.time)
				{
					//if (Random.Range(1, 4) < 3)
					{
						lastRambleTime = Time.time;
						DoRambleAround();
						return true;
					}
				}
			}

			return false;
		}

		protected void SetIsWalking(bool b)
		{
			if (isWalking && !b)
			{
				Owner.SetIsWalking(false);
				//Owner.SetMoveSpeed(GetTemplate().MaxSpeed);
				isWalking = false;
			}
			else if (!isWalking && b)
			{
				Owner.SetIsWalking(true);
				//if (GetTemplate().MaxSpeed > 3)
				//	Owner.SetMoveSpeed(3);

				isWalking = true;
			}
		}

		private void DoRambleAround()
		{
			bool found = false;

			int limit = 5;
			while (!found)
			{
				int dist = GetTemplate().RambleAroundMaxDist;
				Vector3 randomPos = Utils.GenerateRandomPositionAround(homeLocation, dist);

				//TODO region check
				//TODO add object collision check on target around, to check if he can fit

				bool collides = false;
				foreach (RaycastHit2D r2d in Physics2D.RaycastAll(Owner.GetData().GetBody().transform.position, randomPos - Owner.GetData().GetBody().transform.position, Vector3.Distance(Owner.GetData().GetBody().transform.position, randomPos)))
				{
					if (r2d.collider == null)
						continue;

					if (r2d.collider.gameObject.Equals(Owner.GetData().GetBody()))
						continue;

					collides = true;
				}

				if (collides)
				{
					Debug.DrawRay(Owner.GetData().GetBody().transform.position, randomPos - Owner.GetData().GetBody().transform.position, Color.blue, 5);
				}
				else
				{
					found = true;

					SetIsWalking(true);

					MoveTo(randomPos);
					Debug.DrawRay(Owner.GetData().GetBody().transform.position, randomPos - Owner.GetData().GetBody().transform.position, Color.green, 5);
				}

				limit--;
				if (limit <= 0)
					break;
			}
		}

		public override void AddAggro(Character ch, int points)
		{
			if(ch == null)
				return;

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

		public void ClearAggro()
		{
			aggro.Clear();
		}

		public override int GetAggro(Character ch)
		{
			int val = 0;
			aggro.TryGetValue(ch, out val);
			return val;
		}

		public override void CopyAggroFrom(AbstractAI sourceAi)
		{
			if (sourceAi is MonsterAI)
			{
				foreach (KeyValuePair<Character, int> e in ((MonsterAI)sourceAi).aggro)
				{
					AddAggro(e.Key, e.Value);
				}
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

		public override void RemoveAllAggro()
		{
			aggro.Clear();
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

		public virtual IEnumerator RunAway(Character target, float distance, int randomAngleAdd)
		{
			Vector3 dirVector = -Utils.GetDirectionVector(target.GetData().GetBody().transform.position, Owner.GetData().transform.position).normalized * distance;

			int randomDir = Random.Range(-randomAngleAdd, randomAngleAdd);
			Vector3 nv = Quaternion.Euler(new Vector3(0, 0, randomDir)) * dirVector;

			//TODO add wall checking ! 

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

        public virtual IEnumerator MoveAction(Vector3 target, bool fixedRotation, float fixedSpeed=-1)
        {
            MoveTo(target, fixedRotation, fixedSpeed);

            yield return null;

            while (Owner.GetData().HasTargetToMoveTo)
            {
                yield return null;
            }

            currentAction = null;
        }

		public virtual IEnumerator CastSkillWithTargetSet(Vector3 target, ActiveSkill sk, float distSqrToTarget, bool noRangeCheck, bool moveTowardsIfRequired, float skillRangeAdd = 0, float randomSkilLRangeAdd = 0)
		{
			if (!noRangeCheck && sk.range != 0)
			{
				while (Mathf.Pow((sk.range + Random.Range(-randomSkilLRangeAdd, randomSkilLRangeAdd)), 2) < distSqrToTarget)
				{
					//distSqrToTarget = Vector3.Distance(target, Owner.GetData().transform.position);
					distSqrToTarget = Utils.DistanceSqr(target, Owner.GetData().transform.position);

					if (moveTowardsIfRequired)
					{
						MoveTo(target); //TODO not working with pathnodes - replace with setmovementtarget
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

			Owner.CastSkill(sk, target);
			currentAction = null;
		}

		public virtual IEnumerator CastSkill(Vector3 target, ActiveSkill sk, float distSqrToTarget, bool noRangeCheck, bool moveTowardsIfRequired, float skillRangeAdd=0, float randomSkilLRangeAdd=0)
		{
			if (!noRangeCheck && sk.range != 0)
			{
				while (Mathf.Pow((sk.range + Random.Range(-randomSkilLRangeAdd, randomSkilLRangeAdd)), 2) < distSqrToTarget)
				{
					//distSqrToTarget = Vector3.Distance(target, Owner.GetData().transform.position);
					distSqrToTarget = Utils.DistanceSqr(target, Owner.GetData().transform.position);

					if (moveTowardsIfRequired)
					{
						MoveTo(target); //TODO not working with pathnodes - replace with setmovementtarget
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

			Owner.CastSkill(sk, null, true);
			currentAction = null;
		}

		public virtual IEnumerator CastSkill(Character target, ActiveSkill sk, float distSqrToTarget, bool noRangeCheck, bool moveTowardsIfRequired=true, float skillRangeAdd=0, float randomSkilLRangeAdd=0)
		{
			if (target != null && !noRangeCheck && sk.range != 0)
			{
				while (Mathf.Pow((sk.range + Random.Range(-randomSkilLRangeAdd, randomSkilLRangeAdd)), 2) * (0.6f+skillRangeAdd) < distSqrToTarget)
				{
					distSqrToTarget = Utils.DistanceSqr(target.GetData().transform.position, Owner.GetData().transform.position);

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

			if (target != null)
				RotateToTarget(target);

			Owner.CastSkill(sk);
			currentAction = null;
		}

		public bool LaunchAttackModule(Character target, float distSqr, float hpPercentage)
		{
			if (currentPrioridyModule != null)
			{
				if (currentPrioridyModuleTime + currentPrioridyModule.keepActiveFor >= Time.time)
				{
					if (currentPrioridyModule.ForceLaunch(target, distSqr, hpPercentage))
						return true;
				}
				else
				{
					currentPrioridyModule = null;
				}
			}

			foreach (AIAttackModule module in attackModules)
			{
				if (module.Launch(target, distSqr, hpPercentage, attackStartedTime))
				{
					if (module.keepActiveFor > 0)
					{
						currentPrioridyModule = module;
						currentPrioridyModuleTime = Time.time;
					}

					return true;
				}
			}

			return false;
		}

		protected override void OnSwitchIdle()
		{
			ThinkInterval = 3f;
		}

		protected override void OnSwitchActive()
		{
			ThinkInterval = 0.25f;
			attackStartedTime = 0;
		}

		protected override void OnSwitchAttacking()
		{
			ThinkInterval = 0.2f;
			attackStartedTime = Time.time;
		}

		public MonsterTemplate GetTemplate()
		{
			return ((Monster)Owner).Template;
		}
	}
}
