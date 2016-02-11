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
	public class HpUpgrade : AbstractUpgrade
	{
		public static int rarity = 1;

		public HpUpgrade(int level) : base(level)
		{
		}

		protected override void InitInfo()
		{
			Name = "hp_upgrade";
			VisibleName = "HP Upgrade";
			Description = "Increases maximum HP by " + (6 * Level) + "%.";
		}

		public override void ModifyMaxHp(ref int maxHp)
		{
			maxHp = (int) (maxHp*(Level*1.06f));
		}
	}

	public class HpUpgradeAdd : AbstractUpgrade
	{
		public static int rarity = 1;

		public HpUpgradeAdd(int level)
			: base(level)
		{
			GoesIntoBasestatSlot = true;
		}

		protected override void InitInfo()
		{
			Name = "hp_upgrade";
			VisibleName = "HP Upgrade";
			Description = "Increases maximum HP by " + (5 * Level) + ".";
		}

		public override void ModifyMaxHp(ref int maxHp)
		{
			maxHp += (Level*5);
		}
	}
}
