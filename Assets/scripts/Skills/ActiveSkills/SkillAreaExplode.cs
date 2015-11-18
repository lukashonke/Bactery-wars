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
		private GameObject particleSystemObject;
		private int launchStart;
		private bool exploded;

		private readonly int radius = 3;

        public SkillAreaExplode(string name, int id) : base(name, id)
		{
			castTime = 1f;
			coolDown = 0;
			reuse = 5f;
			requireConfirm = true;
			MovementBreaksConfirmation = true;
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
			RotatePlayerTowardsMouse(); // TODO add everywhere

			particleSystemObject = CreateParticleEffect("SkillAreaExplode", "CastingEffect", true);
			StartParticleEffect(particleSystemObject);

			return true;
		}

		public override void OnLaunch()
		{
			DeleteParticleEffect(particleSystemObject);

			exploded = false;

			activeProjectile = GetPlayerData().CreateProjectile("SkillAreaExplode", "projectile_blacktest_i00");

			if (activeProjectile != null)
			{
				Rigidbody2D rb = activeProjectile.GetComponent<Rigidbody2D>();
				rb.velocity = (GetOwnerData().GetForwardVector(0) * 5);

				Debug.DrawRay(GetOwnerData().GetShootingPosition().transform.position, rb.velocity, Color.green, 5f);

				AddMonoReceiver(activeProjectile);

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

			GameObject explosion = GetOwnerData().CreateSkillResource("SkillAreaExplode", "Explosion", false, projectile.transform.position);

			explosion.GetComponent<ParticleSystem>().Play();

			foreach (Collider2D c in Physics2D.OverlapCircleAll(projectile.transform.position, radius))
			{
				// dont hit player himself
				if (c.gameObject.Equals(GetOwnerData().GetBody()))
					continue;

				ApplyEffects(Owner, c.gameObject);
			}

			projectile.GetComponent<SpriteRenderer>().enabled = false;
			projectile.GetComponent<Collider2D>().enabled = false;
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
