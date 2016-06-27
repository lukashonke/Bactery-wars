// Copyright (c) 2015, Lukas Honke
// ========================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.scripts.Skills.Base
{
	public enum SkillId
	{
		SkillTemplate,
		CustomRemove,

		// generic skills
		Beam,
		Jump,
		Selfbuff,
		ProjectileShower,
		Aura,
		AuraAllies,
		Projectile,
		BlankProjectile,
		Smash,

		// player skills - level 1
		CellShot,
		CellSmash,
		CellJump,

		SneezeShot,
		CoughBullet,
		PoisonShot,
		Dodge,
		Charge,
		PullTarget,
		CellFury,
		ChargeSkill,
		Haste,
		MucusWarrior,
		RhinoBeam,
		ColdShuriken,
		ColdPush,

		SummonWarrior,
		SpawnTurret,
		SpawnTrap,

		// level 2
		Explode,

       

        // other, obsolete skills
        ProjectileAllAround,
		ProjectileTriple,
		ProjectileStrong,

		SkillAreaExplode,
		MissileProjectile,
		JumpShort,
		ChainSkill,
		ChainedProjectile,

		NeutrophileProjectile,
	    PushbackProjectile,

		MeleeAttack,
		CollisionDamageAttack,
		CreateCoverMob,
		SwarmerSpawnSkill,

		SlowBeam,
		Teleport,
		HealBeam,
		PushBeam,
		Push,

		SwarmSkill,

        // turrety
        SpawnTurretClass1,

		DieDamage,
		DieEffect,
	}
}
