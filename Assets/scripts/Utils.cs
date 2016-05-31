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

		public static RaycastHit2D[] CastBoxInDirection(GameObject obj, Vector3 direction, float width, float distance) //TODO optimize using layermask
		{
			float angle = obj.transform.rotation.eulerAngles.z;
			return Physics2D.BoxCastAll(obj.transform.position, new Vector2(width, width), angle, direction, distance);
		}

		public static bool VectorEquals(Vector3 a, Vector3 b, float precision=0.5f)
		{
			return Vector3.SqrMagnitude(a - b) < precision;
		}

		public static bool IsInCone(GameObject source, Vector3 direction, GameObject target, int angle, int range)
		{
			Vector3 rayDirection = target.transform.position - source.transform.position;

			if ((Vector2.Angle(direction, rayDirection)) <= angle * 0.5f)
			{
				Debug.Log("angle is " + (Vector2.Angle(direction, rayDirection)));
				RaycastHit2D hit = Physics2D.Raycast(source.transform.position, rayDirection, range);

				if (hit)
				{
					return true;
				}
			}

			return false;
		}

		public static bool CanSee(GameObject obj, GameObject target)
		{
			if (Physics2D.Linecast(obj.transform.position, target.transform.position, 1 << OBSTACLES_LAYER))
				return false;
			return true;
		}

		public static bool CanSee(GameObject obj, Vector3 target)
		{
			if (Physics2D.Linecast(obj.transform.position, target, 1 << OBSTACLES_LAYER))
				return false;
			return true;
		}

		public static bool CanSee(Vector3 source, Vector3 target)
		{
			if (Physics2D.Linecast(source, target, 1 << OBSTACLES_LAYER))
				return false;
			return true;
		}

		public static bool ChanceCheck(int chance)
		{
			int roll = Random.Range(0, 100);
			return roll < chance;
		}

		public static RaycastHit2D[] DoubleRaycast(Vector3 origin, Vector3 direction, int range, float width, bool includeCenter=false)
		{
			Vector3 shiftDir1 = new Vector3(-origin.y, origin.x, 0).normalized * width;
			Vector3 shiftDir2 = new Vector3(-origin.y, origin.x, 0).normalized * -width;

			//Debug.DrawRay(origin1, direction, Color.blue, 0.5f);
			//Debug.DrawRay(origin2, direction, Color.blue, 0.5f);

			RaycastHit2D[] first = Physics2D.RaycastAll(origin + shiftDir1, direction, range);
			RaycastHit2D[] second = Physics2D.RaycastAll(origin + shiftDir2, direction, range);
			RaycastHit2D[] final = null;

			if (includeCenter)
			{
				RaycastHit2D[] middle = Physics2D.RaycastAll(origin, direction, range);
				final = new RaycastHit2D[first.Length + second.Length + middle.Length];

				Array.Copy(first, final, first.Length);
				Array.Copy(second, 0, final, first.Length, second.Length);
				Array.Copy(middle, 0, final, first.Length + second.Length, middle.Length);
			}
			else
			{
				final = new RaycastHit2D[first.Length + second.Length];

				Array.Copy(first, final, first.Length);
				Array.Copy(second, 0, final, first.Length, second.Length);
			}

			return final;
		}

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

		public static Quaternion GetRotationToTarget(Transform t, GameObject obj)
		{
			Vector3 pos = obj.transform.position;
			pos.z = 0;

			Vector3 target = pos - t.position;
			Quaternion r = Quaternion.LookRotation(target);
			return r;
		}

		public static Vector3 GetPerpendicularVector(Vector3 from, Vector3 target)
		{
			Vector3 temp = from - target;
			Vector3 perpend = new Vector3(-temp.y, temp.x).normalized;
			return perpend;
		}

		public static Vector3 GetPerpendicalVectorFromDirection(Vector3 from, Vector3 dir)
		{
			return GetPerpendicularVector(from, from + dir);
		}

		public static Vector3 GeneratePerpendicularPositionAround(Vector3 from, Vector3 target, float minDist, float maxDist)
		{
			Vector3 temp = from - target;
			Vector3 perpend = new Vector3(-temp.y, temp.x).normalized;

			Debug.DrawRay(target, perpend*10, Color.blue, 3f);

			int limit = 12;
			while (--limit > 0)
			{
				float dist = Random.Range(minDist, maxDist);
				if (Random.Range(0, 2) == 0)
					dist *= -1;

				Vector3 result = target + perpend*dist;

				Debug.DrawLine(from, result, Color.yellow, 3f);

				if (IsNotAccessible(result))
					continue;

				return result;
			}

			return target;
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

			Debug.LogError("couldnt find a position that doesnt go into walls!");

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
			int iteration = 0;
			while (point != v) // Try to reach the point starting from the far off point.  This will pass through faces to reach its objective.
			{
				iteration++;
				if (iteration > 100)
				{
					Debug.LogError("stucked!!");
					return true;
				}
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
			Debug.LogError("reg mon");
			EnemyData d = o.GetData() as EnemyData;
			GameSystem.Instance.RegisterNewCustomMonster(d, d.monsterTypeName, false, 1, null);
		}

		public static void RegisterAsNpc(this GameObject o)
		{
			EnemyData d = o.GetData() as EnemyData;
			MonsterId mId = (MonsterId)Enum.Parse(typeof(MonsterId), "" + d.monsterId);
			GameSystem.Instance.RegisterNewNpc(d, mId);
		}

		public static float DistanceSqr(Vector3 v1, Vector3 v2)
		{
			return (v1 - v2).sqrMagnitude;
		}

		public static float Distance(Vector3 v1, Vector3 v2)
		{
			return (v1 - v2).magnitude;
		}

		public static float DistanceObjectsSqr(GameObject source, GameObject target)
		{
			Collider2D coll = target.GetComponent<Collider2D>();
			float size = 0;

			if (coll is CircleCollider2D)
				size = ((CircleCollider2D)coll).radius;
			else if (coll is BoxCollider2D)
				size = ((BoxCollider2D)coll).size.x / 2f;

			float dist = DistanceSqr(source.transform.position, target.transform.position) - (size * size);

			return dist;
		}

		public static float DistanceObjects(GameObject source, GameObject target)
		{
			Collider2D coll = target.GetComponent<Collider2D>();
			float size = 0;

			if (coll is CircleCollider2D)
				size = ((CircleCollider2D)coll).radius;
			else if (coll is BoxCollider2D)
				size = ((BoxCollider2D)coll).size.x / 2f;

			float dist = Distance(source.transform.position, target.transform.position) - (size);

			return dist;
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

		static char[] splitChars = new char[] { ' ', '-', '\t' };

		public static string StringWrap(string str, int width)
		{
			string[] words = Explode(str, splitChars);

			int curLineLength = 0;
			StringBuilder strBuilder = new StringBuilder();
			for (int i = 0; i < words.Length; i += 1)
			{
				string word = words[i];
				// If adding the new word to the current line would be too long,
				// then put it on a new line (and split it up if it's too long).
				if (curLineLength + word.Length > width)
				{
					// Only move down to a new line if we have text on the current line.
					// Avoids situation where wrapped whitespace causes emptylines in text.
					if (curLineLength > 0)
					{
						strBuilder.Append(Environment.NewLine);
						curLineLength = 0;
					}

					// If the current word is too long to fit on a line even on it's own then
					// split the word up.
					while (word.Length > width)
					{
						strBuilder.Append(word.Substring(0, width - 1) + "-");
						word = word.Substring(width - 1);

						strBuilder.Append(Environment.NewLine);
					}

					// Remove leading whitespace from the word so the new line starts flush to the left.
					word = word.TrimStart();
				}
				strBuilder.Append(word);
				curLineLength += word.Length;
			}

			return strBuilder.ToString();
		}

		private static string[] Explode(string str, char[] splitChars)
		{
			List<string> parts = new List<string>();
			int startIndex = 0;
			while (true)
			{
				if (splitChars == null) parts.ToArray();

				int index = str.IndexOfAny(splitChars, startIndex);

				if (index == -1)
				{
					parts.Add(str.Substring(startIndex));
					return parts.ToArray();
				}

				string word = str.Substring(startIndex, index - startIndex);
				char nextChar = str.Substring(index, 1)[0];
				// Dashes and the likes should stick to the word occuring before it. Whitespace doesn't have to.
				if (char.IsWhiteSpace(nextChar))
				{
					parts.Add(word);
					parts.Add(nextChar.ToString());
				}
				else
				{
					parts.Add(word + nextChar);
				}

				startIndex = index + 1;
			}
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
