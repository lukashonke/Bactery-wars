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

		public bool skipTutorial = false;

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

			skipTutorial = GameSession.skipTutorial;

			darkPlaneTemplate = GameObject.Find("Total Background");

			// create the first map
			GenerateFirstLevel();
		}

		private void GenerateFirstLevel()
		{
			MapHolder newMap = null;

			if (!skipTutorial)
			{
				newMap = new MapHolder(this, "Level 1", new Cords(0, 0), MapType.LevelOne, width, height);
			}
			else
			{
				MapType type = GetNextLevelType(1);
				newMap = new MapHolder(this, "Level 6", new Cords(0, 0), type, width, height);
			}
			
			newMap.CreateMap();
			maps.Add(new Cords(0, 0), newMap);

			SetActiveLevel(0, 0, false);
		}

		public Cords GenerateNextLevel(int teleporterType)
		{
			Cords old = activeMap.Position;
			Cords newCords = new Cords(old.x + 1, old.y);

			int level = old.x + 2;
			if (skipTutorial)
			{
				level += 5;
				Debug.Log("skipping tutorial..");
			}

			MapType type = MapType.LevelOne;

			switch (level)
			{
				case 2: // tutorial
					type = MapType.LevelTwo;
					break;
				case 3: // tutorial
					type = MapType.LevelThree;
					break;
				case 4: // tutorial
					type = MapType.LevelFour;
					break;
				case 5: // tutorial
					type = MapType.LevelFive;
					break;
				default:
					type = GetNextLevelType(level-5);
					break;
			}

			MapHolder newMap = new MapHolder(this, "Level " + (newCords.x+1), newCords, type, 100, 50);
			newMap.CreateMap();

			maps.Add(newCords, newMap);
			return newCords;
		}

		private MapType GetNextLevelType(int level)
		{
			Debug.Log("next level " + level);
			return MapType.Test;
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

			GameSystem.Instance.BroadcastMessage(activeMap.name);

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
				Camera.main.orthographicSize = 55;
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

				GameObject player = GameObject.Find("Player");
				player.transform.position = WorldHolder.instance.GetStartPosition();
			}

			if (Input.GetKeyDown(KeyCode.P))
			{
				Cords c = activeMap.Position;
				Cords newC = new Cords(c.x - 1, c.y);

				SetActiveLevel(newC.x, newC.y);

				GameObject player = GameObject.Find("Player");
				player.transform.position = WorldHolder.instance.GetStartPosition();
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
			return Random.Range(-1000, 1000) + "";
		}

		public Vector3 GetStartPosition()
		{
			if (activeMap != null)
			{
				return activeMap.GetStartPosition();
			}

			return new Vector3();
		}

		private List<string> tempSeeds = new List<string>();

	    public void SaveCurrentMap()
	    {
		    Camera.main.orthographicSize = 55;

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
						tempSeeds.Add(region.seed);
	                    sb.Append(region.x + " " + region.y + " " + region.fillPercent + " " + region.seed + " (s:" + region.sizeX + ", " + region.sizeY + ")");
	                    sb.AppendLine();
	                }
	            }
	        }

            sb.AppendLine();

		    foreach (string s in tempSeeds)
			    sb.Append(s + ", ");

		    sb.AppendLine();

            System.IO.File.WriteAllText("mapSeeds.txt", sb.ToString());
            Debug.Log("Saved!");
	    }
	}
}
