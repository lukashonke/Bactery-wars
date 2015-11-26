using System.Collections.Generic;
using Assets.scripts.Actor;
using Assets.scripts.Base;
using Assets.scripts.Mono;
using Assets.scripts.Skills.SkillEffects;
using UnityEngine;

namespace Assets.scripts.Skills.ActiveSkills
{
	public class ChainedProjectile : ActiveSkill
	{
		private GameObject activeProjectile;
		private Dictionary<GameObject, GameObject> subProjectiles = new Dictionary<GameObject, GameObject>(); 

		private readonly int numProjectiles = 3;
		private readonly float radiusForChainedTargets = 20;
		private readonly int speed = 15;

		public ChainedProjectile(string name, int id)
			: base(name, id)
		{
			castTime = 1f;
			reuse = 0;
			coolDown = 0;
			requireConfirm = true;
		}

		public override Skill Instantiate()
		{
			return new ChainedProjectile(Name, Id);
		}

		public override SkillEffect[] CreateEffects()
		{
			return new SkillEffect[] {new EffectDamage(100, 2)};
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

			subProjectiles.Clear();

			activeProjectile = CreateSkillProjectile("projectile_blacktest_i00", true);

			if (activeProjectile != null)
			{
				Rigidbody2D rb = activeProjectile.GetComponent<Rigidbody2D>();
				rb.velocity = (GetOwnerData().GetForwardVector(0) * speed);

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

		public override void MonoTriggerEnter(GameObject gameObject, Collider2D other)
		{
			if (other.gameObject.Equals(GetOwnerData().GetBody()))
				return;

			Character ch = GetCharacterFromObject(other.gameObject);

			if (ch != null && Owner.CanAttack(ch))
			{
				// main projectile hit - create subprojectiles
				if (activeProjectile != null && activeProjectile.Equals(gameObject))
				{
					ApplyEffects(Owner, other.gameObject);

					int projectilesLeft = numProjectiles;

					foreach (Collider2D c in Physics2D.OverlapCircleAll(other.gameObject.transform.position, radiusForChainedTargets))
					{
						
						if (projectilesLeft > 0)
						{
							Character charTarget = GetCharacterFromObject(c.gameObject);
							if (charTarget != null && Owner.CanAttack(charTarget))
							{
								CreateSubProjectile(gameObject, c.gameObject);

								projectilesLeft--;
							}
						}
						else break;
					}

					DestroyProjectile(gameObject);
				}
				else // subprojectile hit
				{
					if (subProjectiles.ContainsKey(gameObject))
					{
						GameObject target;
						if (subProjectiles.TryGetValue(gameObject, out target))
						{
							if (other.gameObject.Equals(target))
							{
								ApplyEffects(Owner, other.gameObject);
								DestroyProjectile(gameObject);
								subProjectiles.Remove(gameObject);
							}
						}
					}
				}
			}
		}

		private void CreateSubProjectile(GameObject mainProjectile, GameObject target)
		{
			GameObject subProjectile = CreateSkillProjectile("projectile_blacktest_i00", true, mainProjectile.transform);

			if (subProjectile != null)
			{
				Rigidbody2D rb = subProjectile.GetComponent<Rigidbody2D>();
				rb.velocity = (Utils.GetDirectionVector(target.transform.position, mainProjectile.transform.position).normalized * speed);

				Object.Destroy(subProjectile, 5f);

				subProjectiles.Add(subProjectile, target);
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
			return !IsBeingCasted();
		}

		public override bool CanRotate()
		{
			return CanMove();
		}
	}
}
