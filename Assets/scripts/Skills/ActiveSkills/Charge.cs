// Copyright (c) 2015, Lukas Honke
// ========================
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.AI;
using Assets.scripts.Mono;
using Assets.scripts.Skills.Base;
using Assets.scripts.Skills.SkillEffects;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.scripts.Skills.ActiveSkills
{
	public class Charge : ActiveSkill
	{
		public int jumpSpeed = 50;

		public int hitEnemyDamage = 10;

		public int firstEnemyHitDamage = 0;
		private bool firstEnemyHit;
		public bool spreadshotOnLand = false;
		public int spreadshotDamage = 0;

		public bool penetrateThroughTargets = false;

		public bool areaDamage = false;
		public float areaDamageRadius = 4f;

		public bool jumpBack = false;
		private Vector3 jumpFrom;

		public bool rechargeOnKill = false;
		public int autoattacksOnJump = 0;

		public Charge()
		{
			castTime = 0f;
			reuse = 7f;
			coolDown = 0.3f;
			requireConfirm = true;
			breaksMouseMovement = false;
			resetMoveTarget = false;
			triggersOwnerCollision = true;

			range = 10;

			AvailableToPlayer = true;
		}

		public override SkillId GetSkillId()
		{
			return SkillId.Charge;
		}

		public override string GetVisibleName()
		{
			return "Charge";
		}

		public override string GetDescription()
		{
			return "Charges the player forward, damages and pushes away the first enemy you hit.";
		}

		public override string GetBaseInfo()
		{
			return "Reuse " + reuse + " sec | Damage on hit " + hitEnemyDamage + " | Range" + range;
		}

		public override Skill Instantiate()
		{
			return new Charge();
		}

		public override SkillEffect[] CreateEffects(int param)
		{
			int count = 1;

			if (hitEnemyDamage > 0)
				count ++;

			SkillEffect[] effects = new SkillEffect[count];
			effects[0] = new EffectPushaway(50);

			if (hitEnemyDamage > 0)
			{
				if(!areaDamage)
					effects[1] = new EffectDamage(hitEnemyDamage);
			}

			return effects;
		}

		public override void InitTraits()
		{
			AddTrait(SkillTraits.Jump);
			AddTrait(SkillTraits.Damage);
		}

		public override bool OnCastStart()
		{
			if (castTime > 0)
				CreateCastingEffect(true, "SkillTemplate");
			return true;
		}

		public override void OnLaunch()
		{
			if (penetrateThroughTargets)
			{
				Owner.GetData().GetBody().GetComponent<Collider2D>().isTrigger = true;
			}

			GetOwnerData().cancelForcedVelocityOnCollision = true;

			firstEnemyHit = false;

			jumpFrom = Owner.GetData().GetBody().transform.position;

			if (GetOwnerData().GetOwner().AI is PlayerAI)
				GetOwnerData().JumpForward(mouseDirection, GetRange(), jumpSpeed);
			else
			{
				GetOwnerData().JumpForward(GetOwnerData().GetForwardVector(), GetRange(), jumpSpeed);
			}

			particleSystem = CreateParticleEffect("JumpEffect", true);
			StartParticleEffect(particleSystem);
		}

		public override void UpdateLaunched()
		{
		}

		public override void OnAbort()
		{ 
		}

		public override void OnFinish()
		{
			PauseParticleEffect(particleSystem);
			DeleteParticleEffect(particleSystem, 5f);

			if (penetrateThroughTargets)
			{
				Owner.GetData().GetBody().GetComponent<Collider2D>().isTrigger = false;
			}

			if (areaDamage)
			{
				SkillEffect auraDmg = new EffectAuraDamage(hitEnemyDamage, 0, areaDamageRadius);
				ApplyEffect(Owner, Owner.GetData().GetBody(), auraDmg);

				GameObject explosion = CreateParticleEffect("Explosion", false, Owner.GetData().GetBody().transform.position);
				explosion.GetComponent<ParticleSystem>().Play();
				Object.Destroy(explosion, 2f);
			}

			if (autoattacksOnJump > 0)
			{
				for (int i = 0; i < autoattacksOnJump; i++)
				{
					ActiveSkill sk = Owner.GetMeleeAttackSkill();

					if (sk != null)
					{
						sk.TriggerSkill(Owner.GetData().GetForwardVector(i*45));
					}

					/*GameObject activeProjectile = CreateSkillProjectile("projectile_00", true);
					if (activeProjectile != null)
					{
						activeProjectile.name = "DodgeProjectile";
						Rigidbody2D rb = activeProjectile.GetComponent<Rigidbody2D>();
						rb.velocity = (GetOwnerData().GetForwardVector(i * 90) * 30);

						Object.Destroy(activeProjectile, 5f);
					}*/
				}
			}

			if (jumpBack)
			{
				float range = (Owner.GetData().GetBody().transform.position - jumpFrom).magnitude;

				Owner.GetData().GetBody().GetComponent<Collider2D>().isTrigger = true;
				GetOwnerData().cancelForcedVelocityOnCollision = false;

				if (GetOwnerData().GetOwner().AI is PlayerAI)
					GetOwnerData().JumpForward(-mouseDirection, range, jumpSpeed);
				else
				{
					GetOwnerData().JumpForward(-GetOwnerData().GetForwardVector(), range, jumpSpeed);
				}

				particleSystem = CreateParticleEffect("JumpEffect", true);
				StartParticleEffect(particleSystem);

				Owner.StartTask(StopJumpBack());
			}
		}

		private IEnumerator StopJumpBack()
		{
			yield return new WaitForSeconds(coolDown);

			Owner.GetData().GetBody().GetComponent<Collider2D>().isTrigger = false;
			GetOwnerData().cancelForcedVelocityOnCollision = true;

			PauseParticleEffect(particleSystem);
			DeleteParticleEffect(particleSystem, 5f);
		}

		public override void MonoUpdate(GameObject gameObject, bool fixedUpdate)
		{
		}

		public void OnTargetKilled(Character ch, Character killer, SkillId skillId)
		{
			if (rechargeOnKill && ch != null && killer != null && killer.Equals(Owner) && skillId == GetSkillId())
			{
				this.LastUsed = -10000;
				GetOwnerData().SetSkillReuseTimer(this, true);
			}
		}

		public override void MonoTriggerEnter(GameObject gameObject, Collider2D coll)
		{
			if (Owner.GetData().GetBody().GetComponent<Collider2D>().isTrigger)
				return;

			if (IsActive() && coll != null && coll.gameObject != null && gameObject != null)
			{
				if (coll.gameObject.GetChar() != null && GetOwnerData().GetOwner().CanAttack(coll.gameObject.GetChar()))
				{
					if (!firstEnemyHit)
					{
						ApplyEffect(Owner, coll.gameObject, new EffectDamage(firstEnemyHitDamage));
						firstEnemyHit = true;

						Character ch = coll.gameObject.GetChar();
						if (ch != null)
						{
							ch.AddOnKillHook(OnTargetKilled);
						}
					}

					ApplyEffects(Owner, coll.gameObject);
				}
			}

			if (spreadshotOnLand && gameObject.name.Equals("DodgeProjectile"))
			{
				if (coll.gameObject.Equals(GetOwnerData().GetBody()))
					return;

				Character ch = coll.gameObject.GetChar();
				if (ch == null)
				{
					Destroyable d = coll.gameObject.GetComponent<Destroyable>();
					if (d != null && !Owner.CanAttack(d))
						return;
				}
				else if (!Owner.CanAttack(ch))
					return;

				ApplyEffect(Owner, coll.gameObject, new EffectDamage(spreadshotDamage, 0));
				DestroyProjectile(gameObject);
			}
		}

		public override void MonoCollisionEnter(GameObject gameObject, Collision2D coll)
		{
			if (Owner.GetData().GetBody().GetComponent<Collider2D>().isTrigger)
				return;

			if (IsActive() && coll != null && coll.gameObject != null && gameObject != null)
			{
				if (coll.gameObject.GetChar() != null && GetOwnerData().GetOwner().CanAttack(coll.gameObject.GetChar()))
				{
					if (!firstEnemyHit)
					{
						ApplyEffect(Owner, coll.gameObject, new EffectDamage(firstEnemyHitDamage));
						firstEnemyHit = true;

						Character ch = coll.gameObject.GetChar();
						if (ch != null)
						{
							ch.AddOnKillHook(OnTargetKilled);
						}
					}

					ApplyEffects(Owner, coll.gameObject);
				}
			}

			if (spreadshotOnLand && gameObject.name.Equals("DodgeProjectile"))
			{
				if (coll.gameObject.Equals(GetOwnerData().GetBody()))
					return;

				Character ch = coll.gameObject.GetChar();
				if (ch == null)
				{
					Destroyable d = coll.gameObject.GetComponent<Destroyable>();
					if (d != null && !Owner.CanAttack(d))
						return;
				}
				else if (!Owner.CanAttack(ch))
					return;

				ApplyEffect(Owner, coll.gameObject, new EffectDamage(spreadshotDamage, 0));
				DestroyProjectile(gameObject);
			}
		}

		public override bool CanMove()
		{
			return true; // cant move unless the jump is finished
		}

		public override bool CanRotate()
		{
			return true;
		}
	}
}
