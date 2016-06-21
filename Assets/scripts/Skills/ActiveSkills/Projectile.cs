// Copyright (c) 2015, Lukas Honke
// ========================
using System.Collections.Generic;
using Assets.scripts.Actor;
using Assets.scripts.Base;
using Assets.scripts.Mono;
using Assets.scripts.Skills.Base;
using Assets.scripts.Skills.SkillEffects;
using UnityEngine;

namespace Assets.scripts.Skills.ActiveSkills
{
	public class Projectile : ActiveSkill
	{
		private GameObject activeProjectile;

		// pokud true, tak projektil bude ignorovat pratelske objekty a projde jimi jako kdyby tam nebyli
		public bool penetrateFriendly = true;

		// jak rychle se projektil pohybuje, default 15
		public int force = 15;

		// kolik projektilu je vystreleno
		public int projectilesCount = 1;

		// pokud jich je vic, jaky velky uhel mezi temito projektily je
		public int projectilesAngle = 0;

		// nahodne cislo 0 az tato hodnota, ktera se muze pricist nebo odecist od "projectileAngle"
		public int projectilesAngleRandomAdd = 0;

		// int pocet targetu, ktery dany projektil muze penetrovat
		public int penetrateTargets = 0;

		// pri kazdem penetrovani targetu se kazdy dalsi damage pri zasahu vynasobi timto cislem (0.5 = kazdym penetrovanim se damage snizi o polovinu)
		public float penetrateChangeDamage = 0.5f;

		// kdyz true, tak bude projektil automaticky navigovan k targetu
		public bool navigateToTarget = false;

		// jak rychle meni smer pri automatickem navigovani
		public float navigateAimRate = 0.1f;

		// jak daleko pred sebe "vidi" - jak daleko predem zameruje targety, atd.
		public float navigateAimArea = 15f;

		//TODO make better
		public bool navigateChangeTargetAfterHit = true;

		// pri automatickem navigovani skenuje nove targety pouze v oblasti pred sebou (ne okolo sebe)
		public bool navigateLookOnlyForward = true;

		public bool navigateUnreliable = true;

		// pri narazu do nepritele vybouchne - spusti explodeEffectName
		public bool explodeEffect = true;

		// jmeno efektu ktery se spusti pri explozi
		public string explodeEffectName = "Explosion";

		//TODO boomerang? 

		private List<ProjectileData> projectiles = new List<ProjectileData>();

		public Projectile()
		{
			castTime = 0f;
			reuse = 1;
			coolDown = 0;
			requireConfirm = true;
			baseDamage = 10;

			range = 20;
		}

		public override SkillId GetSkillId()
		{
			return SkillId.Projectile;
		}

		public override string GetVisibleName()
		{
			return "Projectile";
		}

		public override Skill Instantiate()
		{
			return new Projectile();
		}

		public override SkillEffect[] CreateEffects(int param)
		{
			if (param > 0)
			{
				return new SkillEffect[] { new EffectDamage((int) (baseDamage * (Mathf.Pow(penetrateChangeDamage, param))), 1) };
			}
			else
			{
				return new SkillEffect[] {new EffectDamage(baseDamage, 1)};
			}
		}

		public override void InitTraits()
		{
			AddTrait(SkillTraits.Damage);
		}

		public override bool OnCastStart()
		{
			RotatePlayerTowardsMouse();

			CreateCastingEffect(true, GetName());

			return true;
		}

		public override void OnLaunch()
		{
			DeleteCastingEffect();

			GameObject activeProjectile;

			for (int i = 0; i < projectilesCount; i++)
			{
				activeProjectile = CreateSkillProjectile("projectile_blacktest_i00", true);
				if (activeProjectile != null)
				{
					Rigidbody2D rb = activeProjectile.GetComponent<Rigidbody2D>();

					rb.velocity = (GetOwnerData().GetForwardVector((Random.Range(-projectilesAngleRandomAdd, projectilesAngleRandomAdd) + CalcAngleForProjectile(i, projectilesCount, projectilesAngle))) * force);

					Object.Destroy(activeProjectile, GetProjectileLifetime(force));

					if (SaveProjectiles())
					{
						ProjectileData data = new ProjectileData();
						data.proj = activeProjectile;
						data.target = null;
						data.interpolTimer = 0;
						data.rb = rb;
						data.penetratedTargets = 0;

						projectiles.Add(data);
					}
				}
			}
		}

