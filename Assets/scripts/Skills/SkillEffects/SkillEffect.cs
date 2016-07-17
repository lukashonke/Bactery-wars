// Copyright (c) 2015, Lukas Honke
// ========================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Skills.Base;
using UnityEngine;

namespace Assets.scripts.Skills.SkillEffects
{
	/// <summary>
	/// Represents an effect of a skill
	/// </summary>
	public abstract class SkillEffect : ICloneable
	{
		public SkillId SourceSkill { get; set; }
		public Character Source { get; set; }
		public Skill SourceSkillObject { get; set; }

		public float lastUpdateTime;

		protected bool isOffensive;

		public float period = -1;
		public int count = -1;

		public bool removed;
		public bool remove;

		protected SkillEffect()
		{
			isOffensive = true;
			removed = false;
			remove = false;
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

		public void Remove()
		{
			if (SourceSkillObject != null)
			{
				SourceSkillObject.NotifyEffectRemoved(this);
			}
		}

		public virtual void ModifySkillCasttime(ActiveSkill sk, ref float reuse)
		{
			
		}

		public virtual void ModifySkillReuse(ActiveSkill sk, ref float reuse, bool skillBeingCast)
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

		public virtual void OnCharDie()
		{
			
		}

		public virtual void OnReceiveDamage(Character source, int damage, SkillId skillId = 0, bool wasCrit = false)
		{
			
		}

		protected bool IsOffensive()
		{
			return isOffensive;
		}

		public abstract SkillTraits[] GetTraits();
		public object Clone()
		{
			return this.MemberwiseClone();
		}
	}
}
