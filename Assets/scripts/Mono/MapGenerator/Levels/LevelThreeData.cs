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
	public class LevelThreeData : AbstractLevelData
	{
	    public MapRegion start, mid, end;

		private int[] levelOneSeeds =
		{
			-138, 127, 759, 479, 394, -304, -283, 837, -596, 400, -757, 481, -191, -364, -637, 967, -893, 220, 993, -916, 610, 780, -76, -106, 236, 59, 949
		};

		private int[] levelTwoSeeds =
		{
			573, -653, -630, -976, -854, -520, -498, -124, 308, 572, -364, 471, -958, -524, 531, -13, 581, 196,  -109,
		};

		private int[] levelThreeSeeds =
		{
			-867, -475, 10, -755, 363, -754, 91, -902, -502, 224, -467, 702, 694, 318, 10, 775, 42, 4, -978, 243, -988, -685, -237, 282, 125,
			417, 522, -606, 195, 498, 689, -306, -223, -313, -532, 557, -238, -372, -473,
		};

		public LevelThreeData(MapHolder holder) : base(holder)
		{
			type = MapType.LevelThree;
			tutorialLevel = true;
		}

		public override void Generate()
		{
            start = map.GenerateDungeonRegion(0, 1, 43, true, false, false, levelOneSeeds, 2, 1); // 0 0
			mid = map.GenerateDungeonRegion(2, 1, 48, false, true, false, levelTwoSeeds, 1, 1); // 0, 2
			end = map.GenerateDungeonRegion(3, 0, 46, false, true, true, levelThreeSeeds, 2, 2); // 0, 2
		}

		public override void SpawnMonsters()
		{
            Utils.Timer.StartTimer("spawnmap");

		    foreach (MapRoom room in map.GetMapRooms()) // TODO throws NPE sometimes
		    {
		        if (room.region.GetParentOrSelf().Equals(start))
		        {
					foreach (Tile t in room.GetSubRooms(MapRoom.RoomType.LARGE, MapRoom.DIRECTION_LEFT, 4))
					{
						if (t == null) break;

						SpawnMonsterToRoom(room, MonsterId.NonaggressiveHelperCell, t, false, 1, 66);
					}

					foreach (Tile t in room.GetSubRooms(MapRoom.RoomType.MEDIUM, MapRoom.DIRECTION_CENTER, 2))
					{
						if (t == null) break;

						SpawnMonsterToRoom(room, MonsterId.ChargerCell, t, false, 1);
					}

					foreach (Tile t in room.GetSubRooms(MapRoom.RoomType.TINY, MapRoom.DIRECTION_RIGHT, 1))
					{
						if (t == null) break;

						SpawnMonsterToRoom(room, MonsterId.SmallTankCell, t, false, 1);
						SpawnMonsterToRoom(room, MonsterId.SmallTankCell, t, true, 1).AddHealDrop(100, 2);
					}
				}
                else if (room.region.GetParentOrSelf().Equals(mid))
                {
					foreach (Tile t in room.GetSubRooms(MapRoom.RoomType.TINY, MapRoom.DIRECTION_CENTER, 3))
					{
						if (t == null) break;

						SpawnMonsterToRoom(room, MonsterId.NonaggressiveHelperCell, t, false, 1);
						SpawnMonsterToRoom(room, MonsterId.NonaggressiveHelperCell, t, false, 1);
						SpawnMonsterToRoom(room, MonsterId.NonaggressiveHelperCell, t, true, 1);
					}
				}
                else if (room.region.GetParentOrSelf().Equals(end))
                {
					foreach (Tile t in room.GetSubRooms(MapRoom.RoomType.TINY, MapRoom.DIRECTION_UP, 2))
					{
						if (t == null) break;

						SpawnMonsterToRoom(room, MonsterId.NonaggressiveHelperCell, t, false, 1);
					}

					foreach (Tile t in room.GetSubRooms(MapRoom.RoomType.MEDIUM, MapRoom.DIRECTION_CENTER, 3))
					{
						if (t == null) break;

						SpawnMonsterToRoom(room, MonsterId.Lymfocyte_ranged, t, false, 1);
						SpawnMonsterToRoom(room, MonsterId.Lymfocyte_melee, t, true, 1);
					}

					foreach (Tile t in room.GetSubRooms(MapRoom.RoomType.TINY, MapRoom.DIRECTION_CENTER, 1))
					{
						if (t == null) break;

						SpawnMonsterToRoom(room, MonsterId.HelperCell, t, false, 1);
					}

					foreach (Tile t in room.GetSubRooms(MapRoom.RoomType.MEDIUM, MapRoom.DIRECTION_CENTER, 1))
					{
						if (t == null) break;

						SpawnMonsterToRoom(room, MonsterId.ChargerCell, t, false, 1);
						SpawnMonsterToRoom(room, MonsterId.DementCell, t, true, 1).AddHealDrop(100);
					}

					foreach (Tile t in room.GetSubRooms(MapRoom.RoomType.TINY, MapRoom.DIRECTION_DOWN, 1))
					{
						if (t == null) break;

						SpawnMonsterToRoom(room, MonsterId.TurretCell, t, false, 1).AddHealDrop(80);
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
	        return 2;
	    }

		public override void OnPlayerTeleportOut(Player player)
		{
			if (conquered)
			{
				player.UnlockSkill(3, true);
				conquered = false;

				Skill sk = (ActiveSkill)player.Skills.GetSkill(3);
				string skillName = sk.GetVisibleName();
				string desc = sk.GetDescription().ToLower();

				if (GameSession.className.Equals("CommonCold"))
					player.GetData().ui.ShowHelpWindow(Messages.ShowHelpWindow("third_skill_unlocked_commoncold", skillName, desc), 0);
				else
					player.GetData().ui.ShowHelpWindow(Messages.ShowHelpWindow("third_skill_unlocked", skillName, desc), 0);
			}
		}
	}
}
