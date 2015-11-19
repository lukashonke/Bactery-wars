using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Skills.SkillEffects;
using UnityEngine;

namespace Assets.scripts.Skills.ActiveSkills
{
	public class ChainSkill : ActiveSkill
	{
		protected GameObject ray;

		private int lastDmg = 0;

		public ChainSkill(string name, int id) : base(name, id)
		{
			castTime = 0f;
			coolDown = 3f;
			reuse = 5f;
			updateFrequency = 0.01f;
			requireConfirm = true;
		}

		public override Skill Instantiate()
		{
			return new ChainSkill(Name, Id);
		}

		public override SkillEffect[] CreateEffects()
		{
			return new SkillEffect[] { new EffectDamage(2, 0) }; // deal 2 dmg / 250ms
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

				if (lastDmg + 250 < System.Environment.TickCount)
				{
					RaycastHit2D[] hits = Physics2D.RaycastAll(ray.transform.position, mouseDirection, 20);

					foreach (RaycastHit2D hit in hits)
					{
						// dont hit yourself
						if (hit.transform.gameObject.Equals(GetOwnerData().GetBody()))
							continue;

						GameObject targetBody = hit.transform.gameObject;
						ApplyEffects(Owner, targetBody);
					}

					lastDmg = System.Environment.TickCount;
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
