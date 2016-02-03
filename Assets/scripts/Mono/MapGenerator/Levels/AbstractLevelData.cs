using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.Base;

namespace Assets.scripts.Mono.MapGenerator.Levels
{
	public abstract class AbstractLevelData
	{
		public MapType type;
		protected MapHolder map;

		public AbstractLevelData(MapHolder holder)
		{
			map = holder;
		}

		public abstract void Generate();
		public abstract void SpawnMonsters();
		public abstract int GetRegionWidth();
		public abstract int GetRegionHeight();
		public abstract int GetMaxRegionsX();
	    public abstract int GetMaxRegionsY();

		public MonsterSpawnInfo SpawnMonsterToRoom(MonsterId id, Tile roomTile, MapRoom room, int level=1)
		{
			MonsterSpawnInfo info = new MonsterSpawnInfo(map, id, map.GetTileWorldPosition(roomTile));
			info.level = level;
			info.SetRegion(room.region.GetParentOrSelf());

			map.AddMonsterToMap(info);
			return info;
		}
	}
}
