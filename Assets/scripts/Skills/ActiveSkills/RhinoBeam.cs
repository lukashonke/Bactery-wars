﻿// Copyright (c) 2015, Lukas Honke
// ========================
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
	public class RhinoBeam : ActiveSkill
	{
		protected GameObject ray;

		private float lastDmg = 0;
		public float rotateSpeed = 20;
		private Vector3 aimingDirection;

		public float width;

		public RhinoBeam()
		{
			castTime = 0f;
			coolDown = 4f;
			reuse = 20f;

			// 20dmg/sec
			baseDamage = 5;
			baseDamageFrequency = 0.25f;

			range = 15;
			width = 1f;

			movementAbortsSkill = true;

			updateFrequency = 0.01f;
			requireConfirm = true;

			AvailableToPlayer = true;
		}

		public override SkillId GetSkillId()
		{
			return SkillId.RhinoBeam;
		}

		public override string GetVisibleName()
		{
			return "Rhino Beam";
		}

		public override Skill Instantiate()
		{
			return new RhinoBeam();
		}

		public override string GetDescription()
		{
			return "Fires a beam that deals large damage and slows targets.";
		}

		public override SkillEffect[] CreateEffects(int param)
		{
			return new SkillEffect[] { new EffectDamage(baseDamage, 0), new EffectSlow(0.9f, 2),  };
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

			ParticleSystem.ShapeModule shape = ps.shape;
			shape.radius = width / 2f;

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
					RaycastHit2D[] hits = Utils.DoubleRaycast(ray.transform.position, aimingDirection, GetRange(), width);

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
