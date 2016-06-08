// Copyright (c) 2015, Lukas Honke
// ========================
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.scripts.Mono.MapGenerator
{
	/// <summary>
	/// materska trida pro generovani regionů na mape (dungeon, louka, mesto, atd.)
	/// </summary>
	public abstract class RegionGenerator
	{
		protected int width;
		protected int height;
		protected string seed;
		protected bool doDebug;

		//protected int shiftX;
		//protected int shiftY;
		//protected Vector3 shiftVector;
		//protected float meshSquareSize;

		public RegionGenerator(int width, int height, string seed, bool DoDebug)
		{
			this.width = width;
			this.height = height;
			this.seed = seed;
			this.doDebug = DoDebug;
			//this.shiftVector = shiftVector;
			//this.shiftX = shiftX;
			//this.shiftY = shiftY;
		}

		public abstract Tile[,] GenerateMap();
		//public abstract MeshGenerator GenerateMesh(GameObject parent, int[,] map, float squareSize);
		//public abstract MeshGenerator GetMeshGenerator();

		public abstract Tile[,] GetTiles();
		public abstract List<Room> GetConnectedRooms();
		public abstract List<Room> GetSeparatedRooms();
		public abstract Room GetMainRoom();

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

	public class Room : IComparable<Room>
	{
		public List<Tile> tiles;
		public List<Tile> edgeTiles;
		public List<Room> connectedRooms;
		public int roomSize;
		public bool isAccessibleFromMainRoom;
		public bool isMainRoom;

		public Room()
		{
		}

		public Room(List<Tile> roomTiles, Tile[,] map)
		{
			tiles = roomTiles;
			roomSize = tiles.Count;
			connectedRooms = new List<Room>();

			edgeTiles = new List<Tile>();

			foreach (Tile tile in tiles) //TODO make better
			{
				for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++)
				{
					for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++)
					{
						if (x == tile.tileX || y == tile.tileY)
						{
							try
							{
								if (map[x, y].tileType == 1)
								{
									edgeTiles.Add(tile);
								}
							}
							catch (Exception)
							{
							}
						}
					}
				}
			}
		}

		public void SetAccessibleFromMainRoom()
		{
			if (!isAccessibleFromMainRoom)
			{
				isAccessibleFromMainRoom = true;
				foreach (Room connectedRoom in connectedRooms)
				{
					connectedRoom.SetAccessibleFromMainRoom();
				}
			}
		}

		public static void ConnectRooms(Room roomA, Room roomB)
		{
			if (roomA.isAccessibleFromMainRoom)
			{
				roomB.SetAccessibleFromMainRoom();
			}
			else if (roomB.isAccessibleFromMainRoom)
			{
				roomA.SetAccessibleFromMainRoom();
			}

			roomA.connectedRooms.Add(roomB);
			roomB.connectedRooms.Add(roomA);
		}

		public void ColorTiles(int color, bool replace = false)
		{
			foreach (Tile t in tiles)
			{
				if (t.GetColor() == 0 || replace)
					t.SetColor(color);
			}
		}

		public bool IsConnected(Room otherRoom)
		{
			return connectedRooms.Contains(otherRoom);
		}

		public int CompareTo(Room otherRoom)
		{
			return otherRoom.roomSize.CompareTo(roomSize);
		}
	}
}