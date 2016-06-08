// Copyright (c) 2015, Lukas Honke
// ========================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Skills;
using UnityEngine;

namespace Assets.scripts.Mono
{
	public class Destroyable : MonoBehaviour
	{
		public GameObject owner;
		public int team;

		public int health;

		public void Start()
		{

		}

		public void Update()
		{
			
		}

		public void OnCollisionEnter2D(Collision2D coll)
		{

		}

		public void OnTriggerEnter2D(Collider2D obj)
		{

		}

		public void ReceiveDamage(Character source, int damage)
		{
			health -= damage;
			if (health < 0)
				health = 0;

			if (health == 0)
			{
				DoDie(source);
			}
		}

		public void DoDie(Character killer)
		{
			Destroy(gameObject);
		}
	}
}
