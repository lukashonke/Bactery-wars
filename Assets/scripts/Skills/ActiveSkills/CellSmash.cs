// Copyright (c) 2015, Lukas Honke
// ========================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Skills.Base;
using Assets.scripts.Skills.SkillEffects;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.scripts.Skills.ActiveSkills
{
	public class CellSmash : ActiveSkill
	{
		private GameObject meleeCasting, meleeHit, meleeExplosion;

		public bool checkAngleToo = true;

		private float meleeMaxRangeAdd = 1f;

		public int criticalRate = 10;
		public int angle = 60;

		//TODO melee utok nebere damage od hrace
		public CellSmash()
		{
			castTime = 0f;
			coolDown = 0f;
			reuse = 1.5f;
			updateFrequency = 0.1f;
			baseDamage = 10;
			resetMoveTarget = false; 

			range = 4;
			AvailableToPlayerAsAutoattack = true;
		}

		public override SkillId GetSkillId()
		{
			return SkillId.CellSmash;
		}

		public override string GetVisibleName()
		{
			return "Cell Smash";
		}

		public override string GetDescription()
		{
			return "Autoattack that hits area in front of player that deals 10 damage with 10% critical rate.";
		}

		public override Skill Instantiate()
		{
			return new CellSmash();
		}

		public override SkillEffect[] CreateEffects(int param)
		{
			return new SkillEffect[] { new EffectDamage(baseDamage, 2, criticalRate) };
		}

		public override void InitTraits()
		{
			AddTrait(SkillTraits.Damage);
			AddTrait(SkillTraits.Melee);
		}

		public override bool OnCastStart()
		{
			RotatePlayerTowardsMouse();

			//if (Vector3.Distance(GetOwnerData().GetBody().transform.position, initTarget.transform.position) < range)
			{
				if (castTime > 0)
				{
					meleeCasting = CreateParticleEffect("Melee Preparation", true, GetOwnerData().GetBody().transform.position);
					StartParticleEffect(meleeCasting);
					GetOwnerData().StartMeleeAnimation(castTime);
				}
			}

			return true;
		}

		public override void OnLaunch()
		{
			GetOwnerData().StopMeleeAnimation();

			meleeHit = CreateParticleEffect("Melee Launch", true, GetOwnerData().GetShootingPosition().transform.position);
			//meleeHit.transform.rotation = Utils.GetRotationToMouse(meleeHit.transform);
			UpdateMouseDirection(meleeHit.transform);

			if (meleeHit != null)
			{
				meleeHit.transform.rotation = Quaternion.Euler(0, 0, angle);
				meleeHit.transform.localRotation = Quaternion.Euler(0, 0, angle);

				ParticleSystem ps = meleeHit.GetComponent<ParticleSystem>();
				ParticleSystem.ShapeModule shape = ps.shape;
				shape.arc = angle;
				shape.radius = range;
			}

			//meleeHit.transform.rotation = Utils.GetRotationToMouse(meleeHit.transform);

			StartParticleEffect(meleeHit);
		}

		public override void OnFinish()
		{
			if (meleeCasting != null)
				DeleteParticleEffect(meleeCasting);

			if(meleeHit != null)
				DeleteParticleEffect(meleeHit, 1.0f);

			DeleteCastingEffect();

			RaycastHit2D[] hits = Utils.CastBoxInDirection(Owner.GetData().GetBody(), GetOwnerData().GetForwardVector(), range, range);

			foreach (RaycastHit2D h in hits)
			{
				Character ch = h.transform.gameObject.GetChar();
				if (ch == null || !Owner.CanAttack(ch))
					continue;

				if (Utils.IsInCone(Owner.GetData().GetBody(), GetOwnerData().GetForwardVector(), h.transform.gameObject, angle, range))
					ApplyEffects(Owner, h.transform.gameObject);
			}


			/*if (initTarget != null && !Owner.Status.IsDead)
			{
				if (Vector3.Distance(GetOwnerData().GetBody().transform.position, initTarget.transform.position) < range + meleeMaxRangeAdd)
				{
					if (checkAngleToo)
					{
						RaycastHit2D[] hits = Utils.CastBoxInDirection(Owner.GetData().GetBody(), GetOwnerData().GetForwardVector(), 2.5f, range + meleeMaxRangeAdd);

						//RaycastHit2D[] hits = Utils.DoubleRaycast(GetOwnerData().GetBody().transform.position,
						//	GetOwnerData().GetForwardVector(),
						//	(int)(range + meleeMaxRangeAdd + 1), 1.5f, true);

						foreach (RaycastHit2D hit in hits)
						{
							if (hit.collider != null && hit.collider.gameObject != null)
							{
								if (hit.collider.gameObject.Equals(initTarget))
								{
									ApplyEffects(Owner, initTarget);
								}
							}
						}
					}
					else
					{
						ApplyEffects(Owner, initTarget);
					}

					//RotatePlayerTowardsTarget(initTarget);
				}
				else
				{
					Debug.Log("too far");
				}
			}*/
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
		}

		public override void OnAfterEnd()
		{
			/*if (reuse == 0)
			{
				// continue with next attack
				if (GetOwnerData().RepeatingMeleeAttack)
					GetOwnerData().MeleeInterract(target, true);
			}*/
		}

		public override void OnAterReuse()
		{
			// continue with next attack
			/*if (GetOwnerData().RepeatingMeleeAttack)
				GetOwnerData().MeleeInterract(target, true);*/
		}

		public override void UpdateLaunched()
		{
			/*if (target == null)
				return;

			if (Vector3.Distance(GetOwnerData().GetBody().transform.position, target.transform.position) > range+meleeMaxRangeAdd)
			{
				if(meleeCasting != null)
					DeleteParticleEffect(meleeCasting);

				if(meleeHit != null)
					DeleteParticleEffect(meleeHit);
			}*/
		}

		public override void OnAbort()
		{
			GetOwnerData().AbortMeleeAttacking();
		}

		public override bool CanMove()
		{
			return true;
		}

		public override bool CanRotate()
		{
			return true;
		}
	}
}
