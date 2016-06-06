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
	public class ColdShuriken : ActiveSkill
	{
		private GameObject activeProjectile;
		private Rigidbody2D activeProjectileRigidBody;
		private Character currentTarget;

		private List<Character> targetsHit;

		private int targetsLeft;
		private readonly int maxTargets = 8;
		private readonly int destroyTime = 8;
		private readonly float radiusToSelectNextTarget = 20;
		private readonly int speed = 20;

		public ColdShuriken()
		{
			castTime = 0f;
			reuse = 35f;
			coolDown = 0;
			range = -1;
			baseDamage = 20;

			requireConfirm = true;
			updateFrequency = 0.1f;

			AvailableToPlayer = true;
			RequiredSlotLevel = 2;
		}

		public override SkillId GetSkillId()
		{
			return SkillId.ColdShuriken;
		}

		public override string GetVisibleName()
		{
			return "Cold Shuriken";
		}

		public override string GetDescription()
		{
			return ";;";
		}

		public override Skill Instantiate()
		{
			return new ColdShuriken();
		}

		public override SkillEffect[] CreateEffects(int param)
		{
			return new SkillEffect[] {new EffectDamage(baseDamage, 2)};
		}

		public override void InitTraits()
		{
			AddTrait(SkillTraits.Damage);
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

			targetsHit = new List<Character>();
			targetsLeft = maxTargets;
			currentTarget = null;

			activeProjectile = CreateSkillProjectile("projectile_blacktest_i00", true);

			if (activeProjectile != null)
			{
				Rigidbody2D rb = activeProjectile.GetComponent<Rigidbody2D>();
				rb.velocity = (GetOwnerData().GetForwardVector(0) * speed);
				activeProjectileRigidBody = rb;

			    SetNextTarget();

				Object.Destroy(activeProjectile, destroyTime);
			}
		}

		private float interpolTimer;
		private Vector3 origVel;

		private void AdjustToNextTarget()
		{
			if (activeProjectile == null || currentTarget == null || currentTarget.GetData() == null)
				return;

			Vector3 current = origVel;
			Vector3 targetDir = Utils.GetDirectionVector(currentTarget.GetData().GetBody().transform.position, activeProjectile.transform.position).normalized;

			interpolTimer += 0.1f;

			if (interpolTimer > 1)
				interpolTimer = 1f;

			Vector3 newDir = Vector3.Lerp(current, targetDir, interpolTimer);

			//activeProjectileRigidBody.AddForce(targetDir * speed);*/
			activeProjectileRigidBody.velocity = newDir*(speed);
		}

		public override void UpdateLaunched()
		{
			if (targetsLeft <= 0)
			{
				DestroyProjectile(activeProjectile);
				return;
			}

			AdjustToNextTarget();
		}

		private void NewTargetSelected()
		{
			interpolTimer = 0;
			origVel = activeProjectileRigidBody.velocity.normalized;
		}

		protected override bool ReceiveUpdateMethod()
		{
			return activeProjectile != null;
		}

		public override void OnFinish()
		{

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

	    private void SetNextTarget()
	    {
            List<Collider2D> possibleHitsList = new List<Collider2D>();
            Collider2D[] possibleHits = Physics2D.OverlapCircleAll(activeProjectile.gameObject.transform.position, radiusToSelectNextTarget);
            foreach (Collider2D c in possibleHits)
                possibleHitsList.Add(c);

            possibleHitsList.Sort((x, y) => Utils.DistanceSqr(x.transform.position, GetOwnerData().GetBody().transform.position).CompareTo(Utils.DistanceSqr(y.transform.position, GetOwnerData().GetBody().transform.position)));

            bool targetFound = false;

            // v prvnim cyklu zkusim najit blizky target ktery se jeste nehitnul
            foreach (Collider2D c in possibleHitsList)
            {
                Character charTarget = c.gameObject.GetChar();

                if (charTarget != null && Owner.CanAttack(charTarget))
                {
                    if (targetsHit.Contains(charTarget))
                        continue;

                    currentTarget = charTarget;
                    targetFound = true;
                    targetsLeft--;
                    targetsHit.Add(currentTarget);

                    break;
                }
            }

            // target nenalezen - resetuju uz hitnute targety a zkusim to znovu
            if (!targetFound)
            {
                targetsHit.Clear();
                foreach (Collider2D c in possibleHitsList)
                {
                    Character charTarget = c.gameObject.GetChar();

                    if (charTarget != null && Owner.CanAttack(charTarget))
                    {
                        currentTarget = charTarget;
                        targetFound = true;
                        targetsLeft--;
                        targetsHit.Add(currentTarget);

                        break;
                    }
                }
            }

            if (!targetFound)
            {
                //Debug.Log("target not found");
                DestroyProjectile(activeProjectile);
            }
            else
            {
                //Debug.DrawLine(activeProjectile.transform.position, currentTarget.GetData().transform.position, Color.green, 1f);
                NewTargetSelected();
            }
	    }

		public override void MonoTriggerEnter(GameObject gameObject, Collider2D coll)
		{
			// ignorovat kolizi se samotnym hracem
			if (targetsLeft > 0 && activeProjectile == null || coll.gameObject.Equals(GetOwnerData().GetBody()))
				return;

			Character ch = coll.gameObject.GetChar();

			if (ch != null && Owner.CanAttack(ch))
			{
				// main projectile hit - create subprojectiles
				if (activeProjectile != null && activeProjectile.Equals(gameObject))
				{
					ApplyEffects(Owner, coll.gameObject);

					SetNextTarget();
				}
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
