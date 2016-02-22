using System.Collections;
using System.Collections.Generic;
using System.Net;
using Assets.scripts.Actor;
using Assets.scripts.Base;
using Assets.scripts.Mono;
using Assets.scripts.Skills.Base;
using Assets.scripts.Skills.SkillEffects;
using UnityEngine;

namespace Assets.scripts.Skills.ActiveSkills
{
	public class CommonColdAutoattack : ActiveSkill
	{
		private GameObject target;

		private GameObject activeProjectile;

		public int projectileForce = 30;
		public int deviationAngle = 0;
		public int doubleAttackChance = 0;
		public int consecutiveDoubleattackCounter = 0;
		public int doubleAttackProjectileCount = 1;
		public bool toAllDirections = false;
		public bool thunder = false;

		public int shotgunChance = 0;
		public int consecutiveShotgunCounter = 0;
		public int shotgunProjectilesCount = 0;

		private int dsCounter = 0;
		private int shotgunCounter = 0;

		public CommonColdAutoattack()
		{
			castTime = 0;
			reuse = 1f;
			coolDown = 0f;
			requireConfirm = false;
			baseDamage = 10;

			range = 10;
		}

		public override SkillId GetSkillId()
		{
			return SkillId.CommonColdAutoattack;
		}

		public override string GetVisibleName()
		{
			return "Common Cold Melee";
		}

		public override Skill Instantiate()
		{
			return new CommonColdAutoattack();
		}

		public override SkillEffect[] CreateEffects(int param)
		{
			return new SkillEffect[] {new EffectDamage(baseDamage, 5)};
		}

		public override void InitTraits()
		{
			AddTrait(SkillTraits.Damage);
			AddTrait(SkillTraits.Melee);
		}

		public override bool OnCastStart()
		{
			if (initTarget != null)
			{
				target = initTarget;
				RotatePlayerTowardsTarget(initTarget);
			}
			else
			{
				target = null;
				RotatePlayerTowardsMouse();
			}

			if(castTime > 0)
				CreateCastingEffect(true, "SkillTemplate");

			return true;
		}

		private void CalcThunderTargets(Vector3 from, int range, Vector3 direction)
		{
			RaycastHit2D[] hits = Physics2D.RaycastAll(from, direction, range);

			foreach (RaycastHit2D hit in hits)
			{
				Character ch = hit.collider.gameObject.GetChar();
				if (ch == null)
				{
					Destroyable d = hit.collider.gameObject.GetComponent<Destroyable>();
					if (d != null && !Owner.CanAttack(d))
						continue;
				}
				else if (!Owner.CanAttack(ch))
					continue;

				// the only possible collisions are the projectile with target
				ApplyEffects(Owner, hit.collider.gameObject);
			}
		}

