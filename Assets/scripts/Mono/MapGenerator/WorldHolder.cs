using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Base;
using Assets.scripts.Mono.MapGenerator.Levels;
using Assets.scripts.Mono.ObjectData;
using Assets.scripts.Upgrade;
using Assets.scripts.Upgrade.Classic;
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
		public int worldLevel;

		public static string[] allowedSeeds = { "500", "555", "-516", "777", "-876", "643", "725" };

		// data
		public LevelTree mapTree;
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

			if (skipTutorial)
				worldLevel = 1;
			else
				worldLevel = 0;

			darkPlaneTemplate = GameObject.Find("Total Background");

			GenerateWorldLevels();

			// create the first map
			GenerateFirstLevel();
		}

		public bool GoToNextWorld()
		{
			worldLevel ++;
			GenerateWorldLevels();
			GameSystem.Instance.BroadcastMessage("You proceed to world " + worldLevel);
			//GenerateFirstLevel();
			return true;
		}

		private void GenerateWorldLevels()
		{
			int id = 0;
			LevelParams param = new LevelParams(MapType.LevelOne);
			param.worldLevel = worldLevel;

			if (GameSession.arenaMode)
			{
				param.levelType = MapType.Arena;
				mapTree = new LevelTree(LevelTree.LEVEL_MAIN, id++, LevelTree.DIFF_MEDIUM, 0, param, "Start level", "Unknown");
			}
			else
			{
				if (worldLevel > 0)
				{
					param.levelType = MapType.GenericMonster; //TODO
					mapTree = new LevelTree(LevelTree.LEVEL_MAIN, id++, LevelTree.DIFF_MEDIUM, 0, param, "Start level", "Unknown");
				}
				else
				{
					param.levelType = MapType.LevelOne;
					//param.levelType = MapType.Test;
					mapTree = new LevelTree(LevelTree.LEVEL_MAIN, id++, LevelTree.DIFF_EASY, 0, param, "First Tutorial", "Unknown");
				}
			}


			mapTree.AddLevelReward(typeof(HpPotion), 50);
			mapTree.AddLevelReward(typeof(Heal));
			mapTree.AddLevelReward(typeof(CCAADoubleattackUpgrade), 75);

			mapTree.Unlocked = true;

			int levelsCount = 15;
			int mainLineCount = 5;
			int minGenericLevelsCount;
			int minSpecialLevelsCount;
			switch (worldLevel)
			{
				case 1:
					levelsCount = 10;
					mainLineCount = 4;
					break;
				case 2:
					levelsCount = Random.Range(14,16);
					mainLineCount = 5;
					break;
				case 3:
					levelsCount = Random.Range(14, 16);
					mainLineCount = 5;
					break;
				case 4:
					levelsCount = Random.Range(14, 16);
					mainLineCount = 5;
					break;
				default:
					levelsCount = 5;
					mainLineCount = 5;
					break;
			}

			int maxSpecialLevelsCount = Mathf.CeilToInt((levelsCount - mainLineCount)/4);

			if (maxSpecialLevelsCount <= 0)
				maxSpecialLevelsCount = 1;

			// one level is already created
			levelsCount--;
			mainLineCount --;

			int counter = levelsCount;

			if (worldLevel > 0)
			{
				int shopLevel = Random.Range(0, mainLineCount-1);

				// main line - all medium
				for (int i = 0; i < mainLineCount; i++)
				{
					LevelTree newNode = null;
					param = new LevelParams(MapType.LevelOne);
					param.worldLevel = worldLevel;

					// grand boss is the last level
					if (i + 1 == mainLineCount)
					{
						param.levelType = MapType.FindBoss; // TODO change
						param.mapLevel = worldLevel;
						param.worldLevel = worldLevel;
						newNode = new LevelTree(LevelTree.LEVEL_MAIN, id++, LevelTree.DIFF_MEDIUM, i + 1, param, "Main" + id);
					}
					else
					{
						newNode = CreateRandomMainNodeLevel(id++, i + 1, shopLevel == i, LevelTree.DIFF_MEDIUM);
					}

					if (newNode == null) continue;
					
					mapTree.GetLastMainNode().AddChild(newNode);

					if (i + 1 == mainLineCount)
					{
						newNode.IsLastNode = true;
					}

					counter--;
				}
			}
			else
			{
				// main line - tutorial levels - all easy
				for (int i = 0; i < mainLineCount; i++)
				{
					LevelTree newNode = null;
					param = new LevelParams(MapType.LevelOne);
					param.worldLevel = worldLevel;

					switch (i)
					{
						case 0:
							param.levelType = MapType.LevelTwo;
							newNode = new LevelTree(LevelTree.LEVEL_MAIN, id++, LevelTree.DIFF_EASY, i + 1, param, "Second Tutorial", "Unknown");
							break;
						case 1:
							param.levelType = MapType.LevelThree;
							newNode = new LevelTree(LevelTree.LEVEL_MAIN, id++, LevelTree.DIFF_EASY, i + 1, param, "Third Tutorial", "Unknown");
							break;
						case 2:
							param.levelType = MapType.LevelFour;
							newNode = new LevelTree(LevelTree.LEVEL_MAIN, id++, LevelTree.DIFF_EASY, i + 1, param, "Fourth Tutorial", "Unknown");
							break;
						case 3:
							param.levelType = MapType.LevelFive;
							newNode = new LevelTree(LevelTree.LEVEL_MAIN, id++, LevelTree.DIFF_EASY, i + 1, param, "Last Tutorial", "Unknown");
							break;
					}

					if (newNode == null) continue; // shouldnt happen
					
					mapTree.GetLastMainNode().AddChild(newNode);

					if (i + 1 == mainLineCount)
					{
						newNode.IsLastNode = true;
					}

					counter--;
				}
			}

			if (worldLevel > 0)
			{
				for (int i = 0; i < counter; i++)
				{
					LevelTree mainNode = mapTree.GetRandomMainNode();
					LevelTree nextNode = CreateRandomExtraNodeLevel(id++, mainNode.Depth);
					mainNode.AddChild(nextNode);
				}

				for (int i = 0; i < maxSpecialLevelsCount; i++)
				{
					LevelTree mainNode = mapTree.GetRandomSpecialNode();
					LevelTree nextNode = CreateRandomExtraNodeLevel(id++, mainNode.Depth);
					mainNode.AddChild(nextNode);
				}
			}

			Debug.Log(mapTree.PrintInfo());
		}

		public LevelTree CreateRandomMainNodeLevel(int id, int depth, bool addShop, int difficulty)
		{
			LevelParams param = new LevelParams(MapType.GenericMonster);
			param.difficulty = difficulty;

			if (addShop)
			{
				param.shop = new ShopData();
				param.shop.GenerateRandomShopItems(worldLevel, 2);
			}

			LevelTree newNode = new LevelTree(LevelTree.LEVEL_MAIN, id, difficulty, depth, param, "Level " + id, "This level is filled with random monsters.");

			switch (difficulty)
			{
				case 1:
					newNode.AddLevelRewardRandom(100, ItemType.STAT_UPGRADE, 1, 1);
					newNode.AddLevelReward(typeof(DnaItem), 100, 3, 10);
					break;
				case 2:
					newNode.AddLevelRewardRandom(100, ItemType.STAT_UPGRADE, 1, 1);
					newNode.AddLevelReward(typeof(DnaItem), 100, 3, 20);
					break;
				case 3:
					newNode.AddLevelRewardRandom(100, ItemType.STAT_UPGRADE, 1, 1);
					newNode.AddLevelReward(typeof(DnaItem), 100, 3, 30);
					break;
			}

			return newNode;
		}

		public LevelTree CreateRandomExtraNodeLevel(int id, int depth)
		{
			LevelParams param = new LevelParams(MapType.GenericMonster); //TODO new types

			int difficulty = LevelTree.DIFF_MEDIUM;
			int rnd = Random.Range(0, 100);
			if (rnd < 25)
				difficulty = LevelTree.DIFF_EASY;
			if(rnd < 50)
				difficulty = LevelTree.DIFF_HARD;

			LevelTree newNode = new LevelTree(LevelTree.LEVEL_EXTRA, id, difficulty, depth, param, "Extra Level " + id, "This level is filled with random monsters.");

			switch (difficulty)
			{
				case 1:
					newNode.AddLevelRewardRandom(66, ItemType.STAT_UPGRADE, 1, 1);
					newNode.AddLevelReward(typeof(DnaItem), 100, 3, 10);
					break;
				case 2:
					newNode.AddLevelRewardRandom(100, ItemType.STAT_UPGRADE, 1, 1);
					newNode.AddLevelReward(typeof(DnaItem), 100, 3, 20);
					break;
				case 3:
					newNode.AddLevelRewardRandom(100, ItemType.STAT_UPGRADE, 1, 1);
					newNode.AddLevelRewardRandom(100, ItemType.CLASSIC_UPGRADE, 1, 1);
					newNode.AddLevelReward(typeof(DnaItem), 100, 3, 30);
					break;
			}

			return newNode;
		}

		public bool OnLevelSelect(PlayerData data, LevelTree node)
		{
			Debug.Log("selected " + node.Name + "" + node.Depth);

			if (!CanSelectLevel(node))
			{
				return false;
			}

			activeMap.OnTeleportOut(data.player);

			GenerateAndOpenLevel(node);

			// teleport player to new start
			data.transform.position = GetStartPosition();

			activeMap.OnTeleportIn(data.player);

			return true;
		}

		public void OnLevelFinished()
		{
			LevelTree currentNode = null;

			foreach (LevelTree t in mapTree.GetAllNodes())
			{
				if (activeMap.levelData.Equals(t.levelData))
				{
					currentNode = t;
					break;
				}
			}

			foreach (LevelTree ch in currentNode.Childs)
			{
				ch.Unlocked = true;
			}

			if (currentNode.IsLastNode)
			{
				GoToNextWorld();
			}
		}

		public void OnShopOpen(Player player)
		{
			activeMap.OnShopOpen(player);
		}

		public bool CanSelectLevel(LevelTree node)
		{
			if (node.Unlocked == false)
				return false;

			if (node.CurrentlyActive)
				return false;

			return true;
		}

		public void GenerateAndOpenLevel(LevelTree levelNode)
		{
			LevelParams param = levelNode.LevelParams;

			// map hasnt been generated yet
			if (!maps.ContainsKey(new Cords(levelNode.Id, 0)))
			{
				MapHolder newMap = new MapHolder(this, levelNode.Name, new Cords(levelNode.Id, 0), param.levelType, width, height, param, levelNode.LevelReward);

				newMap.CreateMap();
				maps.Add(new Cords(levelNode.Id, 0), newMap);

				levelNode.levelData = newMap.levelData;
			}

			foreach (LevelTree n in mapTree.GetAllNodes())
				n.CurrentlyActive = false;

			levelNode.CurrentlyActive = true;
			levelNode.Unlocked = true;

			SetActiveLevel(levelNode.Id, 0, false);
		}

		private void GenerateFirstLevel()
		{
			GenerateAndOpenLevel(mapTree);
		}

		public Cords GenerateNextLevel(int teleporterType)
		{
			Cords old = activeMap.Position;
			Cords newCords = new Cords(old.x + 1, old.y);

			int newId = newCords.x;

			LevelTree nextNode = mapTree.GetNode(newId);

			if (nextNode == null)
			{
				WorldHolder.instance.GoToNextWorld();
			}

			GenerateAndOpenLevel(nextNode);

			/*int level = old.x + 2;
			if (skipTutorial)
			{
				level += 5;
				Debug.Log("skipping tutorial..");
			}

			MapType type = MapType.LevelOne;
			int mapLevel = 1;
			LevelParams param = new LevelParams(MapType.LevelOne);

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
					type = GetNextLevelType(level-5, ref mapLevel, param);
					break;
			}

			MapHolder newMap = new MapHolder(this, "Level " + (newCords.x+1), newCords, type, 100, 50, param, mapLevel);
			newMap.CreateMap();

			maps.Add(newCords, newMap);*/
			return newCords;
		}

		private MapType GetNextLevelType(int level, ref int mapLevel, LevelParams param)
		{
			MapType nextLevel = MapType.SixRegions;

			switch (level)
			{
				case 1:
					param.mapLevel = 1;
					nextLevel = MapType.BossRush;
					break;
				case 2:
					param.mapLevel = 1;
					param.variant = Random.Range(1, 3);
					nextLevel = MapType.FourRegion;
					break;
				case 3:
					param.mapLevel = 1;
					param.variant = Random.Range(1, 3);
					nextLevel = MapType.FindBoss;
					break;
			}
			
			return nextLevel;
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
			if (GameSystem.Instance.CurrentPlayer.GetData().ui.consoleActive == false)
			{
				if (Input.GetKeyDown(KeyCode.Space))
				{
					Camera.main.orthographicSize = 55;
					RegenMap();

					activeMap.PrintMapToTxt();
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

					activeMap.levelData.LevelReward.DoDrop(null, GameSystem.Instance.CurrentPlayer, true);

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
