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

		protected override void OnSwitchIdle()
		{
		}

		protected override void OnSwitchActive()
		{
		}

		protected override void OnSwitchAttacking()
		{
		}

		public override void AddAggro(Character ch, int points)
		{
		}

		public override void RemoveAggro(Character ch, int points)
		{
		}

		public override int GetAggro(Character ch)
		{
			return 0;
		}

		public override void CopyAggroFrom(AbstractAI sourceAi)
		{
		}
	}
}