		public override void OnFinish()
		{

		}

		private float lastUpdate;
		private float updateInterval = 0.2f;
		
		private float chooseTargetInterval = 0.25f;

		public override void MonoUpdate(GameObject gameObject, bool fixedUpdate)
		{
			if (navigateToTarget)
			{
				if (fixedUpdate)
				{
					if (lastUpdate + updateInterval <= Time.time)
					{
						ProjectileData d = GetProjectileData(gameObject);
						if (d == null || d.proj == null)
							return;

						lastUpdate = Time.time;

						// find new target
						if (d.target == null)
						{
							if (d.lastChooseTarget + chooseTargetInterval < Time.time)
							{
								d.lastChooseTarget = Time.time;

								if (!navigateLookOnlyForward)
								{
									float angle = d.proj.transform.rotation.eulerAngles.z;

									foreach (Collider2D c in Physics2D.OverlapCircleAll(d.proj.transform.position, navigateAimArea))
									{
										if (c.gameObject.Equals(d.proj))
											continue;

										Character targetCh = c.gameObject.GetChar();
										if (targetCh == null || !Owner.CanAttack(targetCh))
											continue;

										d.target = targetCh;
										break;
									}
								}
								else // scan only forward targets
								{
									float angle = d.proj.transform.rotation.eulerAngles.z;

									Vector3 direction = new Vector3(d.rb.velocity.x, d.rb.velocity.y);
									Vector3 perpend = Utils.GetPerpendicularVector(d.proj.transform.position, direction + d.proj.transform.position);

									Debug.DrawRay(d.proj.transform.position, perpend*10, Color.red, 1f);

									Debug.DrawLine(d.proj.transform.position, d.proj.transform.position + perpend*navigateAimArea/2f + direction.normalized * navigateAimArea, Color.green, 1f);
									Debug.DrawLine(d.proj.transform.position, d.proj.transform.position + perpend * (-navigateAimArea / 2f), Color.blue, 1f);

									foreach (Collider2D c in Physics2D.OverlapAreaAll(d.proj.transform.position + perpend * navigateAimArea / 2f + direction.normalized * navigateAimArea, d.proj.transform.position + perpend * (-navigateAimArea / 2f)))
									{
										if (c.gameObject.Equals(d.proj))
											continue;

										Character targetCh = c.gameObject.GetChar();
										if (targetCh == null || !Owner.CanAttack(targetCh))
											continue;

										d.target = targetCh;
										break;
									}
								}
							}
							
						}

						if(d.target != null)
							AdjustToTarget(d);
					}
				}
			}
		}

