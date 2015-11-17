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
			return new SkillEffect[] { new EffectDamage(5, 0) };
		}

		public override bool OnCastStart()
		{
			return true;
		}

		public override void OnLaunch()
		{
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
