// Copyright (c) 2015, Lukas Honke
// ========================
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
		protected Character source;
		protected Character target;

		protected float duration;

		protected bool targetOwner;

		public EffectStatus(float duration)
		{
			this.duration = duration;

			targetOwner = false;
		}

		public EffectStatus TargetOwner()
		{
			targetOwner = true;
			return this;
		}

		public override void ApplyEffect(Character source, GameObject target)
		{
			this.source = source;

			Character targetCh = Utils.GetCharacter(target);

			if (targetCh == null)
				return;

			this.target = targetCh;

			if (targetOwner)
				this.target = source;

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
