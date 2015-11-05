using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.scripts.Skills.Instances
{
	public class SkillTestProjectile : ActiveSkill
	{
		public SkillTestProjectile(string name, int id) : base(name, id)
		{

		}

		public override Skill Instantiate()
		{
			return new SkillTestProjectile(Name, Id);
		}

		public override bool OnCastStart()
		{
			if (GetPlayerData() == null)
				return false;

			GetPlayerData().SetImmobilized(true);
			return true;
		}

		public override void OnLaunch()
		{
			GetPlayerData().SetImmobilized(false);
		}

		public override void UpdateLaunched()
		{
		}

		public override void OnFinish()
		{
		}
	}
}
