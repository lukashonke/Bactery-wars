using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Skills;
using UnityEngine;

namespace Assets.scripts.AI
{
	public abstract class AbstractAI
	{
		public Character Owner { get; private set; }
		public AIState State { get; private set; }

		public float ThinkInterval { get; set; }
		private bool active;
		private Coroutine task;

		private Character MainTarget { get; set; }
		protected List<Character> Targets { get; private set; } 

		protected AbstractAI(Character o)
		{
			Owner = o;

			State = AIState.IDLE;
			ThinkInterval = 1f;

			Targets = new List<Character>();
		}

		public void StartAITask()
		{
			if (active || task != null)
				return;

			Init();

			active = true;
			task = Owner.StartTask(AITask());
		}

		public void StopAITask()
		{
			if (active && task != null)
			{
				active = false;
				Owner.StopTask(task);
				task = null;
			}
		}

		private void Init()
		{
			AnalyzeSkills();
		}

		private void AnalyzeSkills()
		{
			foreach (Skill sk in Owner.Skills.Skills)
			{
				
			}
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
		public abstract void OnSwitchIdle();
		public abstract void OnSwitchActive();
		public abstract void OnSwitchAttacking();
		public abstract void AddAggro(Character ch, int points);
		public abstract void RemoveAggro(Character ch, int points);

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
	}
}
