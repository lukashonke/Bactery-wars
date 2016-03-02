using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.Base;
using Assets.scripts.Skills;
using Assets.scripts.Upgrade;
using Assets.scripts.Upgrade.Classic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.scripts.Mono.MapGenerator.Levels
{
	public class FindBossLevel : AbstractLevelData
	{
		public MapRegion[] regions;

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

		// 1=4x4 2=5x5
		private int variant;

		public FindBossLevel(MapHolder holder, LevelParams param) : base(holder)
		{
			type = MapType.FindBoss;
			tutorialLevel = true;
			variant = param.variant;
		}

		public override void Generate()
		{
			int width = 4;
			if (variant == 1)
			{
				width = 4;
			}
			else if (variant == 2)
			{
				width = 5;
			}

			regions = new MapRegion[width * width];

			int x = 0;
			int y = 0;

			for (int i = 0; i < regions.Length; i++)
			{
				if (i == 0)
				{
					regions[i] = map.GenerateDungeonRegion(x, y, 45, true, false, false, null, 1, 1); // 0 0
				}
				else if (i == regions.Length - 1)
				{
					regions[i] = map.GenerateDungeonRegion(x, y, 45, false, true, true, levelThreeSeeds, 1, 1); // 0, 2
				}
				else
				{
					regions[i] = map.GenerateDungeonRegion(x, y, 43, false, true, false, levelTwoSeeds, 1, 1); // 0, 2
				}

				Debug.Log(x + ", " + y);

				x++;
				if (x == width)
				{
					x = 0;
					y++;
				}
			}
		}

		public override void SpawnMonsters()
		{
            Utils.Timer.StartTimer("spawnmap");

		    foreach (MapRoom room in map.GetMapRooms()) // TODO throws NPE sometimes
		    {

		    }

            Utils.Timer.EndTimer("spawnmap");
		}

		public override int GetRegionWidth()
		{
			return 30;
		}

		public override int GetRegionHeight()
		{
			return 30;
		}

		public override int GetMaxRegionsX()
		{
			return 5;
		}

	    public override int GetMaxRegionsY()
	    {
		    return 5;
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
