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

		// player skills
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
		MucusWarrior,
		RhinoBeam,
		ColdShuriken,
		ColdPush,

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

		SwarmSkill,
	}
}
