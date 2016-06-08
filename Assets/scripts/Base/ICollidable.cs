// Copyright (c) 2015, Lukas Honke
// ========================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.scripts.Base
{
	internal interface ICollidable
	{
		// spusteno jinymi objekty
		void OnCollisionEnter2D(Collision2D coll);

		void OnCollisionExit2D(Collision2D coll);

		void OnCollisionStay2D(Collision2D coll);

		// spusteno projektily
		void OnTriggerEnter2D(Collider2D obj);

		void OnTriggerExit2D(Collider2D obj);

		void OnTriggerStay2D(Collider2D obj);
	}
}
