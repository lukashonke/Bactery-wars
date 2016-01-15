using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.scripts.Mono.MapGenerator
{
	/// <summary>
	/// Reprezentuje jednu mistnost na mape (vsechny mistnosti jsou propojene tunelem, centralni mistnost muze obsahovat bosse)
	/// Mistnost neobsahuje oblasti ve kterych se hrac muze pohybovat
	/// </summary>
	public class MapRoom
	{
		/// region kteremu tato mistnost nalezi
		public MapRegion region;

        /// obsahuje vsechny Tily ktere tvori mistnost (takze neobsahuje zdi)
		public List<Tile> tiles;

        /// obsahuje okrajove zdi
		public List<Tile> edgeTiles;
		public List<MapRoom> connectedRooms;
		public int roomSize;

		public bool isAccessibleFromStartRoom;

		public MapRoom()
		{
		}

		public MapRoom(List<Tile> roomTiles, Tile[,] map)
		{
			tiles = roomTiles;
			roomSize = tiles.Count;
			connectedRooms = new List<MapRoom>();

			region = tiles[0].region;

			edgeTiles = new List<Tile>();

			// create the edge wall tiles
			foreach (Tile tile in tiles)
			{
				// go through neighbours
				for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++)
				{
					for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++)
					{
						// exlude diagonal
						if (x == tile.tileX || y == tile.tileY)
						{
							try
							{
								if (map[x, y].tileType == 1)
								{
									//map[x, y].SetColor(Tile.BLUE);
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

		public void SetAccessibleFromStartRoom()
		{
			if (!isAccessibleFromStartRoom)
			{
				isAccessibleFromStartRoom = true;
				foreach (MapRoom connectedRoom in connectedRooms)
				{
					connectedRoom.SetAccessibleFromStartRoom();
				}
			}
		}

		public static void ConnectRooms(MapRoom roomA, MapRoom roomB)
		{
			if (roomA.isAccessibleFromStartRoom)
			{
				roomB.SetAccessibleFromStartRoom();
			}
			else if (roomB.isAccessibleFromStartRoom)
			{
				roomA.SetAccessibleFromStartRoom();
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

		public bool IsConnected(MapRoom otherRoom)
		{
			return connectedRooms.Contains(otherRoom);
		}

		public int CompareTo(MapRoom otherRoom)
		{
			return otherRoom.roomSize.CompareTo(roomSize);
		}
	}
}
