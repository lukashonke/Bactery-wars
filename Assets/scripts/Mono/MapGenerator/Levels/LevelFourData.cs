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
	public class LevelFourData : AbstractLevelData
	{
	    public MapRegion start, mid, end;

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

		public LevelFourData(MapHolder holder) : base(holder)
		{
			type = MapType.LevelFour;
			tutorialLevel = true;
		}

		public override void Generate()
		{
			start = map.GenerateDungeonRegion(0, 1, 45, true, false, false, levelOneSeeds, 1, 1); // 0 0
			mid = map.GenerateDungeonRegion(1, 0, 43, false, true, false, levelTwoSeeds, 2, 2); // 0, 2
			end = map.GenerateDungeonRegion(3, 0, 45, false, true, true, levelThreeSeeds, 1, 1); // 0, 2
		}

		public override void SpawnMonsters()
		{
            Utils.Timer.StartTimer("spawnmap");

		    foreach (MapRoom room in map.GetMapRooms()) // TODO throws NPE sometimes
		    {
		        if (room.region.GetParentOrSelf().Equals(start))
		        {
		        }
                else if (room.region.GetParentOrSelf().Equals(mid))
                {
					Tile t = room.GetLargestSubRoom(true);
	                SpawnMonsterToRoom(room, MonsterId.TankSpreadshooter, t, false, 1)
		                .AddHealDrop(100, 10);
                }
                else if (room.region.GetParentOrSelf().Equals(end))
                {
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
			return 4;
		}

	    public override int GetMaxRegionsY()
	    {
	        return 2;
	    }

		public override void OnPlayerTeleportIn(Player player)
		{
			player.GetData().ui.ShowHelpWindow(Messages.ShowHelpWindow("fourth_level_bossinfo"), 3f);
		}

		public override void OnPlayerTeleportOut(Player player)
		{
			if (conquered)
			{
				player.UnlockSkill(4, true);
				conquered = false;

				Skill sk = (ActiveSkill)player.Skills.GetSkill(4);
				string skillName = sk.GetVisibleName();
				string desc = sk.GetDescription().ToLower();

				if (GameSession.className.Equals("CommonCold"))
					player.GetData().ui.ShowHelpWindow(Messages.ShowHelpWindow("fourth_skill_unlocked_commoncold", skillName, desc), 0);
				else
					player.GetData().ui.ShowHelpWindow(Messages.ShowHelpWindow("fourth_skill_unlocked", skillName, desc), 0);
			}
		}
	}
}
