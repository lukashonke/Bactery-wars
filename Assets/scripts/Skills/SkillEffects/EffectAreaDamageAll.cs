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
	public class EffectAreaDamageAll : EffectAreaDamage
	{
		public EffectAreaDamageAll(int damage, int randomOffset, float radius)
			: base(damage, randomOffset, radius)
		{
			this.attackAll = true;
		}
	}
}
