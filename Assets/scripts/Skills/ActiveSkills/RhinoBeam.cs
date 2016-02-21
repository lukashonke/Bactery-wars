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
		private readonly float rotateSpeed = 20;
		private Vector3 aimingDirection;

		public RhinoBeam()
		{
			castTime = 0f;
			coolDown = 4f;
			reuse = 20f;

			// 20dmg/sec
			baseDamage = 5;
			baseDamageFrequency = 0.25f;

			range = 15;

			movementAbortsSkill = true;

			updateFrequency = 0.01f;
			requireConfirm = true;
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

			GetPlayerData().SetRotation(Camera.main.ScreenToWorldPoint(Input.mousePosition), true);

			ray = CreateParticleEffect("ray", true, GetOwnerData().GetShootingPosition().transform.position);
			StartParticleEffect(ray);

			UpdateMouseDirection(ray.transform);
			aimingDirection = mouseDirection;
			ray.transform.rotation = Utils.GetRotationToMouse(ray.transform);
		}

		public override void UpdateLaunched()
		{
			if (ray != null)
			{
				Quaternion newRotation = Quaternion.LookRotation(GetOwnerData().GetBody().transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.forward);
				newRotation.x = 0;
				newRotation.y = 0;

				newRotation = Quaternion.Lerp(GetOwnerData().GetBody().transform.rotation, newRotation, rotateSpeed*0.001f);

				GetOwnerData().SetRotation(newRotation, true);

				UpdateMouseDirection(ray.transform);

				aimingDirection = Vector3.Lerp(aimingDirection, mouseDirection, rotateSpeed*0.001f);

				ray.transform.rotation = Utils.GetRotationToDirectionVector(aimingDirection);

				if (lastDmg + baseDamageFrequency < Time.time)
				{
					RaycastHit2D[] hits = Utils.DoubleRaycast(ray.transform.position, aimingDirection, range, 1);

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
