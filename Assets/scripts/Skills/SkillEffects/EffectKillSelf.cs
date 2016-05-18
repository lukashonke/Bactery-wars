using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Mono;
using Assets.scripts.Skills.Base;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.scripts.Skills.SkillEffects
{
	public class EffectKillSelf : SkillEffect
	{
		public EffectKillSelf()
		{
		}

		public override void ApplyEffect(Character source, GameObject target)
		{
			if (source == null)
				return;

			source.DoDie(source);
		}

		public override void Update()
		{
		}

		public override void OnRemove()
		{
		}

		public override SkillTraits[] GetTraits()
		{
			return new SkillTraits[] { SkillTraits.Kamikadze, };
		}
	}
}
