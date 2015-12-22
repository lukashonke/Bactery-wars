using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.scripts.Mono.MapGenerator
{
	public class MapHolder : MonoBehaviour
	{
		public static MapHolder instance;

		// inspector configuration
		public int width;
		public int height;
		public string seed;
		public bool useRandomSeed;
		[Range(0, 100)]
		public int randomFillPercent;

		public bool doDebug = false;
		private const int SQUARE_SIZE = 1;
		private const int MAX_REGIONS = 6;

		// variables
		public Dictionary<Cords, MapRegion> regions;

		private int regionSize;

		private List<MapGenerator> generators = new List<MapGenerator>();

		public Tile[,] SceneMap { get; set; }

		// for test TODO remove this
		const bool renderAllMap = false;

		/// <summary>
		/// TODO dokoncit - prevest vygenerovane regiony do jedne velke mapy a pak postupne zapinat nebo vypinat meshe podle toho ktere jsou regiony jsou aktivni a ktere ne 
		/// </summary>
		public void CreateSceneMesh()
		{
			// test
			/*SceneMap = new Tile[regionSize * 24, regionSize * 24];
			for (int i = 0; i < SceneMap.GetLength(0); i++)
			{
				for (int j = 0; j < SceneMap.GetLength(1); j++)
				{
					SceneMap[i, j] = new Tile(1,1,1);
				}
			}*/

			Dictionary<Cords, int[,]> map = new Dictionary<Cords, int[,]>();

			for (int x = 0; x < SceneMap.GetLength(0); x++)
			{
				for (int y = 0; y < SceneMap.GetLength(1); y++)
				{
					Cords c = new Cords(x/ regionSize, y/ regionSize);

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
							map[c][x%regionSize, y%regionSize] = SceneMap[x, y].tileType;
						else
							map[c][x%regionSize, y%regionSize] = 0;
					}
				}
			}

			if (renderAllMap)
			{
				for (int i = 0; i < map.Count; i++)
				{
					KeyValuePair<Cords, int[,]> e = map.ElementAt(i);

					Cords c = e.Key;
					int[,] regionMap = e.Value;

					MeshGenerator generator = new MeshGenerator(gameObject);

					float xSize = (regionMap.GetLength(0) - 1) * SQUARE_SIZE;
					float ySize = (regionMap.GetLength(1) - 1) * SQUARE_SIZE;

					Vector3 v = new Vector3(c.x * xSize, c.y * ySize);

					MeshFilter mesh = generator.GenerateMesh("Cave " + c.ToString(), regionMap, SQUARE_SIZE, v);
					mesh.gameObject.transform.position = v;
				}
			}
			

			Debug.Log("pocet regionu na renderovani: " + map.Count);

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

		public void AddToSceneMap(Tile[,] tiles, int regionX, int regionY)
		{
			regionX += MAX_REGIONS/2;
			regionY += MAX_REGIONS/2;

			for (int x = 0; x < tiles.GetLength(0); x++)
			{
				for (int y = 0; y < tiles.GetLength(1); y++)
				{
					SceneMap[(regionX * regionSize) + x, regionY * regionSize + y] = tiles[x, y];
				}
			}
		}

		void Start()
		{
			if (instance == null)
				instance = this;

			regions = new Dictionary<Cords, MapRegion>();

			regionSize = width + 2;
			SceneMap = new Tile[regionSize * MAX_REGIONS, regionSize * MAX_REGIONS];

			// create the first map
			TestGen();
		}

		private void TestGen()
		{
			generators.Add(GenerateRegion(0, 0));
			generators.Add(GenerateRegion(0, 1));
			generators.Add(GenerateRegion(0, -1));
			generators.Add(GenerateRegion(1, 1));
			generators.Add(GenerateRegion(1, 0));
			generators.Add(GenerateRegion(1, -1));
			generators.Add(GenerateRegion(-1, 1));
			generators.Add(GenerateRegion(-1, 0));
			generators.Add(GenerateRegion(-1, -1));

			CreateSceneMesh();

			MapRegion r1 = regions[new Cords(0, 1)];
			MapRegion r2 = regions[new Cords(0, 0)];

			GameSystem.Instance.UpdatePathfinding();

			/*int[,] sharedMap = new int[r1.map.GetLength(0), r1.map.GetLength(1) + r2.map.GetLength(1)];

			for (int i = 0; i < r1.map.GetLength(0); i++)
			{
				for (int j = 0; j < r1.map.GetLength(1); j++)
				{
					sharedMap[i,j] = r1.map[i, j];
				}
			}

			for (int i = 0; i < r2.map.GetLength(0); i++)
			{
				for (int j = 0; j < r2.map.GetLength(1); j++)
				{
					sharedMap[i, j + r1.map.GetLength(1)] = r2.map[i, j];
				}
			}

			Debug.Log(sharedMap.GetLength(0) + ", " + sharedMap.GetLength(1));

			generator.ConnectDungeons(sharedMap);*/
		}


		void Update()
		{
			if (Input.GetKeyDown(KeyCode.Space))
			{
				foreach (MapRegion region in regions.Values)
				{
					region.mapGen.GetMeshGenerator().Delete();
				}

				regions.Clear();
				TestGen();
			}
		}

		void OnDrawGizmos()
		{
			if (doDebug == false)
				return;

			foreach(MapGenerator g in generators)
				g.OnDrawGizmos();
		}

		private MapGenerator GenerateRegion(int x, int y)
		{
			Debug.Log("generating and enabling region .. " + x + ", " + y);

			if (useRandomSeed)
			{
				seed = Random.Range(-1000, 1000).ToString();
			}

			float xSize = (width) * SQUARE_SIZE;
			float ySize = (height) * SQUARE_SIZE;
			Vector3 shiftVector = new Vector3(x*xSize, y*ySize);

			MapGenerator mapGenerator = new DungeonGenerator(width, height, seed, randomFillPercent, doDebug, x, y, shiftVector);

			Tile[,] tileMap = mapGenerator.GenerateMap();
			int[,] map = mapGenerator.GetIntMap();

			AddToSceneMap(tileMap, x, y);

			MeshFilter mesh = null;
            if (!renderAllMap)
			{
				MeshGenerator meshGenerator = mapGenerator.GenerateMesh(gameObject, map, SQUARE_SIZE);
				mesh = meshGenerator.mesh;
			}
			

			MapRegion region = new MapRegion((int)x, (int)y, map, mesh, mapGenerator);
			region.Enable();
			regions.Add(new Cords((int)x, (int)y), region);

			return mapGenerator;
		}

		public void PositionEnter(Vector3 pos)
		{
			/*foreach (MapRegion r in regions.Values)
			{
				r.Disable();
			}*/

			MapRegion reg = GetRegion(pos);
			if (reg != null)
			{
				reg.Enable(); //TODO add setting inactive
			}
			else
			{
				//Debug.LogError("Region for " + pos + " doesnt exist!");
			}
		}

		public MapRegion GetRegion(Vector2 pos)
		{
			float xSize = (width + 1) * SQUARE_SIZE;
			float ySize = (height + 1) * SQUARE_SIZE;

			float regionX = (pos.x + xSize / 2) / xSize;
			float regionY = (pos.y + ySize / 2) / ySize;

			MapRegion reg;
			Cords c = new Cords((int)regionX, (int)regionY);
			regions.TryGetValue(c, out reg);

			return reg;
		}

		public class MapRegion
		{
			private bool active;

			public int x, y;
			public int[,] map;
			public MapGenerator mapGen;
			public MeshFilter mesh;

			public MapRegion(int x, int y, int[,] map, MeshFilter m, MapGenerator mapGen)
			{
				this.map = map;
				this.x = x;
				this.y = y;
				this.mesh = m;
				this.mapGen = mapGen;

				active = true;
			}

			public void Disable()
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
			}
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
