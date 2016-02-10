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
				UnequipUpgrade(u, true);
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

		public void MoveUpgrade(AbstractUpgrade u, bool fromActive, bool toActive, int slot, AbstractUpgrade upgradeInSlot)
		{
			if (fromActive && !toActive)
			{
				UnequipUpgrade(u, (upgradeInSlot != null));

				if (upgradeInSlot != null)
				{
					EquipUpgrade(upgradeInSlot);
				}
			}
			else if (!fromActive && toActive)
			{
				if (upgradeInSlot != null)
				{
					UnequipUpgrade(upgradeInSlot, true);
				}

				EquipUpgrade(u);
			}
		}

		public bool EquipUpgrade(AbstractUpgrade u)
		{
			if (!CanEquip(u))
				return false;

			if (!HasInInventory(u))
				return false;

			activeUpgrades.Add(u);
			upgrades.Remove(u);

			u.Apply();
			return true;
		}

		public bool IsEquipped(AbstractUpgrade u)
		{
			return activeUpgrades.Contains(u);
		}

		public bool UnequipUpgrade(AbstractUpgrade u, bool force=false)
		{
			if (upgrades.Count >= Capacity && !force)
				return false;

			// sundat vsechny upgrady od konce
			for (int i = activeUpgrades.Count - 1; i >= 0; i--)
			{
				AbstractUpgrade upgr = activeUpgrades[i];
				upgr.Remove();
			}

			// smazat z listu ten ktery chceme unequipnout
			activeUpgrades.Remove(u);
			upgrades.Add(u);

			// znovu aplikovat vsechny upgrady
			for (int i = 0; i < activeUpgrades.Count; i++)
			{
				AbstractUpgrade upgr = activeUpgrades[i];
				upgr.Apply();
			}

			return true;
		}

		public AbstractUpgrade GetActiveUpgrade(int order)
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

		public AbstractUpgrade GetUpgrade(int order)
		{
			try
			{
				AbstractUpgrade u = upgrades[order];
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
