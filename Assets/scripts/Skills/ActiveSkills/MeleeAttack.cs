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
	public class MeleeAttack : ActiveSkill
	{
		private GameObject meleeCasting, meleeHit, meleeExplosion;
		private GameObject target;

		public bool checkAngleToo = true;

		private float meleeMaxRangeAdd = 1f;

		//TODO melee utok nebere damage od hrace
		public MeleeAttack()
		{
			castTime = 0.25f;
			coolDown = 0f;
			reuse = 1f;
			updateFrequency = 0.1f;
			baseDamage = 10;
			resetMoveTarget = false; 

			range = 4;
		}

		public override SkillId GetSkillId()
		{
			return SkillId.MeleeAttack;
		}

		public override string GetVisibleName()
		{
			return "Melee attack";
		}

		public override Skill Instantiate()
		{
			return new MeleeAttack();
		}

		public override SkillEffect[] CreateEffects(int param)
		{
			return new SkillEffect[] { new EffectDamage(baseDamage, 0) };
		}

		public override void InitTraits()
		{
			AddTrait(SkillTraits.Damage);
			AddTrait(SkillTraits.Melee);
		}

		public override bool OnCastStart()
		{
			if (initTarget == null)
			{
				AbortCast();
				return false;
			}

			target = initTarget;

			Character chTarget = GetCharacterFromObject(initTarget);

			if (chTarget == null || chTarget.Status.IsDead)
			{
				AbortCast();
				return false;
			}

			RotatePlayerTowardsTarget(initTarget);

			if (Vector3.Distance(GetOwnerData().GetBody().transform.position, initTarget.transform.position) < range)
			{
				meleeCasting = CreateParticleEffect("Melee Preparation", true, GetOwnerData().GetBody().transform.position);
				StartParticleEffect(meleeCasting);
				GetOwnerData().StartMeleeAnimation(castTime);
				return true;
			}

			AbortCast();
			// dont melee, too far
			return false;
		}

		public override void OnLaunch()
		{
			GetOwnerData().StopMeleeAnimation();

			meleeHit = CreateParticleEffect("Melee Launch", true, GetOwnerData().GetBody().transform.position);
			meleeHit.transform.localRotation = Quaternion.Euler(0, 0, 90);

			StartParticleEffect(meleeHit);
		}

		public override void OnFinish()
		{
			if (meleeCasting != null)
				DeleteParticleEffect(meleeCasting);

			if(meleeHit != null)
				DeleteParticleEffect(meleeHit, 1.0f);

			if (initTarget != null && !Owner.Status.IsDead)
			{
				// TODO check angle!!! 
				if (Vector3.Distance(GetOwnerData().GetBody().transform.position, initTarget.transform.position) < range + meleeMaxRangeAdd)
				{
					if (checkAngleToo)
					{
						RaycastHit2D[] hits = Utils.CastBoxInDirection(Owner.GetData().GetBody(), GetOwnerData().GetForwardVector(), 2.5f, range + meleeMaxRangeAdd);

						/*RaycastHit2D[] hits = Utils.DoubleRaycast(GetOwnerData().GetBody().transform.position,
							GetOwnerData().GetForwardVector(),
							(int)(range + meleeMaxRangeAdd + 1), 1.5f, true);*/

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
			}
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
			if (reuse == 0)
			{
				// continue with next attack
				if (GetOwnerData().RepeatingMeleeAttack)
					GetOwnerData().MeleeInterract(target, true);
			}
		}

		public override void OnAterReuse()
		{
			// continue with next attack
			if (GetOwnerData().RepeatingMeleeAttack)
				GetOwnerData().MeleeInterract(target, true);
		}

		public override void UpdateLaunched()
		{
			if (target == null)
				return;

			if (Vector3.Distance(GetOwnerData().GetBody().transform.position, target.transform.position) > range+meleeMaxRangeAdd)
			{
				if(meleeCasting != null)
					DeleteParticleEffect(meleeCasting);

				if(meleeHit != null)
					DeleteParticleEffect(meleeHit);
			}
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
