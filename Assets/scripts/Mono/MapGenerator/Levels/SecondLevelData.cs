using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.Base;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.scripts.Mono.MapGenerator.Levels
{
	public class SecondLevelData : AbstractLevelData
	{
		public MapRegion start, room1, room2, miniboss, end;

		public SecondLevelData(MapHolder holder) : base(holder)
		{
			type = MapType.SecondLevel;
		}

		public override void Generate()
		{
			switch (Random.Range(1, 3))
			{
				case 1:
					start = map.GenerateDungeonRegion(0, 1, 40, true, false, false, null); // vlevo uprostred
					room1 = map.GenerateDungeonRegion(1, 1, 45, false, true, false, null); // uprostred
					miniboss = map.GenerateDungeonRegion(1, 0, 40, false, true, false, null); // dole uprostred

					room2 = map.GenerateDungeonRegion(1, 2, 45, false, true, false, null); // nahore uprostred
					end = map.GenerateDungeonRegion(2, 2, 40, false, true, true, null); // nahore vpravo
					break;
				case 2:
					start = map.GenerateDungeonRegion(0, 1, 40, true, false, false, null); // vlevo uprostred
					room1 = map.GenerateDungeonRegion(1, 1, 45, false, true, false, null); // uprostred
					miniboss = map.GenerateDungeonRegion(1, 2, 40, false, true, false, null); // nahore uprostred

					room2 = map.GenerateDungeonRegion(1, 0, 45, false, true, false, null); // dole uprostred
					end = map.GenerateDungeonRegion(2, 0, 40, false, true, true, null); // dole vpravo
					break;
			}
		}

		public override void SpawnMonsters()
		{
            Utils.Timer.StartTimer("spawnmap");
		    foreach (MapRoom room in map.GetMapRooms()) 
		    {
		        if (room.region.GetParentOrSelf().Equals(start))
		        {
					Tile[] rooms = room.GetSubRooms(MapRoom.RoomType.LARGE, MapRoom.DIRECTION_CENTER, 4);

			        foreach (Tile t in rooms)
			        {
				        if (t == null)
					        break;

				        SpawnMonsterToRoom(MonsterId.FourDiagShooterCell, t, room);
			        }
		        }
		        else if (room.region.GetParentOrSelf().Equals(room1))
		        {
			        Tile[] rooms = room.GetSubRooms(MapRoom.RoomType.MEDIUM, 2, 6);

					foreach (Tile t in rooms)
					{
						if (t == null)
							break;

						SpawnMonsterToRoom(MonsterId.FourDiagShooterCell, t, room);
					}
		        }
				else if (room.region.GetParentOrSelf().Equals(room2))
				{
					Tile[] rooms = room.GetSubRooms(MapRoom.RoomType.MEDIUM, 2, 6);

					foreach (Tile t in rooms)
					{
						if (t == null)
							break;
						SpawnMonsterToRoom(MonsterId.FourDiagShooterCell, t, room);
					}
				}
				else if (room.region.GetParentOrSelf().Equals(miniboss))
				{
					Tile largestRoom = room.GetLargestSubRoom(true);

					SpawnMonsterToRoom(MonsterId.TankSpreadshooter, largestRoom, room, 1);
				}
				else if (room.region.GetParentOrSelf().Equals(end))
				{
					Tile[] rooms = room.GetSubRooms(MapRoom.RoomType.MEDIUM, 2, 6);

					foreach (Tile t in rooms)
					{
						if (t == null)
							break;
						SpawnMonsterToRoom(MonsterId.FourDiagShooterCell, t, room);
					}
				}
		    }

            Utils.Timer.EndTimer("spawnmap");
		}

		public override int GetRegionWidth()
		{
			return 40;
		}

		public override int GetRegionHeight()
		{
			return 40;
		}

		public override int GetMaxRegionsX()
		{
			return -1;
		}

	    public override int GetMaxRegionsY()
	    {
	        return -1;
	    }
	}
}
