using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.scripts.Skills.Instances
{
	public class SkillTestProjectile : ActiveSkill
	{
		public SkillTestProjectile(string name, int id) : base(name, id)
		{
			castTime = 0f;
			reuse = 0;
			coolDown = 0;
		}

		public override Skill Instantiate()
		{
			return new SkillTestProjectile(Name, Id);
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
			GetPlayerData().ShootProjectileForward(this.GetType().Name, "projectile_blacktest_i00", 0);
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
