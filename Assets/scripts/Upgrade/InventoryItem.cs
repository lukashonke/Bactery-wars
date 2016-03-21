using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Mono;
using UnityEngine;

namespace Assets.scripts.Upgrade
{
	public abstract class InventoryItem
	{
		public string FileName { get; protected set; }
		public string TypeName { get; protected set; }
		public string VisibleName { get; protected set; }
		public string Description { get; protected set; }
		public string Price { get; protected set; }
		public string AdditionalInfo { get; protected set; }

		public bool CollectableByPlayer { get; protected set; }

		public ItemType Type { get; protected set; }

		public int Level { get; set; }
		public int MaxLevel { get; set; }

		protected Character owner;
		public Character Owner
		{
			get { return owner; }
		}

		public Sprite MainSprite { get; protected set; }

		public InventoryItem(int level)
		{
			this.Level = level;

			Type = ItemType.CLASSIC;

			VisibleName = FileName;
			TypeName = "Item";
			Description = "No Description";
			Price = "No value";
			AdditionalInfo = null;
			CollectableByPlayer = true;
		}

		protected abstract void InitInfo();

		public virtual bool OnPickup(Character owner)
		{
			return false;
		}

		public virtual InventoryItem Init()
		{
			InitInfo();
			try
			{
				MainSprite = LoadSprite("lvl" + Level);
			}
			catch (Exception e)
			{
				//Debug.LogError(e.StackTrace);
				try
				{
					MainSprite = LoadSprite("lvl1");
				}
				catch (Exception)
				{
					//Debug.LogError(e.StackTrace);
					MainSprite = Resources.Load<Sprite>("Sprite/Upgrades/default");
				}
			}

			return this;
		}

		public GameObject SpawnGameObject(Vector3 pos)
		{
			GameObject o = new GameObject(VisibleName + " " + Level);
			SpriteRenderer r = o.AddComponent<SpriteRenderer>();
			r.sprite = MainSprite;
			o.AddComponent<BoxCollider2D>().isTrigger = true;
			o.AddComponent<UpgradeScript>().item = this;
			o.transform.position = pos;

			GameObject bg = new GameObject("Background");
			r = bg.AddComponent<SpriteRenderer>();
			r.sprite = UpgradeTable.Instance.dropBg;
			r.sortingOrder = -1;
			bg.transform.parent = o.transform;
			bg.transform.localPosition = new Vector3(0, 0, 0);

			return o;
		}

		protected Sprite LoadSprite(string fileName)
		{
			Sprite o = Resources.Load<Sprite>("Sprite/Upgrades/" + FileName + "/" + fileName);
			if (o == null)
				throw new NullReferenceException(fileName + " not found ");

			return o;
		}

		public bool IsUpgrade()
		{
			return this is EquippableItem;
		}

		public InventoryItem SetOwner(Character ch)
		{
			owner = ch;
			return this;
		}
	}
}
