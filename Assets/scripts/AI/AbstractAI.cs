using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;

namespace Assets.scripts.AI
{
	public abstract class AbstractAI
	{
		public Character Owner { get; private set; }
		public AIState State { get; private set; }

		protected AbstractAI(Character o)
		{
			Owner = o;

			State = AIState.IDLE;
        }

		public virtual void Think()
		{
			switch (State)
			{
				case AIState.IDLE:

					break;
				case AIState.ACTIVE:

					break;
				case AIState.ATTACKING:

					break;
			}
		}
	}
}
