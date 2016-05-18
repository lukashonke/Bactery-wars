using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Skills.Base;
using UnityEngine;

namespace Assets.scripts.Skills.SkillEffects
{
	public class EffectDamageOverTime : SkillEffect
	{
		private Character source;
		private Character target;

		private int damage;
		private bool crit;

		public EffectDamageOverTime(int damage, int count, float period, bool crit=false)
		{
			this.damage = damage;
			this.count = count;
			this.period = period;
			this.crit = crit;
		}

		public override void ApplyEffect(Character source, GameObject target)
		{
			Character targetCh = Utils.GetCharacter(target);

			if (targetCh == null)
				return;

			this.source = source;
			this.target = targetCh;

			ApplyDmg();

			AddToTarget(targetCh, 0);
		}

		private void ApplyDmg()
		{
			if (source == null || target == null)
				return;

			if (source.CanAttack(target))
			{
				bool wasCrit;
				int dmg = source.CalculateDamage(damage, target, crit, out wasCrit);

				source.OnAttack(target);

				target.ReceiveDamage(source, dmg, SourceSkill, crit);
			}
		}

		public override void Update()
		{
			ApplyDmg();
		}

		public override void OnRemove()
		{
		}

		public override SkillTraits[] GetTraits()
		{
			return new SkillTraits[] { SkillTraits.Damage, };
		}
	}
}
