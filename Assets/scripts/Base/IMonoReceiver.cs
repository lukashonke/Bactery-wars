﻿// Copyright (c) 2015, Lukas Honke
// ========================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.scripts.Base
{
	public interface IMonoReceiver
	{
		void MonoStart(GameObject gameObject);
		void MonoUpdate(GameObject gameObject, bool fixedUpdate);
		void MonoDestroy(GameObject gameObject);

		void MonoCollisionEnter(GameObject gameObject, Collision2D coll);
		void MonoCollisionExit(GameObject gameObject, Collision2D coll);
		void MonoCollisionStay(GameObject gameObject, Collision2D coll);

		void MonoTriggerEnter(GameObject gameObject, Collider2D coll);
		void MonoTriggerExit(GameObject gameObject, Collider2D other);
		void MonoTriggerStay(GameObject gameObject, Collider2D other);
	}
}
