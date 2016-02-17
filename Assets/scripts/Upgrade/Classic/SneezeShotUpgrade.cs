using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor.PlayerClasses.Base;
using Assets.scripts.Skills;
using Assets.scripts.Skills.ActiveSkills;
using Assets.scripts.Skills.Base;

namespace Assets.scripts.Upgrade.Classic
{
	public class SneezeShotThreeProjectiles : AbstractUpgrade
	{

		public SneezeShotThreeProjectiles(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 1;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			SneezeShot skill = set.GetSkill(SkillId.SneezeShot) as SneezeShot;
			if (skill == null)
				return;

			skill.projectilesCount += 1;
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			SneezeShot skill = set.GetSkill(SkillId.SneezeShot) as SneezeShot;
			if (skill == null)
				return;

			skill.projectilesCount -= 1;
		}

		protected override void InitInfo()
		{
			Name = "sneezeshot_upgrade";
			VisibleName = "Sneeze Shot Projectile Upgrade";
			Description = "Sneeze Shot fires one extra projectile.";
		}
	}

	public class SneezeShotDamageUpgrade : AbstractUpgrade
	{
		public const int DAMAGE = 10;
		public const float LEVEL_ADD = 1f;

		public SneezeShotDamageUpgrade(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 5;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			SneezeShot skill = set.GetSkill(SkillId.SneezeShot) as SneezeShot;
			if (skill == null)
				return;

			skill.baseDamage += (int) AddValueByLevel(DAMAGE, LEVEL_ADD);
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			SneezeShot skill = set.GetSkill(SkillId.SneezeShot) as SneezeShot;
			if (skill == null)
				return;

			skill.baseDamage -= (int)AddValueByLevel(DAMAGE, LEVEL_ADD);
		}

		protected override void InitInfo()
		{
			Name = "sneezeshot_upgrade";
			VisibleName = "Sneeze Shot Damage Upgrade";
			Description = "Sneeze Shot deals " + (int)AddValueByLevel(DAMAGE, LEVEL_ADD) + " extra damage.";
		}
	}

	public class SneezeShotAimedProjectiles : AbstractUpgrade
	{
		public SneezeShotAimedProjectiles(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 1;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			SneezeShot skill = set.GetSkill(SkillId.SneezeShot) as SneezeShot;
			if (skill == null)
				return;

			skill.projectilesAim = true;
			skill.aimArea = 10;
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			SneezeShot skill = set.GetSkill(SkillId.SneezeShot) as SneezeShot;
			if (skill == null)
				return;

			skill.projectilesAim = false;
			skill.aimArea = 20;
		}

		protected override void InitInfo()
		{
			Name = "sneezeshot_upgrade";
			VisibleName = "Sneeze Shot Aim Upgrade";
			Description = "Projectiles automatically scan area in front of them to adjust their path to hit enemies.";
		}
	}

	public class SneezeShotHomingMissile : AbstractUpgrade
	{
		private int temp;
		private int temp2;

		public SneezeShotHomingMissile(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 1;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			SneezeShot skill = set.GetSkill(SkillId.SneezeShot) as SneezeShot;
			if (skill == null)
				return;

			temp = skill.projectilesCount;
			temp2 = skill.range;
			skill.range *= 3;
			skill.projectilesCount = 1;
			skill.projectilesAim = true;
			skill.aimArea = skill.range;
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			SneezeShot skill = set.GetSkill(SkillId.SneezeShot) as SneezeShot;
			if (skill == null)
				return;

			skill.projectilesCount = temp;
			skill.range = temp2;
			skill.projectilesAim = false;
			skill.aimArea = 0;
		}

		protected override void InitInfo()
		{
			Name = "sneezeshot_upgrade";
			VisibleName = "Sneeze Shot Missile Upgrade";
			Description = "Sneeze Shot fires one projectile that is guided to its target. Range is tripled.";
		}
	}
}
