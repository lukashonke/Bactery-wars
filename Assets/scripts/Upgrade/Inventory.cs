using System;
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

		public int DnaPoints { get; set; }

		// these only affect base stats! not skills, etc
		private List<EquippableItem> basestatUpgrades;
		public List<EquippableItem> BasestatUpgrades
		{
			get { return basestatUpgrades; }
		}

		private List<InventoryItem> items;
		public List<InventoryItem> Items
		{
			get { return items; }
		}

		private List<EquippableItem> activeUpgrades;
		public List<EquippableItem> ActiveUpgrades
		{
			get { return activeUpgrades; }
		}

		public Inventory(Character owner, int capacity, int activeCapacity)
		{
			this.Owner = owner;
			this.Capacity = capacity;
			this.ActiveCapacity = activeCapacity;
			BasestatCapacity = 5;

			items = new List<InventoryItem>();
			activeUpgrades = new List<EquippableItem>();
			basestatUpgrades = new List<EquippableItem>(5);
		}

		public void LoadUpgrades()
		{
			//load upgrades from file 

			foreach (EquippableItem u in activeUpgrades)
			{
				u.Apply();
			}
		}

		public bool HasInInventory(InventoryItem u)
		{
			return items.Contains(u);
		}

		public bool CanAdd(InventoryItem u)
		{
			if (items.Count >= Capacity)
				return false;

			return true;
		}

		public bool CanEquip(InventoryItem u)
		{
			if (u.IsUpgrade())
			{
				if (activeUpgrades.Count >= ActiveCapacity)
					return false;
			}

			return true;
		}

		public void AddBasestatUpgrade(EquippableItem u)
		{
			bool contains = false;
			foreach (EquippableItem upg in BasestatUpgrades)
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

		public void AddItem(InventoryItem u, bool force=false)
		{
			if (!CanAdd(u) && !force)
				return;

			items.Add(u);
		}

		public void RemoveItem(InventoryItem u)
		{
			if (u.IsUpgrade() && IsEquipped(u as EquippableItem))
			{
				UnequipUpgrade(u as EquippableItem, true);
			}

			items.Remove(u);
		}

		public void RemoveActiveUpgrade(int orderSlot)
		{
			try
			{
				EquippableItem u = activeUpgrades[orderSlot];
				RemoveItem(u);
			}
			catch (Exception)
			{
			}
		}

		public void MoveUpgrade(EquippableItem u, int fromSlot, int toSlot, int slot, EquippableItem upgradeInSlot)
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

		public bool EquipUpgrade(EquippableItem u)
		{
			foreach (EquippableItem upg in activeUpgrades)
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
			items.Remove(u);

			u.Apply();
			return true;
		}

		public bool ForceEquipUpgrade(EquippableItem u)
		{
			foreach (EquippableItem upg in activeUpgrades)
			{
				if (u.GetType().Name.Equals(upg.GetType().Name))
				{
					Owner.Message("You cannot equip two upgrades of the same type.");
					return false;
				}
			}

			if (!CanEquip(u))
				return false;

			activeUpgrades.Add(u);

			if(HasInInventory(u))
				items.Remove(u);

			u.Apply();
			return true;
		}

		public bool IsEquipped(EquippableItem u)
		{
			return activeUpgrades.Contains(u);
		}

		public bool UnequipUpgrade(EquippableItem u, bool force=false)
		{
			if (items.Count >= Capacity && !force)
				return false;

			// sundat vsechny upgrady od konce
			for (int i = activeUpgrades.Count - 1; i >= 0; i--)
			{
				EquippableItem upgr = activeUpgrades[i];
				upgr.Remove();
			}

			// smazat z listu ten ktery chceme unequipnout
			activeUpgrades.Remove(u);
			items.Add(u);

			// znovu aplikovat vsechny upgrady
			for (int i = 0; i < activeUpgrades.Count; i++)
			{
				EquippableItem upgr = activeUpgrades[i];
				upgr.Apply();
			}

			return true;
		}

		public EquippableItem GetActiveUpgrade(int order)
		{
			try
			{
				EquippableItem u = activeUpgrades[order];
				return u;
			}
			catch (Exception)
			{
			}

			return null;
		}

		public InventoryItem GetItem(int order)
		{
			try
			{
				InventoryItem u = items[order];
				return u;
			}
			catch (Exception)
			{
			}

			return null;
		}

		public EquippableItem GetBasestatUpgrade(int order)
		{
			try
			{
				EquippableItem u = basestatUpgrades[order];
				return u;
			}
			catch (Exception)
			{
			}

			return null;
		}

		public InventoryItem GetUpgrade(Type type)
		{
			foreach (InventoryItem u in items)
			{
				if (u.GetType().Equals(type))
					return u;
			}
			return null;
		}

		public void AddDna(int ammount)
		{
			DnaPoints += ammount;
		}

		public bool RemoveDna(int ammount)
		{
			if (DnaPoints < ammount)
				return false;

			DnaPoints -= ammount;
			return true;
		}
	}
}
