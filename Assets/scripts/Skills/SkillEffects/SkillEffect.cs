﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Skills.Base;
using UnityEngine;

namespace Assets.scripts.Skills.SkillEffects
{
	public abstract class SkillEffect
	{
		public SkillId SourceSkill { get; set; }
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

		public virtual void ModifySkillReuse(ActiveSkill sk, ref float reuse)
		{

		}

		public virtual void ModifySkillCooldown(ActiveSkill sk, ref float cooldown)
		{
			
		}

		public virtual void ModifySkillRange(ActiveSkill sk, ref int range)
		{
			
		}

		public virtual void ModifyCharDamage(ref int damage)
		{
			
		}

		public virtual void ModifyCritRate(ref int critRate)
		{
			
		}

		protected bool IsOffensive()
		{
			return isOffensive;
		}
	}
}
