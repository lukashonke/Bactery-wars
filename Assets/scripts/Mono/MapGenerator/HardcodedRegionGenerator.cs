// Copyright (c) 2015, Lukas Honke
// ========================
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
			int y = 0;
			string line;

			// Read the file and display it line by line.
			System.IO.StreamReader file = new System.IO.StreamReader(name + ".txt");
			while ((line = file.ReadLine()) != null)
			{
				for (int i = 0; i < width; i++)
				{
					int c = Int32.Parse(line[i].ToString());

					if (c == 1 || c == 2 )
					{
						tiles[y, i].tileType = WorldHolder.GROUND;
					}
					else if (c == 0 || c == 3)
					{
						tiles[y, i].tileType = WorldHolder.WALL;
					}
				}

				y++;

				if (y > height)
					break;
			}

			file.Close();
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