		private void AdjustToTarget(ProjectileData data)
		{
			if (data.target.Status.IsDead)
			{
				data.target = null;
				return;
			}

			Vector3 currentVelocity = data.rb.velocity.normalized;
			Vector3 targetDir = Utils.GetDirectionVector(data.target.GetData().GetBody().transform.position, data.proj.transform.position).normalized;

			data.interpolTimer += navigateAimRate;

			if (data.interpolTimer > 1)
				data.interpolTimer = 1f;

			Vector3 newDir = Vector3.Lerp(currentVelocity, targetDir, data.interpolTimer);

			if (navigateUnreliable)
				newDir = Utils.RotateDirectionVector(newDir, Random.Range(-30, 30));

			data.rb.velocity = newDir * 15;
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

			ProjectileData d = GetProjectileData(gameObject);

			// if projectile was saved, it means special effects are to be applied
			if (d != null)
			{
				if (penetrateFriendly)
				{
					if (ch == null)
					{
						Destroyable des = coll.gameObject.GetComponent<Destroyable>();
						if (des != null && !Owner.CanAttack(des))
							return;
					}
					else if (!Owner.CanAttack(ch))
						return;
				}

				// missile hit target
				if (d.target != null)
				{
					if (coll.gameObject.Equals(d.target.GetData().GetBody()))
					{
						d.target = null;
					}
				}

				if (penetrateTargets > 0 && d.penetratedTargets > 0 && (penetrateChangeDamage < 0 || penetrateChangeDamage > 0))
					ApplyEffects(Owner, coll.gameObject, false, d.penetratedTargets);
				else
					ApplyEffects(Owner, coll.gameObject);

				bool destroy = false;

				if (penetrateTargets > 0)
				{
					// check if penetrated too many
					d.penetratedTargets++;
					if (d.penetratedTargets >= penetrateTargets)
						destroy = true;

					// find next target
					if (!destroy && navigateChangeTargetAfterHit)
					{
						float angle = d.proj.transform.rotation.eulerAngles.z;
						if (!navigateLookOnlyForward)
						{
							/*foreach (Collider2D hit in Physics2D.OverlapCircleAll(d.proj.transform.position, navigateAimArea))
							{
								if (hit.gameObject.Equals(d.proj))
									continue;

								Character targetCh = hit.gameObject.GetChar();
								if (targetCh == null || !Owner.CanAttack(targetCh) || hit.gameObject.Equals(coll.gameObject))
									continue;

								d.target = targetCh;
								AdjustToTarget(d);
								break;
							}*/

							foreach (Collider2D c in Physics2D.OverlapCircleAll(d.proj.transform.position, navigateAimArea))
							{
								if (c.gameObject.Equals(d.proj))
									continue;

								Character targetCh = c.gameObject.GetChar();
								if (targetCh == null || !Owner.CanAttack(targetCh) || c.gameObject.Equals(coll.gameObject))
									continue;

								d.target = targetCh;
								AdjustToTarget(d);
								break;
							}
						}
						else
						{
							Vector3 direction = new Vector3(d.rb.velocity.x, d.rb.velocity.y);
							Vector3 perpend = Utils.GetPerpendicularVector(d.proj.transform.position, direction + d.proj.transform.position);

							foreach (Collider2D c in Physics2D.OverlapAreaAll(d.proj.transform.position + perpend * navigateAimArea / 2f + direction.normalized * navigateAimArea, d.proj.transform.position + perpend * (-navigateAimArea / 2f)))
							{
								if (c.gameObject.Equals(d.proj))
									continue;

								Character targetCh = c.gameObject.GetChar();
								if (targetCh == null || !Owner.CanAttack(targetCh) || c.gameObject.Equals(coll.gameObject))
									continue;

								d.target = targetCh;
								AdjustToTarget(d);
								break;
							}
						}
					}
				}

				if (penetrateTargets == 0 || destroy)
				{
					if (explodeEffect)
					{
						GameObject explosion = CreateParticleEffect(explodeEffectName, false, gameObject.transform.position);
						explosion.GetComponent<ParticleSystem>().Play();
						Object.Destroy(explosion, 2f);
					}

					DestroyProjectile(gameObject);
				}
			}
			else
			{
				if (penetrateFriendly)
				{
					if (ch == null)
					{
						Destroyable des = coll.gameObject.GetComponent<Destroyable>();
						if (des != null && !Owner.CanAttack(des))
							return;
					}
					else if (!Owner.CanAttack(ch))
						return;
				}

				ApplyEffects(Owner, coll.gameObject);
				DestroyProjectile(gameObject);
			}
		}

		private ProjectileData GetProjectileData(GameObject obj)
		{
			foreach (ProjectileData d in projectiles)
			{
				if (d.proj.Equals(obj))
					return d;
			}
			return null;
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
			if (IsBeingCasted())
				return false;
			return true;
		}

		public override bool CanRotate()
		{
			return CanMove();
		}

		private bool SaveProjectiles()
		{
			return penetrateTargets > 0 || navigateToTarget; // || .. ; //TODO
		}
	}

	public class ProjectileData
	{
		public GameObject proj;
		public Character target;
		public Rigidbody2D rb;
		public float interpolTimer;
		public int penetratedTargets;

		public float lastUpdateTime;
		public float lastChooseTarget;
	}
}
