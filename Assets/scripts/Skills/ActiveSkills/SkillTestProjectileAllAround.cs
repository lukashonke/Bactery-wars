﻿using Assets.scripts.Skills.SkillEffects;
using UnityEngine;

namespace Assets.scripts.Skills.ActiveSkills
{
	public class SkillTestProjectileAllAround : SkillTestProjectile
	{
		public SkillTestProjectileAllAround(string name, int id) : base(name, id)
		{
			castTime = 2f;
			reuse = 0.5f;
			coolDown = 0.5f;
			requireConfirm = false;
		}

		public override Skill Instantiate()
		{
			return new SkillTestProjectileAllAround(Name, Id);
		}

		public override void OnLaunch()
		{
			if (particleSystemObject != null)
				Object.Destroy(particleSystemObject);

			GameObject activeProjectile;

			for (int i = 0; i < 360; i+=30)
			{
				activeProjectile = GetPlayerData().CreateProjectile("SkillTestProjectile", "projectile_blacktest_i00");

				if (activeProjectile != null)
				{
					Rigidbody2D rb = activeProjectile.GetComponent<Rigidbody2D>();
					rb.velocity = GetOwnerData().GetForwardVector(i) * 15;

					Debug.DrawRay(GetOwnerData().GetShootingPosition().transform.position, rb.velocity, Color.green, 5f);

					AddMonoReceiver(activeProjectile);

					Object.Destroy(activeProjectile, 5f);
				}
			}
		}
	}
}
