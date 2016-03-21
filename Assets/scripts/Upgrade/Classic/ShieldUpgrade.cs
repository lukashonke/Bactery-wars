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
	public class ShieldUpgrade : EquippableItem
	{
		public static int rarity = 1;

		public ShieldUpgrade(int level)
			: base(level)
		{
			GoesIntoBasestatSlot = true;
		}

		public override void ModifyShield(ref float shield)
		{
			shield += 0.05f * (Level);			
		}

		protected override void InitInfo()
		{
			FileName = "shield_upgrade";
			VisibleName = "Shield Module";
			Description = "Increases your shield by " + (0.05f * (Level) * 100) + "%.";
		}
	}
}
