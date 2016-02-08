using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Mono;
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

			source.DoDie();
		}

		public override void Update()
		{
		}

		public override void OnRemove()
		{
		}
	}
}
