// Copyright (c) 2015, Lukas Honke
// ========================
using Assets.scripts.Actor;
using Assets.scripts.Base;
using Assets.scripts.Mono;
using Assets.scripts.Skills.Base;
using Assets.scripts.Skills.SkillEffects;
using UnityEngine;

namespace Assets.scripts.Skills.ActiveSkills
{
	public class ProjectileStrong : Projectile
	{
		public ProjectileStrong()
		{
			castTime = 0f;
			reuse = 1;
			coolDown = 0;
			requireConfirm = true;
			baseDamage = 20;

			range = 5;

			force = 10;
		}

		public override SkillId GetSkillId()
		{
			return SkillId.ProjectileStrong;
		}

		public override string GetVisibleName()
		{
			return "Projectile";
		}

		public override Skill Instantiate()
		{
			return new ProjectileStrong();
		}
	}
}
