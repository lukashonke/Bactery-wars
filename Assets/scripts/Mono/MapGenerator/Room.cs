using System;
using System.Collections.Generic;
using Assets.scripts.Mono.MapGenerator;
using Tile = Assets.scripts.Mono.MapGenerator.DungeonGenerator.Tile;

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
			if (t.color == 0 || replace)
				t.color = color;
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
