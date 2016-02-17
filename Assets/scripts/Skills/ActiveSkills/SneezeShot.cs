using System.Collections.Generic;
using Assets.scripts.Actor;
using Assets.scripts.Base;
using Assets.scripts.Mono;
using Assets.scripts.Skills.Base;
using Assets.scripts.Skills.SkillEffects;
using UnityEngine;

namespace Assets.scripts.Skills.ActiveSkills
{
	public class SneezeShot : ActiveSkill
	{
		public int projectilesCount = 2;
		public bool projectilesAim = false;
		public int aimArea = 0;

		private List<ProjectileData> projectiles = new List<ProjectileData>();

		public SneezeShot()
		{
			castTime = 0f;
			reuse = 5f;
			coolDown = 0;
			requireConfirm = true;
			baseDamage = 20;

			range = 13;
		}

		public override SkillId GetSkillId()
		{
			return SkillId.SneezeShot;
		}

		public override string GetVisibleName()
		{
			return "Sneeze Shot";
		}

		public override Skill Instantiate()
		{
			return new SneezeShot();
		}

		public override SkillEffect[] CreateEffects()
		{
			return new SkillEffect[] {new EffectDamage(baseDamage, 0)};
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
			if (projectilesAim)
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

					rb.velocity = (GetOwnerData().GetForwardVector(CalcAngleForProjectile(i, projectilesCount, 10)) * 15);

					Object.Destroy(activeProjectile, 5f);

					if (projectilesAim)
					{
						ProjectileData data = new ProjectileData();
						data.proj = activeProjectile;
						data.target = null;
						data.interpolTimer = 0;
						data.rb = rb;
							
						projectiles.Add(data);
					}
				}
			}
		}

		public override void OnFinish()
		{

		}

		public override void MonoUpdate(GameObject gameObject, bool fixedUpdate)
		{
			if (projectilesAim && fixedUpdate)
			{
				// updates: 10/sec
				if (System.Environment.TickCount % 10 == 0)
				{
					ProjectileData d = GetData(gameObject);
					if (d == null || d.proj == null)
						return;

					if (d.target == null)
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
				return;

			Vector3 currentVelocity = data.rb.velocity.normalized;
			Vector3 targetDir = Utils.GetDirectionVector(data.target.GetData().GetBody().transform.position, data.proj.transform.position).normalized;

			data.interpolTimer += 0.1f;

			if (data.interpolTimer > 1)
				data.interpolTimer = 1f;

			Vector3 newDir = Vector3.Lerp(currentVelocity, targetDir, data.interpolTimer);

			data.rb.velocity = newDir*15;
		}

		public class ProjectileData
		{
			public GameObject proj;
			public Character target;
			public Rigidbody2D rb;
			public float interpolTimer;
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
