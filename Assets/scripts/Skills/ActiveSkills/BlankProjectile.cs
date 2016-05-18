using Assets.scripts.Actor;
using Assets.scripts.Base;
using Assets.scripts.Mono;
using Assets.scripts.Skills.Base;
using Assets.scripts.Skills.SkillEffects;
using UnityEngine;

namespace Assets.scripts.Skills.ActiveSkills
{
	/// <summary>
	/// a Projectile that has no effects
	/// </summary>
	public class BlankProjectile : Projectile
	{
		public BlankProjectile() : base()
		{
		}

		public override SkillId GetSkillId()
		{
			return SkillId.BlankProjectile;
		}

		public override string GetVisibleName()
		{
			return "Projectile";
		}

		public override Skill Instantiate()
		{
			return new BlankProjectile();
		}

		public override SkillEffect[] CreateEffects(int param)
		{
			return new SkillEffect[] {};
		}

		public override void InitTraits()
		{
			AnalyzeEffectsForTrais();
		}
	}
}
