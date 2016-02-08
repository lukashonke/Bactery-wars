using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.Base;
using Assets.scripts.Mono.MapGenerator.Levels;
using UnityEngine;
using UnityEngine.SocialPlatforms.GameCenter;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Assets.scripts.Mono.MapGenerator
{
	public enum MapType
	{
		Test,
		DungeonAllOpen,
		DungeonCentralClosed,
		Hardcoded,

		StartClassic,
		SecondLevel,
	}

    /// <summary>
    /// Obsahuje jeden region (oblast o rozloze 50x50 nebo podle nastaveni), vcetne vsech zdi, podlah apod.
    /// Kazdy region obvykle obsahuje jednu mistnost (MapRoom), ktera reprezentuje oblast v regionu, po ktere muze hrac chodit
    /// </summary>
	public class MapRegion
    {
		// the map this region belongs to
	    public MapHolder map;

		public int Status { get; set; }

        public MapRegion parentRegion;

        public static readonly int STATUS_HOSTILE = 1;
        public static readonly int STATUS_CONQUERED = 2;

        public bool empty;
		public int x, y;
        public int sizeX, sizeY;
		public Tile[,] tileMap;
		public RegionGenerator regionGen; //TODO remove, not neccessary

		public bool isAccessibleFromStart;
		public bool isLockedRegion;
		public bool isStartRegion; // marks that the player starts in this region upon spawning into the map
		public bool onlyOnePassage = false;
		public bool hasOutTeleporter = false;

        public int fillPercent;
        public string seed;
        public bool hadRandomSeed;

		public MapRegion(MapHolder map, int x, int y, Tile[,] tileMap, RegionGenerator regionGen)
		{
			this.map = map;
			this.x = x;
			this.y = y;
			this.regionGen = regionGen;
			this.tileMap = tileMap;

		    Status = STATUS_HOSTILE;
		}

        public bool HasParentRegion()
        {
            return parentRegion != null;
        }

	    public MapRegion GetParentOrSelf()
	    {
		    if (parentRegion != null)
			    return parentRegion;
		    return this;
	    }

	    public bool IsInFamily(MapRegion reg)
	    {
			// jeden z nich je rodic
		    if (this.Equals(reg.parentRegion) || reg.Equals(parentRegion))
			    return true;

			// maji spolecneho rodice
		    if (reg.parentRegion != null && reg.parentRegion.Equals(parentRegion))
			    return true;

		    return false;
	    }

		public void AssignTilesToThisRegion()
		{
			for (int i = 0; i < tileMap.GetLength(0); i++)
			{
				for (int j = 0; j < tileMap.GetLength(1); j++)
				{
				    try
				    {
                        tileMap[i, j].AssignRegion(this);
				    }
				    catch (Exception)
				    {
				        Debug.LogError("error for ij " + i + ", " + j);
				    }
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
		public AbstractLevelData levelData;
		public Tile[,] SceneMap { get; set; }
		public Dictionary<WorldHolder.Cords, MapRegion> regions;

		public List<MapPassage> passages; 

		public MeshFilter mesh;
		public MeshGenerator meshGen;

		private List<Monster> activeMonsters;
		private List<Npc> activeNpcs;
		private List<MonsterSpawnInfo> spawnableMonsters;
		private List<MonsterSpawnInfo> spawnableNpcs; 

		private int maxRegionsX = 3;
        private int maxRegionsY = 3;
		private int regionSizeX;
        private int regionSizeY;

	    private int regionWidth;
        private int regionHeight;

		public WorldHolder.Cords Position
		{
			get { return position; }
		}

		public MapType MapType
		{
			get { return mapType; }
		}

		public MapHolder(WorldHolder world, string name, WorldHolder.Cords position, MapType mapType, int regionWidth, int regionHeight)
		{
			this.World = world;

			this.name = name;
			this.position = position;
			this.mapType = mapType;
		    this.regionWidth = regionWidth;
		    this.regionHeight = regionHeight;

			levelData = null;

			switch (mapType)
			{
				case MapType.StartClassic:
					levelData = new StartLevelData(this);
					break;
				case MapType.Test:
					levelData = new TestLevelData(this);
					break;
				case MapType.SecondLevel:
					levelData = new SecondLevelData(this);
					break;
			}

			if(levelData.GetRegionWidth() > 0)
				this.regionWidth = levelData.GetRegionWidth();

			if (levelData.GetRegionHeight() > 0)
				this.regionHeight = levelData.GetRegionHeight();

			if (levelData.GetMaxRegionsX() > 0)
				maxRegionsX = levelData.GetMaxRegionsX();

            if (levelData.GetMaxRegionsY() > 0)
                maxRegionsY = levelData.GetMaxRegionsY();
		}

		public void AddPassage(MapPassage p)
		{
			//Debug.Log("adding passage for " + p.roomA.region.x + ", " + p.roomA.region.y + " AND " + p.roomB.region.x + ", " + p.roomB.region.y);
			passages.Add(p);
		}

        public MapPassage GetPassage(MapRegion regionA, MapRegion regionB)
        {
	        if (regionA.HasParentRegion())
		        regionA = regionA.parentRegion;

			if (regionB.HasParentRegion())
				regionB = regionB.parentRegion;

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
			if(p.enabled)
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
			return new Vector3((-regionWidth/2) + t.tileX, (-regionHeight/2) + t.tileY, 0);
		}

		public Tile GetClosestGroundTile(Vector3 pos)
		{
			Tile cTile = GetTileFromWorldPosition(pos);

			// check direct neighbours
			Tile t = GetTile(cTile.tileX - 1, cTile.tileY);
			if (t != null && t.tileType == WorldHolder.GROUND)
				return t;

			t = GetTile(cTile.tileX + 1, cTile.tileY);
			if (t != null && t.tileType == WorldHolder.GROUND)
				return t;

			t = GetTile(cTile.tileX, cTile.tileY + 1);
			if (t != null && t.tileType == WorldHolder.GROUND)
				return t;

			t = GetTile(cTile.tileX, cTile.tileY - 1);
			if (t != null && t.tileType == WorldHolder.GROUND)
				return t;

			t = GetTile(cTile.tileX - 1, cTile.tileY - 1);
			if (t != null && t.tileType == WorldHolder.GROUND)
				return t;

			t = GetTile(cTile.tileX + 1, cTile.tileY - 1);
			if (t != null && t.tileType == WorldHolder.GROUND)
				return t;

			t = GetTile(cTile.tileX - 1, cTile.tileY + 1);
			if (t != null && t.tileType == WorldHolder.GROUND)
				return t;

			t = GetTile(cTile.tileX + 1, cTile.tileY + 1);
			if (t != null && t.tileType == WorldHolder.GROUND)
				return t;

			Debug.LogError("couldnt find!");
			return cTile;
		}

	    public Tile GetTileFromWorldPosition(Vector3 pos)
	    {
	        int x = Mathf.RoundToInt(pos.x + regionWidth/2);
	        int y = Mathf.RoundToInt(pos.y + regionHeight/2);

	        return SceneMap[x, y];
	    }

        public MapRegion GetRegionFromWorldPosition(Vector3 pos)
        {
            Tile t = GetTileFromWorldPosition(pos);
            if (t != null)
                return t.region;

            return null;
        }

		private int[,] generatedRegionsMap;

		private List<GameObject> darknessPlane; 


		public void CreateMap()
		{
			regions = new Dictionary<WorldHolder.Cords, MapRegion>();
			passages = new List<MapPassage>();

			activeMonsters = new List<Monster>();
			activeNpcs = new List<Npc>();
			spawnableMonsters = new List<MonsterSpawnInfo>();
			spawnableNpcs = new List<MonsterSpawnInfo>();

			darknessPlane = new List<GameObject>();

			regionSizeX = regionWidth + 2;
            regionSizeY = regionHeight + 2;
			
			SceneMap = new Tile[regionSizeX * maxRegionsX, regionSizeY * maxRegionsY];

            generatedRegionsMap = new int[maxRegionsX, maxRegionsY];

			if (levelData != null)
			{
				levelData.Generate();
			}
			else
			{
				switch (mapType)
				{
					case MapType.StartClassic:

						GenerateDungeonRegion(0, 0, 35, true, new[] { 344 }, 2, 1); //0-1; 0
						GenerateDungeonRegion(2, 0, World.randomFillPercent, false, true, true, new[] { 290 });

						GenerateEmptyRegion(0, 1);
						GenerateEmptyRegion(1, 1);
						GenerateEmptyRegion(2, 1);

						GenerateEmptyRegion(0, 2);
						GenerateEmptyRegion(1, 2);
						GenerateEmptyRegion(2, 2);

						/*GenerateDungeonRegion(0, 0, 35, true, new []{344});
						GenerateEmptyRegion(0, 1);
						GenerateEmptyRegion(0, 2);
						GenerateDungeonRegion(1, 0, World.randomFillPercent, false, true, true, new []{290});
						GenerateEmptyRegion(1, 1);
						GenerateEmptyRegion(1, 2);
						GenerateEmptyRegion(2, 0);
						GenerateEmptyRegion(2, 1);
						GenerateEmptyRegion(2, 2);*/

						break;
					case MapType.Test:

						/*GenerateDungeonRegion(0, 0, 35, true, new []{344}); // 0 0
						GenerateDungeonRegion(0, 1, World.randomFillPercent, false, true, false, new []{290}, 2, 1); // 0 1; 1 1

						GenerateDungeonRegion(2, 1, 35, false, new[] { 344 }); // 2 1
						GenerateDungeonRegion(2, 0, 35, false, true, true, new[] { 344 }); // 2 0
						GenerateDungeonRegion(0, 2, 35, false, new[] { 344 }); // 2 1

						GenerateEmptyRegion(1, 0);
						GenerateEmptyRegion(1, 2);
						GenerateEmptyRegion(2, 2);*/

						/*GenerateDungeonRegion(0, 0, 35, true, new[] { 344 }); // 0 0
						GenerateDungeonRegion(0, 1, 35, false, true, false, new[] { 344 }, 1, 2); // 0, 2

						GenerateDungeonRegion(1, 1, World.randomFillPercent, false, true, true, new[] { 290 }, 2, 2); // 0 1; 1 1

						GenerateDungeonRegion(1, 0, 35, false, true, false, new[] { 344 }); // 0 0
						GenerateDungeonRegion(2, 0, 35, false, true, true, new[] { 344 }); // 0 0*/

						GenerateDungeonRegion(0, 0, 35, true, new[] { 344 }); // 0 0
						GenerateDungeonRegion(0, 1, 35, false, true, false, new[] { 344 }, 1, 2); // 0, 2

						GenerateDungeonRegion(1, 1, World.randomFillPercent, false, true, true, new[] { 290 }, 2, 1); // 0 1; 1 1
						GenerateDungeonRegion(1, 2, World.randomFillPercent, false, true, true, new[] { 290 }, 2, 1); // 0 1; 1 1

						GenerateDungeonRegion(1, 0, 35, false, true, false, new[] { 344 }); // 0 0
						GenerateDungeonRegion(2, 0, 35, false, true, true, new[] { 344 }); // 0 0

						/*GenerateDungeonRegion(0, 0, 35, true, false, false, new[] { 344 }, 1, 3); // 0 0
						GenerateDungeonRegion(1, 2, 35, false, true, false, new[] { 344 }, 2, 1); // 0 0
						GenerateDungeonRegion(2, 0, 35, false, true, true, new[] { 344 }, 1, 2); // 0 0

						GenerateEmptyRegion(1, 1);
						GenerateEmptyRegion(1, 0);*/

						/*GenerateDungeonRegion(0, 0, World.randomFillPercent, true);
						GenerateDungeonRegion(0, 1, World.randomFillPercent, false);
						GenerateDungeonRegion(0, 2, World.randomFillPercent, false);
						GenerateEmptyRegion(1, 0);
						GenerateEmptyRegion(1, 1);
						GenerateDungeonRegion(1, 2, World.randomFillPercent, false);
						GenerateEmptyRegion(2, 0);
						GenerateEmptyRegion(2, 1);
						GenerateDungeonRegion(2, 2, World.randomFillPercent, false, false, true);*/

						break;
					case MapType.Hardcoded:

						GenerateHardcodedMap(0, 0, "Town", true);

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
			}

			GenerateMissingEmptyRegions();

			Utils.Timer.StartTimer("mapprocess");
			ProcessSceneMap();
			Utils.Timer.EndTimer("mapprocess");
		}

		private void CreateDarkPlanes() //TODO scale the objects to adjust lightning here
		{
			darknessPlane = new List<GameObject>();

			Vector3 bottomLeft = GetTileWorldPosition(SceneMap[0, 0]);
			Vector3 buttomRight = GetTileWorldPosition(SceneMap[SceneMap.GetLength(0)-1, 0]);
			Vector3 topLeft = GetTileWorldPosition(SceneMap[0, SceneMap.GetLength(1) - 1]);
			Vector3 topRight = GetTileWorldPosition(SceneMap[SceneMap.GetLength(0) - 1, SceneMap.GetLength(1) - 1]);
			Vector3 center = Vector3.Lerp(bottomLeft, topRight, 0.5f);

			GameObject templ = WorldHolder.instance.darkPlaneTemplate;

			float height = Vector3.Distance(bottomLeft, topLeft);
			float width = Vector3.Distance(bottomLeft, buttomRight);
			float size = 50;

			/*GameObject background = GameObject.Find("Background");
			background.transform.position = center + new Vector3(0, 0, 1);
			background.transform.localScale = new Vector3(width, height);*/

			GameObject totalBackground = Object.Instantiate(templ, center + new Vector3(0, 0, 11.5f), Quaternion.Euler(new Vector3(0, 0, 0))) as GameObject;

			// scales up by size
			totalBackground.transform.localScale = new Vector3(width+size, height+size);
			totalBackground.transform.position += new Vector3(size/2, size/2);

			// now move a bit down to create borders
			totalBackground.transform.position -= new Vector3(size/4, size/4);
		}

		private void DeleteDarkPlanes()
		{
			foreach (GameObject o in darknessPlane)
			{
				if(o != null)
					Object.Destroy(o);
			}
		}

		private void GenerateMissingEmptyRegions()
		{
			int count = 0;
			for (int i = 0; i < generatedRegionsMap.GetLength(0); i++)
			{
				for (int j = 0; j < generatedRegionsMap.GetLength(1); j++)
				{
					if (generatedRegionsMap[i, j] == 0)
					{
						count ++;
						GenerateEmptyRegion(i, j);
					}
				}
			}

			Debug.Log("generated mising " +count + " empty regions");
		}

		public void ProcessSceneMap()
		{
			mapProcessor = new MapProcessor(this, SceneMap, mapType);

			mapProcessor.Process();

		    //Debug.Log(GetTileWorldPosition(SceneMap[0,0]));
            //Debug.Log(GetTileWorldPosition(SceneMap[50, 50]));

            //GetTile(0, 0).SetColor(Tile.BLUE);
            //GetTile(50, 50).SetColor(Tile.BLUE);

			SceneMap = mapProcessor.Tiles;
		}

		public void GenerateHardcodedMap(int regX, int regY, string name, bool isStartRegion)
		{
			if (World.useRandomSeed)
			{
				World.seed = World.GetRandomSeed();
			}

			HardcodedRegionGenerator regionGenerator = new HardcodedRegionGenerator(regionWidth, regionHeight, World.seed, World.doDebug, name);

			Tile[,] tileMap = regionGenerator.GenerateMap();

			AddToSceneMap(tileMap, regX, regY);

			MapRegion region = new MapRegion(this, regX, regY, tileMap, regionGenerator);
			region.AssignTilesToThisRegion();
			region.isAccessibleFromStart = true;
			region.isStartRegion = isStartRegion;
			regions.Add(new WorldHolder.Cords(regX, regY), region);
		}

		public void GenerateEmptyRegion(int regX, int regY)
		{
			Tile[,] tileMap = new Tile[regionWidth+2,regionHeight+2];

			for (int x = 0; x < tileMap.GetLength(0); x++)
			{
				for (int y = 0; y < tileMap.GetLength(1); y++)
				{
					tileMap[x,y] = new Tile(x,y,WorldHolder.WALL);
				}
			}

			AddToSceneMap(tileMap, regX, regY);

			MapRegion region = new MapRegion(this, regX, regY, tileMap, null);
		    region.SetEmpty();
			region.AssignTilesToThisRegion();
			regions.Add(new WorldHolder.Cords(regX, regY), region);
		}

        public MapRegion GenerateDungeonRegion(int x, int y, int randomFillPercent, bool isStartRegion, int[] allowedSeeds=null, int sizeX=1, int sizeY=1)
		{
            return GenerateDungeonRegion(x, y, randomFillPercent, isStartRegion, false, false, allowedSeeds, sizeX, sizeY);
		}

		public const bool devSeeds = false;

		public MapRegion GenerateDungeonRegion(int x, int y, int randomFillPercent, bool isStartRegion, bool isLockedRegion, bool hasOutTeleporter, int[] allowedSeeds=null, int sizeX=1, int sizeY=1)
		{
			//Debug.Log("generating and enabling NEW region .. " + x + ", " + y);

			String seed = World.seed;

			if (allowedSeeds != null && !devSeeds)
		    {
		        seed = allowedSeeds[Random.Range(0, allowedSeeds.Length)] + "";
		    }
			else if (World.useRandomSeed)
			{
				seed = World.GetRandomSeed();
                Debug.Log(x + " " + y + " is using " + seed);
			}

			//float xSize = (world.width) * world.SQUARE_SIZE;
			//float ySize = (world.height) * world.SQUARE_SIZE;
			//Vector3 shiftVector = new Vector3(x * xSize, y * ySize);

		    MapRegion parentRegion = null;

            RegionGenerator regionGenerator = new DungeonRegionGenerator(regionWidth*sizeX, regionHeight*sizeY, seed, randomFillPercent, World.doDebug);
            Tile[,] tileMap = regionGenerator.GenerateMap();

		    for (int i = 0; i < sizeX; i++)
		    {
		        for (int j = 0; j < sizeY; j++)
		        {
		            Tile[,] subTileMap = GetSubTileMap(tileMap, i, j);

		            if (i == 0 && j == 0)
		            {
                        parentRegion = new MapRegion(this, x + i, y + j, subTileMap, regionGenerator);

		                parentRegion.hadRandomSeed = (allowedSeeds == null || devSeeds);
		                parentRegion.seed = seed;
		                parentRegion.fillPercent = randomFillPercent;

                        parentRegion.AssignTilesToThisRegion();
                        parentRegion.isAccessibleFromStart = true;
                        parentRegion.isStartRegion = isStartRegion;
                        parentRegion.isLockedRegion = isLockedRegion;
                        parentRegion.hasOutTeleporter = hasOutTeleporter;
                        regions.Add(new WorldHolder.Cords(x+i, y+j), parentRegion);
		            }
		            else
		            {
                        MapRegion region = new MapRegion(this, x+i, y+j, subTileMap, regionGenerator);
		                region.parentRegion = parentRegion;

                        region.AssignTilesToThisRegion();
                        region.isAccessibleFromStart = true;
                        region.isStartRegion = isStartRegion;
                        region.isLockedRegion = isLockedRegion;
                        region.hasOutTeleporter = hasOutTeleporter;
                        regions.Add(new WorldHolder.Cords(x+i, y+j), region);
		            }

                    AddToSceneMap(subTileMap, x + i, y + j);
		        }
		    }

			return parentRegion;
		}

	    private Tile[,] GetSubTileMap(Tile[,] tileMap, int i, int j)
	    {
            Tile[,] newMap = new Tile[regionSizeX,regionSizeY];

            for (int x = 0; x < newMap.GetLength(0); x++)
	        {
                for (int y = 0; y < newMap.GetLength(1); y++)
                {
                    Tile t = null;

                    try
                    {
                        t = tileMap[x + (i*regionSizeX), y + (j*regionSizeY)];
                    }
                    catch (Exception)
                    {
                    }

                    if (t == null)
                    {
                        t = new Tile(x + (i*regionSizeX), y + (j*regionSizeY), WorldHolder.WALL);
                    }

                    newMap[x, y] = t;
	            }
	        }

	        return newMap;
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

			spawnableMonsters.Clear();
			spawnableNpcs.Clear();

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

			CreateDarkPlanes();

			try
			{
				foreach (MonsterSpawnInfo info in spawnableMonsters)
				{
				    AddMonsterToMap(info, true);
				}

				foreach (MonsterSpawnInfo info in spawnableNpcs)
				{
				    AddNpcToMap(info.MonsterId, info.SpawnPos, true);
				}
			}
			catch (Exception e)
			{
				Debug.LogError("error");
			}

			ConfigureMonstersAfterSpawn();

			spawnableMonsters.Clear();
			spawnableNpcs.Clear();

			GameSystem.Instance.UpdatePathfinding(); // TODO set correct bounds
			SetActive(true);

			UpdateRegions();
		}

		private void UpdateRegions()
		{
			foreach (MapRegion r in regions.Values)
			{
				if(!r.HasParentRegion())
					UpdateRegionStatus(r);
			}
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

			DeleteDarkPlanes();

			spawnableMonsters.Clear();
			spawnableNpcs.Clear();

			foreach (Monster m in activeMonsters)
			{
				if (m != null && m.Data != null)
				{
					spawnableMonsters.Add(m.SpawnInfo);
					m.Data.DeleteMe();
				}
			}

			foreach (Npc m in activeNpcs)
			{
				if (m != null && m.Data != null)
				{
					spawnableNpcs.Add(new MonsterSpawnInfo(this, m.Template.GetMonsterId(), m.GetData().GetBody().transform.position));
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
					Vector3 pos = new Vector3(((-xSize / 2) + x + .5f) + regionWidth+2, ((-ySize / 2) + y + .5f) + regionHeight+2, 0);
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
				    try
				    {
                        intMap[x, y] = SceneMap[x, y].tileType;
				    }
				    catch (Exception)
				    {
				        Debug.Log("NPE for " + x + ", " + y);
				    }
				}
			}

			Utils.Timer.StartTimer("meshgenerate");
			MeshGenerator generator = new MeshGenerator(World.gameObject);

			float xSize = (intMap.GetLength(0) - 1) * World.SQUARE_SIZE;
			float ySize = (intMap.GetLength(1) - 1) * World.SQUARE_SIZE;

			Vector3 shiftVector = new Vector3((xSize-1) / 2 - (regionWidth / 2), (ySize-1) / 2 - (regionHeight / 2));
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
			generatedRegionsMap[regionX, regionY] = 1;

			for (int x = 0; x < tiles.GetLength(0); x++)
			{
				for (int y = 0; y < tiles.GetLength(1); y++)
				{
					int newX = (regionX*regionSizeX) + x;
					int newY = (regionY*regionSizeY) + y;

					Tile t = tiles[x, y];

					SceneMap[newX, newY] = t;
					t.tileX = newX;
					t.tileY = newY;
				}
			}
		}

		public MapRegion GetRegion(Vector2 pos)
		{
			float xSize = (regionWidth + 1) * World.SQUARE_SIZE;
			float ySize = (regionHeight + 1) * World.SQUARE_SIZE;

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

		public List<MapRegion> GetNeighbourRegions(MapRegion reg)
		{
            List<MapRegion> neighbours = new List<MapRegion>();

			WorldHolder.Cords c;
			int i = 0;

			c = new WorldHolder.Cords(reg.x + 1, reg.y);
		    if (regions.ContainsKey(c))
		        neighbours.Add(regions[c]);

			c = new WorldHolder.Cords(reg.x, reg.y +1);
			if (regions.ContainsKey(c))
                neighbours.Add(regions[c]);

			c = new WorldHolder.Cords(reg.x - 1, reg.y);
			if (regions.ContainsKey(c))
                neighbours.Add(regions[c]);

			c = new WorldHolder.Cords(reg.x, reg.y -1);
			if (regions.ContainsKey(c))
                neighbours.Add(regions[c]);

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

		public Monster FindMonsterBySpawnInfo(MonsterSpawnInfo info)
		{
			foreach (Monster m in activeMonsters)
			{
				if (m != null && m.SpawnInfo.Equals(info))
					return m;
			}
			return null;
		}

		public void ConfigureMonstersAfterSpawn()
		{
			MonsterSpawnInfo spawnInfo;
			foreach (Monster m in activeMonsters)
			{
				if (m == null)
					continue;

				spawnInfo = m.SpawnInfo;

				if (spawnInfo.master != null)
				{
					Monster master = FindMonsterBySpawnInfo(spawnInfo.master);
					m.SetMaster(master);
				}
			}
		}

		public Npc AddNpcToMap(MonsterId monsterId, Vector3 position, bool forceSpawnNow=false)
		{
			if (isActive || forceSpawnNow)
			{
                Npc npc = GameSystem.Instance.SpawnNpc(monsterId, position);

                RegisterNpcToMap(npc);

			    return npc;
			}

			// ulozit NPC do seznamu ke spawnuti (spawne se v momente kdy se tato mapa stane aktivni)
			spawnableNpcs.Add(new MonsterSpawnInfo(this, monsterId, position));
			return null;
		}

	    public Monster AddMonsterToMap(MonsterSpawnInfo info, bool forceSpawnNow=false)
	    {
	        if (isActive || forceSpawnNow)
	        {
                Monster m = GameSystem.Instance.SpawnMonster(info.MonsterId, info.SpawnPos, false, info.level);
                m.SetSpawnInfo(info);

                RegisterMonsterToMap(m, info);
	            return m;
	        }

			// ulozit monster do seznamu ke spawnuti (spawne se v momente kdy se tato mapa stane aktivni)
		    spawnableMonsters.Add(info);
		    return null;
	    }

	    public void RegisterMonsterToMap(Monster m, MonsterSpawnInfo info=null)
	    {
            //auto create monsterspawninfo
            if (info == null)
            {
                Vector3 pos = m.GetData().GetBody().transform.position;
                info = new MonsterSpawnInfo(this, m.Template.GetMonsterId(), pos);

                MapRegion reg = GetRegionFromWorldPosition(pos);

                if (reg == null)
                {
                    Debug.LogError("Cant assign region for monster " + m.Name + " on pos " + pos + ", monster now registered!");
                    return;
                }

	            if (reg.HasParentRegion())
		            reg = reg.parentRegion;
			
                info.SetRegion(reg);
	            m.SetSpawnInfo(info);
            }

			//Debug.Log("monster " + m.Name + " has now region " + m.SpawnInfo.Region.x + ", " + m.SpawnInfo.Region.y + " assigned!");

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
			else if (ch is Boss)
			{
				NotifyBossDied((Boss)ch);
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

		private void NotifyBossDied(Boss boss)
		{
			
		}

	    public void UpdateRegionStatus(MapRegion region)
	    {
			// only parent regions can use this method to open doors, etc
		    if (region.HasParentRegion())
			    return;

	        int countInRegion = 0;

	        foreach (Monster m in activeMonsters)
	        {
	            if (m.SpawnInfo.Region.Equals(region) && m.SpawnInfo.mustDieToProceed)
	            {
	                countInRegion++;
	            }
	        }

            //Debug.Log("there are " + countInRegion + " monsters in " + region.x + ", " + region.y);

	        if (countInRegion > 0)
	            return;

		    if (region.x == 1 && region.y == 1)
		    {
			    
		    }

	        region.Status = MapRegion.STATUS_CONQUERED;

			Queue<MapRegion> neighbours = new Queue<MapRegion>();
			foreach (MapRegion reg in GetNeighbourRegions(region))
				neighbours.Enqueue(reg);

			List<MapRegion> checks = new List<MapRegion>();

			// vzit vsechny sousedy startovni mistnosti
			while (neighbours.Count > 0)
			{
				MapRegion neighbour = neighbours.Dequeue();

				checks.Add(neighbour);

				if (neighbour == null)
					continue;

				if (neighbour.IsInFamily(region)) // soused patri do rodiny - toho nemusime odemykat
				{
					foreach (MapRegion r in GetNeighbourRegions(neighbour))
					{
						if (r != null && !neighbours.Contains(r) && !checks.Contains(r))
							neighbours.Enqueue(r);
					}

					continue;
				}
				else if (neighbour.isLockedRegion && neighbour.empty == false) // soused je zamceny a nepatri do rodiny - odemkneme ho
				{
					MapPassage pas = GetPassage(region, neighbour);

					if (pas != null)
					{
						//Debug.Log("opened! " + region.x + ", " + region.y + " AND " + neighbour.x + ", " + neighbour.y);
						OpenPassage(pas);
					}
					else
					{
						//Debug.LogWarning("cant find passage for " + region.x + ", " + region.y + " AND " + neighbour.x + ", " + neighbour.y); //TODO finish
					}
				}
			}
	    }

		public int GetMonstersLeft(MapRegion reg)
		{
			int c = 0;
			if (reg == null)
			{
				return activeMonsters.Count;
			}
			else
			{
				foreach (Monster m in activeMonsters)
				{
					if (m.SpawnInfo.Region.GetParentOrSelf().Equals(reg))
					{
						c ++;
					}
				}
			}

			return c;
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

		public List<MapRoom> GetMapRooms()
		{
			return mapProcessor.rooms;
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

		public List<MapRegion> GetRegionsInFamilyWith(MapRegion reg)
		{
			List<MapRegion> list = new List<MapRegion>();
			list.Add(reg);

			foreach (MapRegion r in regions.Values)
			{
				if (r.IsInFamily(reg))
				{
					list.Add(r);
				}
			}

			return list;
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
