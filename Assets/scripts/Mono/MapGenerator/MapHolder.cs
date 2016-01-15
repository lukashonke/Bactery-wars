﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.Base;
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
		Hardcoded,


		StartClassic,
	}

    /// <summary>
    /// Obsahuje jeden region (oblast o rozloze 50x50 nebo podle nastaveni), vcetne vsech zdi, podlah apod.
    /// Kazdy region obvykle obsahuje jednu mistnost (MapRoom), ktera reprezentuje oblast v regionu, po ktere muze hrac chodit
    /// </summary>
	public class MapRegion
	{
		public int Status { get; set; }

        public static readonly int STATUS_HOSTILE = 1;
        public static readonly int STATUS_CONQUERED = 2;

        public bool empty;
		public int x, y;
		public Tile[,] tileMap;
		public RegionGenerator regionGen; //TODO remove, not neccessary

		public bool isAccessibleFromStart;
		public bool isLockedRegion;
		public bool isStartRegion; // marks that the player starts in this region upon spawning into the map
		public bool onlyOnePassage = false;
		public bool hasOutTeleporter = false;


		public MapRegion(int x, int y, Tile[,] tileMap, RegionGenerator regionGen)
		{
			this.x = x;
			this.y = y;
			this.regionGen = regionGen;
			this.tileMap = tileMap;

		    Status = STATUS_HOSTILE;
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

        public MapRoom GetMapRoom()
        {
            return WorldHolder.instance.activeMap.GetRoom(this);
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

        public void SetEmpty()
        {
            empty = true;
        }
	}

	/// <summary>
	/// Reprezentuje jednu velkou mapu ve svete, mapy jsou propojene skrze "portaly" pro transport (reprezentovane jako zily)
	/// </summary>
	public class MapHolder
	{
		public WorldHolder World { get; set; }

		public bool isActive;

		public string name;
		private WorldHolder.Cords position;
		private MapType mapType;

	    private MapProcessor mapProcessor;
		public Tile[,] SceneMap { get; set; }
		public Dictionary<WorldHolder.Cords, MapRegion> regions;

		public List<MapPassage> passages; 

		public MeshFilter mesh;
		public MeshGenerator meshGen;

		private List<Monster> activeMonsters;
		private List<Npc> activeNpcs;
		private List<MonsterSpawnInfo> spawnedMonsters;
		private List<MonsterSpawnInfo> spawnedNpcs; 

		private int MAX_REGIONS = 3;
		private int regionSize;

		public WorldHolder.Cords Position
		{
			get { return position; }
		}

		public MapType MapType
		{
			get { return mapType; }
		}

		public MapHolder(WorldHolder world, string name, WorldHolder.Cords position, MapType mapType)
		{
			this.World = world;

			this.name = name;
			this.position = position;
			this.mapType = mapType;
		}

		public void AddPassage(MapPassage p)
		{
			passages.Add(p);
		}

        public MapPassage GetPassage(MapRegion regionA, MapRegion regionB)
	    {
	        foreach (MapPassage passage in passages)
	        {
	            if (regionA.Equals(passage.roomA.region) && regionB.Equals(passage.roomB.region))
	            {
	                return passage;
	            }

                if (regionB.Equals(passage.roomA.region) && regionA.Equals(passage.roomB.region))
                {
                    return passage;
                }
	        }

            return null;
	    }

	    public void OpenPassage(MapPassage p)
	    {
            p.SetEnabled(false);	        
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
			return new Vector3((-World.width/2) + t.tileX, (-World.height/2) + t.tileY, 0);
		}

	    public Tile GetTileFromWorldPosition(Vector3 pos)
	    {
	        int x = Mathf.RoundToInt(pos.x + World.width/2);
	        int y = Mathf.RoundToInt(pos.y + World.width/2);

	        return SceneMap[x, y];
	    }

        public MapRegion GetRegionFromWorldPosition(Vector3 pos)
        {
            Tile t = GetTileFromWorldPosition(pos);
            if (t != null)
                return t.region;

            return null;
        }

		public void CreateMap()
		{
			regions = new Dictionary<WorldHolder.Cords, MapRegion>();
			passages = new List<MapPassage>();

			activeMonsters = new List<Monster>();
			activeNpcs = new List<Npc>();
			spawnedMonsters = new List<MonsterSpawnInfo>();
			spawnedNpcs = new List<MonsterSpawnInfo>();

			regionSize = World.width + 2;
			
			SceneMap = new Tile[regionSize * MAX_REGIONS, regionSize * MAX_REGIONS];

			switch (mapType)
			{
					case MapType.StartClassic:

					GenerateDungeonRegion(0, 0, 35, true);
					GenerateEmptyRegion(0, 1);
					GenerateEmptyRegion(0, 2);
					GenerateDungeonRegion(1, 0, World.randomFillPercent, false, true, true);
					GenerateEmptyRegion(1, 1);
					GenerateEmptyRegion(1, 2);
					GenerateEmptyRegion(2, 0);
					GenerateEmptyRegion(2, 1);
					GenerateEmptyRegion(2, 2);

					break;
					case MapType.Test:

					GenerateDungeonRegion(0, 0, World.randomFillPercent, true);
					GenerateDungeonRegion(0, 1, World.randomFillPercent, false);
					GenerateDungeonRegion(0, 2, World.randomFillPercent, false);
					GenerateEmptyRegion(1, 0);
					GenerateEmptyRegion(1, 1);
					GenerateDungeonRegion(1, 2, World.randomFillPercent, false);
					GenerateEmptyRegion(2, 0);
					GenerateEmptyRegion(2, 1);
					GenerateDungeonRegion(2, 2, World.randomFillPercent, false, false, true);

					break;
					case MapType.Hardcoded:

					GenerateHardcodedMap(0, 0, "Town", true);

					break;

					case MapType.OpenCave:

					GenerateDungeonRegion(0, 0, 43, true);
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

					GenerateDungeonRegion(0, 0, World.randomFillPercent, true);
					GenerateDungeonRegion(0, 1, World.randomFillPercent, false);
					GenerateDungeonRegion(0, 2, World.randomFillPercent, false);
					GenerateDungeonRegion(1, 0, World.randomFillPercent, false);
					GenerateDungeonRegion(1, 1, World.randomFillPercent, false, true, false);
					GenerateDungeonRegion(1, 2, World.randomFillPercent, false);
					GenerateDungeonRegion(2, 0, World.randomFillPercent, false);
					GenerateDungeonRegion(2, 1, World.randomFillPercent, false);
					GenerateDungeonRegion(2, 2, World.randomFillPercent, false, false, true);

					break;
					case MapType.DungeonCentralClosed:

					GenerateDungeonRegion(0, 0, World.randomFillPercent, true);
					GenerateDungeonRegion(0, 1, World.randomFillPercent, false);
					GenerateDungeonRegion(0, 2, World.randomFillPercent, false);
					GenerateDungeonRegion(1, 0, World.randomFillPercent, false);
					GenerateEmptyRegion(1, 1);
					GenerateDungeonRegion(1, 2, World.randomFillPercent, false, false, true);
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
			mapProcessor = new MapProcessor(this, SceneMap, mapType);

			mapProcessor.Process();

		    Debug.Log(GetTileWorldPosition(SceneMap[0,0]));
            Debug.Log(GetTileWorldPosition(SceneMap[50, 50]));

            GetTile(0, 0).SetColor(Tile.BLUE);
            GetTile(50, 50).SetColor(Tile.BLUE);

			SceneMap = mapProcessor.Tiles;
		}

		public void GenerateHardcodedMap(int regX, int regY, string name, bool isStartRegion)
		{
			if (World.useRandomSeed)
			{
				World.seed = World.GetRandomSeed();
			}

			HardcodedRegionGenerator regionGenerator = new HardcodedRegionGenerator(World.width, World.height, World.seed, World.doDebug, name);

			Tile[,] tileMap = regionGenerator.GenerateMap();

			AddToSceneMap(tileMap, regX, regY);

			MapRegion region = new MapRegion(regX, regY, tileMap, regionGenerator);
			region.AssignTilesToThisRegion();
			region.isAccessibleFromStart = true;
			region.isStartRegion = isStartRegion;
			regions.Add(new WorldHolder.Cords(regX, regY), region);
		}

		protected void GenerateEmptyRegion(int regX, int regY)
		{
			Tile[,] tileMap = new Tile[World.width+2,World.height+2];

			for (int x = 0; x < tileMap.GetLength(0); x++)
			{
				for (int y = 0; y < tileMap.GetLength(1); y++)
				{
					tileMap[x,y] = new Tile(x,y,WorldHolder.WALL);
				}
			}

			AddToSceneMap(tileMap, regX, regY);

			MapRegion region = new MapRegion(regX, regY, tileMap, null);
		    region.SetEmpty();
			region.AssignTilesToThisRegion();
			regions.Add(new WorldHolder.Cords(regX, regY), region);
		}

		protected MapRegion GenerateDungeonRegion(int x, int y, int randomFillPercent, bool isStartRegion)
		{
			return GenerateDungeonRegion(x, y, randomFillPercent, isStartRegion, false, false);
		}

		protected MapRegion GenerateDungeonRegion(int x, int y, int randomFillPercent, bool isStartRegion, bool isLockedRegion, bool hasOutTeleporter)
		{
			//Debug.Log("generating and enabling NEW region .. " + x + ", " + y);

			String seed = World.seed;

			if (World.useRandomSeed)
			{
				seed = World.GetRandomSeed();
				Debug.Log(x + " " + y + " is using " + seed);
			}

			//float xSize = (world.width) * world.SQUARE_SIZE;
			//float ySize = (world.height) * world.SQUARE_SIZE;
			//Vector3 shiftVector = new Vector3(x * xSize, y * ySize);

			RegionGenerator regionGenerator = new DungeonRegionGenerator(World.width, World.height, seed, randomFillPercent, World.doDebug);

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
			region.isAccessibleFromStart = true;
			region.isStartRegion = isStartRegion;
			region.isLockedRegion = isLockedRegion;
			region.hasOutTeleporter = hasOutTeleporter;
			regions.Add(new WorldHolder.Cords(x, y), region);

			return region;
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

			foreach (Monster m in activeMonsters)
			{
				m.Data.DeleteMe();
			}

			foreach (Npc m in activeNpcs)
			{
				m.Data.DeleteMe();
			}

			activeMonsters.Clear();
			activeNpcs.Clear();

			spawnedMonsters.Clear();
			spawnedNpcs.Clear();

			regions.Clear();
			SetActive(false);
		}

		public void LoadMap(bool reloading)
		{
			GenerateMapMesh();

			foreach (MapPassage p in passages)
			{
                if(p.enabled)
				    InitPassage(p);
			}

			activeMonsters.Clear();
			activeNpcs.Clear();

			try
			{
				foreach (MonsterSpawnInfo info in spawnedMonsters)
				{
				    AddMonsterToMap(info, true);
				}

				foreach (MonsterSpawnInfo info in spawnedNpcs)
				{
				    AddNpcToMap(info.MonsterId, info.SpawnPos, true);
				}
			}
			catch (Exception)
			{
				Debug.LogError("error");
			}

			spawnedMonsters.Clear();
			spawnedNpcs.Clear();

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

			spawnedMonsters.Clear();
			spawnedNpcs.Clear();

			foreach (Monster m in activeMonsters)
			{
				if (m != null && m.Data != null)
				{
					spawnedMonsters.Add(m.SpawnInfo);
					m.Data.DeleteMe();
				}
			}

			foreach (Npc m in activeNpcs)
			{
				if (m != null && m.Data != null)
				{
					spawnedNpcs.Add(new MonsterSpawnInfo(m.Template.GetMonsterId(), m.GetData().GetBody().transform.position));
					m.Data.DeleteMe();
				}
			}

			activeMonsters.Clear();
			activeNpcs.Clear();

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

			float xSize = (SceneMap.GetLength(0) - 1) * World.SQUARE_SIZE;
			float ySize = (SceneMap.GetLength(1) - 1) * World.SQUARE_SIZE;

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
					Vector3 pos = new Vector3(((-xSize / 2) + x + .5f) + World.width+2, ((-ySize / 2) + y + .5f) + World.height+2, 0);
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
			MeshGenerator generator = new MeshGenerator(World.gameObject);

			float xSize = (intMap.GetLength(0) - 1) * World.SQUARE_SIZE;
			float ySize = (intMap.GetLength(1) - 1) * World.SQUARE_SIZE;

			Vector3 shiftVector = new Vector3((xSize-1) / 2 - (World.width / 2), (ySize-1) / 2 - (World.height / 2));
			MeshFilter mesh = generator.GenerateMesh("Map mesh", intMap, World.SQUARE_SIZE, shiftVector);

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
			float xSize = (World.width + 1) * World.SQUARE_SIZE;
			float ySize = (World.height + 1) * World.SQUARE_SIZE;

			float regionX = (pos.x + xSize / 2) / xSize;
			float regionY = (pos.y + ySize / 2) / ySize;

			MapRegion reg;
			WorldHolder.Cords c = new WorldHolder.Cords((int)regionX, (int)regionY);
			regions.TryGetValue(c, out reg);

			return reg;
		}

        //TODO can there be more than one room? if so, return it as array
        public MapRoom GetRoom(MapRegion region)
	    {
            foreach (MapRoom room in mapProcessor.rooms)
            {
                if (room.region.Equals(region))
                    return room;
            }
            return null;
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

        /////////////////////

		/*private Npc SpawnNpcToWorld(MonsterId monsterId, Vector3 position)
		{
			Npc npc = GameSystem.Instance.SpawnNpc(monsterId, position);

			RegisterNpcToMap(npc);

			return npc;
		}*/

		/*private Monster SpawnMonsterToWorld(MonsterId monsterId, Vector3 position)
		{
			Monster npc = GameSystem.Instance.SpawnMonster(monsterId, position, false);

			RegisterMonsterToMap(npc);

			return npc;
		}*/

		public Npc AddNpcToMap(MonsterId monsterId, Vector3 position, bool forceSpawnNow=false)
		{
			if (isActive || forceSpawnNow)
			{
                Npc npc = GameSystem.Instance.SpawnNpc(monsterId, position);

                RegisterNpcToMap(npc);

			    return npc;
			}

			spawnedNpcs.Add(new MonsterSpawnInfo(monsterId, position));
			return null;
		}

	    public Monster AddMonsterToMap(MonsterSpawnInfo info, bool forceSpawnNow=false)
	    {
	        if (isActive || forceSpawnNow)
	        {
                Monster m = GameSystem.Instance.SpawnMonster(info.MonsterId, info.SpawnPos, false);
                m.SetSpawnInfo(info);

                RegisterMonsterToMap(m, info);
	            return m;
	        }
	        else
	        {
                spawnedMonsters.Add(info);
	            return null;
	        }
	    }

	    public void RegisterMonsterToMap(Monster m, MonsterSpawnInfo info=null)
	    {
            //auto create monsterspawninfo
            if (info == null)
            {
                Vector3 pos = m.GetData().GetBody().transform.position;
                info = new MonsterSpawnInfo(m.Template.GetMonsterId(), pos);

                MapRegion reg = GetRegionFromWorldPosition(pos);

                if (reg == null)
                {
                    Debug.LogError("Cant assign region for monster " + m.Name + " on pos " + pos + ", monster now registered!");
                    return;
                }

                info.SetRegion(reg);
            }

            activeMonsters.Add(m);
	    }

	    public void RegisterNpcToMap(Npc m)
	    {
	        activeNpcs.Add(m);
	    }

	    public void NotifyCharacterDied(Character ch)
	    {
	        if (ch is Player)
	        {
	            //do nothing
	        }
            else if (ch is Npc)
            {
                // shouldnt happen
            }
            else if (ch is Monster)
            {
                // this monster was added from editor and is not registered to the map - ignore its dead here
                if (((Monster) ch).SpawnInfo == null)
                    return;

                for(int i = 0; i < activeMonsters.Count; i++)
                {
                    Monster temp = activeMonsters[i];

                    if (temp.Equals(ch))
                    {
                        activeMonsters.Remove(temp);
                        UpdateRegionStatus(temp.SpawnInfo.Region);
                        break;
                    }
                }
            }
	    }

	    private void UpdateRegionStatus(MapRegion region)
	    {
	        int countInRegion = 0;

	        foreach (Monster m in activeMonsters)
	        {
	            if (m.SpawnInfo.Region.Equals(region) && m.SpawnInfo.mustDieToProceed)
	            {
	                countInRegion++;
	            }
	        }

            Debug.Log(countInRegion);

	        if (countInRegion > 0)
	            return;

	        region.Status = MapRegion.STATUS_CONQUERED;

            foreach(MapRegion neighbour in GetNeighbourRegions(region))
	        {
	            if(neighbour == null || neighbour.empty)
                    continue;

	            if (neighbour.isLockedRegion)
	            {
	                MapPassage pas = GetPassage(region, neighbour);

	                if (pas != null)
	                {
                        Debug.Log("opened!");
	                    OpenPassage(pas);
	                }
	                else
	                {
	                    Debug.LogError("cant find passage");
	                }
	            }
	        }

	        //TODO check if any monsters are left inside a region, if yes and the region is locked, open it 
	    }

		public Vector3 GetStartPosition()
		{
			foreach (MapRegion region in regions.Values)
			{
				if (region.isStartRegion)
				{
					Tile mostLeft = null;
					int mostLeftX = Int32.MaxValue;

					foreach (Tile t in SceneMap)
					{
						if (!t.region.Equals(region) || t.tileType != WorldHolder.GROUND)
							continue;

						if (t.tileX < mostLeftX)
						{
							mostLeft = t;
							mostLeftX = t.tileX;
						}
					}

					return GetTileWorldPosition(mostLeft);
				}
			}

			return new Vector3();
		}

	    public bool CanTeleportToNext()
	    {
	        foreach (MapRegion reg in regions.Values)
	        {
	            if (reg.hasOutTeleporter)
	            {
	                UpdateRegionStatus(reg);

                    if(reg.Status == MapRegion.STATUS_CONQUERED)
                        return true;
	            }
	        }

	        return false;
	    }

        /*private class MonsterSpawnInfo
        {
            public MonsterId monster;
            public Vector3 position;

            public MonsterSpawnInfo(MonsterId m, Vector3 pos)
            {
                monster = m;
                position = pos;
            }
        }*/
	}
}
