using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
			if (IsEquipped(u))
			{
				UnequipUpgrade(u);
			}

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

			u.Apply();
		}

		public bool IsEquipped(AbstractUpgrade u)
		{
			return activeUpgrades.Contains(u);
		}

		public void UnequipUpgrade(AbstractUpgrade u)
		{
			// sundat vsechny upgrady od konce
			for (int i = activeUpgrades.Count - 1; i >= 0; i--)
			{
				AbstractUpgrade upgr = activeUpgrades[i];
				upgr.Remove();
			}

			// smazat z listu ten ktery chceme unequipnout
			activeUpgrades.Remove(u);

			// znovu aplikovat vsechny upgrady
			for (int i = 0; i < activeUpgrades.Count; i++)
			{
				AbstractUpgrade upgr = activeUpgrades[i];
				upgr.Apply();
			}
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
