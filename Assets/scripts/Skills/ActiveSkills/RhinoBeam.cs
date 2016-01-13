using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Skills.Base;
using Assets.scripts.Skills.SkillEffects;
using UnityEngine;

namespace Assets.scripts.Skills.ActiveSkills
{
	public class RhinoBeam : ActiveSkill
	{
		protected GameObject ray;

		private float lastDmg = 0;

		public RhinoBeam()
		{
			castTime = 0f;
			coolDown = 4f;
			reuse = 30f;

			// 10dmg/sec
			baseDamage = 5;
			baseDamageFrequency = 0.5f;

			range = 15;

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

		public override SkillEffect[] CreateEffects()
		{
			return new SkillEffect[] { new EffectDamage(baseDamage, 0), new EffectSlow(5, 2),  };
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
			ray.transform.rotation = Utils.GetRotationToMouse(ray.transform);
		}

		public override void UpdateLaunched()
		{
			if (ray != null)
			{
				RotatePlayerTowardsMouse();

				UpdateMouseDirection(ray.transform);
				ray.transform.rotation = Utils.GetRotationToDirectionVector(mouseDirection);

				if (lastDmg + baseDamageFrequency < Time.time)
				{
					RaycastHit2D[] hits = Physics2D.RaycastAll(ray.transform.position, mouseDirection, range);

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

		public override void MonoUpdate(GameObject gameObject)
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
