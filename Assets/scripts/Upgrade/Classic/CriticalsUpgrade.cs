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
	public class CriticalRateUpgrade : AbstractUpgrade
	{
		public static int rarity = 1;

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
			Name = "critrate_upgrade";
			VisibleName = "Critical Rate Upgrade";
			Description = "Increases Critical rate by " + 3 * Level + "%.";
		}
	}

	public class CriticalDamageUpgrade : AbstractUpgrade
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
			Name = "critdmg_upgrade";
			VisibleName = "Critical Damage Upgrade";
			Description = "Increases Critical damage by " + Level * 10 + "%.";
		}
	}
}
