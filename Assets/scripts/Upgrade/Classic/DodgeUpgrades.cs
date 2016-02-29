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
	public class DodgeFirstHitDamageUpgrade : AbstractUpgrade
	{
		public static int rarity = 1;
		public static UpgradeType type = UpgradeType.CLASSIC;

		public const int DAMAGE = 20;
		public const float LEVEL_ADD = 4;

		public DodgeFirstHitDamageUpgrade(int level) : base(level)
		{
			RequiredClass = ClassId.CommonCold;

			MaxLevel = 5;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			Dodge skill = set.GetSkill(SkillId.Dodge) as Dodge;
			if (skill == null)
				return;

			skill.firstEnemyHitDamage += (int) AddValueByLevel(DAMAGE, LEVEL_ADD);
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			Dodge skill = set.GetSkill(SkillId.Dodge) as Dodge;
			if (skill == null)
				return;

			skill.firstEnemyHitDamage -= (int)AddValueByLevel(DAMAGE, LEVEL_ADD);
		}

		protected override void InitInfo()
		{
			Name = "dodge_upgrade";
			VisibleName = "Dodge Hit Damage Upgrade";
			Description = "First enemy hit by you during Dodge jump will receive " + AddValueByLevel(DAMAGE, LEVEL_ADD) + " dmg.";
		}
	}

	public class DodgeHitDamageUpgrade : AbstractUpgrade
	{
		public static int rarity = 2;
		public static UpgradeType type = UpgradeType.CLASSIC;

		public const int DAMAGE = 15;
		public const float LEVEL_ADD = 4;

		public DodgeHitDamageUpgrade(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;

			MaxLevel = 5;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			Dodge skill = set.GetSkill(SkillId.Dodge) as Dodge;
			if (skill == null)
				return;

			skill.hitEnemyDamage += (int)AddValueByLevel(DAMAGE, LEVEL_ADD);
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			Dodge skill = set.GetSkill(SkillId.Dodge) as Dodge;
			if (skill == null)
				return;

			skill.hitEnemyDamage -= (int)AddValueByLevel(DAMAGE, LEVEL_ADD);
		}

		protected override void InitInfo()
		{
			Name = "dodge_upgrade";
			VisibleName = "Dodge Hit Damage Upgrade";
			Description = "All enemies hit by you during Dodge jump will receive " + AddValueByLevel(DAMAGE, LEVEL_ADD) +" dmg.";
		}
	}

	public class DodgePenetrateThroughTargets : AbstractUpgrade
	{
		public static int rarity = 1;
		public static UpgradeType type = UpgradeType.RARE;

		public const int DAMAGE = 10;
		public const float LEVEL_ADD = 3;

		public DodgePenetrateThroughTargets(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;

			MaxLevel = 5;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			Dodge skill = set.GetSkill(SkillId.Dodge) as Dodge;
			if (skill == null)
				return;

			skill.penetrateThroughTargets = true;
			skill.hitEnemyDamage += (int)AddValueByLevel(DAMAGE, LEVEL_ADD);
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			Dodge skill = set.GetSkill(SkillId.Dodge) as Dodge;
			if (skill == null)
				return;

			skill.penetrateThroughTargets = false;
			skill.hitEnemyDamage -= (int)AddValueByLevel(DAMAGE, LEVEL_ADD);
		}

		protected override void InitInfo()
		{
			Name = "dodge_upgrade";
			VisibleName = "Dodge Penetrate Upgrade";
			Description = "You will jump through targets, dealing " + AddValueByLevel(DAMAGE, LEVEL_ADD) + " dmg to each.";
		}
	}

	public class DodgeRangeIncrease : AbstractUpgrade
	{
		public static int rarity = 2;
		public static UpgradeType type = UpgradeType.CLASSIC;

		public const int VALUE = 5;
		public const float LEVEL_ADD = 1f;

		public DodgeRangeIncrease(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;

			MaxLevel = 5;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			Dodge skill = set.GetSkill(SkillId.Dodge) as Dodge;
			if (skill == null)
				return;

			skill.range += (int) AddValueByLevel(VALUE, LEVEL_ADD);
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			Dodge skill = set.GetSkill(SkillId.Dodge) as Dodge;
			if (skill == null)
				return;

			skill.range -= (int) AddValueByLevel(VALUE, LEVEL_ADD);
		}

		protected override void InitInfo()
		{
			Name = "dodge_upgrade";
			VisibleName = "Dodge Range Upgrade";
			Description = "Increases Dodge range by " + AddValueByLevel(VALUE, LEVEL_ADD) + " points (default is 10).";
		}
	}

	public class DodgeCharges : AbstractUpgrade
	{
		public static int rarity = 1;
		public static UpgradeType type = UpgradeType.RARE;

		public DodgeCharges(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;

			MaxLevel = 5;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			Dodge skill = set.GetSkill(SkillId.Dodge) as Dodge;
			if (skill == null)
				return;

			skill.maxConsecutiveCharges += 1;
			skill.consecutiveTimelimit = AddValueByLevel(3, 1f);
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			Dodge skill = set.GetSkill(SkillId.Dodge) as Dodge;
			if (skill == null)
				return;

			skill.maxConsecutiveCharges -= 1;
			skill.consecutiveTimelimit -= AddValueByLevel(3, 1f);
		}

		protected override void InitInfo()
		{
			Name = "dodge_upgrade";
			VisibleName = "Dodge Charges Upgrade";
			Description = "Gives you two charges of Dodge skill. The second charge must be triggered within " + AddValueByLevel(3, 1f) + " seconds from launching the first charge.";
		}
	}

	public class DodgeRechargeShotUpgrade : AbstractUpgrade
	{
		public static int rarity = 1;
		public static UpgradeType type = UpgradeType.RARE;

		public DodgeRechargeShotUpgrade(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			Dodge skill = set.GetSkill(SkillId.Dodge) as Dodge;
			if (skill == null)
				return;

			skill.skillToBeRechargedOnThisUse = SkillId.SneezeShot;
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			Dodge skill = set.GetSkill(SkillId.Dodge) as Dodge;
			if (skill == null)
				return;

			skill.skillToBeRechargedOnThisUse = 0;
		}

		protected override void InitInfo()
		{
			Name = "dodge_upgrade";
			VisibleName = "Dodge Combo Upgrade";
			Description = "After using Dodge skill, skill Sneeze Shot is instantly recharged and available for use.";
		}
	}

	public class DodgeSpreadshotOnLand : AbstractUpgrade
	{
		public static int rarity = 2;
		public static UpgradeType type = UpgradeType.CLASSIC;

		public const int DAMAGE = 15;
		public const float LEVEL_ADD = 3f;

		public DodgeSpreadshotOnLand(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;

			MaxLevel = 5;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			Dodge skill = set.GetSkill(SkillId.Dodge) as Dodge;
			if (skill == null)
				return;

			skill.spreadshotOnLand = true;
			skill.spreadshotDamage = (int) AddValueByLevel(DAMAGE, LEVEL_ADD);
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			Dodge skill = set.GetSkill(SkillId.Dodge) as Dodge;
			if (skill == null)
				return;

			skill.spreadshotOnLand = false;
			skill.spreadshotDamage = 0;
		}

		protected override void InitInfo()
		{
			Name = "dodge_upgrade";
			VisibleName = "Dodge Spreadshot Upgrade";
			Description = "Shoots out 4 projectiles in all directions that deal " + AddValueByLevel(DAMAGE, LEVEL_ADD) + " to enemies.";
		}
	}
}
