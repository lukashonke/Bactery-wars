﻿// Copyright (c) 2015, Lukas Honke
// ========================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Base;
using UnityEngine;

namespace Assets.scripts.Mono
{
	public class UpdateSender : MonoBehaviour
	{
		public IMonoReceiver target;

		void Start()
		{
			if (target != null)
				target.MonoStart(gameObject);
		}

		void Update()
		{
			if (target != null)
				target.MonoUpdate(gameObject, false);
		}

		void FixedUpdate()
		{
			if (target != null)
				target.MonoUpdate(gameObject, true);
		}

		void OnDestroy()
		{
			if (target != null)
				target.MonoDestroy(gameObject);
		}

		void OnCollisionEnter2D(Collision2D coll)
		{
			if (target != null)
				target.MonoCollisionEnter(gameObject, coll);
		}

		void OnCollisionExit2D(Collision2D coll)
		{
			if (target != null)
				target.MonoCollisionExit(gameObject, coll);
		}

		void OnCollisionStay2D(Collision2D coll)
		{
			if (target != null)
				target.MonoCollisionStay(gameObject, coll);
		}

		void OnTriggerEnter2D(Collider2D other)
		{
			if (target != null)
				target.MonoTriggerEnter(gameObject, other);
		}

		void OnTriggerExit2D(Collider2D other)
		{
			if (target != null)
				target.MonoTriggerExit(gameObject, other);
		}

		void OnTriggerStay2D(Collider2D other)
		{
			if (target != null)
				target.MonoTriggerStay(gameObject, other);
		}
	}
}
