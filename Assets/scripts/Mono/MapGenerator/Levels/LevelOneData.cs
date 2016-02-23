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

		private int[] levelOneSeeds =
		{
			953, 953, 999, -69, -942, -656, 677, 872, 319, -737, 305, 522, 416, 865, -801, -286, 86,
			617, -48, 835
		};

		private int[] levelTwoSeeds =
		{
			-606, -77, 508, 227, -183, -389, -958, 634, 762, -788, 231, 948, 601, -444, -347, -538, 568, 957, -423
		};

		private int[] levelThree =
		{
			-631, -981, -641, -163, 436, 388, 422, -903, 593, -537, -364, 551, 967, -873, -578, 869, 127, 270, -224, 723, 941, 661, 686, 873, -654, -376, 47, -979
		};

		public LevelOneData(MapHolder holder) : base(holder)
		{
			type = MapType.LevelOne;
		}

		public override void Generate()
		{
			start = map.GenerateDungeonRegion(0, 0, 40, true, false, false, levelOneSeeds); // 0 0
			mid = map.GenerateDungeonRegion(1, 0, 43, false, true, false, levelTwoSeeds); // 0, 2
			end = map.GenerateDungeonRegion(2, 0, 45, false, true, true, levelThree, 1, 2); // 0, 2
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
