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

		private static int[] startSeeds = { -564, -803, -317, 820, 592, 712, 746, 665, -888, 660, -337 };
		private static int[] room1Seeds = { 624, 675, 946, -213, 703, 840, -180, 166, 34, 688, -50, -247, -616, -692, 561, -580, 301 };
		private static int[] room2Seeds = { -323, -378, -856, 432, 352, 54, -111, 214, 964, -48, 606, -232, 765 };
		private static int[] minibossSeeds = { 998, -454, 940, 547, -372, 462, -771, -327, -878, 764, 852, -575 };

		public SecondLevelData(MapHolder holder) : base(holder)
		{
			type = MapType.SecondLevel;
		}

		public override void Generate()
		{
			switch (Random.Range(1, 3))
			{
				case 1:
					start = map.GenerateDungeonRegion(0, 1, 47, true, false, false, startSeeds); // vlevo uprostred
					room1 = map.GenerateDungeonRegion(1, 1, 43, false, true, false, room1Seeds); // uprostred
					miniboss = map.GenerateDungeonRegion(1, 0, 40, false, true, false, minibossSeeds, 2, 1); // dole uprostred

					room2 = map.GenerateDungeonRegion(1, 2, 43, false, true, true, room2Seeds, 2, 1); // nahore uprostred
					break;
				case 2:
					start = map.GenerateDungeonRegion(0, 1, 47, true, false, false, startSeeds); // vlevo uprostred
					room1 = map.GenerateDungeonRegion(1, 1, 43, false, true, false, room1Seeds); // uprostred
					miniboss = map.GenerateDungeonRegion(1, 2, 40, false, true, false, minibossSeeds, 2, 1); // nahore uprostred

					room2 = map.GenerateDungeonRegion(1, 0, 43, false, true, true, room2Seeds, 2, 1); // dole uprostred
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
			        SpawnMonstersToRoom(room, MonsterId.FloatingBasicCell.ToString(), MapRoom.RoomType.MEDIUM, MapRoom.DIRECTION_RIGHT, 1, 1);
					SpawnMonstersToRoom(room, MonsterId.PassiveHelperCell.ToString(), MapRoom.RoomType.MEDIUM, MapRoom.DIRECTION_RIGHT, 2, 1);
		        }
		        else if (room.region.GetParentOrSelf().Equals(room1))
		        {
			        SpawnMonstersToRoom(room, MonsterId.BasicCell.ToString(), MapRoom.RoomType.SMALL, MapRoom.DIRECTION_CENTER, 6, 1);
		        }
				else if (room.region.GetParentOrSelf().Equals(room2))
				{
					//SpawnMonstersToRoom(room, MonsterId.HelperCell, MapRoom.RoomType.TINY, MapRoom.DIRECTION_LEFT, 5, 1, 1, 100, false);

					SpawnMonstersToRoom(room, MonsterId.TurretCell.ToString(), MapRoom.RoomType.VERYLARGE, MapRoom.DIRECTION_CENTER, 1, 1);
					SpawnMonstersToRoom(room, MonsterId.FourDiagShooterCell.ToString(), MapRoom.RoomType.MEDIUM, MapRoom.DIRECTION_LEFT, 2, 1);

					Tile t = room.GetTileWithSpaceAround(5, MapRoom.DIRECTION_RIGHT);
					MonsterSpawnInfo patrol = SpawnMonsterToRoom(room, MonsterId.Neutrophyle_Patrol.ToString(), t);
					//MonsterSpawnInfo patrol = SpawnMonstersToRoom(room, MonsterId.Neutrophyle_Patrol, MapRoom.RoomType.MEDIUM, MapRoom.DIRECTION_RIGHT, 1, 1);
					if (patrol != null)
					{
						try
						{
							SpawnMonsterToRoom(room, MonsterId.PassiveHelperCell.ToString(), Utils.GenerateRandomPositionAround(patrol.SpawnPos, 5, 3)).master = patrol;
							SpawnMonsterToRoom(room, MonsterId.PassiveHelperCell.ToString(), Utils.GenerateRandomPositionAround(patrol.SpawnPos, 5, 3)).master = patrol;
							SpawnMonsterToRoom(room, MonsterId.PassiveHelperCell.ToString(), Utils.GenerateRandomPositionAround(patrol.SpawnPos, 5, 3)).master = patrol;

							SpawnMonsterToRoom(room, MonsterId.Lymfocyte_melee.ToString(), Utils.GenerateRandomPositionAround(patrol.SpawnPos, 5, 3));
							SpawnMonsterToRoom(room, MonsterId.Lymfocyte_melee.ToString(), Utils.GenerateRandomPositionAround(patrol.SpawnPos, 5, 3));
							SpawnMonsterToRoom(room, MonsterId.Lymfocyte_melee.ToString(), Utils.GenerateRandomPositionAround(patrol.SpawnPos, 5, 3));
							SpawnMonsterToRoom(room, MonsterId.Lymfocyte_melee.ToString(), Utils.GenerateRandomPositionAround(patrol.SpawnPos, 5, 3));
						}
						catch (Exception)
						{
						}
					}
				}
				else if (room.region.GetParentOrSelf().Equals(miniboss))
				{
					Tile largestRoom = room.GetLargestSubRoom();

					//SpawnMonsterToRoom(room, MonsterId.TankSpreadshooter, largestRoom, 1);
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
