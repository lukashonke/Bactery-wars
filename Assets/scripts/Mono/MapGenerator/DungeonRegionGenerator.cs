// Copyright (c) 2015, Lukas Honke
// ========================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Mono.MapGenerator;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Assets.scripts.Mono.MapGenerator
{
	/// <summary>
	/// Generuje jeden region typu Dungeon
	/// </summary>
	public class DungeonRegionGenerator : RegionGenerator
	{
		public enum Direction
		{
			TOP, RIGHT, DOWN, LEFT
		}

		public const int FLOOR = WorldHolder.GROUND;
		public const int WALL = WorldHolder.WALL;

		private const int smoothCount = 4;
		private int randomFillPercent;
		private Tile[,] tiles;

		private List<Room> connectedRooms;
		private Room mainRoom;

		public DungeonRegionGenerator(int width, int height, String seed, int wallFillPercent, bool debug) : base(width, height, seed, debug)
		{
			this.randomFillPercent = wallFillPercent;
		}

		public override Tile[,] GenerateMap()
		{
			// make the tile field
			tiles = new Tile[width, height];

			// initial init
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
					tiles[x,y] = new Tile(x,y);
			}

			// fill the map with random tiles
			RandomFillMap();

			for (int i = 0; i < smoothCount; i++)
			{
				SmoothMap();
			}

			// process the map, remove small regions
			ProcessMap();

			const int borderSize = 1;
			Tile[,] borderedMap = new Tile[width + borderSize * 2, height + borderSize * 2];

			for (int x = 0; x < borderedMap.GetLength(0); x++)
			{
				for (int y = 0; y < borderedMap.GetLength(1); y++)
				{
					if (x >= borderSize && x < width + borderSize && y >= borderSize && y < height + borderSize)
					{
						borderedMap[x, y] = tiles[x - borderSize, y - borderSize];
					}
					else
					{
						borderedMap[x, y] = new Tile(x, y, WALL);
					}
				}
			}

			if (doDebug)
			{
				Camera.main.orthographicSize = 60;
			}

			tiles = borderedMap;

			return tiles;
		}

		private void SmoothMap()
		{
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					int neighbourWalls = GetTileCount(GetNeighbourTypes(x, y, true), WALL, true);

					if (neighbourWalls > 4)
					{
						SetTile(x, y, WALL);
					}
					else if(neighbourWalls < 4)
					{
						SetTile(x, y, FLOOR);
					}
				}
			}
		}

		private void ProcessMap()
		{
			List<Region> wallRegions = GetRegions(WALL);
			int wallThresholdSize = 50;

			int i = 0;
			foreach (Region r in wallRegions)
			{
				if (r.tiles.Count < wallThresholdSize)
				{
					i += r.SetAllTo(FLOOR);
				}

				r.SetTilesChecked(false);
			}

			//Debug.Log("setting " + i + " tiles to FLOOR");

			List<Region> floorRegions = GetRegions(FLOOR);
			int roomThresholdSize = 50;

			List<Room> rooms = new List<Room>();

			i = 0;
			foreach (Region r in floorRegions)
			{
				if (r.tiles.Count < roomThresholdSize)
				{
					i += r.SetAllTo(WALL);
				}

				r.SetTilesChecked(false);
			}

			floorRegions = GetRegions(FLOOR);
			foreach (Region r in floorRegions)
			{
				rooms.Add(new Room(r.tiles, tiles));

				r.SetTilesChecked(false);
			}

			//Debug.Log("setting " + i + " tiles to WALL");

			connectedRooms = rooms; // uloz mistnosti tak jak jsou predtim nez se vsechny propoji

			ConnectAllRooms(rooms, false);

			floorRegions = GetRegions(FLOOR);

			foreach (Region r in floorRegions) // ulozit hlavni mistnost pro pozdeji referenci
			{
				mainRoom = new Room(r.tiles, tiles); 
				//mainRoom.ColorTiles(5, true);
			}
		}

		private void GetClosestTiles(Room roomA, Room roomB, out Tile bestTileA, out Tile bestTileB, out int bestDistance)
		{
			bestDistance = 0;
			bestTileA = null;
			bestTileB = null;

			// projedeme vsechny okrajove body roomA
			for (int tileIndexA = 0; tileIndexA < roomA.edgeTiles.Count; tileIndexA++)
			{
				// projedeme vsechny okrajovy body roomB
				for (int tileIndexB = 0; tileIndexB < roomB.edgeTiles.Count; tileIndexB++)
				{
					Tile tileA = roomA.edgeTiles[tileIndexA];
					Tile tileB = roomB.edgeTiles[tileIndexB];

					// vypocitame vzdalenost kazdych dvou okrajovych bodu obou mistnosti
					int dx = tileA.tileX - tileB.tileX;
					int dy = tileA.tileY - tileB.tileY;
					int dist = (int)(Math.Pow(dx, 2) + Math.Pow(dy, 2));

					// tato vzdalenost je bud nejlepsi nebo possibleConnectionFound je false pri prvni iteraci
					if (dist < bestDistance)
					{
						bestDistance = dist;

						bestTileA = tileA;
						bestTileB = tileB;
					}
				}
			}
		}

		private void ConnectRooms(Room roomA, Room roomB)
		{
			int bestDistance;
			Tile bestTileA;
			Tile bestTileB;

			GetClosestTiles(roomA, roomB, out bestTileA, out bestTileB, out bestDistance);

			if(bestTileA != null && bestTileB != null)
				CreatePassage(roomA, roomB, bestTileA, bestTileB);
		}

		private void ConnectAllRooms(List<Room> rooms, bool ensureMainRoomConnection)
		{
			rooms.Sort();
			rooms[0].isMainRoom = true;
			rooms[0].isAccessibleFromMainRoom = true;
			rooms[0].ColorTiles(3);

			List<Room> roomListA = new List<Room>();
			List<Room> roomListB = new List<Room>();

			if (ensureMainRoomConnection)
			{
				foreach (Room r in rooms)
				{
					if(r.isAccessibleFromMainRoom)
						roomListB.Add(r);
					else
						roomListA.Add(r);
				}
			}
			else
			{
				roomListA = rooms;
				roomListB = rooms;
			}

			int bestDistance = 0;
			Tile bestTileA = null;
			Tile bestTileB = null;
			Room bestRoomA = null;
			Room bestRoomB = null;

			bool possibleConnectionFound = false;

			foreach (Room roomA in roomListA)
			{
				if (!ensureMainRoomConnection)
				{
					possibleConnectionFound = false;
					// pokud nezajistujeme pripojeni hlavni mistnosti + kazda mistnost muze byt pripojena pouze k jedne jine
					if(roomA.connectedRooms.Count > 0)
						continue;
				}

				foreach (Room roomB in roomListB)
				{
					if (roomA.Equals(roomB))
						continue;

					if (roomA.IsConnected(roomB))
					{
						possibleConnectionFound = false;
						break;
					}

					// projedeme vsechny okrajove body roomA
					for (int tileIndexA = 0; tileIndexA < roomA.edgeTiles.Count; tileIndexA++)
					{
						// projedeme vsechny okrajovy body roomB
						for (int tileIndexB = 0; tileIndexB < roomB.edgeTiles.Count; tileIndexB++)
						{
							Tile tileA = roomA.edgeTiles[tileIndexA];
							Tile tileB = roomB.edgeTiles[tileIndexB];

							// vypocitame vzdalenost kazdych dvou okrajovych bodu obou mistnosti
							int dx = tileA.tileX - tileB.tileX;
							int dy = tileA.tileY - tileB.tileY;
							int dist = (int) (Math.Pow(dx, 2) + Math.Pow(dy, 2));

							// tato vzdalenost je bud nejlepsi nebo possibleConnectionFound je false pri prvni iteraci
							if (dist < bestDistance || !possibleConnectionFound)
							{
								bestDistance = dist;
								possibleConnectionFound = true;

								bestTileA = tileA;
								bestTileB = tileB;
								bestRoomA = roomA;
								bestRoomB = roomB;
							}
						}
					}
				}

				if (possibleConnectionFound && !ensureMainRoomConnection)
				{
					CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
				}
			}

			if (possibleConnectionFound && ensureMainRoomConnection)
			{
				CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
				ConnectAllRooms(rooms, true);
			}

			if (!ensureMainRoomConnection)
			{
				ConnectAllRooms(rooms, true);
			}
		}

		private void CreatePassage(Room roomA, Room roomB, Tile tileA, Tile tileB)
		{
			Room.ConnectRooms(roomA, roomB);

			foreach (Tile edge in roomA.edgeTiles)
			{
				//edge.highlighted = true;
			}

			tileA.SetColor(Tile.GREEN);
			tileB.SetColor(Tile.GREEN);

			//Debug.DrawLine(TileToWorldPoint(tileA), TileToWorldPoint(tileB), Color.blue, 100f);
			List<Tile> line = GetLine(tileA, tileB);

			foreach (Tile t in line)
			{
				t.SetColor(Tile.BLUE);
				DrawCircle(t, 5, FLOOR);
			}
		}

		private void DrawCircle(Tile t, int r, int tileType)
		{
			for (int x = -r; x <= r; x++)
			{
				for (int y = -r; y <= r; y++)
				{
					if (x * x + y * y <= r * r)
					{
						int drawX = t.tileX + x;
						int drawY = t.tileY + y;

						Tile drawTile = GetTile(drawX, drawY);

						if (drawTile != null)
							drawTile.tileType = tileType;
					}
				}
			}
		}

		private List<Tile> GetLine(Tile from, Tile to)
		{
			//Debug.Log("drawing line");
			List<Tile> tiles = new List<Tile>();

			int x = from.tileX;
			int y = from.tileY;

			int dx = to.tileX - from.tileX;
			int dy = to.tileY - from.tileY;

			int absdx = Math.Abs(dx);
			int absdy = Math.Abs(dy);

			int step; // urcuje jakym smerem pujdeme (po ose zapornym nebo kladnym
			int gradientStep;
			int longest;
			int shortest;
			bool poOseY;

			// zjistime jestli je kratsi vzdalenost dx nebo dy
			if (absdx < absdy)
			{
				longest = absdy;
				shortest = absdx;
				step = Math.Sign(dy);
				gradientStep = Math.Sign(dx);
				poOseY = true;
			}
			else
			{
				longest = absdx;
				shortest = absdy;
				step = Math.Sign(dx);
				gradientStep = Math.Sign(dy);
				poOseY = false;
			}

			int gradientAccumulation = longest / 2;
			for (int i = 0; i < longest; i++)
			{
				tiles.Add(GetTile(x, y)); // pridat prvni bod
				GetTile(x, y).SetColor(Tile.GREEN);

				if (poOseY)
				{
					y += step;
				}
				else
				{
					x += step;
				}

				gradientAccumulation += shortest;
				if (gradientAccumulation >= longest)
				{
					if (poOseY)
					{
						x += gradientStep;
					}
					else
					{
						y += gradientStep;
					}
					gradientAccumulation -= longest;
				}
			}

			return tiles;
		} 

		//TODO not needed anymore
		/*private Vector3 TileToWorldPoint(Tile tile)
		{
			return new Vector3(-width / 2 + .5f + tile.tileX, -height / 2 + .5f + tile.tileY, 0) + shiftVector;
		}*/

		/*public override void OnDrawGizmos()
		{
			//Debug.Log("drawing");
			if (doDebug == false)
				return;

			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					float xSize = (width) * 1;
					float ySize = (height) * 1;

					Tile t = GetTile(x, y);

					if (t == null)
						continue;

					Gizmos.color = (t.tileType == WALL) ? Color.black : Color.white;
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

					Vector3 pos = new Vector3((-width / 2 + x + .5f) + shiftX * xSize, (-height / 2 + y + .5f) + shiftY * ySize, 0);
					Gizmos.DrawCube(pos, Vector3.one);
				}
			}
		}*/

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

			List<Tile> tiles = new List<Tile>();
			reg.tiles = tiles;

			Queue<Tile> queue = new Queue<Tile>();
			queue.Enqueue(GetTile(startX, startY));
			GetTile(startX, startY).Check();

			while (queue.Count > 0)
			{
				Tile t = queue.Dequeue();
				tiles.Add(t);

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

		private void SetTile(int x, int y, int type)
		{
			Tile t = GetTile(x, y);
			t.tileType = type;
		}

		private int GetTileCount(int[] tiles, int tileType, bool includeNonExisting)
		{
			int i = 0;
			foreach (int tile in tiles)
			{
				if (tile == tileType || (includeNonExisting && tile == -1))
					i++;
			}

			return i;
		}

		private Tile[] GetNeighbours(int x, int y, bool includeDiag)
		{
			Tile[] neighb;
			if (includeDiag)
				neighb = new Tile[8];
			else
				neighb = new Tile[4];

			neighb[0] = Exists(x, y + 1) ? GetTile(x, y + 1) : null;
			neighb[1] = Exists(x, y - 1) ? GetTile(x, y - 1) : null;
			neighb[2] = Exists(x + 1, y) ? GetTile(x + 1, y) : null;
			neighb[3] = Exists(x - 1, y) ? GetTile(x - 1, y) : null;

			if (includeDiag)
			{
				neighb[4] = Exists(x + 1, y + 1) ? GetTile(x + 1, y + 1) : null;
				neighb[5] = Exists(x - 1, y - 1) ? GetTile(x - 1, y - 1) : null;
				neighb[6] = Exists(x + 1, y - 1) ? GetTile(x + 1, y - 1) : null;
				neighb[7] = Exists(x - 1, y + 1) ? GetTile(x - 1, y + 1) : null;
			}

			return neighb;
		}

		private int[] GetNeighbourTypes(int x, int y, bool includeDiag)
		{
			int[] neighb;
            if (includeDiag)
				neighb = new int[8];
			else
				neighb = new int[4];

			neighb[0] = Exists(x, y + 1) ? GetTile(x, y + 1).tileType : -1;
			neighb[1] = Exists(x, y - 1) ? GetTile(x, y - 1).tileType : -1;
			neighb[3] = Exists(x + 1, y) ? GetTile(x + 1, y).tileType : -1;
			neighb[6] = Exists(x - 1, y) ? GetTile(x - 1, y).tileType : -1;

			if (includeDiag)
			{
				neighb[2] = Exists(x + 1, y + 1) ? GetTile(x + 1, y + 1).tileType : -1;
				neighb[7] = Exists(x - 1, y - 1) ? GetTile(x - 1, y - 1).tileType : -1;
				neighb[4] = Exists(x + 1, y - 1) ? GetTile(x + 1, y - 1).tileType : -1;
				neighb[5] = Exists(x - 1, y + 1) ? GetTile(x - 1, y + 1).tileType : -1;
			}

			return neighb;
		}

		private void RandomFillMap()
		{
			System.Random pseudoRandom = new System.Random(seed.GetHashCode());

			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					if (IsEdge(x, y))
					{
						SetTile(x, y, WALL);
					}
					else
					{
						SetTile(x, y, GetRandomTile(pseudoRandom));
					}
				}
			}
		}

		private int GetRandomTile(System.Random pseudoRandom)
		{
			int rnd = pseudoRandom.Next(0, 100);
			if (rnd < randomFillPercent)
			{
				return WALL;
			}
			else
			{
				return FLOOR;
			}
		}

		private bool Exists(int x, int y)
		{
			return x >= 0 && x < width && y >= 0 && y < height;
		}

		private Tile GetTile(int x, int y)
		{
			try
			{
				return tiles[x, y];
			}
			catch (Exception)
			{
				return null;
			}
		}

		private bool IsEdge(int x, int y)
		{
			if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
				return true;
			return false;
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
					if(b) t.Check();
					else t.Uncheck();
				}
            }
		}

		public override Tile[,] GetTiles()
		{
			return tiles;
		}

		public override List<Room> GetConnectedRooms()
		{
			return connectedRooms;
		}

		public override List<Room> GetSeparatedRooms()
		{
			List<Room> list = new List<Room>();
			list.Add(mainRoom);
			return list;
		}

		public override Room GetMainRoom()
		{
			return mainRoom;
		}
	}
}
