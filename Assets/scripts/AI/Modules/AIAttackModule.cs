// Copyright (c) 2015, Lukas Honke
// ========================
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using UnityEngine;
using Random = System.Random;

namespace Assets.scripts.AI.Modules
{
	public abstract class AIAttackModule
	{
		public int id;

		public bool enabled;
		public bool canTrigger;

		// how often is this module Triggered (-1 = every AI tick)
		public float interval;

		// what is the chance that this module will be Triggered (-1 = skip chance check)
		public int chance;

		// minHp / maxHp - required HP in percent the character needs to have in order to trigger this module
		// 100 - 100% hp (-1 to skip this check)
		public float minHp;
		public float maxHp;

		// the min/max distance between this character and his target in order to trigger this module
		// -1 to skip this check
		public float minDistance;
		public float maxDistance;

		public float keepActiveFor;
		public float keepActiveForInterval; // pokud je aktivovan pres parametr keepActiveFor, pouzije se keepActiveForInterval jako casovac misto prom. interval
		public float tryInterval;
		public float activateAfterTime;
		public float chanceCheckReuse;

		private float lastTryTime;
		private float lastActionTime;
		private float lastChanceTryTime;
		protected MonsterAI ai;

		public AIAttackModule(MonsterAI ai)
		{
			this.ai = ai;

			enabled = true;

			canTrigger = false;
			interval = -1;
			chance = -1;

			minHp = -1;
			maxHp = -1;

			minDistance = -1;
			maxDistance = -1;

			keepActiveFor = -1;
			keepActiveForInterval = -1;
			tryInterval = -1;
			chanceCheckReuse = -1;

			activateAfterTime = -1;
		}

		public abstract void Init();

		public bool Launch(Character target, float distSqr, float hpPercentage, float attackStartedTime)
		{
			if (CanTrigger(target, distSqr, hpPercentage, attackStartedTime))
			{
				if (Trigger(target, distSqr))
				{
					lastActionTime = Time.time;
					return true;
				}
			}

			return false;
		}

		public bool ForceLaunch(Character target, float distSqr, float hpPercentage)
		{
			if (enabled && canTrigger && ((lastActionTime + keepActiveForInterval < Time.time) || keepActiveForInterval < 0))
			{
				if (Trigger(target, distSqr))
				{
					lastActionTime = Time.time;
					return true;
				}
			}

			return false;
		}

		/// <returns>true if launched, false if not launched</returns>
		public abstract bool Trigger(Character target, float distSqr);

		public bool CanTrigger(Character target, float distSqr, float hpPercentage, float attackStartedTime)
		{
			if (!canTrigger || !enabled)
				return false;

			if (tryInterval < 0 || (lastTryTime + tryInterval <= Time.time))
			{
				lastTryTime = Time.time;

				if (interval < 0 || lastActionTime + interval < Time.time)
				{
					if (minHp < 0 || hpPercentage >= minHp)
					{
						if (maxHp < 0 || hpPercentage <= maxHp)
						{
							if (minDistance < 0 || distSqr >= Mathf.Pow(minDistance, 2))
							{
								if (maxDistance < 0 || distSqr <= Mathf.Pow(maxDistance, 2))
								{
									if (activateAfterTime < 0 || (attackStartedTime + activateAfterTime <= Time.time))
									{
										// vsechny podminky splnene, cas zkontrolovat sanci
										if (chanceCheckReuse < 0 || lastChanceTryTime + chanceCheckReuse < Time.time)
										{
											if (chance < 0 || UnityEngine.Random.Range(1, 100) < chance)
											{
												lastChanceTryTime = Time.time;
												return true;
											}
											// neproslo pres kontrolu sance
											else if (chance > 0)
											{
												lastChanceTryTime = Time.time;
											}
										}
									}
								}
							}
						}
					}
				}
			}

			return false;
		}

		public int GetHpPercentage()
		{
			return (int)((ai.GetStatus().Hp / (float)ai.GetStatus().MaxHp) * 100);
		}
	}
}
