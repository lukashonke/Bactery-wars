// Copyright (c) 2015, Lukas Honke
// ========================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Skills;
using Assets.scripts.Skills.ActiveSkills;
using Assets.scripts.Skills.Base;
using Assets.scripts.Skills.SkillEffects;

namespace Assets.scripts.Upgrade.Classic
{
	public class CriticalRateUpgrade : EquippableItem
	{
		public static int rarity = 1;
		public static ItemType type = ItemType.STAT_UPGRADE;

		public CriticalRateUpgrade(int level)
			: base(level)
		{
			GoesIntoBasestatSlot = true;
		}

		public override void ModifyCriticalRate(ref int critRate)
		{
			critRate += (30*Level);
		}

		protected override void InitInfo()
		{
			FileName = "critrate_upgrade";
			VisibleName = "Critical Rate Module";
			Description = "Increases Critical rate by " + 3 * Level + "%.";
		}
	}

	public class CriticalDamageUpgrade : EquippableItem
	{
		public static int rarity = 1;

		public CriticalDamageUpgrade(int level)
			: base(level)
		{
			GoesIntoBasestatSlot = true;
		}

		public override void ModifyCriticalDmg(ref float critDmg)
		{
			critDmg += (0.1f*Level);
		}

		protected override void InitInfo()
		{
			FileName = "critdmg_upgrade";
			VisibleName = "Critical Damage Module";
			Description = "Increases Critical damage by " + Level * 10 + "%.";
		}
	}
}
