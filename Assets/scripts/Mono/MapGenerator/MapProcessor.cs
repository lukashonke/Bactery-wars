using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.Base;
using Assets.scripts.Mono.MapGenerator.Levels;
using UnityEngine;

namespace Assets.scripts.Mono.MapGenerator
{
	/// <summary>
	/// Dostava jako parametr velkou mapu jako 2D Tile matici a zpracuje ji z globalniho hlediska (propoji regiony, vytvori dvere, spawne portaly atd.)
	/// </summary>
	public class MapProcessor
	{
		private MapHolder mapHolder;

		private Tile[,] tiles;
		private MapType mapType;

		public Tile[,] Tiles { get { return tiles; } }
		public List<MapRoom> rooms;

		private int width;
		private int height;

		public MapProcessor(MapHolder mapHolder, Tile[,] tiles, MapType type)
		{
			this.mapHolder = mapHolder;

			this.tiles = tiles;
			this.mapType = type;

			width = tiles.GetLength(0);
			height = tiles.GetLength(1);
		}

		public void Process()
		{
			UncheckAllTiles();

			//GetTile(40, 26).SetColor(Tile.GREEN);
			//int count = GetNeighbourWallsCount(GetTile(40, 26), 2);
			//GetTile(40, 26).SetColor(Tile.GREEN);
			//Debug.Log(count);

			AnalyzeRooms();
			ConnectRoomsToStart();

			UncheckAllTiles();

			SpawnTeleporters();

		    SpawnMonsters();

			//WiddenThinPassages(null, 6, 2);
		}

		private void UpdateRegions()
		{
			foreach (MapRoom room in rooms)
			{
				mapHolder.UpdateRegionStatus(room.region.GetParentOrSelf());
			}
		}

        private void SpawnMonsters()
        {
	        AbstractLevelData data = mapHolder.levelData;

	        if (data != null)
	        {
		        data.SpawnMonsters();
	        }
	        else // just for test
	        {
				Debug.LogWarning("a map doesnt have abstractleveldata set! spawning default mobs");

				foreach (MapRoom room in rooms)
				{
					MonsterSpawnInfo info = new MonsterSpawnInfo(MonsterId.Lymfocyte_melee, mapHolder.GetTileWorldPosition(room.tiles[25]));
					info.SetRegion(room.region.GetParentOrSelf());

					mapHolder.AddMonsterToMap(info);
				}
			}
	    }

		private void SpawnTeleporters()
		{
			foreach (MapRoom room in rooms)
			{
				if (room.region.isStartRegion)
				{
					Tile mostLeft = null;
					int mostLeftX = Int32.MaxValue;

					foreach (Tile t in room.edgeTiles)
					{
						if (t.tileX < mostLeftX)
						{
							mostLeft = t;
							mostLeftX = t.tileX;
						}
					}

					mapHolder.AddNpcToMap(MonsterId.TeleporterIn, mapHolder.GetTileWorldPosition(mostLeft) + Vector3.right*2);
				}

				if (room.region.hasOutTeleporter)
				{
					Tile mostRight = null;
					int mostRightX = Int32.MinValue;

					foreach (Tile t in room.edgeTiles)
					{
						if (t.tileX > mostRightX)
						{
							mostRight = t;
							mostRightX = t.tileX;
						}
					}

					mapHolder.AddNpcToMap(MonsterId.TeleporterOut, mapHolder.GetTileWorldPosition(mostRight) + Vector3.left*2);
				}
			}
		}

		private void WiddenThinPassages(Tile fixedTile, int threshold, int radius)
		{
			Utils.Timer.StartTimer("thinpassages");

			List<Tile> toCheck = new List<Tile>();

			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					Tile t = GetTile(x, y);

					if (t != null && t.tileType == WorldHolder.GROUND)
					{
						if (GetNeighbourWallsCount(t) == 0)
							continue;

						toCheck.Add(t);
					}
				}
			}

			foreach (Tile t in toCheck)
			{
				if (fixedTile != null && !t.Equals(fixedTile))
					continue;

				int count = GetNeighbourWallsCount(t, radius);

				if (count >= threshold)
				{
					SetSurroundingTiles(t, WorldHolder.GROUND);
				}
			}

