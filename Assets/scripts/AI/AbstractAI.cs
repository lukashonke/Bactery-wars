﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
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

		protected AbstractAI(Character o)
		{
			Owner = o;

			State = AIState.IDLE;
			ThinkInterval = 1f;
		}

		public void StartAITask()
		{
			if (active || task != null)
				return;

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

		private IEnumerator AITask()
		{
			while (active)
			{
				if (ThinkInterval == 0) break;

				Think();
				yield return new WaitForSeconds(ThinkInterval);
			}
		}
	}
}
