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
	public class LevelTwoData : AbstractLevelData
	{
	    public MapRegion start, mid, end;

		private int[] levelOneSeeds =
		{
			508, 787, -892, -19, 624, -64, -89, 616, 178, 662, -790, -82, -339, -340, 472, -825, -88, 62, -589, -721, -755, -734, 5, 313, 349, -664, 443, 575, -998, -155, -969, 153, 25, -490
		};

		private int[] levelTwoSeeds =
		{
			558, 189, 552, -558, -338, -316, -865, 603, -770, -698, 824, 979, -766, -946, 347, 232, -50, -520, -374, -182, 493, 867, -237, -727, 971, -249, -191, -78
		};

		private int[] levelThreeSeeds =
		{
			635, -225, 393, -862, -57, -57, 153, -9, 987, 784, -394, 287, -630, 383, 694, 326, 970, -818, -531, 66, -266, -746, -63, 35, -427, 724, -103, 638, 257,
		};

		public LevelTwoData(MapHolder holder) : base(holder)
		{
			type = MapType.LevelTwo;
		}

		public override void Generate()
		{
            start = map.GenerateDungeonRegion(0, 1, 42, true, false, false, levelOneSeeds); // 0 0
			mid = map.GenerateDungeonRegion(1, 0, 44, false, true, false, levelTwoSeeds, 2, 2); // 0, 2
			end = map.GenerateDungeonRegion(3, 0, 45, false, true, true, levelThreeSeeds, 1, 1); // 0, 2
		}

		public override void SpawnMonsters()
		{
            Utils.Timer.StartTimer("spawnmap");

		    foreach (MapRoom room in map.GetMapRooms()) // TODO throws NPE sometimes
		    {
		        if (room.region.GetParentOrSelf().Equals(start))
		        {
					foreach (Tile t in room.GetSubRooms(MapRoom.RoomType.MEDIUM, MapRoom.DIRECTION_RIGHT, 1))
					{
						if (t == null) break;

						SpawnMonsterToRoom(room, MonsterId.DurableMeleeCell, t, false, 1).AddHealDrop(50);
						SpawnMonsterToRoom(room, MonsterId.NonaggressiveHelperCell, t, true, 1, 50);
						SpawnMonsterToRoom(room, MonsterId.NonaggressiveHelperCell, t, true, 1, 50);
					}
				}
                else if (room.region.GetParentOrSelf().Equals(mid))
                {
					foreach (Tile t in room.GetSubRooms(MapRoom.RoomType.SMALL, MapRoom.DIRECTION_CENTER, 2))
					{
						if (t == null) break;

						SpawnMonsterToRoom(room, MonsterId.ChargerCell, t, false, 1).AddHealDrop(50);
					}

					foreach (Tile t in room.GetSubRooms(MapRoom.RoomType.LARGE, MapRoom.DIRECTION_CENTER, 6))
					{
						if (t == null) break;

						SpawnMonsterToRoom(room, MonsterId.Lymfocyte_melee, t, true, 1, 50);
					}

					foreach (Tile t in room.GetSubRooms(MapRoom.RoomType.LARGE, MapRoom.DIRECTION_CENTER, 4))
					{
						if (t == null) break;

						SpawnMonsterToRoom(room, MonsterId.NonaggressiveHelperCell, t, true, 1, 50);
					}
				}
                else if (room.region.GetParentOrSelf().Equals(end))
                {
					foreach (Tile t in room.GetSubRooms(MapRoom.RoomType.TINY, MapRoom.DIRECTION_CENTER, 1))
					{
						if (t == null) break;

						SpawnMonsterToRoom(room, MonsterId.DurableMeleeCell, t, false, 1).AddHealDrop(50);
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
			return 4;
		}

	    public override int GetMaxRegionsY()
	    {
	        return 2;
	    }

		public override void OnPlayerTeleportOut(Player player)
		{
			if (conquered)
			{
				player.UnlockSkill(2, true);
				conquered = false;

				Skill sk = (ActiveSkill)player.Skills.GetSkill(2);
				string skillName = sk.GetVisibleName();
				string desc = sk.GetDescription().ToLower();

				if (GameSession.className.Equals("CommonCold"))
					player.GetData().ui.ShowHelpWindow(Messages.ShowHelpWindow("second_skill_unlocked_commoncold", skillName, desc), 0);
				else
					player.GetData().ui.ShowHelpWindow(Messages.ShowHelpWindow("second_skill_unlocked", skillName, desc), 0);
			}
		}
	}
}