			Utils.Timer.EndTimer("thinpassages");
		}

		private void SetSurroundingTiles(Tile t, int tileType)
		{
			foreach (Tile n in GetNeighbours(t.tileX, t.tileY, false))
			{
				if (n.tileType != tileType)
				{
					SetTile(n, tileType);
					n.SetColor(Tile.GREEN);
				}
			}
		}

		private int GetNeighbourWallsCount(Tile t)
		{
			int count = 0;
			foreach (Tile n in GetNeighbours(t.tileX, t.tileY, false))
			{
				if(n.tileType == WorldHolder.WALL)
					count++;
			}
			return count;
		}

		/// <summary>
		/// vrati vsechny sousedy vsech Tiles predanych v parametru tiles
		/// includeSources - zahrne do vystupu i predavane tiles
		/// </summary>
		private List<Tile> GetNeighbourGroundTilesFromTiles(List<Tile> tiles, bool includeSources)
		{
			List<Tile> all = new List<Tile>();

			foreach (Tile t in tiles)
			{
				if (includeSources)
				{
					if (all.Contains(t) == false)
						all.Add(t);
				}

				foreach (Tile n in GetNeighbours(t.tileX, t.tileY, false))
				{
					if (n.tileType == WorldHolder.WALL)
						continue;

					if(all.Contains(n) == false)
						all.Add(n);
				}
			}
			return all;
		}

		private int GetNeighbourWallsCount(Tile t, int radius)
		{
			List<Tile> walls = new List<Tile>();

			if(radius > 1)
			{
				List<Tile> neighbours = new List<Tile>();
				neighbours.Add(t);

				Queue<Tile> toCheck = new Queue<Tile>();
				toCheck.Enqueue(t);

				for (int i = 0; i < radius; i++)
				{
					neighbours = GetNeighbourGroundTilesFromTiles(neighbours, true);
				}

				foreach (Tile n in neighbours)
				{
					//n.SetColor(Tile.BLUE);
					toCheck.Enqueue(n);
				}

				while (toCheck.Count > 0)
				{
					Tile temp = toCheck.Dequeue();

					foreach (Tile wall in GetNeighbours(temp.tileX, temp.tileY, false))
					{
						if (wall.tileType == WorldHolder.WALL && !walls.Contains(wall))
						{
							//wall.SetColor(Tile.RED);
							walls.Add(wall);
						}
					}
				}
				
			}
			return walls.Count;
		}

		private void UncheckAllTiles()
		{
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					Tile t = GetTile(x, y);
					if(t != null)
						t.Uncheck();
				}
			}
		}

		private void GetClosestTiles(MapRoom roomA, MapRoom roomB, out Tile bestTileA, out Tile bestTileB, out int bestDistance)
		{
			bestDistance = -1;
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
					int dist = dx*dx + dy*dy;

					if (dist < bestDistance || bestDistance == -1)
					{
						bestDistance = dist;

						bestTileA = tileA;
						bestTileB = tileB;
					}
				}
			}
		}

		private void ConnectRoomsToStart()
		{
			Queue<MapRoom> toConnect = new Queue<MapRoom>();
			List<MapRoom> connectedToStart = new List<MapRoom>();
			MapRoom startRoom = null;

			List<MapRegion> checks;

			foreach (MapRoom room in rooms)
			{
				if (room.region.isStartRegion)
				{
					startRoom = room;
					startRoom.SetAccessibleFromStartRoom();

					connectedToStart.Add(startRoom);

                    Queue<MapRegion> neighbours = new Queue<MapRegion>();
                    foreach(MapRegion reg in mapHolder.GetNeighbourRegions(startRoom.region)) 
                        if(!reg.empty)
                            neighbours.Enqueue(reg);

					checks = new List<MapRegion>();

					// vzit vsechny sousedy startovni mistnosti
					while(neighbours.Count > 0)
					{
					    MapRegion reg = neighbours.Dequeue();

						checks.Add(reg);

						// jen ty ktere MAJI BYT pripojene
						if (reg == null || !reg.isAccessibleFromStart)
							continue;

                        // soused startovniho regionu je jeho potomek - jsou spojeny - pridat do seznamu connectedToStart
					    if (reg.IsInFamily(startRoom.region))
					    {
                            foreach (MapRoom r in GetRoomsInRegion(reg))
                                connectedToStart.Add(r);

                            // pridat vsechny sousedy child regionu
					        foreach (MapRegion r in mapHolder.GetNeighbourRegions(reg))
					        {
					            if(r != null && r.isAccessibleFromStart && !neighbours.Contains(r) && !checks.Contains(r))
                                    neighbours.Enqueue(r);
					        }

					        continue;
					    }

						// pridat do seznamu k pripojeni
						foreach (MapRoom r in GetRoomsInRegion(reg))
							if (!r.isAccessibleFromStartRoom)
								toConnect.Enqueue(r);
					}
				}
			}

			while (toConnect.Count > 0)
			{
				// 1. vzit kazdou mistnost k pripojeni a zkusit ji pripojit k jedne z mistnosti v "connected"

				MapRoom toConnectRoom = toConnect.Dequeue();

				//Debug.Log("** connecting " + toConnectRoom.region.x + ", " + toConnectRoom.region.y);

				int max = connectedToStart.Count;
				for(int i = 0; i < max; i++)
				{
					MapRoom connectedRoom = connectedToStart[i];

					// mistnosti jsou sousedi
					if (CanBeConnected(toConnectRoom, connectedRoom) && !AreRoomsConnected(toConnectRoom, connectedRoom))
					{
						if (connectedRoom.isAccessibleFromStartRoom && connectedRoom.region.onlyOnePassage && connectedRoom.connectedRooms.Count > 0)
							continue;

						//Debug.Log("propojuji " + toConnectRoom.region.x + ", " + toConnectRoom.region.y + " s " + connectedRoom.region.x + ", " + connectedRoom.region.y);

						ConnectRooms(toConnectRoom, connectedRoom);
						connectedToStart.Add(toConnectRoom);

						Queue<MapRegion> neighbours = new Queue<MapRegion>();
						foreach (MapRegion reg in mapHolder.GetNeighbourRegions(toConnectRoom.region))
                            if (!reg.empty)
							    neighbours.Enqueue(reg);

						checks = new List<MapRegion>();

						// vzit vsechny sousedy startovni mistnosti
						while (neighbours.Count > 0)
						{
							MapRegion reg = neighbours.Dequeue();

							checks.Add(reg);

							// jen ty ktere MAJI BYT pripojene
							if (reg == null || !reg.isAccessibleFromStart) // TODO check if already connected?
								continue;

							// soused startovniho regionu je jeho potomek - jsou spojeny - pridat do seznamu connectedToStart
							if (reg.IsInFamily(toConnectRoom.region))
							{
								//Debug.Log("region " + reg.x + ", " + reg.y + " je ve family s toConnect (" + toConnectRoom.region.x + ", " + toConnectRoom.region.y + ")");
								foreach (MapRoom r in GetRoomsInRegion(reg))
									connectedToStart.Add(r);

								// pridat vsechny sousedy child regionu
								foreach (MapRegion r in mapHolder.GetNeighbourRegions(reg))
								{
									if (r != null && r.isAccessibleFromStart && !neighbours.Contains(r) && !r.IsInFamily(reg) && !checks.Contains(r))
									{
										//Debug.Log("pridavam souseda (reg " + reg.x + ", " + reg.y + ")  : " + r.x + ", " + r.y);
										neighbours.Enqueue(r);
									}
								}

								continue;
							}

							// pridat do seznamu k pripojeni
							foreach (MapRoom r in GetRoomsInRegion(reg.GetParentOrSelf()))
								if (!r.isAccessibleFromStartRoom)
									toConnect.Enqueue(r);
						}

						// pripojit k prvni nalezene
						if(toConnectRoom.region.onlyOnePassage)
							break;
					}
				}
			}
		}

		private void ConnectRooms(MapRoom roomA, MapRoom roomB)
		{
			roomA.connectedRooms.Add(roomB);
			roomB.connectedRooms.Add(roomA);

			if (roomA.isAccessibleFromStartRoom || roomB.isAccessibleFromStartRoom)
			{
				roomA.SetAccessibleFromStartRoom();
				roomB.SetAccessibleFromStartRoom();
			}

			CreatePassage(roomA, roomB);
		}

		private void CreatePassage(MapRoom roomA, MapRoom roomB)
		{
			Tile bestTileA;
			Tile bestTileB;
			int bestDistance;

			GetClosestTiles(roomA, roomB, out bestTileA, out bestTileB, out bestDistance);

			bestTileA.SetColor(Tile.ORANGE);
			bestTileB.SetColor(Tile.PURPLE);

			List<Tile> line = GetLine(bestTileA, bestTileB);
			List<Tile> passageTiles = new List<Tile>();

			foreach (Tile t in line)
			{
				t.SetColor(Tile.BROWN);
				List<Tile> circle = DrawCircle(t, 5, WorldHolder.GROUND);

				foreach(Tile c in circle)
					passageTiles.Add(c);
			}

			bool makeDoor = roomA.region.isLockedRegion || roomB.region.isLockedRegion;
			CreatePassageInPoint(line[line.Count/2], makeDoor, roomA, roomB);
		}

		private void CreatePassageInPoint(Tile center, bool makeDoor, MapRoom roomA, MapRoom roomB)
		{
			center.SetColor(Tile.ORANGE);

			Tile start = GetClosestTileFrom(center, WorldHolder.WALL);

			int dx = start.tileX - center.tileX;
			int dy = start.tileY - center.tileY;

			Tile end = GetTile(center.tileX - dx, center.tileY - dy);
			if (end.tileType != WorldHolder.WALL)
			{
				end = GetClosestTileFrom(end, WorldHolder.WALL);
			}

			List<Tile> line = GetLine(start, end);

			foreach (Tile t in line)
			{
				t.SetColor(Tile.GREEN);
			}

			start.SetColor(Tile.MAGENTA);
			end.SetColor(Tile.PINK);

			MapPassage passage = new MapPassage(line, center, start, end, roomA, roomB);
			passage.isDoor = makeDoor;
			mapHolder.AddPassage(passage);
		}

		private Tile GetClosestTileFrom(Tile from, int tileType)
		{
			if (from.tileType == tileType)
				return from;

			Tile wall = null;

			int i = 1;

			while (wall == null)
			{
				wall = GetTile(from.tileX + i, from.tileY, tileType);
				if (wall != null)
					break;

				wall = GetTile(from.tileX, from.tileY + i, tileType);
				if (wall != null)
					break;

				wall = GetTile(from.tileX - i, from.tileY, tileType);
				if (wall != null)
					break;

				wall = GetTile(from.tileX, from.tileY - i, tileType);
				if (wall != null)
					break;

				i++;

				if (i == 100)
					break;
			}

			return wall;
		}

		public void ThrowError()
		{
			Debug.LogError("error while processing map of seed " + mapHolder.World.seed);
		}

		// obvykle vraci jen jednu mistnost, ale v budoucnu by v regionu mohla byt vice nez jedna mistnost
		private List<MapRoom> GetRoomsInRegion(MapRegion reg)
		{
			List<MapRoom> inRegion = new List<MapRoom>();
			foreach (MapRoom room in rooms)
			{
				if (room.region.Equals(reg.GetParentOrSelf()))
				{
					inRegion.Add(room);
				}
			}

			return inRegion;
		}

		private bool CanBeConnected(MapRoom roomA, MapRoom roomB)
		{
			if (roomA.region.Equals(roomB.region))
				return false;

            MapRegion regA = roomA.region;
            MapRegion regB = roomB.region;

            List<MapRegion> regionsA = new List<MapRegion>();
            List<MapRegion> regionsB = new List<MapRegion>();

            regionsA.Add(regA);
            regionsB.Add(regB);

            // najit child regiony
            foreach (MapRegion region in mapHolder.regions.Values)
		    {
		        if (region.HasParentRegion())
		        {
                    if (region.parentRegion.Equals(regA))
                        regionsA.Add(region);

                    else if (region.parentRegion.Equals(regB))
                        regionsB.Add(region);
		        }
		    }

		    bool areNeighbours = false;
		    foreach (MapRegion ra in regionsA)
		    {
		        foreach (MapRegion rb in regionsB)
		        {
		            if (mapHolder.IsNeighbour(ra, rb))
		            {
		                areNeighbours = true;
		                return areNeighbours;
		            }
		        }
		    }

		    return areNeighbours;
		}

		private bool AreRoomsConnected(MapRoom roomA, MapRoom roomB)
		{
			return roomA.IsConnected(roomB);
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
		}

		private List<Region> GetRegions(int tileType)
		{
			List<Region> reg = new List<Region>();

			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					Tile t = GetTile(x, y);

					if (t != null && !t.isChecked && t.tileType == tileType)
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

		private Tile GetTile(int x, int y, int tileType)
		{
			try
			{
				Tile t = Tiles[x, y];
				if (t != null && t.tileType == tileType)
					return t;
			}
			catch (Exception)
			{
				return null;
			}

			return null;
		}

		private List<Tile> DrawCircle(Tile t, int r, int tileType)
		{
			List<Tile> lineTiles = new List<Tile>();
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
						{
							SetTile(drawTile, tileType);

							// changed properly, add to list to return
							if(drawTile.tileType == tileType)
								lineTiles.Add(drawTile);
						}
					}
				}
			}

			return lineTiles;
		}

		public void SetTile(Tile t, int tileType)
		{
			if (t.tileX == 0 || t.tileY == 0 || t.tileX == (width - 1) || t.tileY == (height - 1))
			{
				if (tileType != WorldHolder.WALL)
					return;
			}

			t.tileType = tileType;
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
