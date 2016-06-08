// Copyright (c) 2015, Lukas Honke
// ========================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Skills;
using Assets.scripts.Skills.Base;
using UnityEngine;

namespace Assets.scripts.AI.Modules
{
	/// <summary>
	/// vybere nejblizsiho spojence jako sveho mastera
	/// </summary>
	public class FindLeaderModule : AIAttackModule
	{
		public float radius = 20f;

		public FindLeaderModule(MonsterAI ai)
			: base(ai)
		{
		}

		public override void Init()
		{
			canTrigger = ai.HasMaster() == false;
		}

		public override bool Trigger(Character target, float distSqr)
		{
			if (ai.HasMaster())
				return false;

			List<Character> candidates = new List<Character>();

			Vector3 thisPos = ai.Owner.GetData().GetBody().transform.position;
			foreach (Collider2D c in Physics2D.OverlapCircleAll(thisPos, radius))
			{
				GameObject g = c.gameObject;
				Character character = g.GetChar();

				if (character == null)
					continue;

				if (character.Team == ai.Owner.Team)
				{
					candidates.Add(character);
				}
			}

			Character newAlly = null;

			float lowestDist = 99999999999;

			foreach (Character cand in candidates)
			{
				float d = Utils.DistanceSqr(thisPos, cand.GetData().GetBody().transform.position);

				if (d < lowestDist)
				{
					newAlly = cand;
					lowestDist = d;
				}
			}

			if (newAlly != null)
			{
				ai.SetMaster(newAlly);
			}

			return true;
		}
	}
}
