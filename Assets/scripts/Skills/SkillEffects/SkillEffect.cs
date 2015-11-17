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
		protected SkillEffect()
		{
			
		}

		public abstract void ApplyEffect(Character source, GameObject target);
	}
}
