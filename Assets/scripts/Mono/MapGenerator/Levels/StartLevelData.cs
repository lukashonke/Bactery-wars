using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.Base;
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
			mid = map.GenerateDungeonRegion(1, 0, 47, false, true, false, new [] {-524, 862, 161, 460, -726, -167, -559, -528, -279, 743, -656}); // 0, 2
            end = map.GenerateDungeonRegion(2, 0, 45, false, true, true, new [] {645, 262, 900, -496, -739, -798, 892, -804, 765, 768, 690, -552, -437, 782}, 1, 2); // 0, 2
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

			        SpawnMonsterToRoom(MonsterId.DementCell, room.GetLargestSubRoom(), room);
			        //SpawnMonsterToRoom(MonsterId.DementCell, room.GetLargestSubRoom(), room);
			        //SpawnMonsterToRoom(MonsterId.DementCell, room.GetLargestSubRoom(), room);
		        }
                else if (room.region.GetParentOrSelf().Equals(mid))
                {
                    Tile[] rooms = room.GetSubRooms(MapRoom.RoomType.MEDIUM, 2, 6);

	                foreach (Tile t in rooms)
	                {
		                if (t == null)
			                break;

		                try
		                {
							MonsterSpawnInfo info = new MonsterSpawnInfo(map, MonsterId.Lymfocyte_ranged, map.GetTileWorldPosition(t));
							info.SetRegion(room.region.GetParentOrSelf());

							map.AddMonsterToMap(info);
		                }
		                catch (Exception)
		                {
			                Debug.LogError("null world pos for tile " + t.tileX + ", " + t.tileY + " " + t.tileType);
		                }
						
	                }
                }
                else if (room.region.GetParentOrSelf().Equals(end))
                {
					Tile[] rooms = room.GetSubRooms(MapRoom.RoomType.VERYLARGE, 2, 1);

					foreach (Tile t in rooms)
					{
						if (t == null)
							break;

						Vector3 leaderPos = map.GetTileWorldPosition(t);

						MonsterSpawnInfo info = new MonsterSpawnInfo(map, MonsterId.Neutrophyle_Patrol, leaderPos);
						info.SetRegion(room.region.GetParentOrSelf());
						map.AddMonsterToMap(info);

						for (int i = 0; i < 2; i++)
						{
							Vector3 pos = Utils.GenerateRandomPositionAround(leaderPos, 5, 1);
							info = new MonsterSpawnInfo(map, MonsterId.Lymfocyte_melee, pos);
							info.SetRegion(room.region.GetParentOrSelf());
							map.AddMonsterToMap(info);
						}

						for (int i = 0; i < 2; i++)
						{
							Vector3 pos = Utils.GenerateRandomPositionAround(leaderPos, 5, 1);
							info = new MonsterSpawnInfo(map, MonsterId.Lymfocyte_ranged, pos);
							info.SetRegion(room.region.GetParentOrSelf());
							map.AddMonsterToMap(info);
						}
					}

					rooms = room.GetSubRooms(MapRoom.RoomType.MEDIUM, 1, 2);

					foreach (Tile t in rooms)
					{
						if (t == null)
							break;

						Vector3 leaderPos = map.GetTileWorldPosition(t);

						MonsterSpawnInfo info = new MonsterSpawnInfo(map, MonsterId.Neutrophyle_Patrol, leaderPos);
						info.SetRegion(room.region.GetParentOrSelf());
						map.AddMonsterToMap(info);
					}

					rooms = room.GetSubRooms(MapRoom.RoomType.MEDIUM, MapRoom.DIRECTION_CENTER, 6, true);

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
