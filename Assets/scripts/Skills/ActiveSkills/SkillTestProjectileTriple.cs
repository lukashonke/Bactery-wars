using Assets.scripts.Skills.Base;
using Assets.scripts.Skills.SkillEffects;
using UnityEngine;

namespace Assets.scripts.Skills.ActiveSkills
{
	public class SkillTestProjectileTriple : SkillTestProjectile
	{
		public SkillTestProjectileTriple()
		{
			castTime = 0;
			reuse = 0.5f;
			coolDown = 0.5f;
			baseDamage = 10;

			requireConfirm = true;
		}

		public override SkillId GetSkillId()
		{
			return SkillId.SkillTestProjectileTriple;
		}

		public override string GetVisibleName()
		{
			return "Triple Projectile";
		}

		public override Skill Instantiate()
		{
			return new SkillTestProjectileTriple();
		}

		public override void OnBeingConfirmed()
		{
			if (confirmObject == null)
			{
				confirmObject = GetPlayerData().CreateSkillResource(GetName(), "directionarrow", true, GetOwnerData().GetShootingPosition().transform.position);

				UpdateMouseDirection(confirmObject.transform);
				confirmObject.transform.rotation = Utils.GetRotationToDirectionVector(mouseDirection);

				GameObject arrow2 = GetPlayerData().CreateSkillResource(GetName(), "directionarrow", true, GetOwnerData().GetShootingPosition().transform.position);
				arrow2.transform.rotation = Utils.GetRotationToDirectionVector(Utils.RotateDirectionVector(mouseDirection, -15));
				arrow2.transform.parent = confirmObject.transform;

				GameObject arrow3 = GetPlayerData().CreateSkillResource(GetName(), "directionarrow", true, GetOwnerData().GetShootingPosition().transform.position);
				arrow3.transform.rotation = Utils.GetRotationToDirectionVector(Utils.RotateDirectionVector(mouseDirection, 15));
				arrow3.transform.parent = confirmObject.transform;
			}

			UpdateMouseDirection(confirmObject.transform);
			confirmObject.transform.rotation = Utils.GetRotationToDirectionVector(mouseDirection);
		}

		public override void OnLaunch()
		{
			DeleteCastingEffect();

			GameObject activeProjectile;

			//Debug.DrawRay(GetOwnerData().GetShootingPosition().transform.position, GetOwnerData().GetForwardVector()*15, Color.green, 5f);
			//Debug.DrawRay(GetOwnerData().GetShootingPosition().transform.position, direction, Color.blue, 5f);

			int angle = -15;
			for (int i = 0; i < 3; i++)
			{
				activeProjectile = CreateSkillProjectile(GetName(), "projectile_blacktest_i00", true);

				if (activeProjectile != null)
				{
					Rigidbody2D rb = activeProjectile.GetComponent<Rigidbody2D>();
					rb.velocity = Utils.RotateDirectionVector(GetOwnerData().GetForwardVector(), angle) * 15;
					angle += 15;

					Debug.DrawLine(GetOwnerData().GetShootingPosition().transform.position, rb.velocity, Color.green, 5f);

					Object.Destroy(activeProjectile, 5f);
				}
			}
		}
	}
}
