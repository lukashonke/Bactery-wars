using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Skills;
using Assets.scripts.Skills.Base;
using UnityEngine;
using Random = System.Random;

namespace Assets.scripts.AI.Modules
{
	public class EvadeModule : AIAttackModule
	{
		public float maxRange = 4f;
		public float minRange = 3f;

		public EvadeModule(MonsterAI ai) : base(ai)
		{
		}

		public override void Init()
		{
			available = true;
		}

		public override bool Trigger(Character target, float distSqr)
		{
			if (interval > -1) // interval must be at least -1
			{
				Vector3 pos = Utils.GenerateRandomPositionAround(ai.Owner.GetData().GetBody().transform.position, maxRange, minRange);

				if (ai.StartAction(ai.MoveAction(pos, true), 3f))
				{
					Debug.DrawLine(pos, ai.Owner.GetData().transform.position, Color.blue, 4f);
					return true;
				}
			}

			return false;
		}
	}
}
