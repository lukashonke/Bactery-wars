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

			canHaveBase = true;
		}

		public override void Generate()
		{
			start = map.GenerateDungeonRegion(0, 0, WorldHolder.instance.randomFillPercent, true, null, 2, 1); // 0 0

			theBase = map.GenerateDungeonRegion(2, 0, 40, false, true, false, null, 1, 1); // 0, 2

			end = map.GenerateDungeonRegion(3, 0, WorldHolder.instance.randomFillPercent, false, true, true, null, 2, 1); // 0 0
		}

		public override void SpawnMonsters()
		{
			foreach (MapRoom room in map.GetMapRooms())
			{
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

		public override int GetMaxRegionsX()
		{
			return 5;
		}

	    public override int GetMaxRegionsY()
	    {
	        return 1;
	    }

		public override Vector3 GetBaseLocation()
		{
			foreach (MapRoom room in map.GetMapRooms())
			{
				if (room.region.GetParentOrSelf().Equals(theBase))
				{
					Tile t = room.GetLargestSubRoom(false);
					return map.GetTileWorldPosition(t);
				}
			}
			return new Vector3();
		}
	}
}
