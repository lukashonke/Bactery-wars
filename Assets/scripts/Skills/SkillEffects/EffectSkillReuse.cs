using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Skills.Base;
using UnityEngine;

namespace Assets.scripts.Skills.SkillEffects
{
	public class EffectSkillReuse : EffectStatus
	{
		private float multiplier;
		private float fixedValue;
		private SkillTraits traitToAffect;
		//private SkillId[] idsToAffect;

		private int uses = -1;

		public EffectSkillReuse(float mul, float value, float duration, SkillTraits toAffect/*, params SkillId[] idsToAffect*/) : base(duration)
		{
			this.multiplier = mul;
			this.fixedValue = value;
			this.traitToAffect = toAffect;

			isOffensive = false;
		}

		public EffectSkillReuse(float mul, float duration)
			: base(duration)
		{
			this.multiplier = mul;
			this.fixedValue = -1;
			this.traitToAffect = SkillTraits.None;

			isOffensive = false;
		}

		protected override void ApplyEffect()
		{
		}

		protected override void RemoveEffect()
		{
		}

		public override void ModifySkillReuse(ActiveSkill sk, ref float reuse, bool skillBeingCast)
		{
			if (traitToAffect == SkillTraits.None || sk.HasTrait(traitToAffect))
			{
				bool allow = false;
				if (uses > 0)
				{
					if (skillBeingCast)
					{
						Exception e = new Exception();
						Debug.Log(e.StackTrace);
						uses --;
					}

					allow = true;

					if(uses <= 0) // remove this effect
						this.remove = true;
				}
				else if (uses == -1)
				{
					allow = true;
				}

				if (allow)
				{
					reuse *= multiplier;

					if (fixedValue > -1)
					{
						reuse = fixedValue;
					}
				}
				else
					this.remove = true;
			}
		}

		public override void ModifySkillCasttime(ActiveSkill sk, ref float reuse)
		{
			/*if (sk.HasTrait(traitToAffect))
			{
				reuse *= multiplier;

				if (fixedValue > -1)
				{
					reuse = fixedValue;
				}
			}*/
		}

		public EffectSkillReuse SetCountUses(int count)
		{
			uses = count;
			return this;
		}

		public override SkillTraits[] GetTraits()
		{
			return new SkillTraits[] { SkillTraits.BuffDamage, };
		}
	}
}
