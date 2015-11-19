using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Skills.SkillEffects;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.scripts.Skills.ActiveSkills
{
	public class SkillAreaExplode : ActiveSkill
	{
		private GameObject activeProjectile;
		private int launchStart;
		private bool exploded;

		private readonly int radius = 3;

        public SkillAreaExplode(string name, int id) : base(name, id)
		{
			castTime = 1f;
			coolDown = 0;
			reuse = 5f;
			requireConfirm = true;
		}

		public override Skill Instantiate()
		{
			return new SkillAreaExplode(Name, Id);
		}

		public override SkillEffect[] CreateEffects()
		{
			return new SkillEffect[] {new EffectDamage(5, 2)};
		}

		public override bool OnCastStart()
		{
			RotatePlayerTowardsMouse();

			CreateCastingEffect(true);

			return true;
		}

		public override void OnLaunch()
		{
			DeleteCastingEffect();

			exploded = false;

			activeProjectile = CreateSkillProjectile("projectile_blacktest_i00", true);

			if (activeProjectile != null)
			{
				Rigidbody2D rb = activeProjectile.GetComponent<Rigidbody2D>();
				rb.velocity = (GetOwnerData().GetForwardVector(0) * 5);

				Debug.DrawRay(GetOwnerData().GetShootingPosition().transform.position, rb.velocity, Color.green, 5f);

				Object.Destroy(activeProjectile, 5f);
			}

			launchStart = System.Environment.TickCount;
		}

		public override void UpdateLaunched()
		{
		}

		public override void OnAbort()
		{
		}

		public override void OnFinish()
		{
		}

		public override void MonoUpdate(GameObject gameObject)
		{
			if (exploded || launchStart == 0)
				return;

			if (launchStart + 2000 < System.Environment.TickCount)
			{
				launchStart = 0;
				Explode(gameObject);
			}
		}

		public override void MonoTriggerEnter(GameObject gameObject, Collider2D other)
		{
			if (other.gameObject.Equals(GetOwnerData().GetBody()))
				return;

			if(!exploded)
				Explode(gameObject);
		}

		private void Explode(GameObject projectile)
		{
			exploded = true;

			GameObject explosion = CreateParticleEffect("Explosion", false, projectile.transform.position);

			explosion.GetComponent<ParticleSystem>().Play();

			foreach (Collider2D c in Physics2D.OverlapCircleAll(projectile.transform.position, radius))
			{
				// dont hit player himself
				if (c.gameObject.Equals(GetOwnerData().GetBody()))
					continue;

				ApplyEffects(Owner, c.gameObject);
			}

			DisableProjectile(projectile);

			Object.Destroy(projectile, 2f);
			Object.Destroy(explosion, 2f);
		}

		public override bool CanMove()
		{
			return !IsBeingCasted();
		}

		public override bool CanRotate()
		{
			return !IsBeingCasted();
		}
	}
}
