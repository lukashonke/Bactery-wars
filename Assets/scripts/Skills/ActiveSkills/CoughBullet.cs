// Copyright (c) 2015, Lukas Honke
// ========================
using System.Collections.Generic;
using System.Linq;
using Assets.scripts.Actor;
using Assets.scripts.Base;
using Assets.scripts.Mono;
using Assets.scripts.Skills.Base;
using Assets.scripts.Skills.SkillEffects;
using UnityEngine;

namespace Assets.scripts.Skills.ActiveSkills
{
	public class CoughBullet : ActiveSkill
	{
		public int force = 2;

		public int projectilesCount = 1;
		public bool navigateToTarget = true;
		public int projectilesAngle = 0;
		public bool penetrateTargets = true;
		public int randomAngle = 0;
		public int nullReuseChance = 0;

		public float navigateAimRate = 0.1f;
		public float navigateAimArea = 20f;

		public bool navigateUnreliable = false;
		public string explodeEffectName = "Explosion";

		public bool navigateLookOnlyForward = false;

		public float penetrateChangeDamage = 1.0f;
		public int maxPenetratedTargets = 2;
		public bool navigateChangeTargetAfterHit = true;

		public bool explodeEffect = false;

		public bool returnBack = false;

		public bool chainEffect = false;
		public int chainProjectilesCount = 1;
		public int chainPenetrations = 1;

		private bool returningBack = false;
		private List<ProjectileData> projectiles = new List<ProjectileData>();

		public CoughBullet()
		{
			castTime = 0f;
			reuse = 4f;
			coolDown = 0;
			requireConfirm = false;
			baseDamage = 10;

			range = 25;

			AvailableToPlayer = true;
		}

		public override SkillId GetSkillId()
		{
			return SkillId.CoughBullet;
		}

		public override string GetVisibleName()
		{
			return "Cough Bullet";
		}

		public override string GetDescription()
		{
			return "Shoots a missile projectile that navigates itself to a random target, which it damages and continues to fly against a randomly chosen second target.";
		}

		public override string GetBaseInfo()
		{
			return "Reuse " + reuse + " sec | Damage " + baseDamage + " | Range" + range;
		}

		public override void DestroyProjectile(GameObject proj, float delay = 1)
		{
			if (!returnBack)
				base.DestroyProjectile(proj, delay);
			else
			{
				returningBack = true;

				ProjectileData d = GetProjectileData(proj);

				if(d != null)
					AdjustToTarget(d);
			}
		}

		private void ForceDestroyProjectile(GameObject proj)
		{
			base.DestroyProjectile(proj);
		}

		public override Skill Instantiate()
		{
			return new CoughBullet();
		}

		public override SkillEffect[] CreateEffects(int param)
		{
			if (param > 0)
			{
				return new SkillEffect[] { new EffectDamage((int)(baseDamage * (Mathf.Pow(penetrateChangeDamage, param))), 1) };
			}
			else
			{
				return new SkillEffect[] { new EffectDamage(baseDamage, 1) };
			}
		}

		public override void InitTraits()
		{
			AddTrait(SkillTraits.Damage);
		}

		public override bool OnCastStart()
		{
			RotatePlayerTowardsMouse();

			CreateCastingEffect(true, "SkillTemplate");

			return true;
		}

		public override void OnLaunch()
		{
			if (projectiles.Count > 0)
			{
				foreach (ProjectileData d in projectiles)
				{
					if (d.proj != null)
					{
						ForceDestroyProjectile(d.proj);
					}
				}
			}

			projectiles.Clear();

			DeleteCastingEffect();

			GameObject activeProjectile;

			for (int i = 0; i < projectilesCount; i++)
			{
				activeProjectile = CreateSkillProjectile("projectile_00", true);
				if (activeProjectile != null)
				{
					Rigidbody2D rb = activeProjectile.GetComponent<Rigidbody2D>();

					rb.velocity = (GetOwnerData().GetForwardVector((Random.Range(-randomAngle, randomAngle) + CalcAngleForProjectile(i, projectilesCount, projectilesAngle))) * force);

					Object.Destroy(activeProjectile, GetProjectileLifetime(force));

					if (SaveProjectiles())
					{
						ProjectileData data = new ProjectileData();
						data.proj = activeProjectile;
						data.target = null;
						data.interpolTimer = 0;
						data.rb = rb;
						data.penetratedTargets = 0;
						data.maxPenetrateTargets = maxPenetratedTargets;

						if(maxPenetratedTargets > 0)
							data.hits = new GameObject[maxPenetratedTargets];

						if(returnBack)
							data.waypoints.Push(Owner.GetData().GetBody().transform.position);

						projectiles.Add(data);
						MonoUpdate(rb.gameObject, true);
					}
				}
			}
		}

