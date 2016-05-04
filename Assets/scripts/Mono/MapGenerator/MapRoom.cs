using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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

		    region = tiles[0].region.GetParentOrSelf();

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

		public Tile GetTileWithSpaceAround(int minFreeRadius, int preferredDirection)
		{
			return GetTilesWithSpaceAround(minFreeRadius, preferredDirection, 1)[0];
		}

        public Tile[] GetTilesWithSpaceAround(int minFreeRadius, int preferredDirection, int count)
        {
            Tile[] toReturn = new Tile[count];

            List<Tile> sortedTiles = new List<Tile>();
            foreach (Tile t in tiles)
                sortedTiles.Add(t);

            switch (preferredDirection)
            {
                case DIRECTION_UP:
                    sortedTiles = sortedTiles.OrderByDescending(o => o.tileY).ToList();
                    break;
                case DIRECTION_DOWN:
                    sortedTiles = sortedTiles.OrderBy(o => o.tileY).ToList();
                    break;
                case DIRECTION_RIGHT:
                    sortedTiles = sortedTiles.OrderByDescending(o => o.tileX).ToList();
                    break;
                case DIRECTION_LEFT:
                    sortedTiles = sortedTiles.OrderBy(o => o.tileX).ToList();
                    break;
				case DIRECTION_CENTER:
					List<MapRegion> roomRegions = region.map.GetRegionsInFamilyWith(region);

			        int smallestX = 100000000;
			        int smallestY = 100000000;
			        int largestX = -100000000;
			        int largestY = -100000000;
			        int centerX;
			        int centerY;

			        foreach (MapRegion r in roomRegions)
			        {
				        for (int i = 0; i < r.tileMap.GetLength(0); i++)
				        {
					        for (int j = 0; j < r.tileMap.GetLength(1); j++)
					        {
						        Tile t = r.tileMap[i, j];
						        if (t == null)
							        continue;

						        if (t.tileX < smallestX)
							        smallestX = t.tileX;
								else if (t.tileX > largestX)
									largestX = t.tileX;

						        if (t.tileY < smallestY)
							        smallestY = t.tileY;
								else if (t.tileY > largestY)
									largestY = t.tileY;
					        }
				        }
			        }

			        centerX = (smallestX + largestX) / 2;
					centerY = (smallestY + largestY) / 2;

					sortedTiles.Sort(
					delegate(Tile t1, Tile t2)
					{
						int dx1 = (int)Math.Pow(t1.tileX - centerX, 2);
						int dy1 = (int)Math.Pow(t1.tileY - centerY, 2);

						int dx2 = (int)Math.Pow(t2.tileX - centerX, 2);
						int dy2 = (int)Math.Pow(t2.tileY - centerY, 2);

						return (dx1 + dy1).CompareTo(dx2 + dy2);
					}
					);

					break;
            }

            int index = 0;

            List<Tile> checkedTiles = new List<Tile>();

            foreach (Tile t in sortedTiles)
            {
                if (checkedTiles.Contains(t))
                    continue;

                if (CheckFreeRadiusAroundTile(t, sortedTiles, minFreeRadius, checkedTiles))
                {
                    t.SetColor(Tile.MAGENTA);
                    toReturn[index++] = t;
                }

                if (index >= count)
                    break;
            }

            return toReturn;
        }

	    public enum RoomType
	    {
			EPIC,HUGE,VERYLARGE,LARGE,MEDIUM,SMALL,TINY
	    }

		public Tile GetLargestSubRoom(bool exclude=true)
		{
			Tile room = null;

			foreach (RoomType type in Enum.GetValues(typeof (RoomType)))
			{
				Tile[] rooms = GetSubRooms(type, DIRECTION_CENTER, 1, exclude);

				bool exists = false;

				foreach (Tile t in rooms)
				{
					if (t != null)
						exists = true;
				}

				if (exists)
				{
					Debug.Log("found type " + Enum.GetName(typeof(RoomType), type));
					room = rooms[0];
					break;
				}
			}

			return room;
		}

		public Tile GetSubRoom(RoomType type, int preferredDirection, bool exclude = true)
		{
			try
			{
				return GetSubRooms(type, preferredDirection, 1, exclude)[0];
			}
			catch (Exception)
			{
				return null;
			}
		}

		public Tile[] GetSubRooms(RoomType type, int preferredDirection, int count, bool exclude=true)
	    {
			foreach (Tile t in tiles)
			{
				if (t.isChecked)
				{
					t.SetColor(Tile.RED);
				}
			}

	        switch (type)
	        {
	            case RoomType.TINY:
	                return GetSubRooms(40, 3, preferredDirection, count, exclude);
	                break;
	            case RoomType.SMALL:
					return GetSubRooms(75, 4, preferredDirection, count, exclude);
	                break;
	            case RoomType.MEDIUM:
					return GetSubRooms(105, 5, preferredDirection, count, exclude);
	                break;
	            case RoomType.LARGE:
					return GetSubRooms(200, 7, preferredDirection, count, exclude);
	                break;
	            case RoomType.VERYLARGE:
					return GetSubRooms(265, 8, preferredDirection, count, exclude);
	                break;
				case RoomType.HUGE:
					return GetSubRooms(380, 10, preferredDirection, count, exclude);
					break;
				case RoomType.EPIC:
					return GetSubRooms(500, 12, preferredDirection, count, exclude);
					break;
	        }

	        return null;
	    }

		public const int DIRECTION_UP = 1;
		public const int DIRECTION_DOWN = 2;
		public const int DIRECTION_RIGHT = 3;
		public const int DIRECTION_LEFT = 4;
		public const int DIRECTION_CENTER = 5;
		public const int DIRECTION_CENTER_LEFT = 6;
		public const int DIRECTION_CENTER_RIGHT = 7;

		public const int DIRECTION_LARGEST_ROOM = 10;

		public Tile GetSubRoom(int minTiles, int maxRadius, int preferredDirection, bool exclude = false)
		{
			try
			{
				return GetSubRooms(minTiles, maxRadius, preferredDirection, 1, exclude)[0];
			}
			catch (Exception)
			{
				return null;
			}
		}

	    public Tile[] GetSubRooms(int minTiles, int maxRadius, int preferredDirection, int count, bool exclude=false)
	    {
	        Tile[] toReturn = new Tile[count];

	        List<Tile> sortedTiles = new List<Tile>();
	        foreach (Tile t in tiles)
	            sortedTiles.Add(t);

	        switch (preferredDirection)
	        {
				case DIRECTION_UP:
	                sortedTiles = sortedTiles.OrderByDescending(o => o.tileY).ToList();
	                break;
				case DIRECTION_DOWN:
	                sortedTiles = sortedTiles.OrderBy(o => o.tileY).ToList();
	                break;
	            case DIRECTION_RIGHT:
	                sortedTiles = sortedTiles.OrderByDescending(o => o.tileX).ToList();
	                break;
	            case DIRECTION_LEFT:
	                sortedTiles = sortedTiles.OrderBy(o => o.tileX).ToList();
	                break;
				case DIRECTION_CENTER:

			        List<MapRegion> roomRegions = region.map.GetRegionsInFamilyWith(region);
					//List<Tile> allRoomTiles = new List<Tile>();

			        int smallestX = 100000000;
			        int smallestY = 100000000;
			        int largestX = -100000000;
			        int largestY = -100000000;
			        int centerX;
			        int centerY;

			        foreach (MapRegion r in roomRegions)
			        {
				        for (int i = 0; i < r.tileMap.GetLength(0); i++)
				        {
					        for (int j = 0; j < r.tileMap.GetLength(1); j++)
					        {
						        Tile t = r.tileMap[i, j];
						        if (t == null)
							        continue;

						        //allRoomTiles.Add(t);

						        if (t.tileX < smallestX)
							        smallestX = t.tileX;
								else if (t.tileX > largestX)
									largestX = t.tileX;

						        if (t.tileY < smallestY)
							        smallestY = t.tileY;
								else if (t.tileY > largestY)
									largestY = t.tileY;
					        }
				        }
			        }

			        centerX = (smallestX + largestX) / 2;
					centerY = (smallestY + largestY) / 2;

					sortedTiles.Sort(
					delegate(Tile t1, Tile t2)
					{
						int dx1 = (int)Math.Pow(t1.tileX - centerX, 2);
						int dy1 = (int)Math.Pow(t1.tileY - centerY, 2);

						int dx2 = (int)Math.Pow(t2.tileX - centerX, 2);
						int dy2 = (int)Math.Pow(t2.tileY - centerY, 2);

						return (dx1 + dy1).CompareTo(dx2 + dy2);
					}
					);

					break;
	        }

	        int index = 0;

	        List<Tile> checkedTiles = new List<Tile>();

	        foreach (Tile t in sortedTiles)
	        {
	            if (checkedTiles.Contains(t) || t.isChecked)
	                continue;

	            if (CheckForTilesInRadius(t, sortedTiles, maxRadius, minTiles, checkedTiles, exclude))
	            {
	                t.SetColor(Tile.ORANGE);
	                toReturn[index++] = t;
	            }

	            if (index >= count)
	                break;
	        }

	        return toReturn;
	    }

	    private bool CheckForTilesInRadius(Tile t, List<Tile> sortedTiles, int maxRadius, int minTiles, List<Tile> roomTiles, bool exclude)
	    {
	        List<Tile> temp = new List<Tile>();

	        int minX = t.tileX - maxRadius;
	        int maxX = t.tileX + maxRadius;

	        int minY = t.tileY - maxRadius;
	        int maxY = t.tileY + maxRadius;

	        int count = 0;

	        foreach (Tile neighbour in sortedTiles)
	        {
				if(neighbour.isChecked)
					continue;

	            if (neighbour.tileX >= minX && neighbour.tileX <= maxX && neighbour.tileY >= minY && neighbour.tileY <= maxY)
	            {
	                temp.Add(neighbour);
	                count++;

	                if (count >= minTiles)
	                    break;
	            }
	        }

	        if (count >= minTiles)
	        {
	            if (roomTiles != null)
	            {
	                foreach (Tile tt in temp)
	                {
						if(exclude)
							tt.Check();

	                    tt.SetColor(Tile.PURPLE);
	                    roomTiles.Add(tt);
	                }
	            }

	            return true;
	        }

	        return false;
	    }

	    /// <summary>
	    /// Misto okolo pozadovaneho tilu musi byt UPLNE prazdne od vsech zdi
	    /// </summary>
	    /// <returns></returns>
	    private bool CheckFreeRadiusAroundTile(Tile t, List<Tile> sortedTiles, int radiusToCheck, List<Tile> neighbours)
	    {
	        List<Tile> temp = new List<Tile>();
	        int minX = t.tileX - radiusToCheck;
	        int maxX = t.tileX + radiusToCheck;

	        int minY = t.tileY - radiusToCheck;
	        int maxY = t.tileY + radiusToCheck;

	        int count = 0;
	        int minCount = (maxX - minX + 1)*(maxY - minY + 1);

	        foreach (Tile neighbour in sortedTiles)
	        {
	            if (neighbour.tileX >= minX && neighbour.tileX <= maxX && neighbour.tileY >= minY && neighbour.tileY <= maxY)
	            {
	                temp.Add(neighbour);
	                // tile je uvnitr kontrolovane oblasti kolem naseho bodu
	                count ++;
	            }
	        }

	        if (count >= minCount)
	        {
	            if (neighbours != null)
	            {
	                foreach (Tile tt in temp)
	                    neighbours.Add(tt);
	            }

	            return true;
	        }

	        return false;
	    }
	}
}
