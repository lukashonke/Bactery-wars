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

		public const bool DoDebug = true;

		// variables
		public Dictionary<Cords, MapRegion> regions;

		private MapGenerator generator;

		private const int SQUARE_SIZE = 1;

		void Start()
		{
			if (instance == null)
				instance = this;

			regions = new Dictionary<Cords, MapRegion>();
			generator = GetComponent<DungeonGenerator>();

			// create the first map
			TestGen();
		}

		private void TestGen()
		{
			GenerateRegion(0, 0);
			/*GenerateRegion(0, 1);
			GenerateRegion(0, -1);
			GenerateRegion(1, 1);
			GenerateRegion(1, 0);
			GenerateRegion(1, -1);
			GenerateRegion(-1, 1);
			GenerateRegion(-1, 0);
			GenerateRegion(-1, -1);*/

			MapRegion r1 = regions[new Cords(0, 1)];
			MapRegion r2 = regions[new Cords(0, 0)];

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
				regions.Clear();
				TestGen();
			}
		}

		void OnDrawGizmos()
		{
			if (DoDebug == false || generator is DungeonGenerator)
				return;

			foreach (MapRegion reg in regions.Values)
			{
				for (int x = 0; x < width; x++)
				{
					for (int y = 0; y < height; y++)
					{
						float xSize = (width) * SQUARE_SIZE;
						float ySize = (height) * SQUARE_SIZE;

						Gizmos.color = (reg.map[x, y] == 1) ? Color.black : Color.white;
						Vector3 pos = new Vector3((-width / 2 + x + .5f)+reg.x*xSize, (-height / 2 + y + .5f)+reg.y*ySize, 0);
						Gizmos.DrawCube(pos, Vector3.one);
					}
				}
			}
		}

		private void GenerateRegion(int x, int y)
		{
			Debug.Log("generating and enabling region .. " + x + ", " + y);

			if (useRandomSeed)
			{
				seed = Random.Range(-1000, 1000).ToString();
			}

			float xSize = (width) * SQUARE_SIZE;
			float ySize = (height) * SQUARE_SIZE;
			Vector3 shiftVector = new Vector3(x*xSize, y*ySize);

			int[,] map = generator.GenerateMap(width, height, seed, randomFillPercent, DoDebug, x, y, shiftVector);
			MeshFilter mesh = null;

			if (!DoDebug)
			{
				MeshGenerator meshGen = GetComponent<MeshGenerator>();
				mesh = meshGen.GenerateMesh("Cave", map, SQUARE_SIZE);

				// dimensions of the map
				xSize = (map.GetLength(0) - 1) * SQUARE_SIZE;
				ySize = (map.GetLength(1) - 1) * SQUARE_SIZE;

				mesh.gameObject.transform.position = new Vector3(x * xSize, y * ySize);
			}

			MapRegion region = new MapRegion((int)x, (int)y, map, mesh);
			region.Enable();
			regions.Add(new Cords((int)x, (int)y), region);
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
			public MeshFilter mesh;

			public MapRegion(int x, int y, int[,] map, MeshFilter m)
			{
				this.map = map;
				this.x = x;
				this.y = y;
				this.mesh = m;

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
			private int x, y;
			public Cords(int x, int y)
			{
				this.x = x;
				this.y = y;
			}

			public bool Equals(Cords f, Cords s)
			{
				return f.x == s.x && f.y == s.y;
			}
		}
	}
}
