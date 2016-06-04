using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.scripts.Actor.MonsterClasses.Base
{
	public enum MonsterId
	{
		TestMonster = 0,

		Leukocyte_melee = 1,
		Leukocyte_ranged = 2,

		Neutrophyle_Patrol = 3,

		MucusWarrior = 4,
		SummonWarrior = 2000,

        Lymfocyte_melee = 5,
        Lymfocyte_ranged = 6,
		BasicCell = 7,
		PassiveHelperCell = 8,
		SpiderCell = 9,
		DementCell = 10,
		TankCell = 11,
		TurretCell = 12,

		TeleporterOut = 100,
		TeleporterIn = 101,
		Shop = 102,

		JumpCell = 13,
		SuiciderCell = 14,
		FourDiagShooterCell = 15,
		ChargerCell = 16,
		ArmoredCell = 17,
		FloatingBasicCell = 18,
		ObstacleCell = 19,
		IdleObstacleCell = 20,
		MorphCellBig = 21,
		MorphCellMedium = 22,
		MorphCellSmall = 23,

		NonaggressiveBasicCell = 24,
		MissileTurretCell = 25,

		DurableMeleeCell = 26,

		SmallTankCell = 27,
		SwarmerMeleeCell = 28,
		BigPassiveFloatingCell = 29,
		BigPassiveCell = 30,

		SniperCell = 31,

		SlowerCell = 40,
		RogueCell = 41,
		HealerCell = 42,
		PusherCell = 43,
		SwarmCell = 44,

		BasicMeleeCell = 45,

		// bosses
		TestBoss = 200,
		TankSpreadshooter = 201,
		SwarmerBoss = 202,

		CustomCell = 300,

		SimpleBase = 500,

		CustomMonster = 999,
	}
}