		public override void OnLaunch()
		{
			int range = GetUpgradableRange();

			DeleteCastingEffect();

			if(consecutiveDoubleattackCounter > 0)
				dsCounter ++;
			if(consecutiveShotgunCounter > 0)
				shotgunCounter ++;

			if (target != null)
			{
				RotatePlayerTowardsTarget(target);
			}
			else
			{
				RotatePlayerTowardsMouse();
			}

			// double shoot
			if (doubleAttackChance == 0 && consecutiveDoubleattackCounter == 0 && shotgunChance == 0 && consecutiveShotgunCounter == 0 )
			{
				if (!toAllDirections)
				{
					if (thunder)
					{
						GameObject thunderObject = CreateSkillObject("Thunder", true, false, GetOwnerData().GetShootingPosition().transform.position);
						if (thunderObject != null)
						{
							LineRenderer line = thunderObject.GetComponent<LineRenderer>();
							line.SetPosition(1, new Vector3(0, range+2, 0));
							thunderObject.transform.rotation = Owner.GetData().GetBody().transform.rotation;

							CalcThunderTargets(GetOwnerData().GetShootingPosition().transform.position, range + 1, GetOwnerData().GetForwardVector());

							Object.Destroy(thunderObject, 0.25f);
						}
					}
					else
					{
						GameObject activeProjectile = CreateSkillProjectile("projectile_00", true);
						if (activeProjectile != null)
						{
							Rigidbody2D rb = activeProjectile.GetComponent<Rigidbody2D>();
							rb.velocity = (GetOwnerData().GetForwardVector(Random.Range(-deviationAngle, deviationAngle)) * projectileForce);

							Object.Destroy(activeProjectile, 5f);
						}
					}
				}
				else
				{
					for (int i = 0; i < 4; i++)
					{
						if (thunder)
						{
							GameObject thunderObject = CreateSkillObject("Thunder", true, false, GetOwnerData().GetShootingPosition().transform.position);
							if (thunderObject != null)
							{
								LineRenderer line = thunderObject.GetComponent<LineRenderer>();
								line.SetPosition(1, new Vector3(0, range + 2, 0));
								thunderObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, GetOwnerData().GetBody().transform.rotation.eulerAngles.z + i*90));

								CalcThunderTargets(GetOwnerData().GetShootingPosition().transform.position, range + 1, GetOwnerData().GetForwardVector(i*90));

								Object.Destroy(thunderObject, 0.25f);
							}
						}
						else
						{
							GameObject activeProjectile = CreateSkillProjectile("projectile_00", true);
							if (activeProjectile != null)
							{
								Rigidbody2D rb = activeProjectile.GetComponent<Rigidbody2D>();
								rb.velocity = (GetOwnerData().GetForwardVector(i * 90) * projectileForce);

								Object.Destroy(activeProjectile, 5f);
							}
						}
					}
				}
			}
			else
			{
				bool shotgun = false;

				int count = 1;
				if (doubleAttackChance > 0 && Random.Range(0, 100) < doubleAttackChance)
				{
					count = doubleAttackProjectileCount;
				}
				else if (consecutiveDoubleattackCounter > 0 && dsCounter >= consecutiveDoubleattackCounter)
				{
					dsCounter = 0;
					count = doubleAttackProjectileCount;
				}
				else if (shotgunChance > 0 && Random.Range(0, 100) < shotgunChance)
				{
					shotgun = true;
					count = shotgunProjectilesCount;
				}
				else if (consecutiveShotgunCounter > 0 && shotgunCounter >= consecutiveShotgunCounter)
				{
					shotgun = true;
					shotgunCounter = 0;
					count = shotgunProjectilesCount;
				}

				if (!shotgun)
				{
					float wait = 0;
					for (int i = 0; i < count; i++)
					{
						for (int j = 0; j < 4; j++)
						{
							Owner.StartTask(ShootDelayedProjectile(wait, GetOwnerData().GetForwardVector(j*90 + Random.Range(-deviationAngle, deviationAngle))*projectileForce));
						}

						wait += 0.05f;
					}
				}
				else
				{
					int temp = 30/count;
					for (int i = 0; i < count; i++)
					{
						for (int j = 0; j < 4; j++)
						{
							GameObject activeProjectile = CreateSkillProjectile("projectile_00", true);
							if (activeProjectile != null)
							{
								Rigidbody2D rb = activeProjectile.GetComponent<Rigidbody2D>();
								rb.velocity = (GetOwnerData().GetForwardVector(j * 90 + (0 - (count / 2) * temp) + (i * temp)) * projectileForce);

								Object.Destroy(activeProjectile, 5f);
							}
						}
					}
				}
			}
		}

		private IEnumerator ShootDelayedProjectile(float time, Vector3 dir)
		{
			if(time > 0)
				yield return new WaitForSeconds(time);

			GameObject activeProjectile = CreateSkillProjectile("projectile_00", true);
			if (activeProjectile != null)
			{
				Rigidbody2D rb = activeProjectile.GetComponent<Rigidbody2D>();
				rb.velocity = dir;

				Object.Destroy(activeProjectile, 5f);
			}
		}

		public override void OnFinish()
		{

		}

		public override void OnAfterEnd()
		{
			if (castTime > 0 && target != null)
			{
				// continue with next attack
				if (GetOwnerData().RepeatingMeleeAttack)
					GetOwnerData().MeleeInterract(target, true);
			}
		}

		public override void OnAterReuse()
		{
			if (target != null)
			{
				// continue with next attack
				if (GetOwnerData().RepeatingMeleeAttack)
					GetOwnerData().MeleeInterract(target, true);
			}
		}

		public override void OnMove()
		{
			 AbortCast();
		}

		public override void MonoUpdate(GameObject gameObject, bool fixedUpdate)
		{
		}

		public override void MonoStart(GameObject gameObject)
		{
			
		}

		public override void MonoDestroy(GameObject gameObject)
		{
		}

		public override void MonoTriggerEnter(GameObject gameObject, Collider2D coll)
		{
			if (coll.gameObject.Equals(GetOwnerData().GetBody()))
				return;

			Character ch = coll.gameObject.GetChar();
			if (ch == null)
			{
				Destroyable d = coll.gameObject.GetComponent<Destroyable>();
				if (d != null && !Owner.CanAttack(d))
					return;
			}
			else if (!Owner.CanAttack(ch))
				return;

			// the only possible collisions are the projectile with target
			ApplyEffects(Owner, coll.gameObject);
			DestroyProjectile(gameObject);
		}

		public override void MonoTriggerExit(GameObject gameObject, Collider2D other)
		{
			
		}

		public override void MonoTriggerStay(GameObject gameObject, Collider2D other)
		{
			
		}

		public override void MonoCollisionEnter(GameObject gameObject, Collision2D coll)
		{
			
		}

		public override void MonoCollisionExit(GameObject gameObject, Collision2D coll)
		{
		}

		public override void MonoCollisionStay(GameObject gameObject, Collision2D coll)
		{
		}

		public override void UpdateLaunched()
		{

		}

		public override void OnAbort()
		{
		}

		public override bool CanMove()
		{
			return !IsActive() && !IsBeingCasted();
		}

		public override bool CanRotate()
		{
			return CanMove();
		}
	}
}
