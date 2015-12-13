using System.Collections.Generic;
using Assets.scripts.MapGenerator;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.scripts.Mono
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

		// variables
		public Dictionary<Cords, MapRegion> regions;

		private List<AbstractMapGenerator> generators = new List<AbstractMapGenerator>(); 

		private const int SQUARE_SIZE = 1;

		void Start()
		{
			if (instance == null)
				instance = this;

			regions = new Dictionary<Cords, MapRegion>();

			// create the first map
			GenerateFirst();
		}

		private void GenerateFirst()
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

			MapRegion r1 = regions[new Cords(0, 1)];
			MapRegion r2 = regions[new Cords(0, 0)];
		}

		private AbstractMapGenerator GenerateRegion(int x, int y)
		{
		    /**/Debug.Log("generating and enabling region .. " + x + ", " + y);

			if (useRandomSeed)
				seed = Random.Range(-1000, 1000).ToString();

			float xSize = (width) * SQUARE_SIZE;
			float ySize = (height) * SQUARE_SIZE;
			Vector3 shiftVector = new Vector3(x*xSize, y*ySize);

			AbstractMapGenerator mapGenerator = new DungeonGenerator(width, height, seed, randomFillPercent, doDebug, x, y, shiftVector);

			mapGenerator.GenerateDungeon();
			int[,] map = mapGenerator.ToIntArray();

			MeshGenerator meshGenerator = mapGenerator.GenerateMesh(gameObject, SQUARE_SIZE);
			MeshFilter mesh = meshGenerator.mesh;

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

		void Update()
		{
			if (Input.GetKeyDown(KeyCode.Space))
			{
				foreach (MapRegion region in regions.Values)
				{
					region.mapGen.GetMeshGenerator().Delete();
				}

				regions.Clear();
				GenerateFirst();
			}
		}

		void OnDrawGizmos()
		{
			if (doDebug == false)
				return;

			foreach (AbstractMapGenerator g in generators)
				g.OnDrawGizmos();
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
			public AbstractMapGenerator mapGen;
			public MeshFilter mesh;

			public MapRegion(int x, int y, int[,] map, MeshFilter m, AbstractMapGenerator mapGen)
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
