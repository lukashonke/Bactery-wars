using System;
using System.Collections.Generic;
using System.Diagnostics;
using Coord = Assets.scripts.Mono.MapGenerator.MapGenerator.Coord;
using Debug = UnityEngine.Debug;

class MapRoom : IComparable<MapRoom>
{
	public List<Coord> tiles;
	public List<Coord> edgeTiles;
	public List<MapRoom> connectedRooms;
	public int roomSize;
	public bool isAccessibleFromMainRoom;
	public bool isMainRoom;

	public MapRoom()
	{
	}

	public MapRoom(List<Coord> roomTiles, int[,] map)
	{
		tiles = roomTiles;
		roomSize = tiles.Count;
		connectedRooms = new List<MapRoom>();

		int count = 0;

		edgeTiles = new List<Coord>();
		foreach (Coord tile in tiles)
		{
			for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++)
			{
				for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++)
				{
					if (x == tile.tileX || y == tile.tileY)
					{
						try
						{
							if (map[x, y] == 1)
							{
								edgeTiles.Add(tile);
							}
						}
						catch (Exception)
						{
							count ++;
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
			foreach (MapRoom connectedRoom in connectedRooms)
			{
				connectedRoom.SetAccessibleFromMainRoom();
			}
		}
	}

	public static void ConnectRooms(MapRoom roomA, MapRoom roomB)
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

	public bool IsConnected(MapRoom otherRoom)
	{
		return connectedRooms.Contains(otherRoom);
	}

	public int CompareTo(MapRoom otherRoom)
	{
		return otherRoom.roomSize.CompareTo(roomSize);
	}
}
