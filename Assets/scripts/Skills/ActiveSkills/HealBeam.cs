using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.AI;
using Assets.scripts.Skills.Base;
using Assets.scripts.Skills.SkillEffects;
using UnityEngine;

namespace Assets.scripts.Skills.ActiveSkills
{
	public class HealBeam : ActiveSkill
	{
		protected GameObject ray;

		private float lastHeal = 0;
		public float rotateSpeed = 20;
		private Vector3 aimingDirection;

		public float width;

		public HealBeam()
		{
			castTime = 0f;
			coolDown = 4f;
			reuse = 1f;

			// 20dmg/sec
			baseDamage = 1;
			baseDamageFrequency = 0.5f;

			range = 15;
			width = 1f;

			movementAbortsSkill = true;

			updateFrequency = 0.01f;
			requireConfirm = true;
		}

		public override SkillId GetSkillId()
		{
			return SkillId.HealBeam;
		}

		public override string GetVisibleName()
		{
			return "Heal Beam";
		}

		public override Skill Instantiate()
		{
			return new HealBeam();
		}

		public override string GetDescription()
		{
			return "Fires a beam that heals allies.";
		}

		public override SkillEffect[] CreateEffects(int param)
		{
			return new SkillEffect[] { new EffectHeal(baseDamage),  };
		}

		public override void InitTraits()
		{
			AddTrait(SkillTraits.BuffDefense);
		}

		public override bool OnCastStart()
		{
			return true;
		}

		public override void OnLaunch()
		{
			lastHeal = 0;

			RotatePlayerTowardsMouse();

			ray = CreateParticleEffect("ray", true, GetOwnerData().GetShootingPosition().transform.position);
			ParticleSystem ps = ray.GetComponent<ParticleSystem>();

			/*SerializedObject so = new SerializedObject(ps);
			so.FindProperty("ShapeModule.radius").floatValue = width/2f;
			so.ApplyModifiedProperties();
			*/

			StartParticleEffect(ray);

			if (GetPlayerData() != null)
			{
				UpdateMouseDirection(ray.transform);
				aimingDirection = mouseDirection;

				ray.transform.rotation = Utils.GetRotationToMouse(ray.transform);
			}
			else if(initTarget != null)
			{
				aimingDirection = Utils.GetDirectionVector(initTarget.transform.position, GetOwnerData().GetBody().transform.position);
				ray.transform.rotation = Utils.GetRotationToTarget(ray.transform, initTarget);
			}
		}

		public override void UpdateLaunched()
		{
			if (ray != null)
			{
				Vector3 target;

				if (GetPlayerData() != null)
				{
					target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				}
				else if(initTarget != null)
				{
					target = initTarget.transform.position;
				}
				else
				{
					AbortCast();
					return;
				}

				Quaternion newRotation = Quaternion.LookRotation(GetOwnerData().GetBody().transform.position - target, Vector3.forward);
				newRotation.x = 0;
				newRotation.y = 0;
				newRotation = Quaternion.Lerp(GetOwnerData().GetBody().transform.rotation, newRotation, rotateSpeed*0.001f);

				GetOwnerData().SetRotation(newRotation, true);

				if (GetPlayerData() != null)
				{
					UpdateMouseDirection(ray.transform);
					aimingDirection = Vector3.Lerp(aimingDirection, mouseDirection, rotateSpeed * 0.001f);
				}
				else if (initTarget != null)
				{
					aimingDirection = Vector3.Lerp(aimingDirection, Utils.GetDirectionVector(initTarget.transform.position, GetOwnerData().GetBody().transform.position), rotateSpeed * 0.001f);
				}

				ray.transform.rotation = Utils.GetRotationToDirectionVector(aimingDirection);

				if (lastHeal + baseDamageFrequency < Time.time)
				{
					RaycastHit2D[] hits = Utils.DoubleRaycast(ray.transform.position, aimingDirection, range, width);

					foreach (RaycastHit2D hit in hits)
					{
						// dont hit yourself
						if (hit.transform.gameObject.Equals(GetOwnerData().GetBody()))
							continue;

						GameObject targetBody = hit.transform.gameObject;
						ApplyEffects(Owner, targetBody);
					}

					lastHeal = Time.time;
				}
			}
		}

		public override void OnAbort()
		{
		}

		public override void OnFinish()
		{
			DeleteParticleEffect(ray);
		}

		public override void MonoUpdate(GameObject gameObject, bool fixedUpdate)
		{
		}

		public override bool CanMove()
		{
			return !IsActive();
		}

		public override bool CanRotate()
		{
			return !IsActive();
		}
	}
}
