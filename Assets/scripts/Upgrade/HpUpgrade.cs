using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Skills;
using Assets.scripts.Skills.ActiveSkills;
using Assets.scripts.Skills.Base;
using Assets.scripts.Skills.SkillEffects;

namespace Assets.scripts.Upgrade
{
	public class HpUpgrade : AbstractUpgrade
	{
		public HpUpgrade(int level) : base(level)
		{
			Name = "hp_upgrade";
			VisibleName = "HP Upgrade";
		}

		public override void ModifyMaxHp(ref int maxHp)
		{
			maxHp *= Level + 1;
		}
	}
}
