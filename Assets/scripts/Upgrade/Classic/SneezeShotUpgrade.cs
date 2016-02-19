using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor.PlayerClasses.Base;
using Assets.scripts.Skills;
using Assets.scripts.Skills.ActiveSkills;
using Assets.scripts.Skills.Base;
using Assets.scripts.Skills.SkillEffects;

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

	public class SneezeShotPoisonUpgrade : AbstractUpgrade
	{
		public const int DAMAGE = 5;
		public const float LEVEL_ADD = 1f;

		public const int SLOW_AMMOUNT_PERCENT = 90;
		public const float SLOW_LEVEL_ADD = 1f;

		public SneezeShotPoisonUpgrade(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 5;
		}

		public override SkillEffect[] CreateAdditionalSkillEffects(Skill sk, SkillEffect[] effects)
		{
			if (sk.GetSkillId() == SkillId.SneezeShot)
			{
				SneezeShot skill = sk as SneezeShot;

				SkillEffect[] newEffects = new SkillEffect[2];
				newEffects[0] = new EffectSlow((AddValueByLevel(SLOW_AMMOUNT_PERCENT, SLOW_LEVEL_ADD) / 100f), 5);
				newEffects[1] = new EffectDamageOverTime((int) AddValueByLevel(DAMAGE, LEVEL_ADD), 5, 1, false);

				return newEffects;
			}

			return null;
		}

		protected override void InitInfo()
		{
			Name = "sneezeshot_upgrade";
			VisibleName = "Sneeze Shot Poison Upgrade";
			Description = "Sneeze Shot gives your target additional " + AddValueByLevel(DAMAGE, LEVEL_ADD) + " dmg/sec for 5 seconds and slows him by " + AddValueByLevel(SLOW_AMMOUNT_PERCENT, SLOW_LEVEL_ADD) + "% for 5 seconds.";
		}
	}

	public class SneezeShotPenetrateUpgrade : AbstractUpgrade
	{
		public SneezeShotPenetrateUpgrade(int level)
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

			skill.penetrateTargets = true;
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			SneezeShot skill = set.GetSkill(SkillId.SneezeShot) as SneezeShot;
			if (skill == null)
				return;

			skill.penetrateTargets = false;
		}

		protected override void InitInfo()
		{
			Name = "sneezeshot_upgrade";
			VisibleName = "Sneeze Shot Penetration Upgrade";
			Description = "Sneeze Shot penetrates through your targets to hit multiple enemies.";
		}
	}

	public class SneezeShotPushUpgrade : AbstractUpgrade
	{
		public SneezeShotPushUpgrade(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 1;
		}

		public override SkillEffect[] CreateAdditionalSkillEffects(Skill sk, SkillEffect[] effects)
		{
			if (sk.GetSkillId() == SkillId.SneezeShot)
			{
				SneezeShot skill = sk as SneezeShot;

				SkillEffect[] newEffects = new SkillEffect[2];
				newEffects[0] = new EffectPushaway(200);
				newEffects[1] = new EffectStun(1f);

				return newEffects;
			}

			return null;
		}

		protected override void InitInfo()
		{
			Name = "sneezeshot_upgrade";
			VisibleName = "Sneeze Shot Push Upgrade";
			Description = "Enemies hit by Sneeze Shot will be pushed away by the projectile.";
		}
	}

	public class SneezeShotExplodeUpgrade : AbstractUpgrade //TODO effect 
	{
		public const float RADIUS = 3;
		public const float RADIUS_LEVEL_ADD = 0.3f;

		private int temp;

		public SneezeShotExplodeUpgrade(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 10;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			SneezeShot skill = set.GetSkill(SkillId.SneezeShot) as SneezeShot;
			if (skill == null)
				return;

			temp = skill.projectilesCount;
			skill.projectilesCount = 1;
			skill.explodeEffect = true;
			skill.DisableOriginalEffects();
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			SneezeShot skill = set.GetSkill(SkillId.SneezeShot) as SneezeShot;
			if (skill == null)
				return;

			skill.projectilesCount = temp;
			skill.explodeEffect = false;
			skill.EnableOriginalEffects();
		}

		public override SkillEffect[] CreateAdditionalSkillEffects(Skill sk, SkillEffect[] effects)
		{
			if (sk.GetSkillId() == SkillId.SneezeShot)
			{
				SneezeShot skill = sk as SneezeShot;

				SkillEffect[] newEffects = new SkillEffect[1];
				newEffects[0] = new EffectAreaDamage(skill.baseDamage, 0, AddValueByLevel(RADIUS, RADIUS_LEVEL_ADD));

				return newEffects;
			}

			return null;
		}

		protected override void InitInfo()
		{
			Name = "sneezeshot_upgrade";
			VisibleName = "Sneeze Shot Push Upgrade";
			Description = "Sneeze Shot fires only one projectile which explodes on impact and deals area damage.";
		}
	}

	public class SneezeShotDoubleExploderUpgrade : AbstractUpgrade //TODO effect 
	{
		public const float RADIUS = 3;
		public const float RADIUS_LEVEL_ADD = 0.3f;

		private int temp;

		public SneezeShotDoubleExploderUpgrade(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 10;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			SneezeShot skill = set.GetSkill(SkillId.SneezeShot) as SneezeShot;
			if (skill == null)
				return;

			temp = skill.projectilesCount;
			skill.projectilesCount = 2;
			skill.explodeEffect = true;
			skill.randomAngle = 35;
			skill.DisableOriginalEffects();
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			SneezeShot skill = set.GetSkill(SkillId.SneezeShot) as SneezeShot;
			if (skill == null)
				return;

			skill.projectilesCount = temp;
			skill.explodeEffect = false;
			skill.randomAngle = 0;
			skill.EnableOriginalEffects();
		}

		public override SkillEffect[] CreateAdditionalSkillEffects(Skill sk, SkillEffect[] effects)
		{
			if (sk.GetSkillId() == SkillId.SneezeShot)
			{
				SneezeShot skill = sk as SneezeShot;

				SkillEffect[] newEffects = new SkillEffect[1];
				newEffects[0] = new EffectAreaDamage(skill.baseDamage, 0, AddValueByLevel(RADIUS, RADIUS_LEVEL_ADD));

				return newEffects;
			}

			return null;
		}

		protected override void InitInfo()
		{
			Name = "sneezeshot_upgrade";
			VisibleName = "Sneeze Shot Explode Upgrade";
			Description = "Sneeze Shot fires 2 projectiles in random direction in front of you, projectiles explode on contact and deal area damage.";
		}
	}

	public class SneezeShotTripleExploderUpgrade : AbstractUpgrade //TODO effect 
	{
		public const float RADIUS = 3;
		public const float RADIUS_LEVEL_ADD = 0.3f;

		private int temp;

		public SneezeShotTripleExploderUpgrade(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 10;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			SneezeShot skill = set.GetSkill(SkillId.SneezeShot) as SneezeShot;
			if (skill == null)
				return;

			temp = skill.projectilesCount;
			skill.projectilesCount = 3;
			skill.explodeEffect = true;
			skill.randomAngle = 35;
			skill.DisableOriginalEffects();
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			SneezeShot skill = set.GetSkill(SkillId.SneezeShot) as SneezeShot;
			if (skill == null)
				return;

			skill.projectilesCount = temp;
			skill.explodeEffect = false;
			skill.randomAngle = 0;
			skill.EnableOriginalEffects();
		}

		public override SkillEffect[] CreateAdditionalSkillEffects(Skill sk, SkillEffect[] effects)
		{
			if (sk.GetSkillId() == SkillId.SneezeShot)
			{
				SneezeShot skill = sk as SneezeShot;

				SkillEffect[] newEffects = new SkillEffect[1];
				newEffects[0] = new EffectAreaDamage(skill.baseDamage, 0, AddValueByLevel(RADIUS, RADIUS_LEVEL_ADD));

				return newEffects;
			}

			return null;
		}

		protected override void InitInfo()
		{
			Name = "sneezeshot_upgrade";
			VisibleName = "Sneeze Shot Explode Upgrade";
			Description = "Sneeze Shot fires 2 projectiles in random direction in front of you, projectiles explode on contact and deal area damage.";
		}
	}

	public class SneezeShotTripleMissilesUpgrade : AbstractUpgrade //TODO effect 
	{
		public const int COUNT = 6;
		public const float COUNT_ADD = 0.5f;

		private int temp, temp2;

		public SneezeShotTripleMissilesUpgrade(int level)
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

			temp = skill.projectilesCount;
			temp2 = skill.baseDamage;

			skill.projectilesCount = (int) AddValueByLevel(COUNT, COUNT_ADD);
			skill.randomAngle = 45;
			skill.navigateToTarget = true;
			skill.baseDamage = 10;
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			SneezeShot skill = set.GetSkill(SkillId.SneezeShot) as SneezeShot;
			if (skill == null)
				return;

			skill.projectilesCount = temp;
			skill.baseDamage = temp2;
			skill.randomAngle = 0;
			skill.navigateToTarget = false;
		}

		protected override void InitInfo()
		{
			Name = "sneezeshot_upgrade";
			VisibleName = "Sneeze Shot Push Upgrade";
			Description = "Sneeze Shot fires only one projectile which explodes on impact and deals area damage.";
		}
	}
}
