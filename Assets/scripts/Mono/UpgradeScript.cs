// Copyright (c) 2015, Lukas Honke
// ========================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Upgrade;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Assets.scripts.Mono
{
	public class UpgradeScript : MonoBehaviour
	{
		public InventoryItem item;

		public float rotationOffset;
		public int rotationSpeed;

		private int state;
		private int targetAngle;

		private float currentSize;
		private float add;

		private float startLife;
		private const int LIFE_TIME = 60;

		public void Start()
		{
			currentSize = 1f;
			add = 0.015f;

			startLife = Time.time;
		}

		public void Update()
		{
			if (startLife + LIFE_TIME < Time.time)
			{
				Destroy(gameObject);
				return;
			}

			currentSize += add;
			if (currentSize >= 1.25f || currentSize <= 0.7f)
			{
				add *= -1;
			}

			transform.localScale = new Vector3(currentSize, currentSize);
		}

		public void OnTriggerEnter2D(Collider2D obj)
		{
			if (startLife + 1 < Time.time)
			{
				if (obj.gameObject != null)
				{
					Character ch = obj.gameObject.GetChar();

					if (ch == null)
						return;

					ch.HitItem(this);
				}
			}
		}

		public void DeleteMe(bool pickedUp)
		{
			Destroy(gameObject);
			// TODO animation, sound etc
		}
	}
}
