﻿using System;
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
	public class FloatModule : AIAttackModule
	{
		public float maxRange = 10;
		public float floatRange = 5;
		public float floatSpeed = 4;

		private int lastAngle;

		public FloatModule(MonsterAI ai) : base(ai)
		{
		}

		public override void Init()
		{
			available = true;
		}

		public override bool Trigger(Character target, float distSqr)
		{
			if (distSqr < Math.Pow(maxRange, 2))
			{
				// circulate target
				Vector3 pos = Utils.GenerateRandomPositionOnCircle(target.GetData().GetBody().transform.position, floatRange, lastAngle);

				if (ai.StartAction(ai.MoveAction(pos, true, floatSpeed), 2f, false))
				{
					lastAngle = lastAngle + UnityEngine.Random.Range(-2, 2);
					Debug.DrawLine(pos, ai.Owner.GetData().transform.position, Color.green, 4f);
					return true;
				}
			}

			return false;
		}
	}
}
