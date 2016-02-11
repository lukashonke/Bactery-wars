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

		private List<UpgradeInfo> upgrades = new List<UpgradeInfo>(); 

		public class UpgradeInfo
		{
			public Type upgrade;
			public UpgradeType upgradeType;
			public int rarity;

			public UpgradeInfo(Type u, UpgradeType type, int rarity)
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

		public CombinedUpgrade CombineUpgrades(AbstractUpgrade first, AbstractUpgrade second)
		{
			CombinedUpgrade upg = new CombinedUpgrade(1, first, second);
			return upg;
		}

		public AbstractUpgrade[] DismantleCombinedUpgrade(CombinedUpgrade upg)
		{
			AbstractUpgrade[] upgrades = new AbstractUpgrade[2];
			upgrades[0] = upg.first;
			upgrades[1] = upg.second;

			return upgrades;
		}

		private void Load()
		{
			List<Type> types = Utils.GetTypesInNamespace("Assets.scripts.Upgrade.Classic", true, typeof(AbstractUpgrade));
			LoadTypes(types, UpgradeType.CLASSIC);

			types = Utils.GetTypesInNamespace("Assets.scripts.Upgrade.Rare", true, typeof(AbstractUpgrade));
			LoadTypes(types, UpgradeType.RARE);

			types = Utils.GetTypesInNamespace("Assets.scripts.Upgrade.Epic", true, typeof(AbstractUpgrade));
			LoadTypes(types, UpgradeType.EPIC);
		}

		private void LoadTypes(List<Type> types, UpgradeType type)
		{
			foreach (Type t in types)
			{
				int rarity = 1;
				try
				{
					rarity = (int)t.GetField("rarity").GetValue(null);
				}
				catch (Exception)
				{
					Debug.LogError("upgrade Type " + t.Name + " deosnt have static property 'rarity' - setting to default 1");
				}

				UpgradeInfo info = new UpgradeInfo(t, type, rarity);
				upgrades.Add(info);
			}
		}

		public AbstractUpgrade GenerateUpgrade(Type type, int level)
		{
			AbstractUpgrade u = Activator.CreateInstance(type, level) as AbstractUpgrade;
			return u;
		}

		public AbstractUpgrade GenerateUpgrade(UpgradeType type, int minRarity, int maxRarity, int level)
		{
			List<UpgradeInfo> possible = new List<UpgradeInfo>();
			foreach (UpgradeInfo info in upgrades)
			{
				if (info.upgradeType == type && (info.rarity >= minRarity || info.rarity <= maxRarity))
				{
					possible.Add(info);
				}
			}

			UpgradeInfo final = possible[Random.Range(0, possible.Count)];
			return GenerateUpgrade(final.upgrade, level);
		}

		public void DropItem(AbstractUpgrade upgrade, Vector3 position, int radius=1)
		{
			Debug.Log("dropped" + upgrade.Name);
			upgrade.Init();
			upgrade.SpawnGameObject(Utils.GenerateRandomPositionAround(position, radius));
		}
	}
}
