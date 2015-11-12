namespace Assets.scripts.Skills.ActiveSkills
{
	public class SkillTestProjectileAllAround : ActiveSkill
	{
		public SkillTestProjectileAllAround(string name, int id) : base(name, id)
		{
			castTime = 2f;
			reuse = 0.5f;
			coolDown = 0.5f;
		}

		public override Skill Instantiate()
		{
			return new SkillTestProjectileAllAround(Name, Id);
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

			for (int i = 0; i < 360; i += 30)
			{
				GetPlayerData().ShootProjectileForward("SkillTestProjectile", "projectile_blacktest_i00", i);
			}
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
			if (IsBeingCasted())
				return false;
			return true;
		}

		public override bool CanRotate()
		{
			return CanMove();
		}
	}
}
