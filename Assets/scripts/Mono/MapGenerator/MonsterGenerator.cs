using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.Mono.MapGenerator.Levels;

namespace Assets.scripts.Mono.MapGenerator
{
	class MonsterGenerator
	{
		private static MonsterGenerator instance = null;
		public static MonsterGenerator Instance
		{
			get
			{
				if (instance == null)
					instance = new MonsterGenerator();

				return instance;
			}
		}

		public MonsterGenerator()
		{
			

		}

		public enum RoomType
		{
			START_ROOM,
			END_ROOM,
			MAIN_ROOM,
			SIDE_ROOM,
			BONUS_ROOM,

			BOSS_ROOM
		}

		public enum RoomSize
		{
			LARGE,
			MEDIUM,
			SMALL
		}

		public void GenerateGenericEnemyGroup(MapRoom room, AbstractLevelData level, RoomType roomType, RoomSize roomSize, int difficulty)
		{
			Player player = GameSystem.Instance.CurrentPlayer;
			int playerLevel = 1;
			if (player != null)
			{
				playerLevel = player.Level;
			}

			switch (roomType)
			{
				case RoomType.START_ROOM:

					switch (roomSize)
					{
						case RoomSize.MEDIUM:
							Tile t = room.GetSubRoom(MapRoom.RoomType.TINY, MapRoom.DIRECTION_RIGHT);
							level.SpawnMonsterToRoom(room, MonsterId.HelperCell, t);
							break;
						case RoomSize.SMALL:
							if (Utils.ChanceCheck(50))
							{
								
							}
							break;
						case RoomSize.LARGE:

							break;
					}
					break;
				case RoomType.MAIN_ROOM:

					break;
				case RoomType.SIDE_ROOM:

					break;
				case RoomType.BONUS_ROOM:

					break;
				case RoomType.BOSS_ROOM:

					break;
				case RoomType.END_ROOM:

					break;
			}

			if (playerLevel <= 5)
			{
				
			}
			else if (playerLevel <= 7)
			{
				
			}
			else if (playerLevel <= 9)
			{
				
			}
		}
	}
}