		public override void OnFinish()
		{

		}

		private float updateInterval = 0.1f;

		private float chooseTargetInterval = 0.15f;

		public override void MonoUpdate(GameObject gameObject, bool fixedUpdate)
		{
			if (returningBack)
			{
				ProjectileData d = GetProjectileData(gameObject);
				if (d == null || d.proj == null)
					return;

				if (d.lastUpdateTime + updateInterval <= Time.time)
				{
					d.lastUpdateTime = Time.time;

					if(!d.hasNextWaypoint)
						AdjustToTarget(d);

					if (d.hasNextWaypoint)
					{
						float dist = Utils.DistanceSqr(d.nextWaypoint, d.proj.transform.position);
						if (dist < 2*2)
						{
							d.hasNextWaypoint = false;
						}
					}
					else
					{
						returningBack = false;

						if (explodeEffect)
						{
							GameObject explosion = CreateParticleEffect(explodeEffectName, false, gameObject.transform.position);
							explosion.GetComponent<ParticleSystem>().Play();
							Object.Destroy(explosion, 2f);
						}

						ForceDestroyProjectile(gameObject);
						return;
					}
				}
			}

			if (navigateToTarget)
			{
				//if (fixedUpdate)
				{
					ProjectileData d = GetProjectileData(gameObject);
					if (d == null || d.proj == null)
						return;

					if (d.lastUpdateTime + updateInterval <= Time.time)
					{
						d.lastUpdateTime = Time.time;

						// find new target
						if (d.target == null)
						{
							//if (d.lastChooseTarget + chooseTargetInterval < Time.time)
							{
								d.lastChooseTarget = Time.time;

								if (!navigateLookOnlyForward)
								{
									float angle = d.proj.transform.rotation.eulerAngles.z;

									List<GameObject> candidates = new List<GameObject>();

									foreach (Collider2D c in Physics2D.OverlapCircleAll(d.proj.transform.position, navigateAimArea))
									{
										if (c.gameObject.Equals(d.proj))
											continue;

										if (d.HasHit(c.gameObject))
											continue;

										if (!Utils.CanSee(c.gameObject, d.proj.transform.position))
											continue;

										Character targetCh = c.gameObject.GetChar();
										if (targetCh == null || !Owner.CanAttack(targetCh))
											continue;

										candidates.Add(c.gameObject);
									}

									candidates.Sort((x, y) => (int)Utils.DistanceSqr(Owner.GetData().GetBody().transform.position, x.transform.position) - (int)Utils.DistanceSqr(Owner.GetData().GetBody().transform.position, y.transform.position));

									foreach (GameObject c in candidates)
									{
										Character targetCh = c.GetChar();
										if (targetCh == null || !Owner.CanAttack(targetCh))
											continue;

										d.target = targetCh;
										d.SaveHit(c.gameObject);
										AdjustToTarget(d);
										break;
									}
								}
								else // scan only forward targets
								{
									float angle = d.proj.transform.rotation.eulerAngles.z;

									Vector3 direction = new Vector3(d.rb.velocity.x, d.rb.velocity.y);
									Vector3 perpend = Utils.GetPerpendicularVector(d.proj.transform.position, direction + d.proj.transform.position);

									Debug.DrawRay(d.proj.transform.position, perpend * 10, Color.red, 1f);

									Debug.DrawLine(d.proj.transform.position, d.proj.transform.position + perpend * navigateAimArea / 2f + direction.normalized * navigateAimArea, Color.green, 1f);
									Debug.DrawLine(d.proj.transform.position, d.proj.transform.position + perpend * (-navigateAimArea / 2f), Color.blue, 1f);

									List<GameObject> candidates = new List<GameObject>();

									foreach (Collider2D c in Physics2D.OverlapAreaAll(d.proj.transform.position + perpend * navigateAimArea / 2f + direction.normalized * navigateAimArea, d.proj.transform.position + perpend * (-navigateAimArea / 2f)))
									{
										if (c.gameObject.Equals(d.proj))
											continue;

										if (d.HasHit(c.gameObject))
											continue;

										if (!Utils.CanSee(c.gameObject, d.proj.transform.position))
											continue;

										candidates.Add(c.gameObject);
									}

									candidates.Sort((x, y) => (int)Utils.DistanceSqr(Owner.GetData().GetBody().transform.position, x.transform.position) - (int)Utils.DistanceSqr(Owner.GetData().GetBody().transform.position, y.transform.position));

									foreach (GameObject c in candidates)
									{
										Character targetCh = c.GetChar();
										if (targetCh == null || !Owner.CanAttack(targetCh))
											continue;

										d.target = targetCh;
										d.SaveHit(c.gameObject);
										AdjustToTarget(d);
										break;
									}
								}
							}

						}

						if (d.target != null)
							AdjustToTarget(d);
					}
				}
			}
		}

