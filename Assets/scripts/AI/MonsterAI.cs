using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;

namespace Assets.scripts.AI
{
	public class MonsterAI : AbstractAI
	{
		public MonsterAI(Character o) : base(o)
		{
			
		}

		public override void Think()
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
