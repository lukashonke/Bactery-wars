using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Assets.scripts.Actor;
using UnityEngine;

namespace Assets.scripts.Base
{
	public class Knownlist
	{
		public Character Owner { get; private set; }
		public List<GameObject> KnownObjects { get; private set; }
		public int VisibleRadius { get; set; }
		public bool Active { get; set; }
		public float UpdateDelay { get; set; }

		private Coroutine task;

		public Knownlist(Character o)
		{
			Owner = o;
			KnownObjects = new List<GameObject>();
			VisibleRadius = 20;
			UpdateDelay = 1f;
			Active = true;
		}

		public void StartUpdating()
		{
			task = Owner.StartTask(UpdateTask());
		}

		public void StopUpdating()
		{
			if (task != null)
			{
				Owner.StopTask(task);
				task = null;
			}
		}

		public void AddKnownObject(GameObject o)
		{
			KnownObjects.Add(o);
		}

		public void RemoveKnownObject(GameObject o)
		{
			KnownObjects.Remove(o);
		}

		public void Update()
		{
			if (Owner.Data.GetBody() == null)
				return;

			Collider2D[] hits = Physics2D.OverlapCircleAll(Owner.Data.GetBody().transform.position, VisibleRadius);

			KnownObjects.Clear();

			//TODO knownlist se nejdriv vymaze, nezpusobi to nejaky problemy?
			foreach(Collider2D h in hits)
			{
				KnownObjects.Add(h.gameObject);
            }
		}

		protected virtual IEnumerator UpdateTask()
		{
			while (Active)
			{
				Update();
				yield return new WaitForSeconds(UpdateDelay);
			}
		}
    }
}
