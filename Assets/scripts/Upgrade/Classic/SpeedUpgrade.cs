﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Skills;
using Assets.scripts.Skills.ActiveSkills;
using Assets.scripts.Skills.Base;
using Assets.scripts.Skills.SkillEffects;

namespace Assets.scripts.Upgrade.Classic
{
	public class SpeedUpgrade : AbstractUpgrade
	{
		public static int rarity = 1;

		public SpeedUpgrade(int level)
			: base(level)
		{
			GoesIntoBasestatSlot = true;
		}

		public override void ModifyRunSpeed(ref float runSpeed)
		{
			runSpeed += (0.5f*Level);
		}

		protected override void InitInfo()
		{
			Name = "speed_upgrade";
			VisibleName = "Speed Upgrade";
			Description = "Increases move speed by " + 0.5f * Level + ".";
		}
	}
}