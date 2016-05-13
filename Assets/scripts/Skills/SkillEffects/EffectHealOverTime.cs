using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using UnityEngine;

namespace Assets.scripts.Skills.SkillEffects
{
	public class EffectHealOverTime : SkillEffect
	{
		private Character source;
		private Character target;

		private int heal;

		public EffectHealOverTime(int heal, int count, float period)
		{
			this.heal = heal;
			this.count = count;
			this.period = period;
		}

		public override void ApplyEffect(Character source, GameObject target)
		{
			Character targetCh = Utils.GetCharacter(target);

			if (targetCh == null || source.Equals(targetCh))
				return;

			this.source = source;
			this.target = targetCh;

			//ApplyHeal();

			AddToTarget(targetCh, 0);
		}

		private void ApplyHeal()
		{
			if (source == null || target == null)
				return;

			target.ReceiveHeal(source, heal, SourceSkill);
		}

		public override void Update()
		{
			ApplyHeal();
		}

		public override void OnRemove()
		{
		}
	}
}
