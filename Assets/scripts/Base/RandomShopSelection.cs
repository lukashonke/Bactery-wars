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
			int classicItems = Random.Range(4, 6);
			int rareItems = Random.Range(1, 2);
			int epicItems = Random.Range(0, worldLevel);

			List<InventoryItem> selected = new List<InventoryItem>();

			for (int i = 0; i < classicItems; i++)
			{
				InventoryItem it = UpgradeTable.Instance.GenerateUpgrade(ItemType.CLASSIC_UPGRADE, 1, 2, 1);
				it.Init();

				bool alreadyInShop = false;

				foreach (InventoryItem sel in selected)
				{
					if (sel.VisibleName.Equals(it.VisibleName))
					{
						alreadyInShop = true;
						break;
					}
				}
				if (alreadyInShop)
				{
					i --;
					continue;
				}

				selected.Add(it);
				
				AddItem(it, 10);
			}

			for (int i = 0; i < rareItems; i++)
			{
				InventoryItem it = UpgradeTable.Instance.GenerateUpgrade(ItemType.RARE_UPGRADE, 1, 2, 1);
				it.Init();

				bool alreadyInShop = false;

				foreach (InventoryItem sel in selected)
				{
					if (sel.VisibleName.Equals(it.VisibleName))
					{
						alreadyInShop = true;
						break;
					}
				}
				if (alreadyInShop)
				{
					i--;
					continue;
				}

				selected.Add(it);

				AddItem(it, 20);
			}

			for (int i = 0; i < epicItems; i++)
			{
				InventoryItem it = UpgradeTable.Instance.GenerateUpgrade(ItemType.EPIC_UPGRADE, 1, 2, 1);
				it.Init();

				bool alreadyInShop = false;

				foreach (InventoryItem sel in selected)
				{
					if (sel.VisibleName.Equals(it.VisibleName))
					{
						alreadyInShop = true;
						break;
					}
				}
				if (alreadyInShop)
				{
					i--;
					continue;
				}

				selected.Add(it);

				AddItem(it, 50);
			}
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
