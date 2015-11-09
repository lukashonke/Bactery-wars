using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.scripts.Skills.Instances
{
	public class SkillTestProjectileTriple : ActiveSkill
	{
		public SkillTestProjectileTriple(string name, int id) : base(name, id)
		{
			castTime = 0.5f;
			reuse = 0.5f;
			coolDown = 0.5f;
		}

		public override Skill Instantiate()
		{
			return new SkillTestProjectileTriple(Name, Id);
		}

		public override bool OnCastStart()
		{
			if (GetPlayerData() == null)
				return false;

			return true;
		}

		public override void OnLaunch()
		{
			// this.GetType().Name vrati jmeno teto tridy ("SkillTestProjectile")
			GetPlayerData().ShootProjectileForward("SkillTestProjectile", "projectile_blacktest_i00", -15);
			GetPlayerData().ShootProjectileForward("SkillTestProjectile", "projectile_blacktest_i00", 0);
			GetPlayerData().ShootProjectileForward("SkillTestProjectile", "projectile_blacktest_i00", 15);
		}

		public override void UpdateLaunched()
		{
		}

		public override void OnFinish()
		{
			
		}

		public override void OnSkillEnd()
		{
		}

		public override bool CanMove()
		{
			if (IsBeingCasted() && state == SKILL_CASTING)
				return false;
			return true;
		}

		public override bool CanRotate()
		{
			return CanMove();
		}
	}
}
