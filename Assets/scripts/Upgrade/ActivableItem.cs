using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;

namespace Assets.scripts.Upgrade.Classic
{
	public abstract class ActivableItem : InventoryItem
	{
		public bool ConsumeOnUse { get; set; }

		public ActivableItem(int level) : base(level)
		{
			ConsumeOnUse = true;
		}

		public abstract bool OnActivate();

		public new ActivableItem SetOwner(Character ch)
		{
			base.SetOwner(ch);
			return this;
		}
	}
}
