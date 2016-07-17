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
	public class PoisonShot : ActiveSkill
	{
		public string effectName = "Poison";
		public int force = 20;

		public int projectilesCount = 1;
		public bool projectilesAim = false;
		public bool penetrateTargets = false;
		public int randomAngle = 0;
		public bool selectTargetOnLaunch = false;
		public int aimArea = 0;
		public int nullReuseChance = 0;

		public int secondDamage = 0;
		public int maxPenetratedTargets = 0;
		public bool navigateAfterPenetration = false;
		public float interpolAdd = 0.1f;

		public bool explodeEffect = false;

		public int duration = 10;

		public bool areaEffect = false;
		public int areaEffectRange = 8;

		private List<ProjectileData> projectiles = new List<ProjectileData>();

		public PoisonShot()
		{
			castTime = 0f;
			reuse = 4f;
			coolDown = 0;
			requireConfirm = true;
			baseDamage = 10;

			range = 12;

			AvailableToPlayer = true;
		}

		public override SkillId GetSkillId()
		{
			return SkillId.PoisonShot;
		}

		public override string GetVisibleName()
		{
			return "Poison Shot";
		}

		public override string GetDescription()
		{
			return "Shoots a bullet that poisons your target, dealing damage over time.";
		}

		public override string GetBaseInfo()
		{
			return "Reuse " + reuse + " sec | Damage " + baseDamage + "/sec for " + duration + " seconds | Range" + range;
		}

		public override Skill Instantiate()
		{
			return new PoisonShot();
		}

		public override SkillEffect[] CreateEffects(int param)
		{
			SkillEffect[] effs = new SkillEffect[1];
			EffectDamageOverTime eff = new EffectDamageOverTime(baseDamage, duration, 1.0f);
			eff.effectName = effectName;
			effs[0] = eff;

			return effs;
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
			bool saveProjectiles = projectilesAim || selectTargetOnLaunch || maxPenetratedTargets > 0;

			if (saveProjectiles)
			{
				projectiles.Clear();
			}

			DeleteCastingEffect();

			GameObject activeProjectile;

			for (int i = 0; i < projectilesCount; i++)
			{
				activeProjectile = CreateSkillProjectile("projectile_00", true);
				if (activeProjectile != null)
				{
					Rigidbody2D rb = activeProjectile.GetComponent<Rigidbody2D>();

					rb.velocity = (GetOwnerData().GetForwardVector((Random.Range(-randomAngle, randomAngle) + CalcAngleForProjectile(i, projectilesCount, 10))) * force);

					Object.Destroy(activeProjectile, 5f);

					if (saveProjectiles)
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

			// vybere target pri vypusteni
			if (selectTargetOnLaunch)
			{
				int range = GetRange();

				List<RaycastHit2D> hits = Utils.CastBoxInDirection(Owner.GetData().GetBody(), GetPlayerData().GetForwardVector(), range, range*2).ToList();
				List<Character> targets = new List<Character>();

				for (int i = 0; i < hits.Count; i++)
				{
					RaycastHit2D hit = hits[i];
					Character targetCh = hit.collider.gameObject.GetChar();
					if (targetCh == null || !Owner.CanAttack(targetCh))
						continue;

					targets.Add(targetCh);
				}

				if (targets.Count > 0)
				{
					foreach (ProjectileData d in projectiles)
					{
						if (d.target == null)
						{
							int randomIndex = Random.Range(0, targets.Count);
							d.target = targets[randomIndex];
						}
					}
				}
			}
		}

		public override void OnFinish()
		{
			if (nullReuseChance > 0)
			{
				if (Random.Range(0, 100) < nullReuseChance)
				{
					this.LastUsed = 0;
					GetOwnerData().SetSkillReuseTimer(this, true);
				}
			}
		}

		public override void MonoUpdate(GameObject gameObject, bool fixedUpdate)
		{
			bool saveProjectiles = projectilesAim || selectTargetOnLaunch || maxPenetratedTargets > 0;

			if (saveProjectiles && fixedUpdate)
			{
				// updates: 10/sec
				if (System.Environment.TickCount % 10 == 0)
				{
					ProjectileData d = GetData(gameObject);
					if (d == null || d.proj == null)
						return;

					if (d.target == null && projectilesAim)
					{
						float angle = d.proj.transform.rotation.eulerAngles.z;
						foreach (RaycastHit2D hit in Physics2D.BoxCastAll(d.proj.transform.position, new Vector2(aimArea, aimArea), angle, d.rb.velocity, aimArea))
						{
							if (hit.collider.gameObject.Equals(d.proj))
								continue;

							Character targetCh = hit.collider.gameObject.GetChar();
							if (targetCh == null || !Owner.CanAttack(targetCh))
								continue;

							d.target = targetCh;
							break;
						}
					}

					if(d.target != null)
						AdjustToTarget(d);
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

			data.interpolTimer += interpolAdd;

			if (data.interpolTimer > 1)
				data.interpolTimer = 1f;

			Vector3 newDir = Vector3.Lerp(currentVelocity, targetDir, data.interpolTimer);

			data.rb.velocity = newDir * force;
		}

		public class ProjectileData
		{
			public GameObject proj;
			public Character target;
			public Rigidbody2D rb;
			public float interpolTimer;
			public int penetratedTargets;
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

			ProjectileData d = GetData(gameObject);
			if (d != null && d.target != null)
			{
				if (coll.gameObject.Equals(d.target.GetData().GetBody()))
				{
					d.target = null;
				}
			}

			if (d != null && maxPenetratedTargets > 0 && d.penetratedTargets > 0 && secondDamage > 0)
				ApplyEffects(Owner, coll.gameObject, false, 1);
			else
			{
				if (!areaEffect)
					ApplyEffects(Owner, coll.gameObject);
				else
				{
					Collider2D[] colls = Physics2D.OverlapCircleAll(gameObject.transform.position, areaEffectRange);

					// explosion effect
					GameObject explosion = CreateParticleEffect("AreaTargetExplosion", false, gameObject.transform.position);
					explosion.GetComponent<ParticleSystem>().Play();
					Object.Destroy(explosion, 2f);

					// give damages
					foreach (Collider2D col in colls)
					{
						if (col != null && col.gameObject != null)
						{
							Character targetCh = col.gameObject.GetChar();

							if (targetCh != null)
							{
								if (Owner.CanAttack(targetCh))
								{
									ApplyEffects(Owner, col.gameObject);
								}
							}
						}
					}
				}
			}

			bool destroy = false;

			if (penetrateTargets)
			{
				if (d != null)
				{
					// check if penetrated too many
					if (maxPenetratedTargets > 0)
					{
						d.penetratedTargets++;
						if (d.penetratedTargets >= maxPenetratedTargets)
							destroy = true;
					}

					if (!destroy && navigateAfterPenetration)
					{
						float angle = d.proj.transform.rotation.eulerAngles.z;
						foreach (Collider2D hit in Physics2D.OverlapCircleAll(d.proj.transform.position, aimArea))
						{
							if (hit.gameObject.Equals(d.proj))
								continue;

							Character targetCh = hit.gameObject.GetChar();
							if (targetCh == null || !Owner.CanAttack(targetCh) || hit.gameObject.Equals(coll.gameObject))
								continue;

							d.target = targetCh;
							AdjustToTarget(d);
							break;
						}
					}
				}
			}

			if (!penetrateTargets || destroy)
			{
				if (explodeEffect)
				{
					GameObject explosion = CreateParticleEffect("Explosion", false, gameObject.transform.position);
					explosion.GetComponent<ParticleSystem>().Play();
					Object.Destroy(explosion, 2f);
				}

				DestroyProjectile(gameObject);
			}
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
	}
}
