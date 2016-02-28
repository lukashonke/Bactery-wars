using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.Base;
using Assets.scripts.Skills;
using Assets.scripts.Upgrade;
using Assets.scripts.Upgrade.Classic;
using UnityEngine;

namespace Assets.scripts.Mono.MapGenerator.Levels
{
	public class LevelBossRush : AbstractLevelData
	{
	    public MapRegion start, second, thirdBig, fourth, fifthBig, boss, end;

		private int[] levelOneSeeds =
		{
			993, 718, -703, -221, 186, 633, -548, 882, -784, 389, 207, -358, 567, -149,
		};

		private int[] levelTwoSeeds =
		{
			217, -760, 97, -958, 579, 216, -217, -262, 169, -387, -72, 616, 659, 877, -984, -851, 151, 702,
		};

		private int[] levelThreeSeeds =
		{
			494, -287, -756, -556, -784, -242, -850, 635,

		};

		public LevelBossRush(MapHolder holder) : base(holder)
		{
			type = MapType.BossRush;
			tutorialLevel = true;
		}

		public override void Generate()
		{
			start = map.GenerateDungeonRegion(0, 1, 45, true, false, false, null, 1, 1); // 1
			second = map.GenerateDungeonRegion(1, 1, 45, false, false, false, null, 1, 1); // 2
			thirdBig = map.GenerateDungeonRegion(2, 1, 45, false, false, false, null, 2, 1); // 3
			fourth = map.GenerateDungeonRegion(3, 0, 45, false, false, false, null, 1, 1); // 4
			fifthBig = map.GenerateDungeonRegion(4, 0, 45, false, false, false, null, 2, 1); // 5
			boss = map.GenerateDungeonRegion(6, 0, 43, false, true, false, null, 1, 1); // B
			end = map.GenerateDungeonRegion(7, 0, 45, false, true, true, null, 1, 1); // 7
		}

		public override void SpawnMonsters()
		{
            Utils.Timer.StartTimer("spawnmap");

		    foreach (MapRoom room in map.GetMapRooms())
		    {
				if (room.region.GetParentOrSelf().Equals(boss))
				{
					Tile t = room.GetLargestSubRoom(true);
					SpawnMonsterToRoom(room, MonsterId.SwarmerBoss, t, false, 1)
						.AddHealDrop(100, 10)
						.AddDrop(33, UpgradeType.CLASSIC, 2, 1)
						.AddDrop(100, UpgradeType.CLASSIC, 1, 1);
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
			return 30;
		}

		public override int GetMaxRegionsX()
		{
			return 8;
		}

	    public override int GetMaxRegionsY()
	    {
	        return 2;
	    }

		public override void OnPlayerTeleportIn(Player player)
		{
		}

		public override void OnPlayerTeleportOut(Player player)
		{
			if (conquered)
			{

			}
		}
	}
}
