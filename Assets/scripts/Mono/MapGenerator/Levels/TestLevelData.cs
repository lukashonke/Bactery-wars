using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.Base;

namespace Assets.scripts.Mono.MapGenerator.Levels
{
	public class TestLevelData : AbstractLevelData
	{
		public TestLevelData(MapHolder holder) : base(holder)
		{
			type = MapType.Test;
		}

		public override void Generate()
		{
			map.GenerateDungeonRegion(0, 0, WorldHolder.instance.randomFillPercent, true, null); // 0 0
			map.GenerateDungeonRegion(0, 1, WorldHolder.instance.randomFillPercent, false, true, false, null, 1, 2); // 0, 2

			map.GenerateDungeonRegion(1, 1, WorldHolder.instance.randomFillPercent, false, true, true, null, 2, 1); // 0 1; 1 1
			map.GenerateDungeonRegion(1, 2, WorldHolder.instance.randomFillPercent, false, true, true, null, 2, 1); // 0 1; 1 1

			map.GenerateDungeonRegion(1, 0, WorldHolder.instance.randomFillPercent, false, true, false, null); // 0 0
			map.GenerateDungeonRegion(2, 0, WorldHolder.instance.randomFillPercent, false, true, true, null); // 0 0

			map.GenerateDungeonRegion(0, 3, WorldHolder.instance.randomFillPercent, false, true, true, null, 3, 1); // 0 0

			map.GenerateDungeonRegion(3, 2, WorldHolder.instance.randomFillPercent, false, true, true, null, 1, 1); // 0 0
		}

		public override void SpawnMonsters()
		{
			foreach (MapRoom room in map.GetMapRooms())
			{
				MonsterSpawnInfo info = new MonsterSpawnInfo(MonsterId.Lymfocyte_melee, map.GetTileWorldPosition(room.tiles[25]));
				info.SetRegion(room.region.GetParentOrSelf());

				map.AddMonsterToMap(info);
			}
		}

		public override int GetRegionWidth()
		{
			return 75;
		}

		public override int GetRegionHeight()
		{
			return 75;
		}

		public override int GetMaxRegionsX()
		{
			return 4;
		}

	    public override int GetMaxRegionsY()
	    {
	        return 4;
	    }
	}
}
