using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;

namespace Assets.scripts.AI
{
	public class PlayerAI : AbstractAI
	{
		public PlayerAI(Character o) : base(o)
		{
			ThinkInterval = 0;
		}

		public override void Think()
		{
			// do nothing
		}

		public override void OnSwitchIdle()
		{
		}

		public override void OnSwitchActive()
		{
		}

		public override void OnSwitchAttacking()
		{
		}
	}
}
