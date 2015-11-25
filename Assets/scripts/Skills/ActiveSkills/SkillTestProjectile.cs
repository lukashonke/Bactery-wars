using Assets.scripts.Actor;
using Assets.scripts.Base;
using Assets.scripts.Mono;
using Assets.scripts.Skills.Base;
using Assets.scripts.Skills.SkillEffects;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.scripts.Skills.ActiveSkills
{
	public class SkillTestProjectile : ActiveSkill
	{
		public GameObject activeProjectile;

		public SkillTestProjectile(string name, int id) : base(name, id)
		{
			castTime = 0f;
			reuse = 0;
			coolDown = 0;
			requireConfirm = true;
		}

		public override Skill Instantiate()
		{
			return new SkillTestProjectile(Name, Id);
		}

		public override SkillEffect[] CreateEffects()
		{
			return new SkillEffect[] {new EffectDamage(10, 2)};
		}

		public override AbstractServerData CreateServerData(GameObject owner)
		{
			return new ServerData(owner, this);
		}

		private class ServerData : AbstractServerData
		{
			private GameObject activeProjectile;

			public ServerData(GameObject o, ActiveSkill template) : base(o, template)
			{
			}

			public override void LaunchOnServer(Vector3 startPos, Vector3 heading)
			{
				activeProjectile = CreateSkillResource("Test Projectile", "projectile_blacktest_i00", false, startPos, Quaternion.identity);

				AddMonoReceiver(activeProjectile);

				if (activeProjectile != null)
				{
					Rigidbody2D rb = activeProjectile.GetComponent<Rigidbody2D>();
					rb.velocity = (heading * 15);

					Object.Destroy(activeProjectile, 5f);

					NetworkServer.Spawn(activeProjectile);
				}
			}

			public override void UpdatePlayerHeading(Vector3 heading)
			{
			}

			public override void UpdatePlayerPosition(Vector3 position)
			{
			}

			public override void MonoTriggerEnter(GameObject gameObject, Collider2D coll)
			{
				template.ApplyEffects(GetOwner(), coll.gameObject);

				Character targetCh = GetCharacter(coll.gameObject);
				if (targetCh == null)
					return;

				if (GetOwner().CanAttack(targetCh))
				{
					ProjectileBlackTestData pd = gameObject.GetComponent<ProjectileBlackTestData>();
					pd.collapse();
				}
			}
		}

		public override bool OnCastStart()
		{
			RotatePlayerTowardsMouse();

			CreateCastingEffect(true, "Test Projectile");

			return true;
		}

		public override void OnLaunch()
		{
			DeleteCastingEffect();
		}

		public void ServerLaunchSkill(Vector3 startPos, Vector3 heading)
		{
			
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

		public override void MonoTriggerEnter(GameObject gameObject, Collider2D other)
		{
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
