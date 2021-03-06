﻿// Copyright (c) 2015, Lukas Honke
// ========================
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
	public class DodgeSpeedUpgrade : EquippableItem
	{
		public static int rarity = 1;
		public static ItemType type = ItemType.CLASSIC_UPGRADE;
		public static bool enabled = true;

		private const float DURATION = 5f;

		public DodgeSpeedUpgrade(int level)
			: base(level)
		{
			MaxLevel = 1;
		}

		public override SkillEffect[] CreateAdditionalSkillEffects(Skill sk, SkillEffect[] effects)
		{
			if (sk.GetSkillId() == SkillId.Dodge)
			{
				Dodge dg = sk as Dodge;
				SkillEffect[] newEffects = new SkillEffect[1];
				newEffects[0] = new EffectDash(0.3f, DURATION);

				return newEffects;
			}

			return null;
		}

		protected override void InitInfo()
		{
			FileName = "ColdDodge_upgrade";
			TypeName = "Cold Dodge";
			VisibleName = "Speed Upgrade";
			Description = "Increases your movement speed by 33% after jump for " + DURATION + " seconds.";
		}
	}

	public class DodgeReuseUpgrade : EquippableItem
	{
		public static int rarity = 1;
		public static ItemType type = ItemType.CLASSIC_UPGRADE;
		public static bool enabled = true;

		private float temp;

		public DodgeReuseUpgrade(int level)
			: base(level)
		{
			MaxLevel = 1;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			Dodge skill = set.GetSkill(SkillId.Dodge) as Dodge;
			if (skill == null)
				return;

			temp = skill.reuse * 0.3f;
			skill.reuse -= temp;
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			Dodge skill = set.GetSkill(SkillId.Dodge) as Dodge;
			if (skill == null)
				return;

			skill.reuse += temp;
		}

		protected override void InitInfo()
		{
			FileName = "ColdDodge_upgrade";
			TypeName = "Cold Dodge";
			VisibleName = "Reuse Module";
			Description = "Decreases Cold Dodge reuse by 30%.";
		}
	}

	public class DodgeDoublejumpUpgrade : EquippableItem
	{
		public static int rarity = 1;
		public static ItemType type = ItemType.CLASSIC_UPGRADE;
		public static bool enabled = true;

		public DodgeDoublejumpUpgrade(int level)
			: base(level)
		{
			MaxLevel = 1;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			Dodge skill = set.GetSkill(SkillId.Dodge) as Dodge;
			if (skill == null)
				return;

			skill.maxConsecutiveCharges += 1;
			skill.consecutiveTimelimit = 3f;
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			Dodge skill = set.GetSkill(SkillId.Dodge) as Dodge;
			if (skill == null)
				return;

			skill.maxConsecutiveCharges -= 1;
			skill.consecutiveTimelimit -= 3f;
		}

		protected override void InitInfo()
		{
			FileName = "ColdDodge_upgrade";
			TypeName = "Cold Dodge";
			VisibleName = "Doublejump Module";
			Description = "You can use Cold Dodge twice in a row.";
		}
	}

	public class DodgeTrapUpgrade : EquippableItem
	{
		public static int rarity = 1;
		public static ItemType type = ItemType.EPIC_UPGRADE;
		public static bool enabled = true;

		public DodgeTrapUpgrade(int level)
			: base(level)
		{
			MaxLevel = 1;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			Dodge skill = set.GetSkill(SkillId.Dodge) as Dodge;
			if (skill == null)
				return;

			skill.numOfTraps += 1;
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			Dodge skill = set.GetSkill(SkillId.Dodge) as Dodge;
			if (skill == null)
				return;

			skill.numOfTraps -= 1;
		}

		protected override void InitInfo()
		{
			FileName = "ColdDodge_upgrade";
			TypeName = "Cold Dodge";
			VisibleName = "Trap Module";
			Description = "You will spawn a trap"; //TODO description
		}
	}

	public class DodgeRangeUpgrade : EquippableItem
	{
		public static int rarity = 1;
		public static ItemType type = ItemType.EPIC_UPGRADE;
		public static bool enabled = true;

		public DodgeRangeUpgrade(int level)
			: base(level)
		{
			MaxLevel = 1;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			Dodge skill = set.GetSkill(SkillId.Dodge) as Dodge;
			if (skill == null)
				return;

			skill.infiniteRange = true;
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			Dodge skill = set.GetSkill(SkillId.Dodge) as Dodge;
			if (skill == null)
				return;

			skill.infiniteRange = false;
		}

		protected override void InitInfo()
		{
			FileName = "ColdDodge_upgrade";
			TypeName = "Cold Dodge";
			VisibleName = "Range Module";
			Description = "You will spawn a trap"; //TODO description
		}
	}


	public class DodgeFirstHitDamageUpgrade : EquippableItem
	{
		public static int rarity = 1;
		public static ItemType type = ItemType.CLASSIC_UPGRADE;

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
			FileName = "dodge_upgrade";
			TypeName = "Dodge";
			VisibleName = "Hit Damage Module";
			Description = "First enemy hit by you during Dodge jump will receive " + AddValueByLevel(DAMAGE, LEVEL_ADD) + " dmg.";
		}
	}

	public class DodgeHitDamageUpgrade : EquippableItem
	{
		public static int rarity = 2;
		public static ItemType type = ItemType.CLASSIC_UPGRADE;

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
			FileName = "dodge_upgrade";
			TypeName = "Dodge";
			VisibleName = "Hit Damage Module";
			Description = "All enemies hit by you during Dodge jump will receive " + AddValueByLevel(DAMAGE, LEVEL_ADD) +" dmg.";
		}
	}

	public class DodgePenetrateThroughTargets : EquippableItem
	{
		public static int rarity = 1;
		public static ItemType type = ItemType.RARE_UPGRADE;

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
			FileName = "dodge_upgrade";
			TypeName = "Dodge";
			VisibleName = "Penetrate Module";
			Description = "You will jump through targets, dealing " + AddValueByLevel(DAMAGE, LEVEL_ADD) + " dmg to each.";
		}
	}

	public class DodgeRangeIncrease : EquippableItem
	{
		public static int rarity = 2;
		public static ItemType type = ItemType.CLASSIC_UPGRADE;

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
			FileName = "dodge_upgrade";
			TypeName = "Dodge";
			VisibleName = "Range Module";
			Description = "Increases Dodge range by " + AddValueByLevel(VALUE, LEVEL_ADD) + " points (default is 10).";
		}
	}

	public class DodgeCharges : EquippableItem
	{
		public static int rarity = 1;
		public static ItemType type = ItemType.RARE_UPGRADE;

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
			FileName = "dodge_upgrade";
			TypeName = "Dodge";
			VisibleName = "Charges Module";
			Description = "Gives you two charges of Dodge skill. The second charge must be triggered within " + AddValueByLevel(3, 1f) + " seconds from launching the first charge.";
		}
	}

	public class DodgeRechargeShotUpgrade : EquippableItem
	{
		public static int rarity = 1;
		public static ItemType type = ItemType.RARE_UPGRADE;

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
			FileName = "dodge_upgrade";
			TypeName = "Dodge";
			VisibleName = "Combo Module";
			Description = "After using Dodge skill, skill Sneeze Shot is instantly recharged and available for use.";
		}
	}

	public class DodgeSpreadshotOnLand : EquippableItem
	{
		public static int rarity = 2;
		public static ItemType type = ItemType.CLASSIC_UPGRADE;

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
			FileName = "dodge_upgrade";
			TypeName = "Dodge";
			VisibleName = "Spreadshot Module";
			Description = "Shoots out 4 projectiles in all directions that deal " + AddValueByLevel(DAMAGE, LEVEL_ADD) + " to enemies.";
		}
	}
}
