// Copyright (c) 2015, Lukas Honke
// ========================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Base;

namespace Assets.scripts.Mono.MapGenerator
{
	public class LevelParams
	{
		public MapType levelType;
		public int variant = 1;
		public int difficulty = 2; // 2 = medium
		public int mapLevel = 1;
		public int worldLevel = 1;

		public ShopData shop = null;

		public LevelParams(MapType t)
		{
			levelType = t;
		}
	}
}
