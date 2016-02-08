using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Mono;
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

		    if (Owner is Player)
		    {
		        VisibleRadius = 17;
		        UpdateDelay = 0.5f;
		    }
		    else // less frequent updates but more visibility
		    {
		        VisibleRadius = 30;
		        UpdateDelay = 2.0f;
		    }

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
			if (Owner == null || Owner.Data == null || Owner.Data.GetBody() == null)
			{
				Active = false;
				return;
			}

		    bool isPlayer = Owner is Player;

			Collider2D[] hits = Physics2D.OverlapCircleAll(Owner.Data.GetBody().transform.position, VisibleRadius);

		    if (isPlayer)
		    {
                foreach (GameObject o in KnownObjects)
                {
	                if (o == null)
		                continue;

                    AbstractData d = o.GetData();

                    if (d != null)
                    {
                        d.IsVisibleToPlayer = false;
                    }
                }
		    }
		    
			KnownObjects.Clear();

			foreach(Collider2D h in hits)
			{
				if ("Cave Generator".Equals(h.gameObject.name) || h.gameObject.Equals(Owner.GetData().gameObject))
					continue;

			    if (isPlayer)
			    {
                    AbstractData d = h.gameObject.GetData();

                    if (d != null)
                    {
                        d.IsVisibleToPlayer = true;
                    }
			    }

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
