using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.scripts.Mono.MapGenerator
{
	/// <summary>
	/// udrzuje v pameti vsechny mapy z aktivni hry a umi mezi nimi prepinat
	/// </summary>
	public class WorldHolder : MonoBehaviour
	{
		public static WorldHolder instance;

		// data
		public Dictionary<Cords, MapHolder> maps;
		public MapHolder activeMap;

		// inspector configuration for generated maps
		public int width;
		public int height;
		public string seed;
		public bool useRandomSeed;
		[Range(0, 100)]
		public int randomFillPercent;

		public bool doDebug = false;
		public int SQUARE_SIZE = 1;
		private int MAX_REGIONS = 3;

		public const int WALL = 1;
		public const int GROUND = 0;

		void Start()
		{
			if (instance == null)
				instance = this;

			maps = new Dictionary<Cords, MapHolder>();

			// create the first map
			GenerateFirstLevel();
		}

		private void GenerateFirstLevel()
		{
			MapHolder newMap = new MapHolder(this, "Start", new Cords(0, 0), MapType.DungeonCentralClosed);
			newMap.CreateMap();
			maps.Add(new Cords(0, 0), newMap);

			SetActiveLevel(0, 0);
		}

		public Cords GenerateNextLevel(int teleporterType)
		{
			Cords old = activeMap.Position;
			Cords newCords = new Cords(old.x + 1, old.y);

			Debug.Log("generating.. " + newCords.ToString());

			MapHolder newMap = new MapHolder(this, "Map " + newCords.ToString(), newCords, MapType.DungeonAllOpen);
			newMap.CreateMap();

			maps.Add(newCords, newMap);
			return newCords;
		}

		public void SetActiveLevel(int x, int y)
		{
			MapHolder map;
			maps.TryGetValue(new Cords(x, y), out map);

			if (map == null)
			{
				Debug.LogError("Null map on " + x + ", " + y);
				return;
			}

			Debug.Log("setting active level to " + x + ", " + y);

			if (activeMap != null)
			{
				activeMap.DeloadMap();
			}

			activeMap = map;
			activeMap.LoadMap();
		}

		void Update()
		{
			if (Input.GetKeyDown(KeyCode.Space))
			{
				activeMap.DeleteMap();
				activeMap.CreateMap();
				activeMap.LoadMap();
			}

			if (Input.GetKeyDown(KeyCode.N))
			{
				Cords c = activeMap.Position;
				Cords newC = new Cords(c.x + 1, c.y);

				if (!maps.ContainsKey(newC))
				{
					GenerateNextLevel(1);
				}

				SetActiveLevel(newC.x, newC.y);
			}

			if (Input.GetKeyDown(KeyCode.P))
			{
				Cords c = activeMap.Position;
				Cords newC = new Cords(c.x - 1, c.y);

				SetActiveLevel(newC.x, newC.y);
			}
		}

		// temp from mobile
		public void Next()
		{
			Cords c = activeMap.Position;
			Cords newC = new Cords(c.x + 1, c.y);

			if (!maps.ContainsKey(newC))
			{
				GenerateNextLevel(1);
			}

			SetActiveLevel(newC.x, newC.y);
		}

		// temp output from mobile
		public void Prev()
		{
			Cords c = activeMap.Position;
			Cords newC = new Cords(c.x - 1, c.y);

			SetActiveLevel(newC.x, newC.y);
		}

		void OnDrawGizmos()
		{
			//if (doDebug == false)
			//	return;

			if(activeMap != null)
				activeMap.DrawGizmos();
		}

		public void PositionEnter(Vector3 pos)
		{
			/*foreach (MapRegion r in regions.Values)
			{
				r.Disable();
			}*/

			/*MapRegion reg = GetRegion(pos);
			if (reg != null)
			{
				reg.Enable(); //TODO add setting inactive
			}
			else
			{
				//Debug.LogError("Region for " + pos + " doesnt exist!");
			}*/
		}

		public MapRegion GetRegion(Vector2 pos)
		{
			if (activeMap != null)
			{
				return activeMap.GetRegion(pos);
			}
			return null;
		}

		public struct Cords
		{
			public int x, y;
			public Cords(int x, int y)
			{
				this.x = x;
				this.y = y;
			}

			public bool Equals(Cords f, Cords s)
			{
				return f.x == s.x && f.y == s.y;
			}

			public string ToString()
			{
				return x + ", " + y;
			}
		}
	}
}
