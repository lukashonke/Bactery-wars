// Copyright (c) 2015, Lukas Honke
// ========================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Mono;
using Assets.scripts.Skills.Base;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.scripts.Skills.SkillEffects
{
	public class EffectDamageAll : EffectDamage
	{
		public EffectDamageAll(int damage, int randomOffset, int criticalRate) : base(damage, randomOffset, criticalRate)
		{
			DamageFriendly = true;
		}

		public EffectDamageAll(int damage, int randomOffset) : base(damage, randomOffset)
		{
			DamageFriendly = true;
		}

		public EffectDamageAll(int damage) : base(damage)
		{
			DamageFriendly = true;
		}
	}
}
