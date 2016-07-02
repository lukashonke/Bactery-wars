﻿// Copyright (c) 2015, Lukas Honke
// ========================
using System;
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
	public class Dodge : ActiveSkill
	{
		public int jumpSpeed = 50;

		public int hitEnemyDamage = 0;

		public int firstEnemyHitDamage = 0;
		private bool firstEnemyHit;
		public bool spreadshotOnLand = false;
		public int spreadshotDamage = 0;

		public bool penetrateThroughTargets = true;

		public Dodge()
		{
			castTime = 0f;
			reuse = 5f;
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
			return SkillId.Dodge;
		}

		public override string GetVisibleName()
		{
			return "Cold Dodge";
		}

		public override string GetDescription()
		{
			return "Makes the player jump in the selected direction, but the player will not push enemies away.";
		}

		public override string GetBaseInfo()
		{
			return "Reuse " + reuse + " sec | Range" + range;
		}

		public override Skill Instantiate()
		{
			return new Dodge();
		}

		public override SkillEffect[] CreateEffects(int param)
		{
			int count = 1;

			if (hitEnemyDamage > 0)
				count ++;

			//SkillEffect[] effects = new SkillEffect[count];
			//effects[0] = new EffectPushaway(50);

			//if (hitEnemyDamage > 0)
			//	effects[1] = new EffectDamage(hitEnemyDamage);

			//return effects;
			return null;
		}

		public override void InitTraits()
		{
			AddTrait(SkillTraits.Jump);
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
				//Owner.GetData().GetBody().GetComponent<Collider2D>().enabled = false;
			}

			//TODO vyresit, nefunguje tak jak ma !!
			GetOwnerData().cancelForcedVelocityOnCollision = true;

			firstEnemyHit = false;

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
			GetOwnerData().cancelForcedVelocityOnCollision = false;
		}

		public override void OnFinish()
		{
			PauseParticleEffect(particleSystem);
			DeleteParticleEffect(particleSystem, 5f);

			if (penetrateThroughTargets)
			{
				Owner.GetData().GetBody().GetComponent<Collider2D>().isTrigger = false;
				//Owner.GetData().GetBody().GetComponent<Collider2D>().enabled = true;
			}

			GetOwnerData().cancelForcedVelocityOnCollision = false;

			if (spreadshotOnLand)
			{
				for (int i = 0; i < 4; i++)
				{
					GameObject activeProjectile = CreateSkillProjectile("projectile_00", true);
					if (activeProjectile != null)
					{
						activeProjectile.name = "DodgeProjectile";
						Rigidbody2D rb = activeProjectile.GetComponent<Rigidbody2D>();
						rb.velocity = (GetOwnerData().GetForwardVector(i * 90) * 30);

						Object.Destroy(activeProjectile, 5f);
					}
				}
			}
		}

		public override void MonoUpdate(GameObject gameObject, bool fixedUpdate)
		{
		}

		public override void MonoTriggerEnter(GameObject gameObject, Collider2D coll)
		{
			if (IsActive() && coll != null && coll.gameObject != null && gameObject != null)
			{
				if (coll.gameObject.GetChar() != null && GetOwnerData().GetOwner().CanAttack(coll.gameObject.GetChar()))
				{
					if (!firstEnemyHit)
					{
						ApplyEffect(Owner, coll.gameObject, new EffectDamage(firstEnemyHitDamage));
						firstEnemyHit = true;
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
			if (IsActive() && coll != null && coll.gameObject != null && gameObject != null)
			{
				if (coll.gameObject.GetChar() != null && GetOwnerData().GetOwner().CanAttack(coll.gameObject.GetChar()))
				{
					if (!firstEnemyHit)
					{
						ApplyEffect(Owner, coll.gameObject, new EffectDamage(firstEnemyHitDamage));
						firstEnemyHit = true;
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
