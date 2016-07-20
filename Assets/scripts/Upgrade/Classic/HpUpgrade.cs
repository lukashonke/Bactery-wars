// Copyright (c) 2015, Lukas Honke
// ========================
using System;
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
	public class HpUpgrade : EquippableItem
	{
		public static int rarity = 1;
		public static ItemType type = ItemType.STAT_UPGRADE;

		public HpUpgrade(int level) : base(level)
		{
		}

		protected override void InitInfo()
		{
			FileName = "hp_upgrade";
			VisibleName = "HP Module";
			Description = "Increases maximum HP by " + (6 * Level) + "%.";
		}

		public override void ModifyMaxHp(ref int maxHp)
		{
			maxHp = (int) (maxHp*(Level*1.06f));
		}
	}

	public class Heal : InventoryItem
	{
		public static int rarity = 1;

		public Heal(int level) : base(level)
		{
		}

		public override bool OnPickup(Character owner)
		{
			owner.ReceiveHeal(owner, Level * 10);
			GameSystem.Instance.CurrentPlayer.GetData().ui.ObjectMessage(owner.GetData().GetBody(), "Heal +" + Level*10, Color.green);
			return true;
		}

		protected override void InitInfo()
		{
			FileName = "hp_upgrade";
			VisibleName = "Heal";
			Description = "Immediately restore " + Level + " HP.";
		}
	}

	public class HpPotion : ActivableItem
	{
		public static int rarity = 1;
		public static ItemType type = ItemType.CONSUMABLE;

		public HpPotion(int level) : base(level)
		{
		}

		public override bool OnActivate()
		{
			owner.ReceiveHeal(owner, Level * 10);
			GameSystem.Instance.CurrentPlayer.GetData().ui.ObjectMessage(owner.GetData().GetBody(), "Heal +" + Level * 10, Color.green);
			return true;
		}

		protected override void InitInfo()
		{
			FileName = "hp_upgrade";
			VisibleName = "Healing Potion";
			Description = "Heals you for " + Level + " HP.";
		}
	}

	public class HpUpgradeAdd : EquippableItem
	{
		public static int rarity = 1;
		public static ItemType type = ItemType.STAT_UPGRADE;

		public HpUpgradeAdd(int level)
			: base(level)
		{
			GoesIntoBasestatSlot = true;
		}

		protected override void InitInfo()
		{
			FileName = "hp_upgrade";
			VisibleName = "HP Module";
			Description = "Increases maximum HP by " + (5 * Level) + ".";
		}

		public override void ModifyMaxHp(ref int maxHp)
		{
			maxHp += (Level*5);
		}
	}
}
