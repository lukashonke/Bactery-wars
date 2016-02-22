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
	public class LevelOneData : AbstractLevelData
	{
	    public MapRegion start, mid, end;

		public LevelOneData(MapHolder holder) : base(holder)
		{
			type = MapType.LevelOne;
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
		        }
                else if (room.region.GetParentOrSelf().Equals(mid))
                {
                }
                else if (room.region.GetParentOrSelf().Equals(end))
                {
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
			return 3;
		}

	    public override int GetMaxRegionsY()
	    {
	        return 2;
	    }
	}
}
