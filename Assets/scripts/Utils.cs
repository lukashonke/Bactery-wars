using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Mono;
using UnityEngine;

namespace Assets.scripts
{
	public sealed class Utils
	{
		public static Vector3 GetDirectionVector(Vector3 from, Vector3 to)
		{
			return from - to;
		}

		public static Quaternion GetRotationToDirectionVector(Vector3 vector)
		{
			return Quaternion.LookRotation(vector, Vector3.forward);
		}

		public static Vector3 GetDirectionVectorToMousePos(Transform from)
		{
			Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			mousePos.z = 0;

			Vector3 target = mousePos - from.position;
			return target;
		}

		public static Quaternion GetRotationToMouse(Transform t)
		{
			Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			mousePos.z = 0;

			Vector3 target = mousePos - t.position;
			Quaternion r = Quaternion.LookRotation(target);
			return r;
		}

		public static Vector3 RotateDirectionVector(Vector3 vector, int angle)
		{
			Vector3 nv = Quaternion.Euler(new Vector3(0, 0, angle)) * vector;
			return nv;
		}

		public static Character GetCharacter(GameObject o)
		{
			AbstractData d = o.GetComponent<AbstractData>();

			if (d == null)
				return null;

			Character ch = d.GetOwner();

			return ch;
		}

		public static float DistancePwr(Vector3 v1, Vector3 v2)
		{
			return (v1 - v2).sqrMagnitude;
		}

		public class Timer
		{
			private static List<Timer> timers = new List<Timer>(); 

			private string name;
			private int start;
			private int end;

			public Timer(string name)
			{
				this.name = name;
			}

			private void Start()
			{
				start = Environment.TickCount;
			}

			private void End()
			{
				end = Environment.TickCount;
				Debug.Log("Timer: " + name + " took " + (end-start) + "ms to complete.");
			}

			public static void StartTimer(string name)
			{
				Timer t = new Timer(name);
				timers.Add(t);

				t.Start();
			}

			public static void EndTimer(string name)
			{
				Timer t = null;

				foreach (Timer tt in timers)
				{
					if (tt.name.Equals(name))
					{
						t = tt;
						break;
					}
				}

				if (t == null)
					return;

				t.End();
				timers.Remove(t);
			}
		}
	}
}
