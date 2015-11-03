using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

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
