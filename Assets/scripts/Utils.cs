using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.Mono;
using Assets.scripts.Mono.MapGenerator;
using Assets.scripts.Mono.ObjectData;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.scripts
{
	public static class Utils
	{
		public const int OBSTACLES_LAYER = 12;

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

		public static Vector3 GenerateFixedPositionAroundObject(GameObject o, float circleRadius, int fixedAngle)
		{
			AbstractData data = o.GetData();
			Vector3 dir = data.GetForwardVector();
			Vector3 newRot = RotateDirectionVector(dir, fixedAngle)*circleRadius;

			Vector3 target = o.transform.position + newRot;

			if (IsNotAccessible(target))
			{
				return GenerateRandomPositionAround(o.transform.position, 5, 2);
			}

			return o.transform.position + newRot;
		}

		public static Vector3 GenerateRandomPositionOnCircle(Vector3 pos, float circleRadius, int fixedAngle=-1)
		{
			int angle = Random.Range(1, 360);

			if (fixedAngle > 0)
				angle = fixedAngle;

			int limit = 12;
			while (--limit > 0)
			{
				Vector3 result = new Vector3(pos.x + circleRadius * Mathf.Sin(angle * Mathf.Deg2Rad), pos.y + circleRadius * Mathf.Cos(angle * Mathf.Deg2Rad), 0);

				//Debug.DrawLine(pos, result, Color.yellow, 3f);
				if (IsNotAccessible(result))
				{
					angle += 30;
					angle = angle%360;
					continue;
				}

				return result;
			}

			return pos;
		}

		public static Vector3 GenerateRandomPositionAround(Vector3 pos, float maxRange, float minRange)
		{
			return GenerateRandomPositionAround(pos, pos, maxRange, minRange);
		}

		public static Vector3 GenerateRandomPositionAround(Vector3 from, Vector3 pos, float maxRange, float minRange)
		{
			int limit = 6;
			while (--limit > 0)
			{
				float randX = Random.Range(minRange, maxRange);
				float randY = Random.Range(minRange, maxRange);

			    if (Random.Range(0, 2) == 0)
			        randX *= -1;

			    if (Random.Range(0, 2) == 0)
			        randY *= -1;

				Vector3 v = new Vector3(pos.x + randX, pos.y +randY, 0);

			    if (IsNotAccessible(from, v))
			    {
			        Debug.DrawLine(from, v, Color.red, 3f);
					continue;
			    }

				return v;
			}

			return pos;
		}

		public static Vector3 GenerateRandomPositionAround(Vector3 pos, float range, MapRegion targetRegion=null)
		{
			return GenerateRandomPositionAround(pos, pos, range, targetRegion);
		}

		public static Vector3 GenerateRandomPositionAround(Vector3 from, Vector3 pos, float range, MapRegion targetRegion=null)
		{
			int limit = 6;

			while (--limit > 0)
			{
				Vector3 v = new Vector3(pos.x + Random.Range(-range, range), pos.y + Random.Range(-range, range), 0);

				//Debug.DrawLine(from, v, Color.red, 0.5f);

				if (IsNotAccessible(from, v))
					continue;

				//Debug.DrawLine(from, v, Color.green, 0.5f);

				return v;
			}

			return pos;
		}

		public static bool IsNotAccessible(Vector3 v)
		{
			Vector3 start = WorldHolder.instance.activeMap.GetStartPosition();
			return IsNotAccessible(start, v);
		}

		//TODO optimize? 
		public static bool IsNotAccessible(Vector3 from, Vector3 v)
		{
			//Debug.DrawRay(from, v - from, Color.green, 2f);
			int count = 0;
			Vector3 dir = v - from;
			dir.Normalize();

			Vector3 point = from;

			while (point != v) // Try to reach the point starting from the far off point.  This will pass through faces to reach its objective.
			{
				RaycastHit2D hit = Physics2D.Linecast(point, v, 1 << OBSTACLES_LAYER);

				if (hit.collider != null && hit.collider.gameObject.name.Equals("Cave Generator")) // Progressively move the point forward, stopping everytime we see a new plane in the way.
				{
					count++;
					point = new Vector3(hit.point.x, hit.point.y, 0) + (dir / 100.0f); // Move the Point to hit.point and push it forward just a touch to move it through the skin of the mesh (if you don't push it, it will read that same point indefinately).
				}
				else
				{
					point = v; // If there is no obstruction to our goal, then we can reach it in one step.
				}
			}

			if (count%2 == 0)
				return false;
			else
				return true;
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

		public static Character GetChar(this GameObject o)
		{
			AbstractData d = o.GetComponent<AbstractData>();

			if (d == null)
			{
				/*Destroyable dest = o.GetComponent<Destroyable>();
				if(dest != null)
					return dest.owner.GetChar();*/
				return null;
			}

			Character ch = d.GetOwner();

			return ch;
		}

		public static AbstractData GetData(this GameObject o)
		{
			AbstractData d = o.GetComponent<AbstractData>();

			return d;
		}

		public static void RegisterAsMonster(this GameObject o)
		{
			EnemyData d = o.GetData() as EnemyData;
			GameSystem.Instance.RegisterNewMonster(d, d.name, d.monsterId, 1, null);
		}

		public static void RegisterAsNpc(this GameObject o)
		{
			EnemyData d = o.GetData() as EnemyData;
			MonsterId mId = (MonsterId)Enum.Parse(typeof(MonsterId), "" + d.monsterId);
			GameSystem.Instance.RegisterNewNpc(d, mId);
		}

		public static float DistancePwr(Vector3 v1, Vector3 v2)
		{
			return (v1 - v2).sqrMagnitude;
		}

		public static List<Type> GetTypesInNamespace(string ns, bool onlyClasses, Type baseClass)
		{
			Assembly asm = Assembly.GetExecutingAssembly();

			List<Type> types = new List<Type>();

			foreach (Type t in asm.GetTypes())
			{
				if (onlyClasses && !t.IsClass)
					continue;

				if (!ns.Equals(t.Namespace))
					continue;

				if (baseClass != null && !t.IsSubclassOf(baseClass))
					continue;

				types.Add(t);
			}

			return types;
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
