using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.scripts.Mono.MapGenerator
{
	public abstract class MapGenerator
	{
		protected int width;
		protected int height;
		protected string seed;
		protected bool doDebug;
		protected int shiftX;
		protected int shiftY;
		protected Vector3 shiftVector;
		protected float meshSquareSize;

		public MapGenerator(int width, int height, string seed, int shiftX, int shiftY, Vector3 shiftVector, bool DoDebug)
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

		public abstract Tile[,] GenerateMap();
		public abstract MeshGenerator GenerateMesh(GameObject parent, int[,] map, float squareSize);
		public abstract MeshGenerator GetMeshGenerator();

		public abstract Tile[,] GetTiles();
		public abstract List<Room> GetConnectedRooms();
		public abstract List<Room> GetSeparatedRooms();
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

		public int[,] GetIntMap()
		{
			int[,] tiles = new int[GetTiles().GetLength(0), GetTiles().GetLength(1)];
			for (int x = 0; x < GetTiles().GetLength(0); x++)
			{
				for (int y = 0; y < GetTiles().GetLength(1); y++)
				{
					tiles[x, y] = GetTiles()[x, y].tileType;
				}
			}

			return tiles;
		}
	}
}