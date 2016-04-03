using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Upgrade;

namespace Assets.scripts.Base
{
	public class ShopData
	{
		public List<ShopItem> Items { get; private set; }

		public ShopData()
		{
			Items = new List<ShopItem>();
		}

		public void AddItem(InventoryItem item, int price)
		{
			item.Init();
			Items.Add(new ShopItem(item, price));
		}

		public void DoPurchase(ShopItem item)
		{
			UpgradeTable.Instance.GiveItem(item.item, GameSystem.Instance.CurrentPlayer);
			Items.Remove(item);
		}
	}

	public class ShopItem
	{
		public InventoryItem item;
		public int price;

		public ShopItem(InventoryItem t, int price)
		{
			this.item = t;
			this.price = price;
		}
	}
}
