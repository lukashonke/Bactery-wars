﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Assets.scripts.Mono.MapGenerator
{
	public enum MapType
	{
		DungeonAllOpen,
		DungeonCentralClosed
	}

	public class MapRegion
	{
		private bool active;

		public int x, y;
		public Tile[,] tileMap;
		public RegionGenerator regionGen; //TODO remove, not neccessary

		public MeshFilter mesh;
		public MeshGenerator meshGen;

		public bool isAccessibleFromStart;
		public bool isStartRegion; // marks that the player starts in this region upon spawning into the map

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

		public void SetMesh(MeshFilter mesh, MeshGenerator meshGen)
		{
			this.mesh = mesh;
			this.meshGen = meshGen;
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
		private WorldHolder world;

		public bool isActive;

		public string name;
		private WorldHolder.Cords position;
		private MapType mapType;

		public Tile[,] SceneMap { get; set; }
		public Dictionary<WorldHolder.Cords, MapRegion> regions;

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

		public void CreateMap()
		{
			regions = new Dictionary<WorldHolder.Cords, MapRegion>();

			regionSize = world.width + 2;
			SceneMap = new Tile[regionSize * MAX_REGIONS, regionSize * MAX_REGIONS];

			switch (mapType)
			{
					case MapType.DungeonAllOpen:

						GenerateDungeonRoom(world.seed, 0, 0, world.randomFillPercent, true, true);
						GenerateDungeonRoom(world.seed, 0, 1, world.randomFillPercent, true, false);
						GenerateDungeonRoom(world.seed, 0, 2, world.randomFillPercent, true, false);
						GenerateDungeonRoom(world.seed, 1, 0, world.randomFillPercent, true, false);
						GenerateDungeonRoom(world.seed, 1, 1, world.randomFillPercent, true, false);
						GenerateDungeonRoom(world.seed, 1, 2, world.randomFillPercent, true, false);
						GenerateDungeonRoom(world.seed, 2, 0, world.randomFillPercent, true, false);
						GenerateDungeonRoom(world.seed, 2, 1, world.randomFillPercent, true, false);
						GenerateDungeonRoom(world.seed, 2, 2, world.randomFillPercent, true, false);

					break;
					case MapType.DungeonCentralClosed:

						GenerateDungeonRoom(world.seed, 0, 0, world.randomFillPercent, true, true);
						GenerateDungeonRoom(world.seed, 0, 1, world.randomFillPercent, true, false);
						GenerateDungeonRoom(world.seed, 0, 2, world.randomFillPercent, true, false);
						GenerateDungeonRoom(world.seed, 1, 0, world.randomFillPercent, true, false);
						GenerateEmptyRegion(1, 1);
						GenerateDungeonRoom(world.seed, 1, 2, world.randomFillPercent, true, false);
						GenerateDungeonRoom(world.seed, 2, 0, world.randomFillPercent, true, false);
						GenerateDungeonRoom(world.seed, 2, 1, world.randomFillPercent, true, false);
						GenerateDungeonRoom(world.seed, 2, 2, world.randomFillPercent, true, false);

					break;

			}

			ProcessSceneMap();
		}

		public void ProcessSceneMap()
		{
			MapProcessor processor = new MapProcessor(SceneMap, mapType);

			processor.CreatePassages();

			SceneMap = processor.Tiles;
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

		protected void GenerateDungeonRoom(string seed, int x, int y, int randomFillPercent, bool isAccessibleFromStart, bool isStartRegion)
		{
			//Debug.Log("generating and enabling NEW region .. " + x + ", " + y);

			if (world.useRandomSeed)
			{
				seed = Random.Range(-1000, 1000).ToString();
			}

			//float xSize = (world.width) * world.SQUARE_SIZE;
			//float ySize = (world.height) * world.SQUARE_SIZE;
			//Vector3 shiftVector = new Vector3(x * xSize, y * ySize);

			//TODO make different generators, all applicable on to this method!
			RegionGenerator regionGenerator = new DungeonRegionGenerator(world.width, world.height, seed, randomFillPercent, world.doDebug);

			Tile[,] tileMap = regionGenerator.GenerateMap();

			AddToSceneMap(tileMap, x, y);

			if (!GameController.DEV_BUILD)
				regionGenerator = null;

			MapRegion region = new MapRegion(x, y, tileMap, regionGenerator);
			region.AssignTilesToThisRegion();
			region.SetAccessibleFromStart(isAccessibleFromStart);
			region.isStartRegion = isStartRegion;
			regions.Add(new WorldHolder.Cords(x, y), region);
		}

		public void DeleteMap()
		{
			foreach (MapRegion region in regions.Values)
			{
				region.meshGen.Delete();
				Object.Destroy(region.mesh);
			}

			regions.Clear();
		}

		public void LoadMap()
		{
			GenerateMapMesh();

			GameSystem.Instance.UpdatePathfinding();
			SetActive(true);
		}

		public void DeloadMap()
		{
			// destroy the meshes
			foreach (MapRegion region in regions.Values)
			{
				region.meshGen.Delete();
				Object.Destroy(region.mesh);
			}

			SetActive(false);
		}

		public void SetActive(bool active)
		{
			isActive = active;
		}

		public void DrawGizmos()
		{
			float xSize = SceneMap.GetLength(0);
			float ySize = SceneMap.GetLength(1);

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
					else if (t.GetColor() == 5)
						Gizmos.color = Color.blue;
					else if (t.GetColor() == 6)
						Gizmos.color = Color.red;

					Vector3 pos = new Vector3(((-xSize / 2) + x + .5f) + world.width+2, ((-ySize / 2) + y + .5f) + world.height+2, 0);
					Gizmos.DrawCube(pos, Vector3.one);
				}
			}

			Debug.Log("highlighted " + i );
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
			Dictionary<WorldHolder.Cords, int[,]> map = new Dictionary<WorldHolder.Cords, int[,]>();

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
			}

			// iterate through int maps and generate meshes
			for (int i = 0; i < map.Count; i++)
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
			}

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
	}
}
