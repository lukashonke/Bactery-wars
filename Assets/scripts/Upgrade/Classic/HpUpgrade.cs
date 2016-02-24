﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Skills;
using Assets.scripts.Skills.ActiveSkills;
using Assets.scripts.Skills.Base;
using Assets.scripts.Skills.SkillEffects;
using UnityEngine;

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

	public class Heal : AbstractUpgrade
	{
		public static int rarity = 1;

		public Heal(int level) : base(level)
		{
		}

		public override bool OnPickup(Character owner)
		{
			owner.ReceiveHeal(owner, Level * 10);
			GameSystem.Instance.CurrentPlayer.GetData().ui.ObjectMessage(owner.GetData().GetBody(), "Heal " + Level*10, Color.green);
			return true;
		}

		protected override void InitInfo()
		{
			Name = "hp_upgrade";
			VisibleName = "Heal";
			Description = "Heals you.";
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
