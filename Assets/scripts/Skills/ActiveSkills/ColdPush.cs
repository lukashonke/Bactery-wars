using System.Reflection.Emit;
using Assets.scripts.Actor;
using Assets.scripts.Base;
using Assets.scripts.Mono;
using Assets.scripts.Skills.Base;
using Assets.scripts.Skills.SkillEffects;
using UnityEngine;

namespace Assets.scripts.Skills.ActiveSkills
{
	public class ColdPush : ActiveSkill
	{
		private GameObject activeProjectile;
		public int pushbackForce = 300;

		public int angle = 60;

		public ColdPush()
		{
			castTime = 0f;
			reuse = 6f;
			coolDown = 0;
			requireConfirm = true;
			baseDamage = 10;

			range = 10;
		}

		public override SkillId GetSkillId()
		{
            return SkillId.ColdPush;
		}

		public override string GetVisibleName()
		{
            return "Cold Push";
		}

		public override Skill Instantiate()
		{
            return new ColdPush();
		}

		public override SkillEffect[] CreateEffects()
		{
			return new SkillEffect[] { new EffectPushaway(pushbackForce), new EffectStun(1.5f) };
		}

		public override void InitTraits()
		{
			AddTrait(SkillTraits.Damage);
		}

		private GameObject[] confirmObjects;

		public override void OnBeingConfirmed()
		{
			if (confirmObjects == null)
			{
				confirmObjects = new GameObject[2];
				GameObject first = GetPlayerData().CreateSkillResource("SkillTemplate", "directionarrow", true, GetPlayerData().GetShootingPosition().transform.position);
				GameObject second = GetPlayerData().CreateSkillResource("SkillTemplate", "directionarrow", true, GetPlayerData().GetShootingPosition().transform.position);

				confirmObjects[0] = first;
				confirmObjects[1] = second;

				UpdateDirectionArrowScale(range > 0 ? range : 5, first);
				UpdateDirectionArrowScale(range > 0 ? range : 5, second);
			}

			UpdateMouseDirection(GetPlayerData().GetShootingPosition().transform);
			for (int i = 0; i < confirmObjects.Length; i++)
			{
				RotateArrowToMouseDirection(confirmObjects[i], i == 0 ? 360+(angle / 2) : 360-(angle/2));				
			}
		}

		public override void CancelConfirmation()
		{
			if (confirmObjects != null)
			{
				foreach (GameObject o in confirmObjects)	
					Object.Destroy(o);
			}

			confirmObjects = null;

			base.CancelConfirmation();
		}

		public override bool OnCastStart()
		{
			RotatePlayerTowardsMouse();

			CreateCastingEffect(true, "SkillTemplate");

			return true;
		}

		public override void OnLaunch()
		{
			DeleteCastingEffect();

			RaycastHit2D[] hits = Utils.CastBoxInDirection(Owner.GetData().GetBody(), GetOwnerData().GetForwardVector(), range, range);

			foreach (RaycastHit2D h in hits)
			{
				Character ch = h.transform.gameObject.GetChar();
				if (ch == null || !Owner.CanAttack(ch))
					continue;

				if(Utils.IsInCone(Owner.GetData().GetBody(), GetOwnerData().GetForwardVector(), h.transform.gameObject, angle, range))
					ApplyEffects(Owner, h.transform.gameObject);
			}


			//TOOD here

			/*activeProjectile = CreateSkillProjectile("projectile_00", true);

			if (activeProjectile != null)
			{
				Rigidbody2D rb = activeProjectile.GetComponent<Rigidbody2D>();
				rb.velocity = (GetOwnerData().GetForwardVector(0) * 27);

				//Debug.DrawRay(GetOwnerData().GetShootingPosition().transform.position, rb.velocity, Color.green, 5f);

				Object.Destroy(activeProjectile, 5f);
			}*/
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
