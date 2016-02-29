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
			return 2;
		}

		public static int GetLevelForExtraSlot(int slotId)
		{
			if (slotId == 3)
			{
				return 5;
			}
			else if (slotId == 4)
			{
				return 20;
			}
			else if (slotId == 5)
			{
				return -1;
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
					return "Unlocked on level 20.";
				}
				else if (slotId == 5)
				{
					return "Unlocked on rebirth.";
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
