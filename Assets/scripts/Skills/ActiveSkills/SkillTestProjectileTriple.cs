using Assets.scripts.Skills.SkillEffects;
using UnityEngine;

namespace Assets.scripts.Skills.ActiveSkills
{
	public class SkillTestProjectileTriple : SkillTestProjectile
	{
		public SkillTestProjectileTriple(string name, int id) : base(name, id)
		{
			castTime = 0;
			reuse = 0.5f;
			coolDown = 0.5f;
			requireConfirm = true;
			MovementBreaksConfirmation = true;
		}

		public override Skill Instantiate()
		{
			return new SkillTestProjectileTriple(Name, Id);
		}

		public override void OnBeingConfirmed()
		{
			if (confirmObject == null)
			{
				confirmObject = GetPlayerData().CreateSkillResource("SkillTestProjectile", "directionarrow", true, GetOwnerData().GetShootingPosition().transform.position);

				UpdateMouseDirection(confirmObject.transform);
				confirmObject.transform.rotation = Utils.GetRotationToDirectionVector(mouseDirection);

				GameObject arrow2 = GetPlayerData().CreateSkillResource("SkillTestProjectile", "directionarrow", true, GetOwnerData().GetShootingPosition().transform.position);
				arrow2.transform.rotation = Utils.GetRotationToDirectionVector(Utils.RotateDirectionVector(mouseDirection, -15));
				arrow2.transform.parent = confirmObject.transform;

				GameObject arrow3 = GetPlayerData().CreateSkillResource("SkillTestProjectile", "directionarrow", true, GetOwnerData().GetShootingPosition().transform.position);
				arrow3.transform.rotation = Utils.GetRotationToDirectionVector(Utils.RotateDirectionVector(mouseDirection, 15));
				arrow3.transform.parent = confirmObject.transform;
			}

			UpdateMouseDirection(confirmObject.transform);
			confirmObject.transform.rotation = Utils.GetRotationToDirectionVector(mouseDirection);
		}

		public override void OnLaunch()
		{
			if (particleSystemObject != null)
				Object.Destroy(particleSystemObject);

			GameObject activeProjectile;

			//Debug.DrawRay(GetOwnerData().GetShootingPosition().transform.position, GetOwnerData().GetForwardVector()*15, Color.green, 5f);
			//Debug.DrawRay(GetOwnerData().GetShootingPosition().transform.position, direction, Color.blue, 5f);

			int angle = -15;
			for (int i = 0; i < 3; i++)
			{
				activeProjectile = GetPlayerData().CreateProjectile("SkillTestProjectile", "projectile_blacktest_i00");

				if (activeProjectile != null)
				{
					Rigidbody2D rb = activeProjectile.GetComponent<Rigidbody2D>();
					rb.velocity = Utils.RotateDirectionVector(mouseDirection.normalized, angle) * 15;
					angle += 15;

					Debug.DrawLine(GetOwnerData().GetShootingPosition().transform.position, rb.velocity, Color.green, 5f);

					AddMonoReceiver(activeProjectile);

					Object.Destroy(activeProjectile, 5f);
				}
			}
		}
	}
}
