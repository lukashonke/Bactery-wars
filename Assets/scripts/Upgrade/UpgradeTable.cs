// Copyright (c) 2015, Lukas Honke
// ========================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
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
			public bool enabled;

			public UpgradeInfo(Type u, ItemType type, int rarity, bool enabled)
			{
				this.upgrade = u;
				this.upgradeType = type;
				this.rarity = rarity;
				this.enabled = enabled;
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
			LoadTypes(types, ItemType.CLASSIC_UPGRADE);

			types = Utils.GetTypesInNamespace("Assets.scripts.Upgrade.Rare", true, typeof(EquippableItem));
			LoadTypes(types, ItemType.RARE_UPGRADE);

			types = Utils.GetTypesInNamespace("Assets.scripts.Upgrade.Epic", true, typeof(EquippableItem));
			LoadTypes(types, ItemType.EPIC_UPGRADE);

			dropBg = Resources.Load<Sprite>("Sprite/inventory/drop_background");
		}

		private void LoadTypes(List<Type> types, ItemType type)
		{
			foreach (Type t in types)
			{
				int rarity = 1;
				ItemType uType = ItemType.CLASSIC_UPGRADE;
				try
				{
					rarity = (int)t.GetField("rarity").GetValue(null);
					uType = (ItemType) t.GetField("type").GetValue(null);
				}
				catch (Exception)
				{
					Debug.LogWarning("upgrade Type " + t.Name + " deosnt have static property 'rarity' or 'type' - setting to default");
				}

				bool enabled = false;
				try
				{
					enabled = (bool)t.GetField("enabled").GetValue(null);
				}
				catch (Exception)
				{
					enabled = false;
				}

				UpgradeInfo info = new UpgradeInfo(t, uType, rarity, enabled);
				upgrades.Add(info);
			}
		}

		public InventoryItem GenerateUpgrade(Type type, int level, ItemType uType=ItemType.CLASSIC_UPGRADE)
		{
			InventoryItem u = Activator.CreateInstance(type, level) as InventoryItem;
			u.Type = uType;
			return u;
		}

		public InventoryItem GenerateUpgrade(ItemType type, int minRarity, int maxRarity, int level)
		{
			List<UpgradeInfo> possible = new List<UpgradeInfo>();
			foreach (UpgradeInfo info in upgrades)
			{
				if (!info.enabled)
					continue;

				if (info.upgradeType == type && (info.rarity >= minRarity && info.rarity <= maxRarity))
				{
					possible.Add(info);
				}
			}

			if (possible.Count > 0)
			{
				UpgradeInfo final = possible[Random.Range(0, possible.Count)];
				return GenerateUpgrade(final.upgrade, level, type);
			}
			else return null;
		}

		public void DropItem(InventoryItem upgrade, Vector3 position, int radius=1)
		{
			Debug.Log("dropped" + upgrade.FileName);
			upgrade.Init();
			upgrade.SpawnGameObject(Utils.GenerateRandomPositionAround(position, radius));
		}

		public void GiveItem(InventoryItem upgrade, Character target)
		{
			if (upgrade == null)
			{
				Debug.LogError("null upgrade?");
				return;
			}

			upgrade.Init();
			GameSystem.Instance.BroadcastMessage("Received " + upgrade.VisibleName);
			target.GiveItem(upgrade);
		}
	}
}
