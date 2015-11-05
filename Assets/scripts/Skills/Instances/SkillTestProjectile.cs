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

			GetPlayerData().SetCanMove(false);
			GetPlayerData().SetCanRotate(false);
			return true;
		}

		public override void OnLaunch()
		{
			GetPlayerData().SetCanMove(true);
			GetPlayerData().SetCanRotate(true);
		}

		public override void UpdateLaunched()
		{
		}

		public override void OnFinish()
		{
			
		}

		public override void OnSkillEnd()
		{
			GetPlayerData().SetCanMove(true);
			GetPlayerData().SetCanRotate(true);
		}
	}
}
