using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.scripts.Mono.MapGenerator
{
	public class LevelParams
	{
		public MapType levelType;
		public int variant = 1;
		public int mapLevel = 1;
		public int worldLevel = 1;

		public LevelParams(MapType t)
		{
			levelType = t;
		}
	}
}
