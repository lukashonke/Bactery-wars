﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.Base;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.scripts.Mono.MapGenerator.Levels
{
	public abstract class AbstractLevelData
	{
		public MapType type;
		protected MapHolder map;

		protected bool conquered = false;

		public AbstractLevelData(MapHolder holder)
		{
			map = holder;
		}

		public abstract void Generate();
		public abstract void SpawnMonsters();
		public abstract int GetRegionWidth();
		public abstract int GetRegionHeight();
		public abstract int GetMaxRegionsX();
	    public abstract int GetMaxRegionsY();

		public bool ChanceCheck(int chance)
		{
			int roll = Random.Range(0, 100);
			return roll < chance;
		}

		public MonsterSpawnInfo SpawnMonstersToRoom(MapRoom room, MonsterId id, MapRoom.RoomType type, int direction, int countRooms, int countMobsPerRoom, bool randomOffset = false, int level = 1, int chance = 100, bool exclude=true)
		{
			if (chance < 100 && !ChanceCheck(chance))
				return null;

			Tile[] rooms = room.GetSubRooms(type, direction, countRooms, exclude);

			MonsterSpawnInfo info = null;

			foreach (Tile t in rooms)
			{
				if (t == null)
					break;

				for (int i = 0; i < countMobsPerRoom; i++)
				{
					info = SpawnMonsterToRoom(room, id, t, randomOffset, level);
				}
			}

			return info;
		}

		public MonsterSpawnInfo SpawnMonsterToRoom(MapRoom room, MonsterId id, Tile roomTile, bool randomOffset=false, int level = 1, int chance = 100)
		{
			if (chance < 100 && !ChanceCheck(chance))
				return null;

			Vector3 pos = map.GetTileWorldPosition(roomTile);
			if (randomOffset)
				pos = Utils.GenerateRandomPositionAround(pos, 5, 2);

			MonsterSpawnInfo info = new MonsterSpawnInfo(map, id, pos);
			info.level = level;
			info.SetRegion(room.region.GetParentOrSelf());

			map.AddMonsterToMap(info);
			return info;
		}

		public MonsterSpawnInfo SpawnMonsterToRoom(MapRoom room, MonsterId id, Vector3 pos, bool randomOffset = false, int level = 1, int chance = 100)
		{
			if (chance < 100 && !ChanceCheck(chance))
				return null;

			if (randomOffset)
				pos = Utils.GenerateRandomPositionAround(pos, 5, 2);

			MonsterSpawnInfo info = new MonsterSpawnInfo(map, id, pos);
			info.level = level;
			info.SetRegion(room.region.GetParentOrSelf());

			map.AddMonsterToMap(info);
			return info;
		}

		public virtual void OnPlayerTeleportOut(Player player)
		{
			
		}

		public virtual void OnPlayerTeleportIn(Player player)
		{

		}

		public void OnConquered()
		{
			conquered = true;
		}
	}
}
