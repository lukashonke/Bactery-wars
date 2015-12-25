using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.scripts.Mono.MapGenerator
{
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
			MapHolder newMap = new MapHolder(this, "Start", new Cords(0, 0), MapType.Dungeon);
			newMap.CreateMap();
			newMap.LoadMap();

			activeMap = newMap;

			maps.Add(new Cords(0, 0), newMap);
		}

		void Update()
		{
			if (Input.GetKeyDown(KeyCode.Space))
			{
				activeMap.DeleteMap();
				activeMap.CreateMap();
				activeMap.LoadMap();
			}
		}

		void OnDrawGizmos()
		{
			if (doDebug == false)
				return;

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
