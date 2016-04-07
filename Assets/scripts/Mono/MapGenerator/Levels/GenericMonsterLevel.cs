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
	public class GenericMonsterLevel : AbstractLevelData
	{
		private int[] startSeeds =
		{
			993, 718, -703, -221, 186, 633, -548, 882, -784, 389, 207, -358, 567, -149,
		};

		private int[] bossRoomSeeds =
		{
			993, 718, -703, -221, 186, 633, -548, 882, -784, 389, 207, -358, 567, -149,
		};

		private int[] mainRoomVerticalSeeds =
		{
			217, -760, 97, -958, 579, 216, -217, -262, 169, -387, -72, 616, 659, 877, -984, -851, 151, 702,
		};

		private int[] mainRoomTallVerticalSeeds =
		{
			217, -760, 97, -958, 579, 216, -217, -262, 169, -387, -72, 616, 659, 877, -984, -851, 151, 702,
		};

		private int[] mainRoomHorizontalSeeds =
		{
			217, -760, 97, -958, 579, 216, -217, -262, 169, -387, -72, 616, 659, 877, -984, -851, 151, 702,
		};

		private int[] mainRoomTallHorizontalSeeds =
		{
			217, -760, 97, -958, 579, 216, -217, -262, 169, -387, -72, 616, 659, 877, -984, -851, 151, 702,
		};

		private int[] mainRoomSquareSeeds =
		{
			217, -760, 97, -958, 579, 216, -217, -262, 169, -387, -72, 616, 659, 877, -984, -851, 151, 702,
		};

		private int[] sideRoomSeeds =
		{
			217, -760, 97, -958, 579, 216, -217, -262, 169, -387, -72, 616, 659, 877, -984, -851, 151, 702,
		};

		private int[] endSeeds =
		{
			494, -287, -756, -556, -784, -242, -850, 635,
		};

		private int[] bonusSeeds =
		{
			494, -287, -756, -556, -784, -242, -850, 635,
		};

		private int variant;
		private int difficulty;
		private const int maxVariant = 14;
		private int bossChance = 50;
		private int bonusRoomChance = 50;
		private int regionsX = 4;
		private int regionsY = 4;

		public GenericMonsterLevel(MapHolder holder, LevelParams param) : base(holder)
		{
			type = MapType.GenericMonster;
			tutorialLevel = false;
			variant = param.variant;
			difficulty = param.difficulty;

			if (variant == 1)
			{
				variant = Random.Range(1, maxVariant + 1);
			}

			if (variant >= 1 && variant <= 4)
			{
				regionsX = 4;
				regionsY = 3;
			}

			if (variant >= 5 && variant <= 6)
			{
				regionsX = 6;
				regionsY = 3;
			}

			if (variant >= 7 && variant <= 9)
			{
				regionsX = 6;
				regionsY = 3;
			}

			if (variant >= 10 && variant <= 14)
			{
				regionsX = 5;
				regionsY = 3;
			}
		}

		private MapRegion start;
		private MapRegion end;
		private MapRegion boss;
		private List<MapRegion> mainRooms = new List<MapRegion>();
		private List<MapRegion> sideRooms = new List<MapRegion>();
		private List<MapRegion> bonusRooms = new List<MapRegion>();

		private const int bossFillPercent = 42;
		private const int bonusRoomFillPercent = 39;
		private const int mainFillPercent = 46;
		private const int sideFillPercent = 46;
		private const int startFillPercent = 46;
		private const int endFillPercent = 46;

		public override void Generate()
		{
			switch (variant)
			{
				case 1:
					start = map.GenerateDungeonRegion(0, 0, startFillPercent, true, false, false, startSeeds, 1, 1);
					sideRooms.Add(map.GenerateDungeonRegion(1, 0, sideFillPercent, false, true, false, sideRoomSeeds, 1, 1));
					sideRooms.Add(map.GenerateDungeonRegion(1, 1, sideFillPercent, false, true, false, sideRoomSeeds, 1, 1));
					mainRooms.Add(map.GenerateDungeonRegion(2, 0, mainFillPercent, false, true, false, mainRoomVerticalSeeds, 1, 2));

					if (ChanceCheck(bossChance))
					{
						boss = map.GenerateDungeonRegion(2, 2, bossFillPercent, false, true, false, bossRoomSeeds, 1, 1);
					}

					end = map.GenerateDungeonRegion(3, 1, endFillPercent, false, true, true, endSeeds, 1, 1);
					break;
				case 2:
					start = map.GenerateDungeonRegion(0, 0, startFillPercent, true, false, false, startSeeds, 1, 1);
					mainRooms.Add(map.GenerateDungeonRegion(1, 0, mainFillPercent, false, true, false, mainRoomHorizontalSeeds, 2, 1));
					mainRooms.Add(map.GenerateDungeonRegion(1, 1, mainFillPercent, false, true, false, mainRoomHorizontalSeeds, 2, 1));

					if (ChanceCheck(bossChance))
					{
						boss = map.GenerateDungeonRegion(2, 2, bossFillPercent, false, true, false, bossRoomSeeds, 1, 1);
					}

					end = map.GenerateDungeonRegion(3, 1, endFillPercent, false, true, true, endSeeds, 1, 1);
					break;
				case 3:
					start = map.GenerateDungeonRegion(0, 2, startFillPercent, true, false, false, startSeeds, 1, 1);
					mainRooms.Add(map.GenerateDungeonRegion(1, 2, mainFillPercent, false, true, false, mainRoomHorizontalSeeds, 2, 1));
					mainRooms.Add(map.GenerateDungeonRegion(1, 1, mainFillPercent, false, true, false, mainRoomHorizontalSeeds, 2, 1));

					if (ChanceCheck(bossChance))
					{
						boss = map.GenerateDungeonRegion(2, 0, bossFillPercent, false, true, false, bossRoomSeeds, 1, 1);
					}

					end = map.GenerateDungeonRegion(3, 1, endFillPercent, false, true, true, endSeeds, 1, 1);
					break;
				case 4:
					start = map.GenerateDungeonRegion(0, 2, startFillPercent, true, false, false, startSeeds, 1, 1);
					sideRooms.Add(map.GenerateDungeonRegion(1, 2, sideFillPercent, false, true, false, sideRoomSeeds, 1, 1));
					sideRooms.Add(map.GenerateDungeonRegion(1, 1, sideFillPercent, false, true, false, sideRoomSeeds, 1, 1));
					mainRooms.Add(map.GenerateDungeonRegion(2, 1, mainFillPercent, false, true, false, mainRoomVerticalSeeds, 1, 2));

					if (ChanceCheck(bossChance))
					{
						boss = map.GenerateDungeonRegion(1, 0, bossFillPercent, false, true, false, bossRoomSeeds, 1, 1);
					}

					end = map.GenerateDungeonRegion(3, 1, endFillPercent, false, true, true, endSeeds, 1, 1);
					break;
				case 5:
					start = map.GenerateDungeonRegion(0, 2, startFillPercent, true, false, false, startSeeds, 1, 1);
					mainRooms.Add(map.GenerateDungeonRegion(1, 1, mainFillPercent, false, true, false, mainRoomVerticalSeeds, 1, 2));
					sideRooms.Add(map.GenerateDungeonRegion(2, 1, sideFillPercent, false, true, false, sideRoomSeeds, 1, 1));
					mainRooms.Add(map.GenerateDungeonRegion(3, 1, mainFillPercent, false, true, false, mainRoomSquareSeeds, 2, 2));

					if (ChanceCheck(bossChance))
					{
						boss = map.GenerateDungeonRegion(2, 0, bossFillPercent, false, true, false, bossRoomSeeds, 1, 1);
					}

					end = map.GenerateDungeonRegion(5, 1, endFillPercent, false, true, true, endSeeds, 1, 1);
					break;
				case 6:
					start = map.GenerateDungeonRegion(0, 0, startFillPercent, true, false, false, startSeeds, 1, 1);
					mainRooms.Add(map.GenerateDungeonRegion(1, 0, mainFillPercent, false, true, false, mainRoomVerticalSeeds, 1, 2));
					sideRooms.Add(map.GenerateDungeonRegion(2, 1, sideFillPercent, false, true, false, sideRoomSeeds, 1, 1));
					mainRooms.Add(map.GenerateDungeonRegion(3, 0, mainFillPercent, false, true, false, mainRoomSquareSeeds, 2, 2));

					if (ChanceCheck(bossChance))
					{
						boss = map.GenerateDungeonRegion(2, 2, bossFillPercent, false, true, false, bossRoomSeeds, 1, 1);
					}

					end = map.GenerateDungeonRegion(5, 0, endFillPercent, false, true, true, endSeeds, 1, 1);
					break;
				case 7:
					start = map.GenerateDungeonRegion(0, 1, startFillPercent, true, false, false, startSeeds, 1, 1);
					sideRooms.Add(map.GenerateDungeonRegion(1, 1, sideFillPercent, false, true, false, sideRoomSeeds, 1, 1));
					mainRooms.Add(map.GenerateDungeonRegion(2, 0, mainFillPercent, false, true, false, mainRoomTallVerticalSeeds, 1, 3));
					mainRooms.Add(map.GenerateDungeonRegion(4, 1, mainFillPercent, false, true, false, mainRoomSquareSeeds, 2, 2));

					boss = map.GenerateDungeonRegion(3, 1, bossFillPercent, false, true, false, bossRoomSeeds, 1, 1);

					if (ChanceCheck(bonusRoomChance))
					{
						bonusRooms.Add(map.GenerateDungeonRegion(1, 2, bonusRoomFillPercent, false, true, false, bonusSeeds, 1, 1));
					}

					end = map.GenerateDungeonRegion(4, 0, endFillPercent, false, true, true, endSeeds, 1, 1);
					break;
				case 8:
					start = map.GenerateDungeonRegion(0, 1, startFillPercent, true, false, false, startSeeds, 1, 1);
					sideRooms.Add(map.GenerateDungeonRegion(1, 1, sideFillPercent, false, true, false, sideRoomSeeds, 1, 1));
					mainRooms.Add(map.GenerateDungeonRegion(2, 1, mainFillPercent, false, true, false, mainRoomHorizontalSeeds, 2, 1));

					if (ChanceCheck(bossChance))
					{
						boss = map.GenerateDungeonRegion(2, 2, bossFillPercent, false, true, false, bossRoomSeeds, 1, 1);
					}

					if (ChanceCheck(bonusRoomChance))
					{
						bonusRooms.Add(map.GenerateDungeonRegion(1, 0, bonusRoomFillPercent, false, true, false, bonusSeeds, 1, 1));
					}

					mainRooms.Add(map.GenerateDungeonRegion(4, 0, mainFillPercent, false, true, true, mainRoomSquareSeeds, 2, 2));

					break;
				case 9:
					start = map.GenerateDungeonRegion(0, 1, startFillPercent, true, false, false, startSeeds, 1, 1);
					sideRooms.Add(map.GenerateDungeonRegion(1, 1, sideFillPercent, false, true, false, sideRoomSeeds, 1, 1));
					mainRooms.Add(map.GenerateDungeonRegion(2, 1, mainFillPercent, false, true, false, mainRoomSquareSeeds, 2, 2));
					mainRooms.Add(map.GenerateDungeonRegion(4, 0, mainFillPercent, false, true, false, mainRoomTallVerticalSeeds, 1, 3));

					boss = map.GenerateDungeonRegion(3, 0, bossFillPercent, false, true, false, bossRoomSeeds, 1, 1);

					if (ChanceCheck(bonusRoomChance))
					{
						bonusRooms.Add(map.GenerateDungeonRegion(1, 2, bonusRoomFillPercent, false, true, false, bonusSeeds, 1, 1));
					}

					end = map.GenerateDungeonRegion(5, 0, endFillPercent, false, true, true, endSeeds, 1, 1);
					break;
				case 10:
					start = map.GenerateDungeonRegion(0, 2, startFillPercent, true, false, false, startSeeds, 1, 1);
					mainRooms.Add(map.GenerateDungeonRegion(1, 1, mainFillPercent, false, true, false, mainRoomTallHorizontalSeeds, 3, 1));
					mainRooms.Add(map.GenerateDungeonRegion(1, 2, mainFillPercent, false, true, false, mainRoomTallHorizontalSeeds, 3, 1));

					if (ChanceCheck(bossChance))
					{
						boss = map.GenerateDungeonRegion(0, 0, bossFillPercent, false, true, false, bossRoomSeeds, 1, 1);
					}
					else if (ChanceCheck(bonusRoomChance))
					{
						bonusRooms.Add(map.GenerateDungeonRegion(0, 0, bonusRoomFillPercent, false, true, false, bonusSeeds, 1, 1));
					}

					mainRooms.Add(map.GenerateDungeonRegion(1, 0, mainFillPercent, false, true, true, mainRoomTallHorizontalSeeds, 3, 1));
					break;
				case 11:
					start = map.GenerateDungeonRegion(0, 2, startFillPercent, true, false, false, startSeeds, 1, 1);

					sideRooms.Add(map.GenerateDungeonRegion(1, 2, sideFillPercent, false, true, false, sideRoomSeeds, 1, 1));
					sideRooms.Add(map.GenerateDungeonRegion(2, 2, sideFillPercent, false, true, false, sideRoomSeeds, 1, 1));

					if (ChanceCheck(bonusRoomChance))
					{
						bonusRooms.Add(map.GenerateDungeonRegion(3, 2, bonusRoomFillPercent, false, true, false, bonusSeeds, 1, 1));
					}
					else
					{
						sideRooms.Add(map.GenerateDungeonRegion(3, 2, sideFillPercent, false, true, false, sideRoomSeeds, 1, 1));
					}
					
					sideRooms.Add(map.GenerateDungeonRegion(1, 1, sideFillPercent, false, true, false, sideRoomSeeds, 1, 1));
					sideRooms.Add(map.GenerateDungeonRegion(3, 1, sideFillPercent, false, true, false, sideRoomSeeds, 1, 1));

					if (ChanceCheck(bossChance))
					{
						boss = map.GenerateDungeonRegion(1, 0, bossFillPercent, false, true, false, bossRoomSeeds, 1, 1);
					}
					else
					{
						sideRooms.Add(map.GenerateDungeonRegion(1, 0, sideFillPercent, false, true, false, sideRoomSeeds, 1, 1));
					}

					sideRooms.Add(map.GenerateDungeonRegion(2, 0, sideFillPercent, false, true, false, sideRoomSeeds, 1, 1));
					sideRooms.Add(map.GenerateDungeonRegion(3, 0, sideFillPercent, false, true, false, sideRoomSeeds, 1, 1));

					mainRooms.Add(map.GenerateDungeonRegion(2, 1, mainFillPercent, false, true, false, bossRoomSeeds, 1, 1));

					end = map.GenerateDungeonRegion(4, Random.Range(0, 3), endFillPercent, false, true, true, endSeeds, 1, 1);
					break;
				case 12:
					start = map.GenerateDungeonRegion(0, 2, startFillPercent, true, false, false, startSeeds, 1, 1);
					mainRooms.Add(map.GenerateDungeonRegion(1, 2, mainFillPercent, false, true, false, mainRoomTallHorizontalSeeds, 3, 1));
					
					sideRooms.Add(map.GenerateDungeonRegion(2, 1, sideFillPercent, false, true, false, sideRoomSeeds, 1, 1));

					if (ChanceCheck(bossChance))
					{
						boss = map.GenerateDungeonRegion(1, 1, bossFillPercent, false, true, false, bossRoomSeeds, 1, 1);
					}
					else if (ChanceCheck(bonusRoomChance))
					{
						bonusRooms.Add(map.GenerateDungeonRegion(1, 1, bonusRoomFillPercent, false, true, false, bonusSeeds, 1, 1));
					}

					mainRooms.Add(map.GenerateDungeonRegion(1, 0, mainFillPercent, false, true, true, mainRoomTallHorizontalSeeds, 3, 1));
					break;
				case 13:
					start = map.GenerateDungeonRegion(0, 2, startFillPercent, true, false, false, startSeeds, 1, 1);
					sideRooms.Add(map.GenerateDungeonRegion(1, 2, sideFillPercent, false, true, false, sideRoomSeeds, 1, 1));
					mainRooms.Add(map.GenerateDungeonRegion(2, 2, mainFillPercent, false, true, false, mainRoomHorizontalSeeds, 2, 1));
					mainRooms.Add(map.GenerateDungeonRegion(1, 0, mainFillPercent, false, true, false, mainRoomVerticalSeeds, 1, 2));

					if (ChanceCheck(bossChance))
					{
						boss = map.GenerateDungeonRegion(0, 0, bossFillPercent, false, true, false, bossRoomSeeds, 1, 1);
					}
					else if (ChanceCheck(bonusRoomChance))
					{
						bonusRooms.Add(map.GenerateDungeonRegion(0, 0, bonusRoomFillPercent, false, true, false, bonusSeeds, 1, 1));
					}

					mainRooms.Add(map.GenerateDungeonRegion(2, 0, mainFillPercent, false, true, true, mainRoomSquareSeeds, 2, 2));
					break;
				case 14:
					start = map.GenerateDungeonRegion(0, 2, startFillPercent, true, false, false, startSeeds, 1, 1);
					mainRooms.Add(map.GenerateDungeonRegion(1, 2, mainFillPercent, false, true, false, mainRoomTallHorizontalSeeds, 3, 1));

					sideRooms.Add(map.GenerateDungeonRegion(2, 1, sideFillPercent, false, true, false, sideRoomSeeds, 1, 1));

					if (ChanceCheck(bossChance))
					{
						boss = map.GenerateDungeonRegion(1, 1, bossFillPercent, false, true, false, bossRoomSeeds, 1, 1);
					}
					else if (ChanceCheck(bonusRoomChance))
					{
						bonusRooms.Add(map.GenerateDungeonRegion(1, 1, bonusRoomFillPercent, false, true, false, bonusSeeds, 1, 1));
					}
					else
					{
						sideRooms.Add(map.GenerateDungeonRegion(1, 1, sideFillPercent, false, true, false, sideRoomSeeds, 1, 1));
					}

					mainRooms.Add(map.GenerateDungeonRegion(1, 0, mainFillPercent, false, true, true, mainRoomTallHorizontalSeeds, 3, 1));
					break;
			}
		}

		public override void SpawnMonsters()
		{
            Utils.Timer.StartTimer("spawnmap");

			if (difficulty == 1) // EASY
			{
				foreach (MapRoom room in map.GetMapRooms()) // TODO throws NPE sometimes
				{
					if (room.region.GetParentOrSelf().Equals(start))
					{
						Tile t = room.GetLargestSubRoom(true);
						SpawnMonsterToRoom(room, MonsterId.FloatingHelperCell, t, false, 1);
					}
					else if (mainRooms.Contains(room.region.GetParentOrSelf()))
					{
						Tile t = room.GetLargestSubRoom(true);
						SpawnMonsterToRoom(room, MonsterId.TankCell, t, false, 1);
					}
					else if (sideRooms.Contains(room.region.GetParentOrSelf()))
					{
						Tile t = room.GetLargestSubRoom(true);
						SpawnMonsterToRoom(room, MonsterId.ArmoredCell, t, false, 1);
					}
					else if (bonusRooms.Contains(room.region.GetParentOrSelf()))
					{
						Tile t = room.GetLargestSubRoom(true);
						SpawnMonsterToRoom(room, MonsterId.DementCell, t, false, 1);
					}
					else if (room.region.GetParentOrSelf().Equals(boss))
					{
						Tile t = room.GetLargestSubRoom(true);
						SpawnMonsterToRoom(room, MonsterId.TankSpreadshooter, t, false, 1);
					}
					else if (room.region.GetParentOrSelf().Equals(end))
					{
						Tile t = room.GetLargestSubRoom(true);
						SpawnMonsterToRoom(room, MonsterId.SuiciderCell, t, false, 1);
					}
				}
			}
			else if (difficulty == 2) // MEDIUM
			{
				foreach (MapRoom room in map.GetMapRooms()) // TODO throws NPE sometimes
				{
					if (room.region.GetParentOrSelf().Equals(start))
					{
						Tile t = room.GetLargestSubRoom(true);
						SpawnMonsterToRoom(room, MonsterId.FloatingHelperCell, t, false, 1);
					}
					else if (mainRooms.Contains(room.region.GetParentOrSelf()))
					{
						Tile t = room.GetLargestSubRoom(true);
						SpawnMonsterToRoom(room, MonsterId.TankCell, t, false, 1);
					}
					else if (sideRooms.Contains(room.region.GetParentOrSelf()))
					{
						Tile t = room.GetLargestSubRoom(true);
						SpawnMonsterToRoom(room, MonsterId.ArmoredCell, t, false, 1);
					}
					else if (bonusRooms.Contains(room.region.GetParentOrSelf()))
					{
						Tile t = room.GetLargestSubRoom(true);
						SpawnMonsterToRoom(room, MonsterId.DementCell, t, false, 1);
					}
					else if (room.region.GetParentOrSelf().Equals(boss))
					{
						Tile t = room.GetLargestSubRoom(true);
						SpawnMonsterToRoom(room, MonsterId.TankSpreadshooter, t, false, 1);
					}
					else if (room.region.GetParentOrSelf().Equals(end))
					{
						Tile t = room.GetLargestSubRoom(true);
						SpawnMonsterToRoom(room, MonsterId.SuiciderCell, t, false, 1);
					}
				}
			}
			else if (difficulty == 3) // HARD
			{
				foreach (MapRoom room in map.GetMapRooms()) // TODO throws NPE sometimes
				{
					if (room.region.GetParentOrSelf().Equals(start))
					{
						Tile t = room.GetLargestSubRoom(true);
						SpawnMonsterToRoom(room, MonsterId.FloatingHelperCell, t, false, 1);
					}
					else if (mainRooms.Contains(room.region.GetParentOrSelf()))
					{
						Tile t = room.GetLargestSubRoom(true);
						SpawnMonsterToRoom(room, MonsterId.TankCell, t, false, 1);
					}
					else if (sideRooms.Contains(room.region.GetParentOrSelf()))
					{
						Tile t = room.GetLargestSubRoom(true);
						SpawnMonsterToRoom(room, MonsterId.ArmoredCell, t, false, 1);
					}
					else if (bonusRooms.Contains(room.region.GetParentOrSelf()))
					{
						Tile t = room.GetLargestSubRoom(true);
						SpawnMonsterToRoom(room, MonsterId.DementCell, t, false, 1);
					}
					else if (room.region.GetParentOrSelf().Equals(boss))
					{
						Tile t = room.GetLargestSubRoom(true);
						SpawnMonsterToRoom(room, MonsterId.TankSpreadshooter, t, false, 1);
					}
					else if (room.region.GetParentOrSelf().Equals(end))
					{
						Tile t = room.GetLargestSubRoom(true);
						SpawnMonsterToRoom(room, MonsterId.SuiciderCell, t, false, 1);
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
