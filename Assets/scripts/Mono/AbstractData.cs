using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.scripts.Mono
{
	public abstract class AbstractData : MonoBehaviour
	{
		protected Dictionary<string, GameObject> childs;

		public AbstractData()
		{
			childs = new Dictionary<string, GameObject>();
		}

		public void Start()
		{
			AddChildObjects(transform);
		}

		private void AddChildObjects(Transform t)
		{
			foreach (Transform child in t)
			{
				childs.Add(child.gameObject.name, child.gameObject);
				AddChildObjects(child);
			}
        }

		protected GameObject GetChildByName(string n)
		{
			GameObject val;

			childs.TryGetValue(n, out val);

			return val;
		}
	}
}
