using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;

namespace Assets.scripts.AI
{
	public class CrystalAI : MonsterAI
	{
		public CrystalAI(Character o) : base(o)
		{
		}

		public override void Think()
		{
			if (Owner == null)
				return;

			if (State != AIState.IDLE)
				SetAIState(AIState.IDLE);
		}

		protected override void AttackTarget(Character target)
		{
		}
	}
}
