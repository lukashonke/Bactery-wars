using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.Mono.MapGenerator;

namespace Assets.scripts.Fort
{
	public static class SiegeManager
	{
		public static Siege currentSiege;

		public static void StartSiege(MapHolder map)
		{
			if (currentSiege != null)
				return;

			currentSiege = new Siege(map);
			currentSiege.InitSiege();

			currentSiege.mobs.Add(new SiegeMobData(MonsterId.Neutrophyle_Patrol, 1, 0, 1, Siege.DIRECTION_RIGHT));
			currentSiege.mobs.Add(new SiegeMobData(MonsterId.Neutrophyle_Patrol, 1, 0, 1, Siege.DIRECTION_RIGHT));
			currentSiege.mobs.Add(new SiegeMobData(MonsterId.FloatingBasicCell, 1, 5, 1, Siege.DIRECTION_RIGHT));
			currentSiege.mobs.Add(new SiegeMobData(MonsterId.FloatingBasicCell, 1, 5, 1, Siege.DIRECTION_RIGHT));

			currentSiege.StartSiege();
		}

		public static void CancelSiege()
		{
			if (currentSiege != null)
			{
				currentSiege.CancelSiege();
				currentSiege = null;
			}
		}

		public static bool IsSiegeActive()
		{
			return currentSiege != null;
		}

		public static void SiegeEnd()
		{
			currentSiege = null;
		}
	}
}
