using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.scripts.Mono.MapGenerator
{
	public class HardcodedRegionGenerator : RegionGenerator
	{
		public string name;
		public Tile[,] tiles;

		public HardcodedRegionGenerator(int width, int height, string seed, bool DoDebug, string name) : base(width, height, seed, DoDebug)
		{
			this.name = name;
		}

		public override Tile[,] GenerateMap()
		{
			// make the tile field
			tiles = new Tile[width, height];

			// initial init
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
					tiles[x, y] = new Tile(x, y);
			}

			ApplyTemplate();

			return tiles;
		}

		private void ApplyTemplate()
		{
			
		}

		public override Tile[,] GetTiles()
		{
			return tiles;
		}

		public override List<Room> GetConnectedRooms()
		{
			return new List<Room>();
		}

		public override List<Room> GetSeparatedRooms()
		{
			return new List<Room>();
		}

		public override Room GetMainRoom()
		{
			return null;
		}
	}
}
