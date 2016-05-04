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
	public class PushBeam : ActiveSkill
	{
		protected GameObject ray;

		private float lastDmg = 0;
		public float rotateSpeed = 25;
		private Vector3 aimingDirection;

		public float width;

		public PushBeam()
		{
			castTime = 0f;
			coolDown = 4f;
			reuse = 1f;

			// 20dmg/sec
			baseDamage = 20;
			baseDamageFrequency = 0.1f;

			range = 25;
			width = 1.5f;

			movementAbortsSkill = true;

			updateFrequency = 0.01f;
			requireConfirm = true;
		}

		public override SkillId GetSkillId()
		{
			return SkillId.PushBeam;
		}

		public override string GetVisibleName()
		{
			return "Push Beam";
		}

		public override Skill Instantiate()
		{
			return new PushBeam();
		}

		public override string GetDescription()
		{
			return "Fires a beam that pushes away enemies.";
		}

		public override SkillEffect[] CreateEffects(int param)
		{
			return new SkillEffect[] { new EffectPushaway(baseDamage),  };
		}

		public override void InitTraits()
		{
			AddTrait(SkillTraits.Damage);
		}

		public override bool OnCastStart()
		{
			return true;
		}

		public override void OnLaunch()
		{
			lastDmg = 0;

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
			else if (initTarget != null)
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
				else if (initTarget != null)
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
				newRotation = Quaternion.Lerp(GetOwnerData().GetBody().transform.rotation, newRotation, rotateSpeed * 0.001f);

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

				if (lastDmg + baseDamageFrequency < Time.time)
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

					lastDmg = Time.time;
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
