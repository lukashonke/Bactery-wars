using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.Base;

namespace Assets.scripts.Mono.MapGenerator.Levels
{
	public class StartLevelData : AbstractLevelData
	{
		public StartLevelData(MapHolder holder) : base(holder)
		{
			type = MapType.StartClassic;
		}

		public override void Generate()
		{
			map.GenerateDungeonRegion(0, 0, 35, true, false, false, new[] { 344 }); // 0 0
			map.GenerateDungeonRegion(1, 0, 35, false, true, false, new[] { 344 }); // 0, 2
            map.GenerateDungeonRegion(2, 0, 35, false, true, true, new[] { 344 }, 1, 2); // 0, 2
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
