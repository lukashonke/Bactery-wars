using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.Base;
using Assets.scripts.Upgrade;
using UnityEngine;

namespace Assets.scripts.Mono.MapGenerator.Levels
{
	public class StartLevelData : AbstractLevelData
	{
	    public MapRegion start, mid, end;

		public StartLevelData(MapHolder holder) : base(holder)
		{
			type = MapType.StartClassic;
		}

		public override void Generate()
		{
			start = map.GenerateDungeonRegion(0, 0, 40, true, false, false, new[] { 118, -909, -167, 569, 949, -43, -696, 281, 434, -156, 987 }); // 0 0
			mid = map.GenerateDungeonRegion(1, 0, 47, false, true, false, new[] { -524, 862, 161, 460, -726, -167, -559, -528, -279, 743, -656 }); // 0, 2
			end = map.GenerateDungeonRegion(2, 0, 45, false, true, true, new[] { 768 }, 1, 2); // 0, 2
		}

		public override void SpawnMonsters()
		{
            Utils.Timer.StartTimer("spawnmap");
		    foreach (MapRoom room in map.GetMapRooms()) // TODO throws NPE sometimes
		    {
		        if (room.region.GetParentOrSelf().Equals(start))
		        {
			        /*MonsterSpawnInfo mob = SpawnMonsterToRoom(MonsterId.FourDiagShooterCell, room.GetLargestSubRoom(), room);
			        MonsterSpawnInfo shield = SpawnMonsterToRoom(MonsterId.ObstacleCell, room.GetLargestSubRoom(), room);
			        shield.master = mob;*/

					//SpawnMonsterToRoom(room, MonsterId.DementCell, room.GetLargestSubRoom(), 1).SetMustDieToProceed(false).AddDrop(100, UpgradeType.CLASSIC, 1, 2, 1);
		        }
                else if (room.region.GetParentOrSelf().Equals(mid))
                {
                    Tile[] rooms = room.GetSubRooms(MapRoom.RoomType.MEDIUM, 2, 6);

	                int count = 0;
	                foreach (Tile t in rooms)
	                {
		                if (t == null)
			                break;

						if(count < 3)
							SpawnMonsterToRoom(room, MonsterId.Lymfocyte_melee, t, 1);

			            count ++;
						SpawnMonsterToRoom(room, MonsterId.Lymfocyte_ranged, t, 1);
	                }
                }
                else if (room.region.GetParentOrSelf().Equals(end))
                {
	                Tile[] rooms = room.GetSubRooms(MapRoom.RoomType.VERYLARGE, MapRoom.DIRECTION_DOWN, 1);

					foreach (Tile t in rooms)
					{
						if (t == null)
							break;

						Vector3 leaderPos = map.GetTileWorldPosition(t);

						MonsterSpawnInfo info = SpawnMonsterToRoom(room, MonsterId.Neutrophyle_Patrol, leaderPos, 1);

						for (int i = 0; i < 2; i++)
						{
							Vector3 pos = Utils.GenerateRandomPositionAround(leaderPos, 4, 2);
							SpawnMonsterToRoom(room, MonsterId.Lymfocyte_melee, pos, 1);
						}

						for (int i = 0; i < 2; i++)
						{
							Vector3 pos = Utils.GenerateRandomPositionAround(leaderPos, 4, 2);
							SpawnMonsterToRoom(room, MonsterId.Lymfocyte_ranged, pos, 1);
						}
					}

	                rooms = room.GetSubRooms(MapRoom.RoomType.MEDIUM, MapRoom.DIRECTION_UP, 2);

					foreach (Tile t in rooms)
					{
						if (t == null)
							break;

						Vector3 leaderPos = map.GetTileWorldPosition(t);
						//SpawnMonsterToRoom(MonsterId.FloatingHelperCell, leaderPos, room, 1);

						for (int i = 0; i < 2; i++)
						{
							Vector3 pos = Utils.GenerateRandomPositionAround(leaderPos, 5, 1);
							SpawnMonsterToRoom(room, MonsterId.Lymfocyte_melee, pos, 1);
						}
					}

	                rooms = room.GetSubRooms(MapRoom.RoomType.MEDIUM, MapRoom.DIRECTION_CENTER, 6);

					foreach (Tile t in rooms)
					{
						if (t == null)
							break;

						t.SetColor(Tile.GREEN);

						Vector3 leaderPos = map.GetTileWorldPosition(t);

						MonsterSpawnInfo info = new MonsterSpawnInfo(map, MonsterId.Lymfocyte_ranged, leaderPos);
						info.SetRegion(room.region.GetParentOrSelf());
						map.AddMonsterToMap(info);
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
	        return 2;
	    }
	}
}
