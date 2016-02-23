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

		private int[] levelOneSeeds =
		{
			-138, 127, 759, 479, 394, -304, -283, 837, -596, 400, -757, 481, -191, -364, -637, 967, -893, 220, 993, -916, 610, 780, -76, -106, 236, 59, 949
		};

		private int[] levelTwoSeeds =
		{
			573, -653, -630, -976, -854, -520, -498, -124, 308, 572, -364, 471, -958, -524, 531, -13, 581, 196,  -109,
		};

		private int[] levelThreeSeeds =
		{
			-867, -475, 10, -755, 363, -754, 91, -902, -502, 224, -467, 702, 694, 318, 10, 775, 42, 4, -978, 243, -988, -685, -237, 282, 125,
		};

		public LevelThreeData(MapHolder holder) : base(holder)
		{
			type = MapType.LevelThree;
		}

		public override void Generate()
		{
            start = map.GenerateDungeonRegion(0, 1, 43, true, false, false, levelOneSeeds, 2, 1); // 0 0
			mid = map.GenerateDungeonRegion(2, 1, 48, false, true, false, levelTwoSeeds, 1, 1); // 0, 2
			end = map.GenerateDungeonRegion(3, 0, 46, false, true, true, levelThreeSeeds, 2, 2); // 0, 2
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
