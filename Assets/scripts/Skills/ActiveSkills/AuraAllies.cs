// Copyright (c) 2015, Lukas Honke
// ========================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.AI;
using Assets.scripts.Skills.Base;
using Assets.scripts.Skills.SkillEffects;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Assets.scripts.Skills.ActiveSkills
{
	public class AuraAllies : Aura
	{
		public AuraAllies() : base()
		{
		}

		public override SkillId GetSkillId()
		{
			return SkillId.AuraAllies;
		}

		public override string GetVisibleName()
		{
			return "Aura";
		}

		public override Skill Instantiate()
		{
			return new AuraAllies();
		}

		public override string GetDescription()
		{
			return "Aura skill around player, affects allies only.";
		}

		public override SkillEffect[] CreateEffects(int param)
		{
			return new SkillEffect[] {   };
		}

		public override bool Offensive()
		{
			return false;
		}
	}
}
