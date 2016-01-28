using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Assets.scripts.Mono.MapGenerator
{
	/// <summary>
	/// udrzuje v pameti vsechny mapy z aktivni hry a umi mezi nimi prepinat
	/// </summary>
	public class WorldHolder : MonoBehaviour
	{
		public static WorldHolder instance;

		public static string[] allowedSeeds = { "500", "555", "-516", "777", "-876", "643", "725" };

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

		public bool completelyRandomSeed;

		public bool doDebug = true;
		public int SQUARE_SIZE = 1;

		public const int WALL = 1;
		public const int GROUND = 0;

		public GameObject darkPlaneTemplate;

		void Start()
		{
			if (instance == null)
				instance = this;

			maps = new Dictionary<Cords, MapHolder>();

			darkPlaneTemplate = GameObject.Find("Dark Plane");

			// create the first map
			GenerateFirstLevel();
		}

		private void GenerateFirstLevel()
		{
			MapHolder newMap = new MapHolder(this, "Start", new Cords(0, 0), MapType.StartClassic, width, height);
			newMap.CreateMap();
			maps.Add(new Cords(0, 0), newMap);

			SetActiveLevel(0, 0, false);
		}

		public Cords GenerateNextLevel(int teleporterType)
		{
			Cords old = activeMap.Position;
			Cords newCords = new Cords(old.x + 1, old.y);

			Debug.Log("generating.. " + newCords.ToString());

			MapHolder newMap = new MapHolder(this, "Map " + newCords.ToString(), newCords, MapType.SecondLevel, 100, 50);
			newMap.CreateMap();

			maps.Add(newCords, newMap);
			return newCords;
		}

		public void SetActiveLevel(int x, int y)
		{
			SetActiveLevel(x,y,true);
		}

		public void SetActiveLevel(int x, int y, bool reloading)
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
			activeMap.LoadMap(reloading);

			// update seeds info for admin
			StringBuilder sb = new StringBuilder();

			foreach (MapRegion region in activeMap.regions.Values)
				if(region.HasParentRegion() == false && !region.empty)
				sb.Append(region.x + ";" + region.y + " " + region.seed + " ");

			try
			{
				GameObject.Find("AdminSeeds").GetComponent<Text>().text = sb.ToString();
			}
			catch (Exception)
			{
			}
		}

		public void RegenMap()
		{
			activeMap.DeleteMap();
			activeMap.CreateMap();
			activeMap.LoadMap(false);
		}

		void Update()
		{
			if (Input.GetKeyDown(KeyCode.Space))
			{
				RegenMap();
			}

			if (Input.GetKeyDown(KeyCode.N))
			{
				Cords c = activeMap.Position;
				Cords newC = new Cords(c.x + 1, c.y);

				bool onlyReload = true;
				if (!maps.ContainsKey(newC))
				{
					GenerateNextLevel(1);
					onlyReload = false;
				}

				SetActiveLevel(newC.x, newC.y, onlyReload);
			}

			if (Input.GetKeyDown(KeyCode.P))
			{
				Cords c = activeMap.Position;
				Cords newC = new Cords(c.x - 1, c.y);

				SetActiveLevel(newC.x, newC.y);
			}

            if (Input.GetKeyDown(KeyCode.S))
            {
                SaveCurrentMap();
            }
		}

		// temp from mobile
		public void LoadNextMap()
		{
			Cords c = activeMap.Position;
			Cords newC = new Cords(c.x + 1, c.y);

			bool onlyReload = true;
			if (!maps.ContainsKey(newC))
			{
				GenerateNextLevel(1);
				onlyReload = false;
			}

			SetActiveLevel(newC.x, newC.y, onlyReload);
		}

		// temp output from mobile
		public void LoadPreviousMap()
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

			public override string ToString()
			{
				return x + ", " + y;
			}

			public override bool Equals(object obj)
			{
				try
				{
					Cords c = (Cords)obj;
					return c.x == x && c.y == y;
				}
				catch (Exception)
				{
					return false;
				}
			}
		}

		public string GetRandomSeed()
		{
			if (completelyRandomSeed)
				return Random.Range(-1000, 1000) + "";

			return allowedSeeds[Random.Range(0, allowedSeeds.Length)];
		}

		public Vector3 GetStartPosition()
		{
			if (activeMap != null)
			{
				return activeMap.GetStartPosition();
			}

			return new Vector3();
		}

	    public void SaveCurrentMap()
	    {
	        String prev = System.IO.File.ReadAllText("mapSeeds.txt");

	        StringBuilder sb = new StringBuilder();

	        sb.Append(prev);

	        sb.AppendLine(Enum.GetName(typeof (MapType), activeMap.MapType));
	        foreach (MapRegion region in activeMap.regions.Values)
	        {
	            if (!region.HasParentRegion())
	            {
	                if (region.hadRandomSeed)
	                {
	                    sb.Append(region.x + " " + region.y + " " + region.fillPercent + " " + region.seed + " (s:" + region.sizeX + ", " + region.sizeY + ")");
	                    sb.AppendLine();
	                }
	            }
	        }

            sb.AppendLine();

            System.IO.File.WriteAllText("mapSeeds.txt", sb.ToString());
            Debug.Log("Saved!");
	    }
	}
}
