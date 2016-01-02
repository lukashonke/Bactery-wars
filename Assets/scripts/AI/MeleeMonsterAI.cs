using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Debug = UnityEngine.Debug;

namespace Assets.scripts.AI
{
	public class MeleeMonsterAI : MonsterAI
	{
		public MeleeMonsterAI(Character o) : base(o)
		{
		}

		protected override void AttackTarget(Character target)
		{
			SetMainTarget(target);

			bool isCasting = Owner.GetData().IsCasting;
			bool isMeleeAttacking = Owner.GetData().IsMeleeAttacking();

			// already doing something
			if (isCasting || currentAction != null)
				return;

			if (Owner.GetData().Target == null || Owner.GetData().Target.Equals(target.GetData().GetBody()))
				Owner.GetData().Target = target.GetData().GetBody();

			Owner.GetData().MeleeInterract(target.GetData().GetBody(), true);
		}
	}
}