		private void AdjustToTarget(ProjectileData data)
		{
			if (returningBack)
			{
				if (!data.hasNextWaypoint && data.waypoints.Count > 0)
				{
					data.nextWaypoint = data.waypoints.Pop();
					data.hasNextWaypoint = true;
				}

				if (data.hasNextWaypoint)
				{
					Vector3 target = data.nextWaypoint;

					Vector3 currentVelocity = data.rb.velocity.normalized;
					Vector3 targetDir = Utils.GetDirectionVector(target, data.proj.transform.position).normalized;

					data.interpolTimer += navigateAimRate;

					if (data.interpolTimer > 1)
						data.interpolTimer = 1f;

					Vector3 newDir = Vector3.Lerp(currentVelocity, targetDir, data.interpolTimer);

					if (navigateUnreliable)
						newDir = Utils.RotateDirectionVector(newDir, Random.Range(-30, 30));

					data.rb.velocity = newDir * 15;
				}
			}
			else
			{
				if (data.target.Status.IsDead || (data.ignore != null && data.ignore.Equals(data.target.GetData().GetBody())))
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
		}

		public class ProjectileData
		{
			public GameObject proj;
			public Character target;
			public Rigidbody2D rb;
			public float interpolTimer;
			public int penetratedTargets;
			public int maxPenetrateTargets;

			public float lastUpdateTime;
			public float lastChooseTarget;
			public GameObject[] hits;

			public Stack<Vector3> waypoints = new Stack<Vector3>();
			public Vector3 nextWaypoint;
			public bool hasNextWaypoint = false;
			public bool chained = false;
			public GameObject ignore = null;

			public bool HasHit(GameObject obj)
			{
				if (hits != null)
				{
					for (int i = 0; i < hits.Length; i++)
					{
						GameObject o = hits[i];
						if (obj.Equals(o))
							return true;
					}
				}
				return false;
			}

			public void SaveHit(GameObject obj)
			{
				if (penetratedTargets >= hits.Length)
					return;

				hits[penetratedTargets] = obj;
				waypoints.Push(obj.transform.position);
			}
		}

		private ProjectileData GetData(GameObject obj)
		{
			foreach (ProjectileData d in projectiles)
			{
				if (d.proj.Equals(obj))
					return d;
			}
			return null;
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
				if (ch == null)
				{
					Destroyable des = coll.gameObject.GetComponent<Destroyable>();

					if (des == null)
					{
						ForceDestroyProjectile(d.proj);
						return;
					}

					if (!Owner.CanAttack(des))
						return;
				}
				else if (!Owner.CanAttack(ch))
					return;

				// missile hit target
				if (d.target != null)
				{
					if (coll.gameObject.Equals(d.target.GetData().GetBody()))
					{
						d.target = null;
					}
				}

				if (d.ignore != null && d.ignore.Equals(coll.gameObject))
					return;

				if (chainEffect && !d.chained)
				{
					for (int i = 0; i < chainProjectilesCount; i++)
					{
						GameObject activeProjectile;

						activeProjectile = CreateSkillProjectile("projectile_00", true, d.proj.transform);
						if (activeProjectile != null)
						{
							Rigidbody2D rb = activeProjectile.GetComponent<Rigidbody2D>();

							Vector3 nv = Quaternion.Euler(new Vector3(0, 0, Random.Range(-90, 90)))*d.rb.velocity.normalized;
							rb.velocity = nv*force;

							Object.Destroy(activeProjectile, GetProjectileLifetime(force));

							if (SaveProjectiles())
							{
								ProjectileData data = new ProjectileData();
								data.proj = activeProjectile;
								data.target = null;
								data.interpolTimer = 0;
								data.rb = rb;
								data.penetratedTargets = 0;
								data.maxPenetrateTargets = chainPenetrations+1;
								data.chained = true;
								data.ignore = coll.gameObject;

								if (chainPenetrations > 0)
									data.hits = new GameObject[chainPenetrations+1];

								if (returnBack)
									data.waypoints.Push(activeProjectile.transform.position);

								data.SaveHit(coll.gameObject);
								data.penetratedTargets++;
								projectiles.Add(data);
								MonoUpdate(rb.gameObject, true);
							}
						}
					}
				}

				if (d.maxPenetrateTargets > 0 && d.penetratedTargets > 0 && (penetrateChangeDamage < 0 || penetrateChangeDamage > 0))
					ApplyEffects(Owner, coll.gameObject, false, d.penetratedTargets);
				else
					ApplyEffects(Owner, coll.gameObject);

				bool destroy = false;

				if (maxPenetratedTargets > 0 && d.maxPenetrateTargets > 0)
				{
					// check if penetrated too many
					d.penetratedTargets++;
					if (d.penetratedTargets >= d.maxPenetrateTargets)
						destroy = true;

					// find next target
					if (!destroy && navigateChangeTargetAfterHit)
					{
						ProjectileHitAnimation(d.proj);

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

							List<GameObject> candidates = new List<GameObject>();

							foreach (Collider2D c in Physics2D.OverlapCircleAll(d.proj.transform.position, navigateAimArea))
							{
								if (c.gameObject.Equals(d.proj))
									continue;

								if (d.HasHit(c.gameObject))
									continue;

								if (!Utils.CanSee(c.gameObject, d.proj.transform.position))
									continue;

								Character targetCh = c.gameObject.GetChar();
								if (targetCh == null || !Owner.CanAttack(targetCh))
									continue;

								candidates.Add(c.gameObject);
							}

							candidates.Sort((x, y) => (int)Utils.DistanceSqr(Owner.GetData().GetBody().transform.position, x.transform.position) - (int)Utils.DistanceSqr(Owner.GetData().GetBody().transform.position, y.transform.position));

							foreach (GameObject c in candidates)
							{
								Character targetCh = c.GetChar();
								if (targetCh == null || !Owner.CanAttack(targetCh))
									continue;

								d.target = targetCh;
								d.SaveHit(c.gameObject);
								AdjustToTarget(d);
								break;
							}
						}
						else
						{
							Vector3 direction = new Vector3(d.rb.velocity.x, d.rb.velocity.y);
							Vector3 perpend = Utils.GetPerpendicularVector(d.proj.transform.position, direction + d.proj.transform.position);

							List<GameObject> candidates = new List<GameObject>();

							foreach (Collider2D c in Physics2D.OverlapAreaAll(d.proj.transform.position + perpend * navigateAimArea / 2f + direction.normalized * navigateAimArea, d.proj.transform.position + perpend * (-navigateAimArea / 2f)))
							{
								if (c.gameObject.Equals(d.proj))
									continue;

								if (d.HasHit(c.gameObject))
									continue;

								if (!Utils.CanSee(c.gameObject, d.proj.transform.position))
									continue;

								candidates.Add(c.gameObject);
								break;
							}

							candidates.Sort((x, y) => (int)Utils.DistanceSqr(Owner.GetData().GetBody().transform.position, x.transform.position) - (int)Utils.DistanceSqr(Owner.GetData().GetBody().transform.position, y.transform.position));

							foreach (GameObject c in candidates)
							{
								Character targetCh = c.GetChar();
								if (targetCh == null || !Owner.CanAttack(targetCh))
									continue;

								d.target = targetCh;
								d.SaveHit(c.gameObject);
								AdjustToTarget(d);
								break;
							}
						}
					}
				}

				if (maxPenetratedTargets == 0 || destroy)
				{
					if (returnBack)
					{
						returningBack = true;
						AdjustToTarget(d);
					}
					else
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
			}
			else
			{
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
			return maxPenetratedTargets > 0 || navigateToTarget; // || .. ; //TODO
		}
	}
}
