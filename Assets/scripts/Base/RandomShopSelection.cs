using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Upgrade;
using Random = UnityEngine.Random;

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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="worldLevel"></param>
		/// <param name="mapDifficulty">1=easy, 2=medium, 3=hard</param>
		public void GenerateRandomShopItems(int worldLevel, int mapDifficulty)
		{
			int maxItems = Random.Range(6, 10);


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
