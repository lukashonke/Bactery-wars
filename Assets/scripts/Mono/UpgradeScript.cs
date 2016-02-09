using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Upgrade;
using UnityEngine;

namespace Assets.scripts.Mono
{
	public class UpgradeScript : MonoBehaviour
	{
		public AbstractUpgrade upgrade;

		public void Start()
		{
			
		}

		public void Update()
		{
			
		}

		public void OnTriggerEnter2D(Collider2D obj)
		{
			if (obj.gameObject != null)
			{
				Character ch = obj.gameObject.GetChar();

				if (ch == null)
					return;

				ch.HitUpgrade(this);
			}
		}

		public void DeleteMe(bool pickedUp)
		{
			Destroy(gameObject);
			// TODO animation, sound etc
		}
	}
}
