// Copyright (c) 2015, Lukas Honke
// ========================
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
using Random = UnityEngine.Random;

namespace Assets.scripts.Mono.MapGenerator.Levels
{
	public class ArenaLevelData : AbstractLevelData
	{
	    public MapRegion start, arena, end;

		public ArenaLevelData(MapHolder holder)
			: base(holder)
		{
			type = MapType.LevelOne;
			tutorialLevel = true;
		}

		public override void Generate()
		{
			shopData = new ShopData();

			shopData.GenerateRandomShopItems(1, 1);

			start = map.GenerateDungeonRegion(0, 0, 43, true, false, false, null, 2, 2); // 0 0
			end = map.GenerateDungeonRegion(2, 0, 45, false, true, true, null); // 0, 2
		}

		public override void SpawnMonsters()
		{
            Utils.Timer.StartTimer("spawnmap");

		    foreach (MapRoom room in map.GetMapRooms())
		    {
		        
		    }

            Utils.Timer.EndTimer("spawnmap");
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
			return 3;
		}

	    public override int GetMaxRegionsY()
	    {
	        return 2;
	    }

		public override void OnPlayerTeleportOut(Player player)
		{
			if (conquered)
			{
			}
		}
	}
}
