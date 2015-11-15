using Assets.scripts.Base;
using Assets.scripts.Mono;
using Assets.scripts.Skills.SkillEffects;
using UnityEngine;

namespace Assets.scripts.Skills.ActiveSkills
{
	public class SkillTestProjectile : ActiveSkill
	{
		private GameObject activeProjectile;
		protected GameObject particleSystemObject;

		public SkillTestProjectile(string name, int id) : base(name, id)
		{
			castTime = 0f;
			reuse = 0;
			coolDown = 0;
		}

		public override Skill Instantiate()
		{
			return new SkillTestProjectile(Name, Id);
		}

		public override SkillEffect CreateEffects()
		{
			SkillEffect effect = new SkillEffect();

			return effect;
		}

		public override bool OnCastStart()
		{
			//TODO abstract this in ActiveSkill
			particleSystemObject = GetOwnerData().CreateProjectileParticleEffect("SkillTestProjectile", "CastingEffect", true);

			ParticleSystem ps = particleSystemObject.GetComponent<ParticleSystem>();
			ps.Play();

			return true;
		}

		public override void OnLaunch()
		{
			if (particleSystemObject != null)
				Object.Destroy(particleSystemObject);

			activeProjectile = GetPlayerData().CreateProjectile(this.GetType().Name, "projectile_blacktest_i00");

			if (activeProjectile != null)
			{
				Rigidbody2D rb = activeProjectile.GetComponent<Rigidbody2D>();
				rb.velocity = (GetOwnerData().GetForwardVector(0) * 15);

				Debug.DrawRay(GetOwnerData().GetShootingPosition().transform.position, rb.velocity, Color.green, 5f);

				AddMonoReceiver(activeProjectile);

				Object.Destroy(activeProjectile, 5f);
			}
		}

		public override void OnFinish()
		{

		}

		public override void MonoUpdate(GameObject gameObject)
		{
		}

		public override void MonoStart(GameObject gameObject)
		{
		}

		public override void MonoDestroy(GameObject gameObject)
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
