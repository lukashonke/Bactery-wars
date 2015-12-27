using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.scripts.Mono.MapGenerator
{
	/// <summary>
	/// Dostava jako parametr velkou mapu jako 2D Tile matici a zpracuje ji z globalniho hlediska (propoji regiony, vytvori dvere atd.)
	/// </summary>
	public class MapProcessor
	{
		private Tile[,] tiles;
		private MapType mapType;

		public Tile[,] Tiles { get { return tiles; } }
		public List<MapRoom> rooms;

		private int width;
		private int height;

		public MapProcessor(Tile[,] tiles, MapType type)
		{
			this.tiles = tiles;
			this.mapType = type;

			width = tiles.GetLength(0);
			height = tiles.GetLength(1);
		}

		public void CreatePassages()
		{
			UncheckAllTiles();
			AnalyzeRooms();

			ConnectRooms();
		}

		private void UncheckAllTiles()
		{
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					Tile t = GetTile(x, y);
					t.Uncheck();
					//t.SetColor(0);
				}
			}
		}

		private void ConnectRooms()
		{
			foreach (MapRoom room in rooms)
			{
				if (room.region.isStartRegion)
					continue;

				if (room.region.isAccessibleFromStart && room.isAccessibleFromStartRoom)
				{

				}
			}
		}

		private void ConnectRooms(MapRoom roomA, MapRoom roomB)
		{
			
		}

		private void AnalyzeRooms()
		{
			List<Region> wallRegions = GetRegions(WorldHolder.WALL);
			List<Region> groundRegions = GetRegions(WorldHolder.GROUND);

			rooms = new List<MapRoom>();

			foreach (Region r in groundRegions)
			{
				MapRoom room = new MapRoom(r.tiles, tiles);
				rooms.Add(room);
			}

			UncheckAllTiles();

			Debug.Log("count of rooms: " + rooms.Count);
		}

		private List<Region> GetRegions(int tileType)
		{
			List<Region> reg = new List<Region>();

			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					Tile t = GetTile(x, y);

					if (!t.isChecked && t.tileType == tileType)
					{
						Region r = CreateRegion(x, y, tileType);

						reg.Add(r);

						foreach (Tile tt in r.tiles)
						{
							tt.Check();
						}
					}
				}
			}

			return reg;
		}

		private Region CreateRegion(int startX, int startY, int tileType)
		{
			Region reg = new Region(tileType);

			List<Tile> tilesList = new List<Tile>();
			reg.tiles = tilesList;

			Queue<Tile> queue = new Queue<Tile>();
			queue.Enqueue(GetTile(startX, startY));
			GetTile(startX, startY).Check();

			while (queue.Count > 0)
			{
				Tile t = queue.Dequeue();
				tilesList.Add(t);

				foreach (Tile n in GetNeighbours(t.tileX, t.tileY, false))
				{
					if (n == null)
						continue;

					if (!n.isChecked && n.tileType == tileType)
					{
						n.Check();
						queue.Enqueue(n);
					}
				}
			}

			return reg;
		}

		private Tile[] GetNeighbours(int x, int y, bool includeDiag)
		{
			Tile[] neighb;
			if (includeDiag)
				neighb = new Tile[8];
			else
				neighb = new Tile[4];

			neighb[0] = GetTile(x, y + 1) ;
			neighb[1] = GetTile(x, y - 1) ;
			neighb[2] = GetTile(x + 1, y) ;
			neighb[3] = GetTile(x - 1, y) ;

			/*neighb[0] = Exists(x, y + 1) ? GetTile(x, y + 1) : null;
			neighb[1] = Exists(x, y - 1) ? GetTile(x, y - 1) : null;
			neighb[2] = Exists(x + 1, y) ? GetTile(x + 1, y) : null;
			neighb[3] = Exists(x - 1, y) ? GetTile(x - 1, y) : null;*/

			if (includeDiag)
			{
				neighb[4] = Exists(x + 1, y + 1) ? GetTile(x + 1, y + 1) : null;
				neighb[5] = Exists(x - 1, y - 1) ? GetTile(x - 1, y - 1) : null;
				neighb[6] = Exists(x + 1, y - 1) ? GetTile(x + 1, y - 1) : null;
				neighb[7] = Exists(x - 1, y + 1) ? GetTile(x - 1, y + 1) : null;
			}

			return neighb;
		}

		private bool Exists(int x, int y)
		{
			return x >= 0 && x < width && y >= 0 && y < height;
		}

		private Tile GetTile(int x, int y)
		{
			try
			{
				return Tiles[x, y];
			}
			catch (Exception)
			{
				return null;
			}
		}

		private class Region
		{
			public List<Tile> tiles;
			public int type;

			public Region(int type)
			{
				this.type = type;
			}

			public void Add(Tile tile)
			{
				tiles.Add(tile);
			}

			public int SetAllTo(int type)
			{
				int i = 0;
				foreach (Tile t in tiles)
				{
					i++;
					t.tileType = type;
				}
				return i;
			}

			public void SetTilesChecked(bool b)
			{
				foreach (Tile t in tiles)
				{
					if (b) t.Check();
					else t.Uncheck();
				}
			}
		}
	}
}
