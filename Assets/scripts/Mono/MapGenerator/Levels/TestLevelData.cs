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
			map.GenerateDungeonRegion(0, 0, 35, true, new[] { 344 }); // 0 0
			map.GenerateDungeonRegion(0, 1, 35, false, true, false, new[] { 344 }, 1, 2); // 0, 2

			map.GenerateDungeonRegion(1, 1, WorldHolder.instance.randomFillPercent, false, true, true, new[] { 290 }, 2, 1); // 0 1; 1 1
			map.GenerateDungeonRegion(1, 2, WorldHolder.instance.randomFillPercent, false, true, true, new[] { 290 }, 2, 1); // 0 1; 1 1

			map.GenerateDungeonRegion(1, 0, 35, false, true, false, new[] { 344 }); // 0 0
			map.GenerateDungeonRegion(2, 0, 35, false, true, true, new[] { 344 }); // 0 0
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
			return 50;
		}

		public override int GetRegionHeight()
		{
			return 50;
		}

		public override int GetMaxRegions()
		{
			return 4;
		}
	}
}
