using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.scripts.Upgrade
{
	public class Inventory
	{
		public int Capacity { get; set; }
		public int ActiveCapacity { get; set; }

		private List<AbstractUpgrade> upgrades;
		public List<AbstractUpgrade> Upgrades
		{
			get { return upgrades; }
		}

		private List<AbstractUpgrade> activeUpgrades;
		public List<AbstractUpgrade> ActiveUpgrades
		{
			get { return activeUpgrades; }
		}

		public Inventory(int capacity, int activeCapacity)
		{
			this.Capacity = capacity;
			this.ActiveCapacity = activeCapacity;

			upgrades = new List<AbstractUpgrade>();
			activeUpgrades = new List<AbstractUpgrade>();
		}

		public void LoadUpgrades()
		{
			//load upgrades from file 

			foreach (AbstractUpgrade u in activeUpgrades)
			{
				u.Apply();
			}
		}

		public bool HasInInventory(AbstractUpgrade u)
		{
			return upgrades.Contains(u);
		}

		public bool CanAdd(AbstractUpgrade u)
		{
			if (upgrades.Count >= Capacity)
				return false;

			return true;
		}

		public bool CanEquip(AbstractUpgrade u)
		{
			if (activeUpgrades.Count >= ActiveCapacity)
				return false;

			return true;
		}

		public void AddUpgrade(AbstractUpgrade u)
		{
			if (!CanAdd(u))
				return;

			upgrades.Add(u);
		}

		public void RemoveUpgrade(AbstractUpgrade u)
		{
			upgrades.Remove(u);
		}

		public void RemoveUpgrade(int order)
		{
			try
			{
				AbstractUpgrade u = activeUpgrades[order];
				RemoveUpgrade(u);
			}
			catch (Exception)
			{
			}
		}

		public void EquipUpgrade(AbstractUpgrade u)
		{
			if (!CanEquip(u))
				return;

			if (!HasInInventory(u))
				return;

			activeUpgrades.Add(u);
		}

		public void UnequipUpgrade(AbstractUpgrade u)
		{
			activeUpgrades.Remove(u);
		}

		public AbstractUpgrade GetUpgrade(int order)
		{
			try
			{
				AbstractUpgrade u = activeUpgrades[order];
				return u;
			}
			catch (Exception)
			{
			}

			return null;
		}

		public AbstractUpgrade GetUpgrade(Type type)
		{
			foreach (AbstractUpgrade u in upgrades)
			{
				if (u.GetType().Equals(type))
					return u;
			}
			return null;
		}

	}
}
