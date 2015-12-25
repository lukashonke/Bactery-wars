using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.scripts.Mono.MapGenerator
{
	public enum MapType
	{
		Dungeon
	}

	public class MapRegion
	{
		private bool active;

		public int x, y;
		public int[,] map;
		public Tile[,] tileMap;
		public MapGenerator mapGen;

		public MeshFilter mesh;
		public MeshGenerator meshGen;

		public MapRegion(int x, int y, int[,] map, Tile[,] tileMap, MapGenerator mapGen)
		{
			this.map = map;
			this.x = x;
			this.y = y;
			this.mapGen = mapGen;
			this.tileMap = tileMap;

			active = true;
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

			GenerateRegion(world.seed, 0, 0);
			GenerateRegion(world.seed, 0, 1);
			GenerateRegion(world.seed, 0, 2);
			GenerateRegion(world.seed, 1, 0);
			GenerateRegion(world.seed, 1, 1);
			GenerateRegion(world.seed, 1, 2);
			GenerateRegion(world.seed, 2, 0);
			GenerateRegion(world.seed, 2, 1);
			GenerateRegion(world.seed, 2, 2);

			ProcessScene();
		}

		public void ProcessScene()
		{
			// TODO generate passages, etc
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
			foreach (MapRegion region in regions.Values)
				region.mapGen.OnDrawGizmos();
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
					SceneMap[(regionX * regionSize) + x, regionY * regionSize + y] = tiles[x, y];
				}
			}
		}

		protected void GenerateRegion(string seed, int x, int y)
		{
			//Debug.Log("generating and enabling NEW region .. " + x + ", " + y);

			if (world.useRandomSeed)
			{
				seed = Random.Range(-1000, 1000).ToString();
			}

			float xSize = (world.width) * world.SQUARE_SIZE;
			float ySize = (world.height) * world.SQUARE_SIZE;
			Vector3 shiftVector = new Vector3(x * xSize, y * ySize);

			MapGenerator mapGenerator = new DungeonGenerator(world.width, world.height, seed, world.randomFillPercent, world.doDebug, x, y, shiftVector);

			Tile[,] tileMap = mapGenerator.GenerateMap();
			int[,] map = mapGenerator.GetIntMap();

			AddToSceneMap(tileMap, x, y);

			MapRegion region = new MapRegion(x, y, map, tileMap, mapGenerator);
			regions.Add(new WorldHolder.Cords(x, y), region);
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
