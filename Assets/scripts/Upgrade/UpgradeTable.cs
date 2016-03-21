using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.scripts.Upgrade
{
	public class UpgradeTable
	{
		private static UpgradeTable instance = null;
		public static UpgradeTable Instance
		{
			get
			{
				if (instance == null)
					instance = new UpgradeTable();

				return instance;
			}
		}

		public List<UpgradeInfo> upgrades = new List<UpgradeInfo>();

		public Sprite dropBg;

		public class UpgradeInfo
		{
			public Type upgrade;
			public ItemType upgradeType;
			public int rarity;

			public UpgradeInfo(Type u, ItemType type, int rarity)
			{
				this.upgrade = u;
				this.upgradeType = type;
				this.rarity = rarity;
			}
		}

		public UpgradeTable()
		{
			Load();
		}

		public CombinedUpgrade CombineUpgrades(EquippableItem first, EquippableItem second)
		{
			CombinedUpgrade upg = new CombinedUpgrade(1, first, second);
			return upg;
		}

		public EquippableItem[] DismantleCombinedUpgrade(CombinedUpgrade upg)
		{
			EquippableItem[] upgrades = new EquippableItem[2];
			upgrades[0] = upg.first;
			upgrades[1] = upg.second;

			return upgrades;
		}

		private void Load()
		{
			List<Type> types = Utils.GetTypesInNamespace("Assets.scripts.Upgrade.Classic", true, typeof(EquippableItem));
			LoadTypes(types, ItemType.CLASSIC);

			types = Utils.GetTypesInNamespace("Assets.scripts.Upgrade.Rare", true, typeof(EquippableItem));
			LoadTypes(types, ItemType.RARE);

			types = Utils.GetTypesInNamespace("Assets.scripts.Upgrade.Epic", true, typeof(EquippableItem));
			LoadTypes(types, ItemType.EPIC);

			dropBg = Resources.Load<Sprite>("Sprite/inventory/drop_background");
		}

		private void LoadTypes(List<Type> types, ItemType type)
		{
			foreach (Type t in types)
			{
				int rarity = 1;
				ItemType uType = ItemType.CLASSIC;
				try
				{
					rarity = (int)t.GetField("rarity").GetValue(null);
					uType = (ItemType) t.GetField("type").GetValue(null);
				}
				catch (Exception)
				{
					Debug.LogWarning("upgrade Type " + t.Name + " deosnt have static property 'rarity' - setting to default 1");
				}

				UpgradeInfo info = new UpgradeInfo(t, uType, rarity);
				upgrades.Add(info);
			}
		}

		public EquippableItem GenerateUpgrade(Type type, int level)
		{
			EquippableItem u = Activator.CreateInstance(type, level) as EquippableItem;
			return u;
		}

		public EquippableItem GenerateUpgrade(ItemType type, int minRarity, int maxRarity, int level)
		{
			List<UpgradeInfo> possible = new List<UpgradeInfo>();
			foreach (UpgradeInfo info in upgrades)
			{
				if (info.upgradeType == type && (info.rarity >= minRarity && info.rarity <= maxRarity))
				{
					possible.Add(info);
				}
			}

			UpgradeInfo final = possible[Random.Range(0, possible.Count)];
			return GenerateUpgrade(final.upgrade, level);
		}

		public void DropItem(EquippableItem upgrade, Vector3 position, int radius=1)
		{
			Debug.Log("dropped" + upgrade.FileName);
			upgrade.Init();
			upgrade.SpawnGameObject(Utils.GenerateRandomPositionAround(position, radius));
		}
	}
}
