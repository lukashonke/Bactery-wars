using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
	}
}
