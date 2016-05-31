using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.Base;
using Assets.scripts.Skills;
using Assets.scripts.Upgrade;
using UnityEngine;

namespace Assets.scripts.Mono.MapGenerator.Levels
{
	public class PresentationLevelData : AbstractLevelData
	{
		public MapRegion start, mid, mid2, mid3, end;

		private int[] levelOneSeeds =
		{
			-128, 175, 847, -540, 634, -928, -675, -213, 303, -188, -125, 411, 414, 229, -89, 441, -54, 808, 262,
		};

		private int[] levelTwoSeeds =
		{
			-59, -758, 886, 895, 805, 863, -256, 188, 976, -676, -802, 943, 814, 122, 949, 700, 256, -159, -648, 609,
		};

		private int[] levelThreeSeeds =
		{
			-170, 568, -819, -711, -176, -427, 547, 921, 245, 917, -963, 773, 359, 588,
		};

		public PresentationLevelData(MapHolder holder) : base(holder)
		{
			type = MapType.PresentationLevel;
			tutorialLevel = true;
		}

		public override void Generate()
		{
			start = map.GenerateDungeonRegion(0, 0, 40, true, false, false, null, 1, 1); // 0 0
			mid = map.GenerateDungeonRegion(1, 0, 47, false, true, false, null, 2, 1); // 0, 2
			mid2 = map.GenerateDungeonRegion(1, 1, 40, false, true, false, null, 1, 1); // 0, 2
			mid3 = map.GenerateDungeonRegion(3, 0, 46, false, true, false, null, 1, 2); // 0, 2
			end = map.GenerateDungeonRegion(4, 1, 41, false, true, true, null, 1, 1); // 0, 2
		}

		public override void SpawnMonsters()
		{
            Utils.Timer.StartTimer("spawnmap");

		    foreach (MapRoom room in map.GetMapRooms())
		    {
		        if (room.region.GetParentOrSelf().Equals(start))
		        {
					foreach (Tile t in room.GetSubRooms(MapRoom.RoomType.TINY, MapRoom.DIRECTION_CENTER, 1))
					{
						if (t == null) break;

						SpawnMonsterToRoom(room, MonsterId.DementCell.ToString(), t, false, 1);
						SpawnMonsterToRoom(room, MonsterId.DementCell.ToString(), t, true, 1);
						SpawnMonsterToRoom(room, MonsterId.DementCell.ToString(), t, true, 1);
						SpawnMonsterToRoom(room, MonsterId.DementCell.ToString(), t, true, 1).AddHealDrop(100, 2);
					}
				}
				else if (room.region.GetParentOrSelf().Equals(mid2))
				{
					Tile tt = room.GetLargestSubRoom(true);

					MonsterSpawnInfo info = SpawnMonsterToRoom(room, MonsterId.TurretCell.ToString(), tt, false, 3);
					SpawnMonsterToRoom(room, MonsterId.PassiveHelperCell.ToString(), tt, true).master = info;
					SpawnMonsterToRoom(room, MonsterId.PassiveHelperCell.ToString(), tt, true).master = info;
					SpawnMonsterToRoom(room, MonsterId.PassiveHelperCell.ToString(), tt, true).master = info;

					tt = room.GetLargestSubRoom(true);

					info = SpawnMonsterToRoom(room, MonsterId.TurretCell.ToString(), tt, false, 3);
					SpawnMonsterToRoom(room, MonsterId.PassiveHelperCell.ToString(), tt, true).master = info;
					SpawnMonsterToRoom(room, MonsterId.PassiveHelperCell.ToString(), tt, true).master = info;
					SpawnMonsterToRoom(room, MonsterId.PassiveHelperCell.ToString(), tt, true).master = info;
				}
				else if (room.region.GetParentOrSelf().Equals(mid3))
				{
					foreach (Tile t in room.GetSubRooms(MapRoom.RoomType.TINY, MapRoom.DIRECTION_DOWN, 6))
					{
						if (t == null) break;

						SpawnMonsterToRoom(room, MonsterId.Lymfocyte_melee.ToString(), t, false, 1).AddHealDrop(40, 3);
						SpawnMonsterToRoom(room, MonsterId.HealerCell.ToString(), t, false, 1).AddHealDrop(40, 3);
					}

					foreach (Tile t in room.GetSubRooms(MapRoom.RoomType.TINY, MapRoom.DIRECTION_UP, 6))
					{
						if (t == null) break;

						SpawnMonsterToRoom(room, MonsterId.SniperCell.ToString(), t, false, 1).AddHealDrop(40, 3);
					}

					foreach (Tile t in room.GetSubRooms(MapRoom.RoomType.TINY, MapRoom.DIRECTION_CENTER, 6))
					{
						if (t == null) break;

						SpawnMonsterToRoom(room, MonsterId.PusherCell.ToString(), t, false, 1).AddHealDrop(40, 3);
					}
				}
				else if (room.region.GetParentOrSelf().Equals(mid))
                {
					foreach (Tile t in room.GetSubRooms(MapRoom.RoomType.SMALL, MapRoom.DIRECTION_CENTER, 2))
					{
						if (t == null) break;

						SpawnMonsterToRoom(room, MonsterId.TankCell.ToString(), t, false, 1).AddHealDrop(100, 3);
					}

					// 8x charger in the middle
					foreach (Tile t in room.GetSubRooms(MapRoom.RoomType.SMALL, MapRoom.DIRECTION_CENTER, 2))
					{
						if (t == null) break;

						SpawnMonsterToRoom(room, MonsterId.ChargerCell.ToString(), t, false, 1, 100);
					}
					
                }
                else if (room.region.GetParentOrSelf().Equals(end))
                {
					foreach (Tile t in room.GetSubRooms(MapRoom.RoomType.TINY, MapRoom.DIRECTION_CENTER, 2))
					{
						if (t == null) break;

						SpawnMonsterToRoom(room, "Hop", t, true, 1, 100);
						SpawnMonsterToRoom(room, "Hop", t, true, 1, 100);
						SpawnMonsterToRoom(room, MonsterId.FourDiagShooterCell.ToString(), t, true, 1, 100);
						SpawnMonsterToRoom(room, MonsterId.FourDiagShooterCell.ToString(), t, true, 1, 100);
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
			return 40;
		}

		public override int GetMaxRegionsX()
		{
			return 5;
		}

	    public override int GetMaxRegionsY()
	    {
	        return 3;
	    }

		public override void OnPlayerTeleportIn(Player player)
		{
			player.GetData().ui.ShowHelpWindow(Messages.ShowHelpWindow("fifth_level_info"), 3f);
		}

		public override void OnPlayerTeleportOut(Player player)
		{
			if (conquered)
			{
				player.GetData().ui.ShowHelpWindow(Messages.ShowHelpWindow("tutorial_finished"), 0f);
			}
		}
	}
}
