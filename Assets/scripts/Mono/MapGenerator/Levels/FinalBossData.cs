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
	public class FinalBossData : AbstractLevelData
	{
		private int difficulty;

		private int regionsX = 4;
		private int regionsY = 4;

		private MapVariant variant;

		public enum MapVariant
		{
			Wide32,

		}

		public FinalBossData(MapHolder holder, LevelParams param)
			: base(holder)
		{
			type = MapType.FinalBoss;
			tutorialLevel = false;
			difficulty = param.difficulty;

			try
			{
				if(param.typeParameter != null)
					variant = (MapVariant) param.typeParameter;
				else 
					variant = MapVariant.Wide32;
			}
			catch (Exception e)
			{
				Debug.LogError(e.StackTrace);
				variant = MapVariant.Wide32;
			}


			switch (variant)
			{
				case MapVariant.Wide32:
					regionsX = 4;
					regionsY = 3;
					break;
			}
		}

		private MapRegion start;
		private MapRegion end;
		private MapRegion boss;

		private const int bossFillPercent = 42;
		private const int startFillPercent = 46;
		private const int endFillPercent = 46;

		public override void Generate()
		{
			switch (variant)
			{
				case MapVariant.Wide32:
					start = map.GenerateDungeonRegion(0, 1, startFillPercent, true, false, false, null, 1, 1);
					
					boss = map.GenerateDungeonRegion(1, 0, bossFillPercent, false, true, false, null, 2, 3);

					end = map.GenerateDungeonRegion(3, 1, endFillPercent, false, false, true, null, 1, 1);

					break;
			}
		}

		public override void SpawnMonsters()
		{
			Utils.Timer.StartTimer("spawnmap");

			foreach (MapRoom room in map.GetMapRooms())
			{
				if (room.region.GetParentOrSelf().Equals(start))
				{
					MonsterGenerator.Instance.GenerateGenericEnemyGroup(room, this, MonsterGenerator.RoomType.START_ROOM, difficulty, room.region.GetParentOrSelf());
				}
				else if (room.region.GetParentOrSelf().Equals(boss))
				{
					MonsterGenerator.Instance.GenerateGenericEnemyGroup(room, this, MonsterGenerator.RoomType.BOSS_ROOM, difficulty, room.region.GetParentOrSelf());
				}
				else if (room.region.GetParentOrSelf().Equals(end))
				{
					MonsterGenerator.Instance.GenerateGenericEnemyGroup(room, this, MonsterGenerator.RoomType.END_ROOM, difficulty, room.region.GetParentOrSelf());
				}
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
			return regionsX;
		}

		public override int GetMaxRegionsY()
		{
			return regionsY;
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
