using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Skills.Base;
using UnityEngine;

namespace Assets.scripts.Skills.SkillEffects
{
	public class EffectHealSelfOverTime : SkillEffect
	{
		private Character source;

		private int heal;

		public EffectHealSelfOverTime(int heal, int count, float period)
		{
			this.heal = heal;
			this.count = count;
			this.period = period;
		}

		public override void ApplyEffect(Character source, GameObject target)
		{
			this.source = source;

			//ApplyHeal();

			AddToTarget(source, 0);
		}

		private void ApplyHeal()
		{
			if (source == null)
				return;
			source.ReceiveHeal(source, heal, SourceSkill);
		}

		public override void Update()
		{
			ApplyHeal();
		}

		public override void OnRemove()
		{
		}

		public override SkillTraits[] GetTraits()
		{
			return new SkillTraits[] { SkillTraits.HealSelf, };
		}
	}
}
