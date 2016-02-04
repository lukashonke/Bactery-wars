using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Actor.Status;
using Assets.scripts.AI.Base;
using Assets.scripts.Skills;
using Assets.scripts.Skills.Base;
using UnityEngine;

namespace Assets.scripts.AI
{
	public abstract class AbstractAI
	{
		public Character Owner { get; private set; }
		public AIState State { get; private set; }

		public float ThinkInterval { get; set; }
		private bool active;
		private Coroutine mainTask;

		protected Coroutine currentAction;
	    protected bool currentActionCancelable;

		private Character MainTarget { get; set; }
		protected List<Character> Targets { get; private set; }

		public bool IsGroupLeader { get; set; }
		public AIGroup Group { get; set; }

		protected Vector3 homeLocation;

		protected AbstractAI(Character o)
		{
			Owner = o;

			State = AIState.IDLE;
			ThinkInterval = 1f;

			Targets = new List<Character>();
		}

		public void StartAITask()
		{
			if (active || mainTask != null)
				return;

			Init();

			active = true;
			mainTask = Owner.StartTask(AITask());
		}

		public void StopAITask()
		{
			if (active && mainTask != null)
			{
				active = false;
				Owner.StopTask(mainTask);
				mainTask = null;
			}
		}

		private void Init()
		{
			homeLocation = Owner.GetData().GetBody().transform.position;
		}

		protected bool StartAction(IEnumerator task, float timeLimit, bool canBeCancelled=false, bool forceReplace=false)
		{
		    if (currentAction == null || currentActionCancelable || forceReplace)
		    {
                //TODO cancel the previous action?
		        if (currentAction != null)
		        {
					Debug.Log("breaking");
		            BreakCurrentAction();
		        }

                currentAction = Owner.StartTask(task);
                currentActionCancelable = canBeCancelled;
                Owner.StartTask(ActionTimeLimit(timeLimit));
		        return true;
            }

		    return false;
		}

		private IEnumerator ActionTimeLimit(float timeLimit)
		{
			yield return new WaitForSeconds(timeLimit);
			BreakCurrentAction();
		}

		protected void BreakCurrentAction()
		{
			if (currentAction != null)
			{
				Owner.StopTask(currentAction);

				currentAction = null;
			    currentActionCancelable = true;
			}
		}

		public virtual void AnalyzeSkills()
		{

		}

		public virtual void SetAIState(AIState st)
		{
			switch (st)
			{
				case AIState.IDLE:
					State = AIState.IDLE;
					OnSwitchIdle();
					break;
				case AIState.ACTIVE:
					State = AIState.ACTIVE;
					OnSwitchActive();
					break;
				case AIState.ATTACKING:
					State = AIState.ATTACKING;
					OnSwitchAttacking();
					break;
			}
		}

		public abstract void Think();
		protected abstract void OnSwitchIdle();
		protected abstract void OnSwitchActive();
		protected abstract void OnSwitchAttacking();
		public abstract void AddAggro(Character ch, int points);
		public abstract void RemoveAggro(Character ch, int points);
		public abstract int GetAggro(Character ch);
		public abstract void CopyAggroFrom(AbstractAI sourceAi);

		private IEnumerator AITask()
		{
			while (active)
			{
				if (ThinkInterval == 0) break;

				Think();
				yield return new WaitForSeconds(ThinkInterval);
			}
		}

		public Character GetMainTarget()
		{
			return MainTarget;
		}

		public void SetMainTarget(Character o)
		{
			MainTarget = o;
		}

		public void AddTarget(Character o)
		{
			Targets.Add(o);
		}

		public void RemoveTarget(Character o)
		{
			Targets.Remove(o);
		}

		public void RemoveMainTarget()
		{
			MainTarget = null;
		}

		public void CreateGroup()
		{
			Group = new AIGroup(Owner);
		}

		public void JoinGroup(Character target)
		{
			if (target.AI.IsInGroup())
			{
				target.AI.Group.AddMember(Owner);
			}
		}

		public Character GetGroupLeader()
		{
			if (!IsInGroup())
				return null;

			return Group.GetLeader();
		}

		public bool IsInGroup()
		{
			return Group != null;
		}

		/// <summary>
		/// Makes the main target the first element in 'Targets' List
		/// </summary>
		public void ReconsiderMainTarget()
		{
			if (Targets.Count > 0)
			{
				MainTarget = Targets.First();
				RemoveTarget(MainTarget);
			}
		}

		public CharStatus GetStatus()
		{
			return Owner.Status;
		}

		public Skill GetSkillWithTrait(SkillTraits trait)
		{
			foreach (Skill sk in Owner.Skills.Skills)
			{
				if (sk.Traits.Contains(trait))
				{
					return sk;
				}
			}
			return null;
		}

		public Skill GetSkillWithTrait(params SkillTraits[] traits)
		{
			bool containsAll;
			foreach (Skill sk in Owner.Skills.Skills)
			{
				containsAll = true;

				foreach (SkillTraits t in traits)
				{
					if (!sk.Traits.Contains(t))
					{
						containsAll = false;
						break;
					}
				}

				if (containsAll)
					return sk;
			}
			return null;
		}

		public List<Skill> GetAllSkillsWithTrait(SkillTraits trait)
		{
			List<Skill> skills = new List<Skill>();
			foreach (Skill sk in Owner.Skills.Skills)
			{
				if (sk.Traits.Contains(trait))
				{
					skills.Add(sk);
				}
			}
			return skills;
		}

		public bool HasMeleeSkill()
		{
			return Owner.MeleeSkill != null;
		}

		public List<Skill> GetAllSkillsWithTrait(params SkillTraits[] traits)
		{
			bool containsAll;
			List<Skill> skills = new List<Skill>();

			foreach (Skill sk in Owner.Skills.Skills)
			{
				containsAll = true;

				foreach (SkillTraits t in traits)
				{
					if (!sk.Traits.Contains(t))
					{
						containsAll = false;
						break;
					}
				}

				if (containsAll)
					skills.Add(sk);
			}
			return skills;
		}

		protected void RotateToTarget(Character target)
		{
			Owner.GetData().SetRotation(target.GetData().GetBody().transform.position, true);
		}

		protected void RotateToTarget(Vector3 target)
		{
			Owner.GetData().SetRotation(target, true);
		}

		protected void MoveTo(Character target)
		{
			Owner.GetData().MoveTo(target.GetData().GetBody());
		}

		protected bool MoveTo(Vector3 target, bool fixedRotation=false, int fixedSpeed=-1)
		{
			return Owner.GetData().MoveTo(target, fixedRotation, fixedSpeed);
		}

		public virtual void SetMaster(Character master)
		{
			
		}
	}
}
