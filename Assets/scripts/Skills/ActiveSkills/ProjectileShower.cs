using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.AI;
using Assets.scripts.Mono;
using Assets.scripts.Skills.Base;
using Assets.scripts.Skills.SkillEffects;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Assets.scripts.Skills.ActiveSkills
{
	public class ProjectileShower : ActiveSkill
	{
		protected GameObject effect;

		private float lastAction = 0;
		private Vector3 aimingDirection;

		// jak rychle muze menit smer
		public float rotateSpeed = 20;

		public int projectilesCount = 4;
		public int projectilesAngleRandomAdd = 20;
		public int projectilesAngle = 10;
		public int force = 15;
		public float fireFrequency = 0.1f;

		public int randomAngleChange = 2;

		public ProjectileShower()
		{
			castTime = 0f;
			coolDown = 4f;
			reuse = 1f;

			baseDamage = 1;

			range = 15;

			movementAbortsSkill = true;

			updateFrequency = 0.01f;
			requireConfirm = true;
			AvailableToPlayer = true;
		}

		public override SkillId GetSkillId()
		{
			return SkillId.ProjectileShower;
		}

		public override string GetVisibleName()
		{
			return "Projectile Shower";
		}

		public override Skill Instantiate()
		{
			return new ProjectileShower();
		}

		public override string GetDescription()
		{
			return "Fires a shower of projectiles.";
		}

		public override SkillEffect[] CreateEffects(int param)
		{
			return new SkillEffect[] { new EffectDamage(10), new EffectPushaway(50),   };
		}

		public override void InitTraits()
		{
			AnalyzeEffectsForTrais();
		}

		public override bool OnCastStart()
		{
			return true;
		}

		public override void OnLaunch()
		{
			DeleteCastingEffect();

			/*GameObject activeProjectile;

			for (int i = 0; i < projectilesCount; i++)
			{
				activeProjectile = CreateSkillProjectile("projectile_blacktest_i00", true);
				if (activeProjectile != null)
				{
					Rigidbody2D rb = activeProjectile.GetComponent<Rigidbody2D>();

					rb.velocity = (GetOwnerData().GetForwardVector((Random.Range(-projectilesAngleRandomAdd, projectilesAngleRandomAdd) + CalcAngleForProjectile(i, projectilesCount, projectilesAngle))) * force);

					Object.Destroy(activeProjectile, GetProjectileLifetime(force));
				}
			}*/

			RotatePlayerTowardsMouse();

			if(effect != null)
				StartParticleEffect(effect);

			if (GetPlayerData() != null)
			{
				UpdateMouseDirection();
				aimingDirection = mouseDirection;

				if(effect != null)
					effect.transform.rotation = Utils.GetRotationToMouse(effect.transform);
			}
			else if (initTarget != null)
			{
				aimingDirection = Utils.GetDirectionVector(initTarget.transform.position, GetOwnerData().GetBody().transform.position);

				if (effect != null)
					effect.transform.rotation = Utils.GetRotationToTarget(effect.transform, initTarget);
			}

			lastAction = 0;
		}

		public override void UpdateLaunched()
		{
			Vector3 target;

			if (GetPlayerData() != null)
			{
				target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			}
			else if (initTarget != null)
			{
				target = initTarget.transform.position;
			}
			else
			{
				AbortCast();
				return;
			}

			Quaternion newRotation = Quaternion.LookRotation(GetOwnerData().GetBody().transform.position - target, Vector3.forward);
			newRotation.x = 0;
			newRotation.y = 0;
			newRotation = Quaternion.Lerp(GetOwnerData().GetBody().transform.rotation, newRotation, rotateSpeed * 0.001f);

			GetOwnerData().SetRotation(newRotation, true);

			if (GetPlayerData() != null)
			{
				UpdateMouseDirection();
				//UpdateMouseDirection(effect.transform);
				aimingDirection = Vector3.Lerp(aimingDirection, mouseDirection, rotateSpeed * 0.001f);
			}
			else if (initTarget != null)
			{
				aimingDirection = Vector3.Lerp(aimingDirection, Utils.GetDirectionVector(initTarget.transform.position, GetOwnerData().GetBody().transform.position), rotateSpeed * 0.001f);
			}

			if(effect != null)
				effect.transform.rotation = Utils.GetRotationToDirectionVector(aimingDirection);

			if (lastAction + fireFrequency < Time.time)
			{
				lastAction = Time.time;

				GameObject activeProjectile;

				for (int i = 0; i < projectilesCount; i++)
				{
					activeProjectile = CreateSkillProjectile("projectile_blacktest_i00", true);
					if (activeProjectile != null)
					{
						Rigidbody2D rb = activeProjectile.GetComponent<Rigidbody2D>();

						rb.velocity = (GetOwnerData().GetForwardVector((Random.Range(-projectilesAngleRandomAdd, projectilesAngleRandomAdd) + CalcAngleForProjectile(i, projectilesCount, projectilesAngle))) * force);

						Object.Destroy(activeProjectile, GetProjectileLifetime(force));
					}
				}
			}
		}

		public override void OnAbort()
		{
		}

		public override void OnFinish()
		{
			if(effect != null)
				DeleteParticleEffect(effect);
		}

		public override void MonoUpdate(GameObject gameObject, bool fixedUpdate)
		{
			if (randomAngleChange > 0)
			{
				Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D>();
				if (rb == null)
					return;

				Vector3 currentVelocity = rb.velocity.normalized;

				currentVelocity = Utils.RotateDirectionVector(currentVelocity, Random.Range(-randomAngleChange, randomAngleChange + 1));

				rb.velocity = currentVelocity * 15;
			}
		}

		public override void MonoTriggerEnter(GameObject gameObject, Collider2D coll)
		{
			if (coll.gameObject.Equals(GetOwnerData().GetBody()))
				return;

			Character ch = coll.gameObject.GetChar();

			if (ch == null)
			{
				Destroyable des = coll.gameObject.GetComponent<Destroyable>();
				if (des != null && !Owner.CanAttack(des))
					return;
			}
			else if (!Owner.CanAttack(ch))
				return;

			ApplyEffects(Owner, coll.gameObject);
			DestroyProjectile(gameObject);
		}

		public override bool CanMove()
		{
			return !IsActive();
		}

		public override bool CanRotate()
		{
			return !IsActive();
		}
	}
}
