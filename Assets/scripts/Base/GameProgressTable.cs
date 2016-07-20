// Copyright (c) 2015, Lukas Honke
// ========================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.scripts.Base
{
	public static class GameProgressTable
	{
		public static int GetXpForLevel(int nextLevel)
		{
			switch (nextLevel)
			{
				case 2:
					return 100;
				case 3:
					return 125;
				case 4:
					return 150;
				case 5:
					return 175;
				case 6:
					return 200;
				case 7:
					return 250;
				case 8:
					return 300;
				case 9:
					return 350;
				case 10:
					return 350;
				case 11:
					return 100000;
			}
			return (nextLevel-10+1)*100000;
		}

		public static int GetLevelForExtraSlot(int slotId)
		{
			if (slotId == 3)
			{
				return 5;
			}
			else if (slotId == 4)
			{
				return 10;
			}
			else if (slotId == 5)
			{
				return 20;
			}
			else if (slotId == 6)
			{
				return -1;
			}

			return 0;
		}

		public static string GetDescriptionOnLockedSlot(int slotId, int slotType)
		{
			if (slotType == 1) // active
			{
				if (slotId == 3)
				{
					return "Unlocked on level 5.";
				}
				else if (slotId == 4)
				{
					return "Unlocked on level 10.";
				}
				else if (slotId == 5)
				{
					return "Unlocked on level 20.";
				}
				else if (slotId == 6)
				{
					return "Unlocked on second rebirth";
				}
			}

			return null;
		}
	}
}
