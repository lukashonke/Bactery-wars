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
	public class DamageUpgrade : EquippableItem
	{
		public static int rarity = 1;
		public static ItemType type = ItemType.STAT_UPGRADE;

		public DamageUpgrade(int level)
			: base(level)
		{
			GoesIntoBasestatSlot = true;
		}

		public override void ModifyDmgMul(ref float dmgMul)
		{
			dmgMul += 0.05f * (Level);
		}

		protected override void InitInfo()
		{
			FileName = "damage_upgrade";
			VisibleName = "Damage Module";
			Description = "Increases all damage done by " + (0.05 * (Level) * 100) + "%.";
		}
	}
}
