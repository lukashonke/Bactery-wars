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

		// concrete skills
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

		CommonColdAutoattack,
		SneezeShot,
		Dodge,
		CellFury,
		MucusWarrior,
		RhinoBeam,
		ColdShuriken,
		ColdPush,

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
