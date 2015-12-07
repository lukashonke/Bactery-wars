﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.scripts.Mono.MapGenerator
{
	public class MapGenerator : MonoBehaviour
	{
		public int width;
		public int height;

		public string seed;
		public bool useRandomSeed;

		[Range(0, 100)]
		public int randomFillPercent;

		public const int WALL = 1;
		public const int FLOOR = 0;

		int[,] map;

		void Start()
		{
			GenerateMap();
		}

		void Update()
		{
			if (Input.GetKeyDown(KeyCode.Space))
			{
				GenerateMap();
			}
		}

		void GenerateMap()
		{
			map = new int[width, height];
			RandomFillMap();

			for (int i = 0; i < 5; i++)
			{
				SmoothMap();
			}

			ProcessMap();

			int borderSize = 1;
			int[,] borderedMap = new int[width + borderSize * 2, height + borderSize * 2];

			for (int x = 0; x < borderedMap.GetLength(0); x++)
			{
				for (int y = 0; y < borderedMap.GetLength(1); y++)
				{
					if (x >= borderSize && x < width + borderSize && y >= borderSize && y < height + borderSize)
					{
						borderedMap[x, y] = map[x - borderSize, y - borderSize];
					}
					else
					{
						borderedMap[x, y] = WALL;
					}
				}
			}

			MeshGenerator meshGen = GetComponent<MeshGenerator>();
			meshGen.GenerateMesh(borderedMap, 1);
		}

		void ProcessMap()
		{
			List<List<Coord>> wallRegions = GetRegions(WALL);
			int wallThresholdSize = 50;

			foreach (List<Coord> wallRegion in wallRegions)
			{
				if (wallRegion.Count < wallThresholdSize)
				{
					foreach (Coord tile in wallRegion)
					{
						map[tile.tileX, tile.tileY] = FLOOR;
					}
				}
			}

			List<List<Coord>> roomRegions = GetRegions(FLOOR);
			int roomThresholdSize = 50;
			List<Room> survivingRooms = new List<Room>();

			foreach (List<Coord> roomRegion in roomRegions)
			{
				if (roomRegion.Count < roomThresholdSize)
				{
					foreach (Coord tile in roomRegion)
					{
						map[tile.tileX, tile.tileY] = WALL;
					}
				}
				else
				{
					survivingRooms.Add(new Room(roomRegion, map));
				}
			}
			survivingRooms.Sort();
			survivingRooms[0].isMainRoom = true;
			survivingRooms[0].isAccessibleFromMainRoom = true;

			ConnectClosestRooms(survivingRooms);
		}

		void ConnectClosestRooms(List<Room> allRooms, bool forceAccessibilityFromMainRoom = false)
		{

			List<Room> roomListA = new List<Room>();
			List<Room> roomListB = new List<Room>();

			if (forceAccessibilityFromMainRoom)
			{
				foreach (Room room in allRooms)
				{
					if (room.isAccessibleFromMainRoom)
					{
						roomListB.Add(room);
					}
					else
					{
						roomListA.Add(room);
					}
				}
			}
			else
			{
				roomListA = allRooms;
				roomListB = allRooms;
			}

			int bestDistance = 0;
			Coord bestTileA = new Coord();
			Coord bestTileB = new Coord();
			Room bestRoomA = new Room();
			Room bestRoomB = new Room();
			bool possibleConnectionFound = false;

			foreach (Room roomA in roomListA)
			{
				if (!forceAccessibilityFromMainRoom)
				{
					possibleConnectionFound = false;
					if (roomA.connectedRooms.Count > 0)
					{
						continue;
					}
				}

				foreach (Room roomB in roomListB)
				{
					if (roomA == roomB || roomA.IsConnected(roomB))
					{
						continue;
					}

					for (int tileIndexA = 0; tileIndexA < roomA.edgeTiles.Count; tileIndexA++)
					{
						for (int tileIndexB = 0; tileIndexB < roomB.edgeTiles.Count; tileIndexB++)
						{
							Coord tileA = roomA.edgeTiles[tileIndexA];
							Coord tileB = roomB.edgeTiles[tileIndexB];
							int distanceBetweenRooms = (int)(Mathf.Pow(tileA.tileX - tileB.tileX, 2) + Mathf.Pow(tileA.tileY - tileB.tileY, 2));

							if (distanceBetweenRooms < bestDistance || !possibleConnectionFound)
							{
								bestDistance = distanceBetweenRooms;
								possibleConnectionFound = true;
								bestTileA = tileA;
								bestTileB = tileB;
								bestRoomA = roomA;
								bestRoomB = roomB;
							}
						}
					}
				}
				if (possibleConnectionFound && !forceAccessibilityFromMainRoom)
				{
					CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB, FLOOR);
				}
			}

			if (possibleConnectionFound && forceAccessibilityFromMainRoom)
			{
				CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB, FLOOR);
				ConnectClosestRooms(allRooms, true);
			}

			if (!forceAccessibilityFromMainRoom)
			{
				ConnectClosestRooms(allRooms, true);
			}
		}

		void CreatePassage(Room roomA, Room roomB, Coord tileA, Coord tileB, int val)
		{
			Room.ConnectRooms(roomA, roomB);
			//Debug.DrawLine (CoordToWorldPoint (tileA), CoordToWorldPoint (tileB), Color.green, 100);

			List<Coord> line = GetLine(tileA, tileB);
			foreach (Coord c in line)
			{
				DrawCircle(c, 5, val);
			}
		}

		void DrawCircle(Coord c, int r, int val)
		{
			for (int x = -r; x <= r; x++)
			{
				for (int y = -r; y <= r; y++)
				{
					if (x * x + y * y <= r * r)
					{
						int drawX = c.tileX + x;
						int drawY = c.tileY + y;
						if (IsInMapRange(drawX, drawY))
						{
							map[drawX, drawY] = val;
						}
					}
				}
			}
		}

		List<Coord> GetLine(Coord from, Coord to)
		{
			List<Coord> line = new List<Coord>();

			int x = from.tileX;
			int y = from.tileY;

			int dx = to.tileX - from.tileX;
			int dy = to.tileY - from.tileY;

			bool inverted = false;
			int step = Math.Sign(dx);
			int gradientStep = Math.Sign(dy);

			int longest = Mathf.Abs(dx);
			int shortest = Mathf.Abs(dy);

			if (longest < shortest)
			{
				inverted = true;
				longest = Mathf.Abs(dy);
				shortest = Mathf.Abs(dx);

				step = Math.Sign(dy);
				gradientStep = Math.Sign(dx);
			}

			int gradientAccumulation = longest / 2;
			for (int i = 0; i < longest; i++)
			{
				line.Add(new Coord(x, y));

				if (inverted)
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
					if (inverted)
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

			return line;
		}

		Vector3 CoordToWorldPoint(Coord tile)
		{
			return new Vector3(-width / 2 + .5f + tile.tileX, 2, -height / 2 + .5f + tile.tileY);
		}

		List<List<Coord>> GetRegions(int tileType)
		{
			List<List<Coord>> regions = new List<List<Coord>>();
			int[,] mapFlags = new int[width, height];

			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					if (mapFlags[x, y] == 0 && map[x, y] == tileType)
					{
						List<Coord> newRegion = GetRegionTiles(x, y);
						regions.Add(newRegion);

						foreach (Coord tile in newRegion)
						{
							mapFlags[tile.tileX, tile.tileY] = 1;
						}
					}
				}
			}

			return regions;
		}

		List<Coord> GetRegionTiles(int startX, int startY)
		{
			List<Coord> tiles = new List<Coord>();
			int[,] mapFlags = new int[width, height];
			int tileType = map[startX, startY];

			Queue<Coord> queue = new Queue<Coord>();
			queue.Enqueue(new Coord(startX, startY));
			mapFlags[startX, startY] = 1;

			while (queue.Count > 0)
			{
				Coord tile = queue.Dequeue();
				tiles.Add(tile);

				for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++)
				{
					for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++)
					{
						if (IsInMapRange(x, y) && (y == tile.tileY || x == tile.tileX))
						{
							if (mapFlags[x, y] == 0 && map[x, y] == tileType)
							{
								mapFlags[x, y] = 1;
								queue.Enqueue(new Coord(x, y));
							}
						}
					}
				}
			}
			return tiles;
		}

		bool IsInMapRange(int x, int y)
		{
			return x >= 0 && x < width && y >= 0 && y < height;
		}

		private bool first = true;

		void RandomFillMap()
		{
			if (!first && useRandomSeed)
			{
				seed = Time.time.ToString();
			}

			first = false;

			System.Random pseudoRandom = new System.Random(seed.GetHashCode());

			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
					{
						map[x, y] = WALL;
					}
					else
					{
						map[x, y] = (pseudoRandom.Next(0, 100) < randomFillPercent) ? WALL : FLOOR;
					}
				}
			}
		}

		void SmoothMap()
		{
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					int neighbourWallTiles = GetSurroundingWallCount(x, y);

					if (neighbourWallTiles > 4)
						map[x, y] = WALL;
					else if (neighbourWallTiles < 4)
						map[x, y] = FLOOR;
				}
			}
		}

		int GetSurroundingWallCount(int gridX, int gridY)
		{
			int wallCount = 0;
			for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
			{
				for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
				{
					if (IsInMapRange(neighbourX, neighbourY))
					{
						if (neighbourX != gridX || neighbourY != gridY)
						{
							int temp = map[neighbourX, neighbourY];

							if (temp == WALL)
								wallCount ++;
						}
					}
					else
					{
						wallCount++;
					}
				}
			}

			return wallCount;
		}

		public struct Coord
		{
			public int tileX;
			public int tileY;

			public Coord(int x, int y)
			{
				tileX = x;
				tileY = y;
			}
		}
	}
}