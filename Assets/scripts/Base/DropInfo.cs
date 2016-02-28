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
			public UpgradeType type;
			public int chance;
			public int level;
			public int minRarity, maxRarity;
			public int category;

			public RandomDrop(UpgradeType type, int chance, int level, int minRarity, int maxRarity, int category)
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

		public void DoDrop(Monster m, Character killer)
		{
			List<int> categoriesUsed = new List<int>();

			foreach (Drop d in drops)
			{
				if (d.category == -1 || !categoriesUsed.Contains(d.category))
				{
					if (d.chance == 100 || Random.Range(0, 100) < d.chance)
					{
						DoDrop(d.type, d.level, m, killer);
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
						UpgradeTable.Instance.DropItem(UpgradeTable.Instance.GenerateUpgrade(d.type, d.minRarity, d.maxRarity, d.level), m.GetData().GetBody().transform.position);
						categoriesUsed.Add(d.category);
					}
				}
			}
		}

		public void DoDrop(Type d, int level, Monster m, Character killer)
		{
			UpgradeTable.Instance.DropItem(UpgradeTable.Instance.GenerateUpgrade(d, level), m.GetData().GetBody().transform.position);
		}
	}
}
