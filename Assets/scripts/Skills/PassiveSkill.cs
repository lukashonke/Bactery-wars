// Copyright (c) 2015, Lukas Honke
// ========================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.scripts.Skills
{
	public abstract class PassiveSkill : Skill
	{
		public PassiveSkill()
		{

		}

		public abstract void ApplyEffect();

		public override void InitTraits()
		{
			
		}

		protected override void InitDynamicTraits()
		{
		}

		public sealed override void SkillAdded()
		{
			ApplyEffect();
		}

		public override bool CanUse()
		{
			return false;
		}

		public override void SetReuseTimer()
		{
		}

		public override void Start()
		{
		}

		public override void End()
		{
		}

		public override string GetBaseInfo()
		{
			return null;
		}

		public override void AbortCast()
		{
		}

		public override bool IsBeingCasted()
		{
			return false;
		}
	}
}
