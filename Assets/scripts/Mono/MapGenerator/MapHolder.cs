using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Actor.MonsterClasses.Base;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Assets.scripts.Mono.MapGenerator
{
	public enum MapType
	{
		Test,
		OpenCave,
		DungeonAllOpen,
		DungeonCentralClosed,
		Hardcoded
	}

	public class MapRegion
	{
		private bool active;

		public int x, y;
		public Tile[,] tileMap;
		public RegionGenerator regionGen; //TODO remove, not neccessary

		public bool isAccessibleFromStart;
		public bool isLockedRegion;
		public bool isStartRegion; // marks that the player starts in this region upon spawning into the map
		public bool onlyOnePassage = false;

		public MapRegion(int x, int y, Tile[,] tileMap, RegionGenerator regionGen)
		{
			this.x = x;
			this.y = y;
			this.regionGen = regionGen;
			this.tileMap = tileMap;

			active = true;
		}

		public void AssignTilesToThisRegion()
		{
			for (int i = 0; i < tileMap.GetLength(0); i++)
			{
				for (int j = 0; j < tileMap.GetLength(1); j++)
				{
					tileMap[i, j].AssignRegion(this);
				}
			}
		}

		public MapRegion SetAccessibleFromStart(bool b)
		{
			isAccessibleFromStart = b;
			return this;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}

			MapRegion reg = (MapRegion) obj;
			if (x == reg.x && y == reg.y)
				return true;
			return false;
		}

		/*public void Disable()
		{
			if (mesh == null)
				return;
			mesh.gameObject.SetActive(false);
			active = false;
		}

		public void Enable()
		{
			if (mesh == null)
				return;
			mesh.gameObject.SetActive(true);
			active = true;
		}*/
	}

	/// <summary>
	/// Reprezentuje jednu velkou mapu ve svete, mapy jsou propojene skrze "portaly" pro transport (reprezentovane jako zily)
	/// </summary>
	public class MapHolder
	{
		public WorldHolder world;

		public bool isActive;

		public string name;
		private WorldHolder.Cords position;
		private MapType mapType;

		public Tile[,] SceneMap { get; set; }
		public Dictionary<WorldHolder.Cords, MapRegion> regions;

		public List<MapPassage> passages; 

		public MeshFilter mesh;
		public MeshGenerator meshGen;

		private List<Monster> monsters = new List<Monster>();
		private List<Npc> npcs = new List<Npc>();

		private int MAX_REGIONS = 3;
		private int regionSize;

		public WorldHolder.Cords Position
		{
			get { return position; }
		}

		public MapType MapType1
		{
			get { return mapType; }
		}

		public MapHolder(WorldHolder world, string name, WorldHolder.Cords position, MapType mapType)
		{
			this.world = world;

			this.name = name;
			this.position = position;
			this.mapType = mapType;
		}

		public void AddPassage(MapPassage p)
		{
			passages.Add(p);
		}

		public void InitPassage(MapPassage p)
		{
			if (p.isDoor)
			{
				Vector3 start = GetTileWorldPosition(p.starTile);
				Vector3 end = GetTileWorldPosition(p.endTile);
				Vector3 centerTile = GetTileWorldPosition(p.centerTile);

				Vector3 line = end - start;

				GameObject doorTemplate = Resources.Load("Prefabs/Map/Dungeon Door") as GameObject;

				Quaternion newRotation = Quaternion.LookRotation(start - end, Vector3.forward);
				newRotation.x = 0;
				newRotation.y = 0;

				GameObject door = Object.Instantiate(doorTemplate, centerTile, newRotation) as GameObject;
				door.transform.localScale = new Vector3(0.2f, (line.magnitude) * 0.1f, 1);
				//door.layer = 12; // TODO include this in pathfinding? but then require real time update

				p.AssignGameObject(door);

				Debug.DrawRay(GetTileWorldPosition(p.starTile), line, Color.yellow, 20f);
			}
		}

		public Vector3 GetTileWorldPosition(Tile t)
		{
			return new Vector3((-world.width/2) + t.tileX, (-world.height/2) + t.tileY, 0);
		}

		public void CreateMap()
		{
			regions = new Dictionary<WorldHolder.Cords, MapRegion>();
			passages = new List<MapPassage>();

			regionSize = world.width + 2;
			
			SceneMap = new Tile[regionSize * MAX_REGIONS, regionSize * MAX_REGIONS];

			switch (mapType)
			{
					case MapType.Test:

						GenerateDungeonRegion(world.seed, 0, 0, world.randomFillPercent, true, true);
						GenerateDungeonRegion(world.seed, 0, 1, world.randomFillPercent, true, false);
						GenerateEmptyRegion(0, 2);

						GenerateEmptyRegion(1, 0);
						GenerateEmptyRegion(1, 1);
						GenerateEmptyRegion(1, 2);
						GenerateEmptyRegion(2, 0);
						GenerateEmptyRegion(2, 1);
						GenerateEmptyRegion(2, 2);

					break;
					case MapType.Hardcoded:

						GenerateHardcodedMap(0, 0, "Town", true);

					break;

					case MapType.OpenCave:

						GenerateDungeonRegion(world.seed, 0, 0, 43, true, true);
						GenerateEmptyRegion(0, 1);
						GenerateEmptyRegion(0, 2);

						GenerateEmptyRegion(1, 0);
						GenerateEmptyRegion(1, 1);
						GenerateEmptyRegion(1, 2);
						GenerateEmptyRegion(2, 0);
						GenerateEmptyRegion(2, 1);
						GenerateEmptyRegion(2, 2);

					break;

					case MapType.DungeonAllOpen:

						GenerateDungeonRegion(world.seed, 0, 0, world.randomFillPercent, true, true);
						GenerateDungeonRegion(world.seed, 0, 1, world.randomFillPercent, true, false);
						GenerateDungeonRegion(world.seed, 0, 2, world.randomFillPercent, true, false);
						GenerateDungeonRegion(world.seed, 1, 0, world.randomFillPercent, true, false);
						GenerateDungeonRegion(world.seed, 1, 1, world.randomFillPercent, true, false, true);
						GenerateDungeonRegion(world.seed, 1, 2, world.randomFillPercent, true, false);
						GenerateDungeonRegion(world.seed, 2, 0, world.randomFillPercent, true, false);
						GenerateDungeonRegion(world.seed, 2, 1, world.randomFillPercent, true, false);
						GenerateDungeonRegion(world.seed, 2, 2, world.randomFillPercent, true, false);

					break;
					case MapType.DungeonCentralClosed:

						GenerateDungeonRegion(world.seed, 0, 0, world.randomFillPercent, true, true);
						GenerateDungeonRegion(world.seed, 0, 1, world.randomFillPercent, true, false);
						GenerateDungeonRegion(world.seed, 0, 2, world.randomFillPercent, true, false);
						GenerateDungeonRegion(world.seed, 1, 0, world.randomFillPercent, true, false);
						GenerateEmptyRegion(1, 1);
						GenerateDungeonRegion(world.seed, 1, 2, world.randomFillPercent, true, false);
						GenerateEmptyRegion(2, 0);
						GenerateEmptyRegion(2, 1);
						GenerateEmptyRegion(2, 2);
						//GenerateDungeonRoom(world.seed, 2, 0, world.randomFillPercent, true, false);
						//GenerateDungeonRoom(world.seed, 2, 1, world.randomFillPercent, true, false);
						//GenerateDungeonRoom(world.seed, 2, 2, world.randomFillPercent, true, false);

					break;

			}

			Utils.Timer.StartTimer("mapprocess");
			ProcessSceneMap();
			Utils.Timer.EndTimer("mapprocess");
		}

		public void ProcessSceneMap()
		{
			Debug.Log("processing scene map");
			MapProcessor processor = new MapProcessor(this, SceneMap, mapType);

			processor.CreatePassages();

			SceneMap = processor.Tiles;
		}

		public void GenerateHardcodedMap(int regX, int regY, string name, bool isStartRegion)
		{
			if (world.useRandomSeed)
			{
				world.seed = world.GetRandomSeed();
			}

			HardcodedRegionGenerator regionGenerator = new HardcodedRegionGenerator(world.width, world.height, world.seed, world.doDebug, name);

			Tile[,] tileMap = regionGenerator.GenerateMap();

			AddToSceneMap(tileMap, regX, regY);

			MapRegion region = new MapRegion(regX, regY, tileMap, regionGenerator);
			region.AssignTilesToThisRegion();
			region.SetAccessibleFromStart(true);
			region.isStartRegion = isStartRegion;
			regions.Add(new WorldHolder.Cords(regX, regY), region);
		}

		protected void GenerateEmptyRegion(int regX, int regY)
		{
			Tile[,] tileMap = new Tile[world.width+2,world.height+2];

			for (int x = 0; x < tileMap.GetLength(0); x++)
			{
				for (int y = 0; y < tileMap.GetLength(1); y++)
				{
					tileMap[x,y] = new Tile(x,y,WorldHolder.WALL);
				}
			}

			AddToSceneMap(tileMap, regX, regY);

			MapRegion region = new MapRegion(regX, regY, tileMap, null);
			region.AssignTilesToThisRegion();
			regions.Add(new WorldHolder.Cords(regX, regY), region);
		}

		protected void GenerateDungeonRegion(string seed, int x, int y, int randomFillPercent, bool isAccessibleFromStart, bool isStartRegion)
		{
			GenerateDungeonRegion(seed, x, y, randomFillPercent, isAccessibleFromStart, isStartRegion, false);
		}

		protected void GenerateDungeonRegion(string seed, int x, int y, int randomFillPercent, bool isAccessibleFromStart, bool isStartRegion, bool isLockedRegion)
		{
			//Debug.Log("generating and enabling NEW region .. " + x + ", " + y);

			if (world.useRandomSeed)
			{
				seed = world.GetRandomSeed();
				Debug.Log(x + " " + y + " is using " + seed);
			}

			//float xSize = (world.width) * world.SQUARE_SIZE;
			//float ySize = (world.height) * world.SQUARE_SIZE;
			//Vector3 shiftVector = new Vector3(x * xSize, y * ySize);

			RegionGenerator regionGenerator = new DungeonRegionGenerator(world.width, world.height, seed, randomFillPercent, world.doDebug);

			Tile[,] tileMap = regionGenerator.GenerateMap();

			AddToSceneMap(tileMap, x, y);

			if (!GameController.DEV_BUILD)
				regionGenerator = null;

			MapRegion region = new MapRegion(x, y, tileMap, regionGenerator);

			// test
			/*if (x == 1 && y == 1)
			{
				region.onlyOnePassage = true;
			}*/

			region.AssignTilesToThisRegion();
			region.SetAccessibleFromStart(isAccessibleFromStart);
			region.isStartRegion = isStartRegion;
			region.isLockedRegion = isLockedRegion;
			regions.Add(new WorldHolder.Cords(x, y), region);
		}

		public void DeleteMap()
		{
			// destroy the map mesh
			meshGen.Delete();
			Object.Destroy(mesh);

			foreach (MapPassage p in passages)
			{
				p.Delete();
			}

			foreach (Monster m in monsters)
			{
				m.Data.DeleteMe();
			}

			foreach (Npc m in npcs)
			{
				m.Data.DeleteMe();
			}

			monsters.Clear();
			npcs.Clear();

			regions.Clear();
			SetActive(false);
		}

		public void LoadMap(bool reloading)
		{
			GenerateMapMesh();

			foreach (MapPassage p in passages)
			{
				InitPassage(p);
			}

			if (reloading)
			{
				try
				{
					for (int i = 0; i < monsters.Count; i++)
					{
						Monster old = monsters[i];
						monsters[i] = SpawnMonster(old.Template.MonsterId, old.GetData().GetBody().transform.position); //TODO create a dictionary here with containing position because GetData is null!
					}

					for (int i = 0; i < npcs.Count; i++)
					{
						Npc old = npcs[i];
						npcs[i] = SpawnNpc(old.Template.MonsterId, old.GetData().GetBody().transform.position);
					}
				}
				catch (Exception)
				{
					Debug.Log("error");
				}
			}

			GameSystem.Instance.UpdatePathfinding();
			SetActive(true);
		}

		public void DeloadMap()
		{
			// destroy the map mesh
			meshGen.Delete();
			Object.Destroy(mesh);

			foreach (MapPassage p in passages)
			{
				p.Delete();
			}

			foreach (Monster m in monsters)
			{
				if(m != null && m.Data != null)
					m.Data.DeleteMe();
			}

			foreach (Npc m in npcs)
			{
				if (m != null && m.Data != null)
					m.Data.DeleteMe();
			}

			SetActive(false);
		}

		public void SetActive(bool active)
		{
			isActive = active;
		}

		public void DrawGizmos()
		{
			/*float xSize = SceneMap.GetLength(0);
			float ySize = SceneMap.GetLength(1);*/

			float xSize = (SceneMap.GetLength(0) - 1) * world.SQUARE_SIZE;
			float ySize = (SceneMap.GetLength(1) - 1) * world.SQUARE_SIZE;

			int i = 0;
			for (int x = 0; x < xSize; x++)
			{
				for (int y = 0; y < ySize; y++)
				{
					Tile t = GetTile(x, y);
					if (t == null)
						continue;

					Gizmos.color = (t.tileType == WorldHolder.WALL) ? Color.black : Color.white;
					if (t.GetColor() == 1)
						Gizmos.color = Color.green;
					else if (t.GetColor() == 2)
						Gizmos.color = Color.magenta;
					else if (t.GetColor() == 3)
						Gizmos.color = Color.yellow;
					else if (t.GetColor() == 4)
						Gizmos.color = new Color(255, 20, 147);
					else if (t.GetColor() == 5)
						Gizmos.color = Color.blue;
					else if (t.GetColor() == 6)
						Gizmos.color = Color.red;
					else if (t.GetColor() == 7)
						Gizmos.color = new Color(128, 0, 128);
					else if (t.GetColor() == 8)
						Gizmos.color = new Color(255, 165, 0);
					else if (t.GetColor() == 9)
						Gizmos.color = new Color(128, 0, 0);

					//Vector3 pos = new Vector3((xSize - 1) / 2 - (world.width / 2), (ySize - 1) / 2 - (world.height / 2));
					Vector3 pos = new Vector3(((-xSize / 2) + x + .5f) + world.width+2, ((-ySize / 2) + y + .5f) + world.height+2, 0);
					Gizmos.DrawCube(pos, Vector3.one);
				}
			}

			//Debug.Log("highlighted " + i );
		}

		private Tile GetTile(int x, int y)
		{
			try
			{
				return SceneMap[x, y];
			}
			catch (Exception)
			{
				return null;
			}
		}

		protected void GenerateMapMesh()
		{
			// int maps of all regions
			/*Dictionary<WorldHolder.Cords, int[,]> map = new Dictionary<WorldHolder.Cords, int[,]>();

			// create int maps of all regions
			for (int x = 0; x < SceneMap.GetLength(0); x++)
			{
				for (int y = 0; y < SceneMap.GetLength(1); y++)
				{
					WorldHolder.Cords c = new WorldHolder.Cords(x / regionSize, y / regionSize);

					if (map.ContainsKey(c))
					{
						Tile t = SceneMap[x, y];
						if (t != null)
							map[c][x % regionSize, y % regionSize] = SceneMap[x, y].tileType;
						else
							map[c][x % regionSize, y % regionSize] = 0;
					}
					else
					{
						map[c] = new int[regionSize, regionSize];

						Tile t = SceneMap[x, y];
						if (t != null)
							map[c][x % regionSize, y % regionSize] = SceneMap[x, y].tileType;
						else
							map[c][x % regionSize, y % regionSize] = 0;
					}
				}
			}*/

			int[,] intMap = new int[SceneMap.GetLength(0),SceneMap.GetLength(1)];

			for (int x = 0; x < SceneMap.GetLength(0); x++)
			{
				for (int y = 0; y < SceneMap.GetLength(1); y++)
				{
					intMap[x, y] = SceneMap[x, y].tileType;
				}
			}

			Utils.Timer.StartTimer("meshgenerate");
			MeshGenerator generator = new MeshGenerator(world.gameObject);

			float xSize = (intMap.GetLength(0) - 1) * world.SQUARE_SIZE;
			float ySize = (intMap.GetLength(1) - 1) * world.SQUARE_SIZE;

			Vector3 shiftVector = new Vector3((xSize-1) / 2 - (world.width / 2), (ySize-1) / 2 - (world.height / 2));
			MeshFilter mesh = generator.GenerateMesh("Map mesh", intMap, world.SQUARE_SIZE, shiftVector);

			mesh.transform.position = shiftVector;
			mesh.gameObject.SetActive(true);

			this.mesh = mesh;
			this.meshGen = generator;

			Utils.Timer.EndTimer("meshgenerate");

			// iterate through int maps and generate meshes
			/*for (int i = 0; i < map.Count; i++)
			{
				KeyValuePair<WorldHolder.Cords, int[,]> e = map.ElementAt(i);

				WorldHolder.Cords c = e.Key;
				int[,] regionMap = e.Value;

				MeshGenerator generator = new MeshGenerator(world.gameObject);

				float xSize = (regionMap.GetLength(0) - 1) * world.SQUARE_SIZE;
				float ySize = (regionMap.GetLength(1) - 1) * world.SQUARE_SIZE;

				Vector3 v = new Vector3(c.x * xSize, c.y * ySize);

				// create the mesh
				MeshFilter mesh = generator.GenerateMesh("Cave " + c.ToString(), regionMap, world.SQUARE_SIZE, v);
				mesh.gameObject.transform.position = v;
				mesh.gameObject.SetActive(true);

				if (world.doDebug)
				{
					mesh.gameObject.SetActive(false);
				}

				// assign the created mesh to the region
				MapRegion reg;
				regions.TryGetValue(c, out reg);
				reg.SetMesh(mesh, generator);
			}*/

			//Debug.Log("pocet regionu na renderovani: " + map.Count);
			/*for (int i = 0; i < map.Count; i++)
			{
				int jednicky = 0;

				for (int x = 0; x < map.ElementAt(i).Value.GetLength(0); x++)
				{
					for (int y = 0; y < map.ElementAt(i).Value.GetLength(1); y++)
					{
					}
				}

				Debug.Log("region " + map.ElementAt(i).Key.ToString() + " ma " + jednicky);
			}*/
		}

		protected void AddToSceneMap(Tile[,] tiles, int regionX, int regionY)
		{
			for (int x = 0; x < tiles.GetLength(0); x++)
			{
				for (int y = 0; y < tiles.GetLength(1); y++)
				{
					int newX = (regionX*regionSize) + x;
					int newY = (regionY*regionSize) + y;

					Tile t = tiles[x, y];

					SceneMap[newX, newY] = t;
					t.tileX = newX;
					t.tileY = newY;
				}
			}
		}

		public MapRegion GetRegion(Vector2 pos)
		{
			float xSize = (world.width + 1) * world.SQUARE_SIZE;
			float ySize = (world.height + 1) * world.SQUARE_SIZE;

			float regionX = (pos.x + xSize / 2) / xSize;
			float regionY = (pos.y + ySize / 2) / ySize;

			MapRegion reg;
			WorldHolder.Cords c = new WorldHolder.Cords((int)regionX, (int)regionY);
			regions.TryGetValue(c, out reg);

			return reg;
		}

		public MapRegion[] GetNeighbourRegions(MapRegion reg)
		{
			MapRegion[] neighbours = new MapRegion[4];

			WorldHolder.Cords c;
			int i = 0;

			c = new WorldHolder.Cords(reg.x + 1, reg.y);
			if (regions.ContainsKey(c))
				neighbours[i++] = regions[c];

			c = new WorldHolder.Cords(reg.x, reg.y +1);
			if (regions.ContainsKey(c))
				neighbours[i++] = regions[c];

			c = new WorldHolder.Cords(reg.x - 1, reg.y);
			if (regions.ContainsKey(c))
				neighbours[i++] = regions[c];

			c = new WorldHolder.Cords(reg.x, reg.y -1);
			if (regions.ContainsKey(c))
				neighbours[i++] = regions[c];

			return neighbours;
		}

		public bool IsNeighbour(MapRegion regA, MapRegion regB)
		{
			if (regA == null || regB == null) return false;

			foreach (MapRegion reg in GetNeighbourRegions(regB))
			{
				if (regA.Equals(reg))
					return true;
			}
			return false;
		}

		public Npc SpawnNpc(MonsterId monsterId, Vector3 position)
		{
			Npc npc = GameSystem.Instance.SpawnNpc(monsterId, position);

			npcs.Add(npc);

			return npc;
		}

		public Monster SpawnMonster(MonsterId monsterId, Vector3 position)
		{
			Monster m = GameSystem.Instance.SpawnMonster(monsterId, position, false);

			monsters.Add(m);

			return m;
		}
	}
}
