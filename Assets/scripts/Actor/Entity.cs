using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.scripts.Actor
{
	public abstract class Entity
	{
		public string Name { get; private set; }

		protected Entity(string name)
		{
			Name = name;
		}

		public virtual void OnUpdate()
		{
			
		}
	}
}
