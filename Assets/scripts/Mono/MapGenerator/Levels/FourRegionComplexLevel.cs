﻿using System;
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
using Random = UnityEngine.Random;

namespace Assets.scripts.Mono.MapGenerator.Levels
{
	public class FourRegionComplexLevel : AbstractLevelData
	{
		public MapRegion start, second, third, end;

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

		// 1=small 2=complex cave
		private int variant;

		public FourRegionComplexLevel(MapHolder holder, LevelParams param) : base(holder)
		{
			type = MapType.FourRegion;
			tutorialLevel = true;
			variant = param.variant;
		}

		public override void Generate()
		{
			if (variant == 1)
			{
				start = map.GenerateDungeonRegion(0, 0, 45, true, false, false, null, 1, 1); // 0 0
				second = map.GenerateDungeonRegion(1, 0, 43, false, true, false, levelTwoSeeds, 1, 1); // 0, 2
				third = map.GenerateDungeonRegion(0, 1, 43, false, true, false, levelTwoSeeds, 1, 1); // 0, 2
				end = map.GenerateDungeonRegion(1, 1, 45, false, true, true, levelThreeSeeds, 1, 1); // 0, 2
			}
			else if (variant == 2)
			{
				start = map.GenerateDungeonRegion(0, 0, 45, true, false, false, null, 1, 1); // 0 0
				second = map.GenerateDungeonRegion(1, 0, 43, false, true, false, levelTwoSeeds, 1, 1); // 0, 2
				third = map.GenerateDungeonRegion(0, 1, 43, false, true, false, levelTwoSeeds, 1, 1); // 0, 2
				end = map.GenerateDungeonRegion(1, 1, 45, false, true, true, levelThreeSeeds, 1, 1); // 0, 2
			}
		}

		public override void SpawnMonsters()
		{
            Utils.Timer.StartTimer("spawnmap");

		    foreach (MapRoom room in map.GetMapRooms()) // TODO throws NPE sometimes
		    {
		        if (room.region.GetParentOrSelf().Equals(start))
		        {

		        }
                else if (room.region.GetParentOrSelf().Equals(second))
                {
					Tile t = room.GetLargestSubRoom(true);
	                SpawnMonsterToRoom(room, MonsterId.TankSpreadshooter, t, false, 1)
		                .AddHealDrop(100, 10);
                }
				else if (room.region.GetParentOrSelf().Equals(third))
				{

				}
				else if (room.region.GetParentOrSelf().Equals(end))
                {

                }
		    }

            Utils.Timer.EndTimer("spawnmap");
		}

		public override int GetRegionWidth()
		{
			if(variant == 1)
				return 40;
			else if (variant == 2)
				return 60;
			else return 60;
		}

		public override int GetRegionHeight()
		{
			if (variant == 1)
				return 40;
			else if (variant == 2)
				return 60;
			else return 60;
		}

		public override int GetMaxRegionsX()
		{
			return 2;
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