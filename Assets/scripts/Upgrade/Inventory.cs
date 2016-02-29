﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Assets.scripts.Actor;

namespace Assets.scripts.Upgrade
{
	public class Inventory
	{
		public Character Owner { get; private set; }
		public int Capacity { get; set; }
		public int ActiveCapacity { get; set; }
		public int BasestatCapacity { get; set; }

		// these only affect base stats! not skills, etc
		private List<AbstractUpgrade> basestatUpgrades;
		public List<AbstractUpgrade> BasestatUpgrades
		{
			get { return basestatUpgrades; }
		}

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

		public Inventory(Character owner, int capacity, int activeCapacity)
		{
			this.Owner = owner;
			this.Capacity = capacity;
			this.ActiveCapacity = activeCapacity;
			BasestatCapacity = 5;

			upgrades = new List<AbstractUpgrade>();
			activeUpgrades = new List<AbstractUpgrade>();
			basestatUpgrades = new List<AbstractUpgrade>(3);
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

		public void AddBasestatUpgrade(AbstractUpgrade u)
		{
			bool contains = false;
			foreach (AbstractUpgrade upg in BasestatUpgrades)
			{
				if (upg.GetType().Equals(u.GetType()))
				{
					upg.AddUpgradeProgress(u);
					contains = true;
					break;
				}
			}

			if (!contains && BasestatUpgrades.Count < BasestatCapacity)
			{
				BasestatUpgrades.Add(u);
			}
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

		public void MoveUpgrade(AbstractUpgrade u, int fromSlot, int toSlot, int slot, AbstractUpgrade upgradeInSlot)
		{
			if (fromSlot == 1 && toSlot == 0)
			{
				UnequipUpgrade(u, (upgradeInSlot != null));

				if (upgradeInSlot != null)
				{
					EquipUpgrade(upgradeInSlot);
				}
			}
			else if (fromSlot == 0 && toSlot == 1)
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
			foreach (AbstractUpgrade upg in activeUpgrades)
			{
				if (u.GetType().Name.Equals(upg.GetType().Name))
				{
					Owner.Message("You cannot equip two upgrades of the same type.");
					return false;
				}
			}

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

		public AbstractUpgrade GetBasestatUpgrade(int order)
		{
			try
			{
				AbstractUpgrade u = basestatUpgrades[order];
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
