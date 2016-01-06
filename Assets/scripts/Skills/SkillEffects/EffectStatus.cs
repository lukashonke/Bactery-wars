using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using UnityEngine;

namespace Assets.scripts.Skills.SkillEffects
{
	public abstract class EffectStatus : SkillEffect
	{
		protected Character target;

		private float duration;

		public EffectStatus(float duration)
		{
			this.duration = duration;
		}

		public override void ApplyEffect(Character source, GameObject target)
		{
			Character targetCh = Utils.GetCharacter(target);

			if (targetCh == null)
				return;

			this.target = targetCh;

			Update();

			AddToTarget(targetCh, duration);
		}

		public override void Update()
		{
			ApplyEffect();
		}

		public override void OnRemove()
		{
			RemoveEffect();
		}

		protected abstract void ApplyEffect();
		protected abstract void RemoveEffect();
	}
}
