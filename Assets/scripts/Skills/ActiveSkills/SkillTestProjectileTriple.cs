using Assets.scripts.Skills.SkillEffects;
using UnityEngine;

namespace Assets.scripts.Skills.ActiveSkills
{
	public class SkillTestProjectileTriple : SkillTestProjectile
	{
		public SkillTestProjectileTriple(string name, int id) : base(name, id)
		{
			castTime = 1f;
			reuse = 0.5f;
			coolDown = 0.5f;
		}

		public override Skill Instantiate()
		{
			return new SkillTestProjectileTriple(Name, Id);
		}

		public override void OnLaunch()
		{
			if (particleSystemObject != null)
				Object.Destroy(particleSystemObject);

			GameObject activeProjectile;

			int angle = -15;
			for (int i = 0; i < 3; i++)
			{
				activeProjectile = GetPlayerData().CreateProjectile("SkillTestProjectile", "projectile_blacktest_i00");

				if (activeProjectile != null)
				{
					Rigidbody2D rb = activeProjectile.GetComponent<Rigidbody2D>();
					rb.velocity = GetOwnerData().GetForwardVector(angle)*15;
					angle += 15;

					Debug.DrawRay(GetOwnerData().GetShootingPosition().transform.position, rb.velocity, Color.green, 5f);

					AddMonoReceiver(activeProjectile);

					Object.Destroy(activeProjectile, 5f);
				}
			}
		}
	}
}
