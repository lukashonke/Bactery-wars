// Copyright (c) 2015, Lukas Honke
// ========================
using Assets.scripts.Skills.Base;
using Assets.scripts.Skills.SkillEffects;
using UnityEngine;

namespace Assets.scripts.Skills.ActiveSkills
{
	public class ProjectileAllAround : Projectile
	{
		public int projectileCount = 12;

		public ProjectileAllAround()
		{
			castTime = 2f;
			reuse = 0.5f;
			coolDown = 0.5f;
			baseDamage = 10;

			requireConfirm = false;
			AvailableToPlayer = false;
		}

		public override SkillId GetSkillId()
		{
			return SkillId.ProjectileAllAround;
		}

		public override string GetVisibleName()
		{
			return "Projectile Allaround";
		}

		public override Skill Instantiate()
		{
			return new ProjectileAllAround();
		}

		public override void OnLaunch()
		{
			DeleteCastingEffect();

			GameObject activeProjectile;

			for (int i = 0; i < 360; i+=(360/projectileCount))
			{
				activeProjectile = CreateSkillProjectile(GetName(), "projectile_blacktest_i00", true);

				if (activeProjectile != null)
				{
					Rigidbody2D rb = activeProjectile.GetComponent<Rigidbody2D>();
					rb.velocity = GetOwnerData().GetForwardVector(i) * 15;

					//Debug.DrawRay(GetOwnerData().GetShootingPosition().transform.position, rb.velocity, Color.green, 5f);

					Object.Destroy(activeProjectile, 5f);
				}
			}
		}
	}
}
