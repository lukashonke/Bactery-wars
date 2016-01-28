using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.Base;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.scripts.Mono.MapGenerator.Levels
{
	public class SecondLevelData : AbstractLevelData
	{
		public MapRegion start, room1, room2, miniboss, end;

		public SecondLevelData(MapHolder holder) : base(holder)
		{
			type = MapType.SecondLevel;
		}

		public override void Generate()
		{
			switch (Random.Range(1, 3))
			{
				case 1:
					start = map.GenerateDungeonRegion(0, 1, 40, true, false, false, null); // vlevo uprostred
					room1 = map.GenerateDungeonRegion(1, 1, 40, false, true, false, null); // uprostred
					miniboss = map.GenerateDungeonRegion(1, 0, 40, false, true, false, null); // dole uprostred

					room2 = map.GenerateDungeonRegion(1, 2, 40, false, true, false, null); // nahore uprostred
					end = map.GenerateDungeonRegion(2, 2, 40, false, true, true, null); // nahore vpravo
					break;
				case 2:
					start = map.GenerateDungeonRegion(0, 1, 40, true, false, false, null); // vlevo uprostred
					room1 = map.GenerateDungeonRegion(1, 1, 40, false, true, false, null); // uprostred
					miniboss = map.GenerateDungeonRegion(1, 2, 40, false, true, false, null); // nahore uprostred

					room2 = map.GenerateDungeonRegion(1, 0, 40, false, true, false, null); // dole uprostred
					end = map.GenerateDungeonRegion(2, 0, 40, false, true, true, null); // dole vpravo
					break;
			}
		}

		public override void SpawnMonsters()
		{
            Utils.Timer.StartTimer("spawnmap");
		    foreach (MapRoom room in map.GetMapRooms()) // TODO throws NPE sometimes
		    {
		        if (room.region.GetParentOrSelf().Equals(start))
		        {
                    room.GetSubRooms(MapRoom.RoomType.MEDIUM, 2, 3);
		        }
                else if (room.region.GetParentOrSelf().Equals(room1))
                {
                    Tile[] rooms = room.GetSubRooms(MapRoom.RoomType.MEDIUM, 2, 6);
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
			return -1;
		}

	    public override int GetMaxRegionsY()
	    {
	        return -1;
	    }
	}
}
