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
	public class LevelThreeData : AbstractLevelData
	{
	    public MapRegion start, mid, end;

		public LevelThreeData(MapHolder holder) : base(holder)
		{
			type = MapType.LevelThree;
		}

		public override void Generate()
		{
            start = map.GenerateDungeonRegion(0, 1, 40, true, false, false, null, 2, 1); // 0 0
			mid = map.GenerateDungeonRegion(2, 1, 48, false, true, false, null, 1, 1); // 0, 2
			end = map.GenerateDungeonRegion(3, 0, 45, false, true, true, null, 2, 2); // 0, 2
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
			return 5;
		}

	    public override int GetMaxRegionsY()
	    {
	        return 2;
	    }
	}
}
