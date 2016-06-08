// Copyright (c) 2015, Lukas Honke
// ========================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.scripts.Skills.Base
{
	public enum SkillTraits
	{
		Damage,
		AreaDamage,
		AuraDamage,
		Escape,
		Jump,
		Teleport,
		Missile,
		BuffDamage,
		BuffDefense,
		SpawnMinion,

		Pull,
		Push,
		Kamikadze,
		Immobilize,

		Melee,

		Heal,
		HealSelf,


		ShortRange,
		LongRange,
		ShortCastingTime,
		LongCastingTime,
		LongReuse,
		ShortReuse,

		None,
	}
}
