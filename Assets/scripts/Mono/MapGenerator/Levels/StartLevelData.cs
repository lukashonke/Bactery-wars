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
		    foreach (MapRoom room in map.GetMapRooms())
		    {
		        if (room.region.GetParentOrSelf().Equals(start))
		        {

                    room.GetSubRoom(MapRoom.RoomType.MEDIUM, 2, 3);
		        }
                else if (room.region.GetParentOrSelf().Equals(mid))
                {

                    room.GetSubRoom(MapRoom.RoomType.MEDIUM, 2, 3);
                }
                else if (room.region.GetParentOrSelf().Equals(end))
                {
                    //TODO spawn mobs here
                    room.GetSubRoom(MapRoom.RoomType.MEDIUM, 2, 3);
                }
		    }
            Utils.Timer.EndTimer("spawnmap");

		    /*foreach (MapRoom room in map.GetMapRooms())
			{
				MonsterSpawnInfo info = new MonsterSpawnInfo(MonsterId.Lymfocyte_melee, map.GetTileWorldPosition(room.tiles[25]));
				info.SetRegion(room.region.GetParentOrSelf());

				map.AddMonsterToMap(info);
			}*/
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
