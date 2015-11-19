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
		protected GameObject particleSystemObject;

		private int lastDmg = 0;

		public ChainSkill(string name, int id)
			: base(name, id)
		{
			castTime = 0f;
			coolDown = 3f; // lasts 5s
			reuse = 5f;
			updateFrequency = 0.01f;
			requireConfirm = true;
			MovementBreaksConfirmation = true;
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

			particleSystemObject = GetOwnerData().CreateSkillResource("ChainSkill", "ray", true, GetOwnerData().GetShootingPosition().transform.position);
			StartParticleEffect(particleSystemObject);

			UpdateMouseDirection(particleSystemObject.transform);
			particleSystemObject.transform.rotation = Utils.GetRotationToMouse(particleSystemObject.transform);
		}

		public override void UpdateLaunched()
		{
			if (particleSystemObject != null)
			{
				GetPlayerData().SetRotation(Camera.main.ScreenToWorldPoint(Input.mousePosition), true);

				UpdateMouseDirection(particleSystemObject.transform);
				particleSystemObject.transform.rotation = Utils.GetRotationToDirectionVector(mouseDirection);

				RaycastHit2D[] hits = Physics2D.RaycastAll(particleSystemObject.transform.position, mouseDirection, 20);

				if (lastDmg + 250 < System.Environment.TickCount)
				{
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
			DeleteParticleEffect(particleSystemObject);
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
