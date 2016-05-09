using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using UnityEngine;

namespace Assets.scripts.Upgrade.Classic
{
	public class DnaItem : InventoryItem
	{
		public static int rarity = 1;

		public DnaItem(int ammount) : base(ammount)
		{
		}

		public override bool OnPickup(Character owner)
		{
			if (owner is Player)
			{
				((Player)owner).AddDnaPoints(Level);
				GameSystem.Instance.CurrentPlayer.GetData().ui.ObjectMessage(owner.GetData().GetBody(), "DNA +" + Level, Color.blue);
			}
			
			return true;
		}

		protected override void InitInfo()
		{
			FileName = "dna_point";
			VisibleName = "DNA Points";
			Description = "Gives you DNA points.";
		}
	}
}
