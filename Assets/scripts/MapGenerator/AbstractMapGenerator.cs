using System.Collections.Generic;
using UnityEngine;

namespace Assets.scripts.MapGenerator
{
	public abstract class AbstractMapGenerator
	{
		// parameters
		protected int width;
		protected int height;
		protected string seed;
		protected bool doDebug;
		protected int shiftX;
		protected int shiftY;
		protected Vector3 shiftVector;

		// data
		protected Tile[,] tiles;

		public AbstractMapGenerator(int width, int height, string seed, int shiftX, int shiftY, Vector3 shiftVector, bool DoDebug)
		{
			this.width = width;
			this.height = height;
			this.seed = seed;
			this.doDebug = DoDebug;
			this.shiftVector = shiftVector;
			this.shiftX = shiftX;
			this.shiftY = shiftY;
		}

		public virtual void OnDrawGizmos()
		{
			
		}

		public int[,] ToIntArray()
		{
			int[,] map = new int[tiles.GetLength(0), tiles.GetLength(1)];
			for (int x = 0; x < tiles.GetLength(0); x++)
			{
				for (int y = 0; y < tiles.GetLength(1); y++)
				{
					map[x, y] = tiles[x, y].tileType;
				}
			}
			return map;
		}

		public abstract Tile[,] GenerateDungeon();
		public abstract MeshGenerator GenerateMesh(GameObject parent, float squareSize);
		public abstract MeshGenerator GetMeshGenerator();

		/// vrati 2d matici policek
		public abstract Tile[,] GetTiles(); 

		/// vrati vsechny mistnosti i kdyz mezi nimi existuje spojeni (tunel)
		public abstract List<Room> GetConnectedRooms();

		/// vrati vsechny mistnosti mistnosti nejsou zadnym tunelem spojene
		public abstract List<Room> GetSeparatedRooms();

		/// vrati hlavni mistnost
		public abstract Room GetMainRoom();

		public class Tile
		{
			public int tileX;
			public int tileY;
			public int tileType;
			public bool isChecked;
			public int color;

			public Tile(int x, int y, int t)
			{
				tileX = x;
				tileY = y;
				tileType = t;
				isChecked = false;
			}

			public Tile(int x, int y)
			{
				tileX = x;
				tileY = y;
				tileType = 0;
				isChecked = false;
			}

			public void Check()
			{
				isChecked = true;
			}

			public void Uncheck()
			{
				isChecked = false;
			}
		}
	}
}