using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Upgrade;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.scripts.Base
{
	public class DropInfo
	{
		public List<Drop> drops = new List<Drop>(); 
		public List<RandomDrop> randomDrops = new List<RandomDrop>(); 

		public struct Drop
		{
			public Type type;
			public int chance;
			public int level;
			public int category;

			public Drop(Type type, int chance, int level, int category)
			{
				this.type = type;
				this.chance = chance;
				this.level = level;
				this.category = category;
			}
		}

		public struct RandomDrop
		{
			public ItemType type;
			public int chance;
			public int level;
			public int minRarity, maxRarity;
			public int category;

			public RandomDrop(ItemType type, int chance, int level, int minRarity, int maxRarity, int category)
			{
				this.type = type;
				this.chance = chance;
				this.level = level;
				this.minRarity = minRarity;
				this.maxRarity = maxRarity;
				this.category = category;
			}
		}

		public DropInfo()
		{
			
		}

		public void DoDrop(Monster m, Character killer, bool pickup=false)
		{
			List<int> categoriesUsed = new List<int>();

			foreach (Drop d in drops)
			{
				if (d.category == -1 || !categoriesUsed.Contains(d.category))
				{
					if (d.chance == 100 || Random.Range(0, 100) < d.chance)
					{
						if(!pickup)
							DoDrop(d.type, d.level, m, killer);
						else
							GiveItem(d.type, d.level, m, killer);

						categoriesUsed.Add(d.category);
					}
				}
			}

			foreach (RandomDrop d in randomDrops)
			{
				if (d.category == -1 || !categoriesUsed.Contains(d.category))
				{
					if (d.chance == 100 || Random.Range(0, 100) < d.chance)
					{
						if (!pickup)
						{
							UpgradeTable.Instance.DropItem(UpgradeTable.Instance.GenerateUpgrade(d.type, d.minRarity, d.maxRarity, d.level), m.GetData().GetBody().transform.position);
						}
						else
						{
							UpgradeTable.Instance.GiveItem(UpgradeTable.Instance.GenerateUpgrade(d.type, d.minRarity, d.maxRarity, d.level), killer);
						}

						categoriesUsed.Add(d.category);
					}
				}
			}
		}

		public void DoDrop(Type d, int level, Monster m, Character killer)
		{
			UpgradeTable.Instance.DropItem(UpgradeTable.Instance.GenerateUpgrade(d, level), (m != null ? m.GetData().GetBody().transform.position : new Vector3()));
		}

		public void GiveItem(Type d, int level, Monster m, Character killer)
		{
			UpgradeTable.Instance.GiveItem(UpgradeTable.Instance.GenerateUpgrade(d, level), killer);
		}

		public override string ToString()
		{
			if(drops.Count == 0 && randomDrops.Count == 0)
			{
				return null;
			}

			StringBuilder sb = new StringBuilder();

			for (int i = 0; i < drops.Count; i++)
			{
				Drop d = drops[i];

				if (d.chance < 100)
					sb.Append(d.chance + "% chance ");

				sb.Append(d.type.Name);

				if (i + 1 < drops.Count)
					sb.Append("\n");
			}

			for (int i = 0; i < randomDrops.Count; i++)
			{
				RandomDrop d = randomDrops[i];

				if (d.chance < 100)
					sb.Append(d.chance + "% chance ");

				sb.Append(d.type.ToString());

				if (i + 1 < drops.Count)
					sb.Append("\n");
			}

			return Utils.StringWrap(sb.ToString(), 50);
		}
	}
}
