// Copyright (c) 2015, Lukas Honke
// ========================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.Base;
using UnityEngine;

namespace Assets.scripts.Mono.MapGenerator.Levels
{
	public class TestLevelData : AbstractLevelData
	{
		public MapRegion start, theBase, end;

		public TestLevelData(MapHolder holder) : base(holder)
		{
			type = MapType.Test;

			canHaveBase = false;
		}

		public override void Generate()
		{
			start = map.GenerateDungeonRegion(0, 2, 46, true, false, false, null, 1, 1);
			map.GenerateDungeonRegion(1, 2, 46, false, true, false, null, 1, 1);
			map.GenerateDungeonRegion(2, 2, 46, false, true, false, null, 2, 1);
			map.GenerateDungeonRegion(1, 0, 46, false, true, false, null, 1, 2);

			map.GenerateDungeonRegion(0, 0, 39, false, true, false, null, 1, 1);

			map.GenerateDungeonRegion(2, 0, 46, false, true, true, null, 2, 2);
		}

		public override void SpawnMonsters()
		{
			foreach (MapRoom room in map.GetMapRooms())
			{
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
			return 5;
		}

	    public override int GetMaxRegionsY()
	    {
	        return 5;
	    }

		public override Vector3 GetBaseLocation()
		{
			/*foreach (MapRoom room in map.GetMapRooms())
			{
				if (room.region.GetParentOrSelf().Equals(theBase))
				{
					Tile t = room.GetLargestSubRoom(false);
					return map.GetTileWorldPosition(t);
				}
			}*/
			return new Vector3();
		}
	}
}
