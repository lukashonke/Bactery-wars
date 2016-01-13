using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using UnityEngine;

namespace Assets.scripts.Skills.SkillEffects
{
	public abstract class SkillEffect
	{
		public Character Source { get; set; }

		public float lastUpdateTime;

		protected bool isOffensive;

		public float period = -1;
		public int count = -1;

		protected SkillEffect()
		{
			isOffensive = true;
		}

		protected void AddToTarget(Character target, float duration)
		{
			target.AddEffect(this, duration);
		}

		protected void RemoveFromTarget(Character target)
		{
			target.RemoveEffect(this);
		}

		public abstract void ApplyEffect(Character source, GameObject target);

		public abstract void Update();
		public abstract void OnRemove();

		public virtual void ModifySkillCasttime(ActiveSkill sk, ref float reuse)
		{
			
		}

		protected bool IsOffensive()
		{
			return isOffensive;
		}
	}
}
