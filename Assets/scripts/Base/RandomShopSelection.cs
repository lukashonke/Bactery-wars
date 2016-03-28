using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Upgrade;

namespace Assets.scripts.Base
{
	public class RandomShopSelection
	{
		public List<Type> Items { get; private set; }

		public RandomShopSelection()
		{
			Items = new List<Type>();
		}

		public void AddItem(Type t)
		{
			Items.Add(t);
		}
	}
}
