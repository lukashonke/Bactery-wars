// Copyright (c) 2015, Lukas Honke
// ========================

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Skills.Base;
using UnityEngine;
using Object = System.Object;

namespace Assets.scripts.Skills.SkillEffects
{
	public class EffectForceShield : EffectStatus
	{
		private float value;

		private float temp;

		private GameObject forceShield;

		public EffectForceShield(float value, float duration)
			: base(duration)
		{
			this.value = value;
		}

		protected override void ApplyEffect()
		{
			if (target == null)
				return;

			if (forceShield != null)
				UnityEngine.Object.Destroy(forceShield);

			// explosion effect
			forceShield = ((ActiveSkill)SourceSkillObject).CreateParticleEffect("ForceShield", true, source.GetData().GetShootingPosition().transform.position);
			forceShield.GetComponent<ParticleSystem>().Play();

			if(value > 0 || value < 0)
				target.SetShield(target.Status.Shield + value);
		}

		protected override void RemoveEffect()
		{
			if (target == null)
				return;

			if (forceShield != null)
				UnityEngine.Object.Destroy(forceShield);

			if (value > 0 || value < 0)
				target.SetShield(target.Status.Shield - value);
		}

		public override void OnReceiveDamage(Character source, int damage, SkillId skillId = SkillId.SkillTemplate, bool wasCrit = false)
		{
			this.remove = true;
		}

		public override SkillTraits[] GetTraits()
		{
			return new SkillTraits[] { SkillTraits.BuffDefense, };
		}
	}
}
