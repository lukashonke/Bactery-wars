// Copyright (c) 2015, Lukas Honke
// ========================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using UnityEngine;

namespace Assets.scripts.Upgrade.Classic
{
	public class XpItem : InventoryItem
	{
		public static int rarity = 1;

		public XpItem(int ammount)
			: base(ammount)
		{
		}

		public override bool OnPickup(Character owner)
		{
			if (owner is Player)
			{
				owner.AddXp(Level);
				GameSystem.Instance.CurrentPlayer.GetData().ui.ObjectMessage(owner.GetData().GetBody(), "XP +" + Level, Color.blue);
			}
			
			return true;
		}

		protected override void InitInfo()
		{
			FileName = "xp_point";
			VisibleName = "XP Points";
			Description = "Gives you experience points to level up.";
		}
	}
}
