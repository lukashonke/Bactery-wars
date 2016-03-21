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

		private int[] startSeeds =
		{
			-405, -825, 224, 693, 351, 791, -15, 323, -609, -824, -512, -47, 24, 519, 65, 621, -895, -119,
		};

		private int[] secondSeeds =
		{
			187, -196, -877, 870, -59, 6, -890, 205, 895, 234, 968, -979, -455, -340, 694, -416, 871, 576, -681, 32, 9, 139, 700,
		};

		private int[] thirdSeeds =
		{
			-922, -603, 840, 648, -198, -824, -651, -273, 859, 326, -905, 121, -941, 471, 206, -219, 790, 404, 103, 408, -398, 88, 825, 694, -19, 408, -398, 88, 825, 694, -19, 450, 501, 983, -285, -656, 505, -475, -972, 199, 99,
		};

		private int[] fourthSeeds =
		{
			62, -619, -134, -451, 991, -119, 590, 739, -611, -869, -734, 699, -607, -254, -88, 920, -641, -550, -873, -554, 910, -897, -476, -717, 99, 323, -507, 411, -224,
		};

		private int[] bossSeeds =
		{
			703, 541, 511, 329, 614, 849, -916, 778, -274, 468, -469, -326, -478, 245, 681, 469, 223, -77, 166, -500,
		};

		public LevelBossRush(MapHolder holder, int mapLevel) : base(holder, mapLevel)
		{
			type = MapType.BossRush;
			tutorialLevel = true;
		}

		public override void Generate()
		{
			start = map.GenerateDungeonRegion(0, 1, 45, true, false, false, startSeeds, 1, 1); // 1
			second = map.GenerateDungeonRegion(1, 1, 45, false, false, false, secondSeeds, 1, 1); // 2
			thirdBig = map.GenerateDungeonRegion(2, 1, 45, false, false, false, thirdSeeds, 2, 1); // 3
			fourth = map.GenerateDungeonRegion(3, 0, 47, false, false, false, fourthSeeds, 1, 1); // 4
			fifthBig = map.GenerateDungeonRegion(4, 0, 45, false, false, false, thirdSeeds, 2, 1); // 5
			boss = map.GenerateDungeonRegion(6, 0, 43, false, true, false, bossSeeds, 1, 1); // B
			end = map.GenerateDungeonRegion(7, 0, 47, false, true, true, startSeeds, 1, 1); // 7
		}

		public override void SpawnMonsters()
		{
            Utils.Timer.StartTimer("spawnmap");

		    foreach (MapRoom room in map.GetMapRooms())
		    {
				if (room.region.GetParentOrSelf().Equals(boss)) // boss room
				{
					Tile t = room.GetLargestSubRoom(true);
					SpawnMonsterToRoom(room, MonsterId.SwarmerBoss, t, false, 1)
						.AddHealDrop(100, 10)
						.AddDrop(33, ItemType.CLASSIC, 2, 1)
						.AddDrop(100, ItemType.CLASSIC, 1, 1);
				}
				else if (room.region.GetParentOrSelf().Equals(start)) // start room - nothing interesting here
				{
					foreach (Tile t in room.GetSubRooms(MapRoom.RoomType.TINY, MapRoom.DIRECTION_RIGHT, 1))
					{
						if (t == null) break;

						SpawnMonsterToRoom(room, MonsterId.DementCell, t, true, 1).AddHealDrop(33);
					}
				}
				else if (room.region.GetParentOrSelf().Equals(second)) // second room: jump cells, armored cell and floating ranged
				{
					foreach (Tile t in room.GetSubRooms(MapRoom.RoomType.TINY, MapRoom.DIRECTION_CENTER, 2))
					{
						if (t == null) break;

						SpawnMonsterToRoom(room, MonsterId.JumpCell, t, true, mapLevel);
					}

					foreach (Tile t in room.GetSubRooms(MapRoom.RoomType.TINY, MapRoom.DIRECTION_CENTER, 1))
					{
						if (t == null) break;

						SpawnMonsterToRoom(room, MonsterId.ArmoredCell, t, true, mapLevel).AddHealDrop(33);
					}

					foreach (Tile t in room.GetSubRooms(MapRoom.RoomType.TINY, MapRoom.DIRECTION_CENTER, 2))
					{
						if (t == null) break;

						SpawnMonsterToRoom(room, MonsterId.FloatingHelperCell, t, true, mapLevel);
					}
				}
				else if (room.region.GetParentOrSelf().Equals(thirdBig))
				{
					foreach (Tile t in room.GetSubRooms(MapRoom.RoomType.SMALL, MapRoom.DIRECTION_CENTER_RIGHT, 4))
					{
						if (t == null) break;

						SpawnMonsterToRoom(room, MonsterId.SpiderCell, t, true, mapLevel);
					}

					foreach (Tile t in room.GetSubRooms(MapRoom.RoomType.SMALL, MapRoom.DIRECTION_CENTER, 1))
					{
						if (t == null) break;

						SpawnMonsterToRoom(room, MonsterId.MorphCellBig, t, true, mapLevel);
					}
				}
				else if (room.region.GetParentOrSelf().Equals(fourth))
				{
					foreach (Tile t in room.GetSubRooms(MapRoom.RoomType.TINY, MapRoom.DIRECTION_CENTER, 6))
					{
						if (t == null) break;

						SpawnMonsterToRoom(room, MonsterId.SuiciderCell, t, true, mapLevel);
					}
				}
				else if (room.region.GetParentOrSelf().Equals(fifthBig))
				{
					foreach (Tile t in room.GetSubRooms(MapRoom.RoomType.SMALL, MapRoom.DIRECTION_CENTER_LEFT, 1))
					{
						if (t == null) break;

						SpawnMonsterToRoom(room, MonsterId.TankCell, t, true, mapLevel);
					}

					foreach (Tile t in room.GetSubRooms(MapRoom.RoomType.SMALL, MapRoom.DIRECTION_CENTER, 1))
					{
						if (t == null) break;

						SpawnMonsterToRoom(room, MonsterId.MissileTurretCell, t, true, mapLevel);
					}

					foreach (Tile t in room.GetSubRooms(MapRoom.RoomType.SMALL, MapRoom.DIRECTION_CENTER_RIGHT, 1))
					{
						if (t == null) break;

						MonsterSpawnInfo patrol = SpawnMonsterToRoom(room, MonsterId.Neutrophyle_Patrol, t, true, mapLevel);
						SpawnMonsterToRoom(room, MonsterId.Lymfocyte_melee, t, true, mapLevel).master = patrol;
						SpawnMonsterToRoom(room, MonsterId.Lymfocyte_melee, t, true, mapLevel).master = patrol;
						SpawnMonsterToRoom(room, MonsterId.Lymfocyte_ranged, t, true, mapLevel).master = patrol;
						SpawnMonsterToRoom(room, MonsterId.Lymfocyte_ranged, t, true, mapLevel).master = patrol;
					}
				}
				else if (room.region.GetParentOrSelf().Equals(end))
				{
					foreach (Tile t in room.GetSubRooms(MapRoom.RoomType.TINY, MapRoom.DIRECTION_RIGHT, 2))
					{
						if (t == null) break;

						SpawnMonsterToRoom(room, MonsterId.DementCell, t, true, 1).AddHealDrop(33);
					}
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
