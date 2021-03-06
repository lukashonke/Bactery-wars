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
	public class CCAADamageUpgrade : EquippableItem
	{
		public static int rarity = 2;
		public static ItemType type = ItemType.CLASSIC_UPGRADE;

		public const int DAMAGE = 5;
		public const float LEVEL_MUL = 1f;

		public CCAADamageUpgrade(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 5;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			CellShot skill = melee as CellShot;
			if (skill == null)
				return;

			skill.baseDamage += (int)AddValueByLevel(DAMAGE, LEVEL_MUL);
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			CellShot skill = melee as CellShot;
			if (skill == null)
				return;

			skill.baseDamage -= (int)AddValueByLevel(DAMAGE, LEVEL_MUL);
		}

		protected override void InitInfo()
		{
			FileName = "ccaa_upgrade";
			TypeName = "Autoattack";
			VisibleName = "Damage Module";
			Description = "Increase your autoattack damage by " + AddValueByLevel(DAMAGE, LEVEL_MUL) + ".";
		}
	}

	public class CCAARangeUpgrade : EquippableItem
	{
		public static int rarity = 1;
		public static ItemType type = ItemType.CLASSIC_UPGRADE;

		public const int VALUE = 5;
		public const float LEVEL_MUL = 1f;

		public CCAARangeUpgrade(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 5;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			CellShot skill = melee as CellShot;
			if (skill == null)
				return;

			skill.range += (int)AddValueByLevel(VALUE, LEVEL_MUL);
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			CellShot skill = melee as CellShot;
			if (skill == null)
				return;

			skill.range -= (int)AddValueByLevel(VALUE, LEVEL_MUL);
		}

		protected override void InitInfo()
		{
			FileName = "ccaa_upgrade";
			TypeName = "Autoattack";
			VisibleName = "Range Module";
			Description = "Increase your autoattack range by " + AddValueByLevel(VALUE, LEVEL_MUL) + ".";
		}
	}

	public class CCAAReuseUpgrade : EquippableItem
	{
		public static int rarity = 2;
		public static ItemType type = ItemType.CLASSIC_UPGRADE;

		public const int ANGLE = 10;
		public const int ANGLE_LEVEL_ADD = -1;
		private float val;

		public CCAAReuseUpgrade(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 5;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			CellShot skill = melee as CellShot;
			if (skill == null)
				return;

			val = skill.reuse/2;
			skill.reuse -= val;
			skill.deviationAngle = (int) AddValueByLevel(ANGLE, ANGLE_LEVEL_ADD, true);
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			CellShot skill = melee as CellShot;
			if (skill == null)
				return;

			skill.reuse += val;
			skill.deviationAngle = 0;
		}

		protected override void InitInfo()
		{
			FileName = "ccaa_upgrade";
			TypeName = "Autoattack";
			VisibleName = "Reuse Module";
			Description = "Decreases your autoattack reuse by 50%, but adds a random angle (up to " + AddValueByLevel(ANGLE, ANGLE_LEVEL_ADD, true) + " degrees) to shooting.";
		}
	}

	public class CCAADoubleattackChanceUpgrade : EquippableItem
	{
		public static int rarity = 1;
		public static ItemType type = ItemType.CLASSIC_UPGRADE;

		public const int CHANCE = 30;
		public const int LEVEL_ADD = 6;

		public CCAADoubleattackChanceUpgrade(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 5;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			CellShot skill = melee as CellShot;
			if (skill == null)
				return;

			skill.doubleAttackChance += (int)AddValueByLevel(CHANCE, LEVEL_ADD);
			skill.doubleAttackProjectileCount += 1;
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			CellShot skill = melee as CellShot;
			if (skill == null)
				return;

			skill.doubleAttackChance -= (int)AddValueByLevel(CHANCE, LEVEL_ADD);
			skill.doubleAttackProjectileCount -= 1;
		}

		protected override void InitInfo()
		{
			FileName = "ccaa_upgrade";
			TypeName = "Autoattack";
			VisibleName = "Doubleattack Module";
			Description = "Adds a chance of " + AddValueByLevel(CHANCE, LEVEL_ADD) + " that your autoattack will fire 2 projectiles.";
		}
	}

	public class CCAADoubleattackUpgrade : EquippableItem
	{
		public static int rarity = 1;
		public static ItemType type = ItemType.RARE_UPGRADE;

		public const int REUSE_INCREASE = 50;
		public const int LEVEL_ADD = -4;

		private float temp;

		public CCAADoubleattackUpgrade(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 5;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			CellShot skill = melee as CellShot;
			if (skill == null)
				return;

			skill.doubleAttackChance += 100;

			temp = skill.reuse*(AddValueByLevel(REUSE_INCREASE, LEVEL_ADD, true)/100f);
			skill.reuse = skill.reuse + temp;
			skill.doubleAttackProjectileCount += 1;
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			CellShot skill = melee as CellShot;
			if (skill == null)
				return;

			skill.doubleAttackChance -= 100;
			skill.doubleAttackProjectileCount -= 1;
			skill.reuse = skill.reuse - temp;
		}

		protected override void InitInfo()
		{
			FileName = "ccaa_upgrade";
			TypeName = "Autoattack";
			VisibleName = "Doubleattack Module";
			Description = "Autoattack fires two projectiles, but also increases reuse by " + AddValueByLevel(REUSE_INCREASE, LEVEL_ADD, true) + " percent.";
		}
	}

	public class CCAATripleshotUpgrade : EquippableItem
	{
		public static int rarity = 1;
		public static ItemType type = ItemType.EPIC_UPGRADE;

		public const int REUSE_INCREASE = 75;
		public const int LEVEL_ADD = -5;

		private float temp;

		public CCAATripleshotUpgrade(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 5;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			CellShot skill = melee as CellShot;
			if (skill == null)
				return;

			skill.doubleAttackChance += 100;

			temp = skill.reuse * (AddValueByLevel(REUSE_INCREASE, LEVEL_ADD, true) / 100f);
			skill.doubleAttackProjectileCount += 2;
			skill.reuse = skill.reuse + temp;
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			CellShot skill = melee as CellShot;
			if (skill == null)
				return;

			skill.doubleAttackChance -= 100;
			skill.doubleAttackProjectileCount -= 2;
			skill.reuse = skill.reuse - temp;
		}

		protected override void InitInfo()
		{
			FileName = "ccaa_upgrade";
			TypeName = "Autoattack";
			VisibleName = "Tripleshot Module";
			Description = "Autoattack fires 3 projectiles, but also increases reuse by " + AddValueByLevel(REUSE_INCREASE, LEVEL_ADD, true) + " percent.";
		}
	}

	public class CCAAQuatroshotUpgrade : EquippableItem
	{
		public static int rarity = 2;
		public static ItemType type = ItemType.EPIC_UPGRADE;

		public const int REUSE_INCREASE = 100;
		public const int LEVEL_ADD = -5;

		private float temp;

		public CCAAQuatroshotUpgrade(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 5;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			CellShot skill = melee as CellShot;
			if (skill == null)
				return;

			skill.doubleAttackChance += 100;

			temp = skill.reuse * (AddValueByLevel(REUSE_INCREASE, LEVEL_ADD, true) / 100f);
			skill.doubleAttackProjectileCount += 3;
			skill.reuse = skill.reuse + temp;
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			CellShot skill = melee as CellShot;
			if (skill == null)
				return;

			skill.doubleAttackChance -= 100;
			skill.doubleAttackProjectileCount -= 3;
			skill.reuse = skill.reuse - temp;
		}

		protected override void InitInfo()
		{
			FileName = "ccaa_upgrade";
			TypeName = "Autoattack";
			VisibleName = "Quatroshot Module";
			Description = "Autoattack fires 4 projectiles, but also increases reuse by " + AddValueByLevel(REUSE_INCREASE, LEVEL_ADD, true) + " percent.";
		}
	}

	public class CCAAShotgunUpgrade : EquippableItem
	{
		public static int rarity = 2;
		public static ItemType type = ItemType.EPIC_UPGRADE;

		public const int PROJECTILE_COUNT = 3;

		public CCAAShotgunUpgrade(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 1;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			CellShot skill = melee as CellShot;
			if (skill == null)
				return;

			skill.shotgunChance += 100;
			skill.shotgunProjectilesCount += PROJECTILE_COUNT;
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			CellShot skill = melee as CellShot;
			if (skill == null)
				return;

			skill.shotgunChance -= 100;
			skill.shotgunProjectilesCount -= PROJECTILE_COUNT;
		}

		protected override void InitInfo()
		{
			FileName = "ccaa_upgrade";
			TypeName = "Autoattack";
			VisibleName = "Shotgun Module";
			Description = "Every autoattack will fire " + PROJECTILE_COUNT + " projectiles in shotgun-like style.";
		}
	}

	public class CCAAShotgunChanceUpgrade : EquippableItem
	{
		public static int rarity = 2;
		public static ItemType type = ItemType.CLASSIC_UPGRADE;

		public const int CONSECUTIVE_COUNTER = 3;
		public const int PROJECTILE_COUNT = 3;

		public CCAAShotgunChanceUpgrade(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 1;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			CellShot skill = melee as CellShot;
			if (skill == null)
				return;

			skill.consecutiveShotgunCounter += CONSECUTIVE_COUNTER;
			skill.shotgunProjectilesCount += PROJECTILE_COUNT;
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			CellShot skill = melee as CellShot;
			if (skill == null)
				return;

			skill.consecutiveShotgunCounter -= CONSECUTIVE_COUNTER;
			skill.shotgunProjectilesCount -= PROJECTILE_COUNT;
		}

		protected override void InitInfo()
		{
			FileName = "ccaa_upgrade";
			TypeName = "Autoattack";
			VisibleName = "Shotgun Module";
			Description = "Every " + CONSECUTIVE_COUNTER + "th autoattack will fire " + PROJECTILE_COUNT + " projectiles in shotgun-like style.";
		}
	}

	public class CCAASpreadshootUpgrade : EquippableItem
	{
		public static int rarity = 2;
		public static ItemType type = ItemType.RARE_UPGRADE;

		public CCAASpreadshootUpgrade(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 1;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			CellShot skill = melee as CellShot;
			if (skill == null)
				return;

			skill.toAllDirections = true;
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			CellShot skill = melee as CellShot;
			if (skill == null)
				return;

			skill.toAllDirections = false;
		}

		protected override void InitInfo()
		{
			FileName = "ccaa_upgrade";
			TypeName = "Autoattack";
			VisibleName = "Spreadshot Module";
			Description = "Fires projectiles into all four directions (forward, right, left and backwards)";
		}
	}

	public class CCAAThunderUpgrade : EquippableItem
	{
		public static int rarity = 3;
		public static ItemType type = ItemType.EPIC_UPGRADE;

		public CCAAThunderUpgrade(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 1;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			CellShot skill = melee as CellShot;
			if (skill == null)
				return;

			skill.thunder = true;
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			CellShot skill = melee as CellShot;
			if (skill == null)
				return;

			skill.thunder = false;
		}

		protected override void InitInfo()
		{
			FileName = "ccaa_upgrade";
			TypeName = "Autoattack";
			VisibleName = "Thunder Module";
			Description = "Fires projectiles into all four directions (forward, right, left and backwards)";
		}
	}

	public class CCAADamageForRangeUpgrade : EquippableItem
	{
		public static int rarity = 2;
		public static ItemType type = ItemType.CLASSIC_UPGRADE;

		public const int VALUE = 50;
		public const int VALUE_LEVEL_ADD = 4;

		private float temp1, temp2;

		public CCAADamageForRangeUpgrade(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 5;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			CellShot skill = melee as CellShot;
			if (skill == null)
				return;

			temp1 = skill.range/2f;
			temp2 = skill.baseDamage*(AddValueByLevel(VALUE, VALUE_LEVEL_ADD)/100f);

			skill.range -= (int)temp1;
			skill.baseDamage += (int)temp2;
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			CellShot skill = melee as CellShot;
			if (skill == null)
				return;

			skill.range += (int)temp1;
			skill.baseDamage -= (int)temp2;
		}

		protected override void InitInfo()
		{
			FileName = "ccaa_upgrade";
			TypeName = "Autoattack";
			VisibleName = "Damage Module";
			Description = "Decreases autoattack range by 50% and increases damage by " + AddValueByLevel(VALUE, VALUE_LEVEL_ADD) + "%.";
		}
	}

	public class CCAAForceUpgrade : EquippableItem
	{
		public static int rarity = 1;
		public static ItemType type = ItemType.CLASSIC_UPGRADE;

		private int temp;
		private float temp2;

		public CCAAForceUpgrade(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 1;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			CellShot skill = melee as CellShot;
			if (skill == null)
				return;

			temp = skill.projectileForce;
			temp2 = skill.rangeCheckFrequency;

			skill.projectileForce *= 2;
			skill.rangeCheckFrequency = 0.01f;
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			CellShot skill = melee as CellShot;
			if (skill == null)
				return;

			skill.projectileForce = temp;
			skill.rangeCheckFrequency = temp2;
		}

		protected override void InitInfo()
		{
			FileName = "ccaa_upgrade";
			TypeName = "Autoattack";
			VisibleName = "Force Module";
			Description = "Doubles the speed of autoattack projectiles.";
		}
	}

	public class CCAASlowUpgrade : EquippableItem //TODO bugged - doesnt refresh counter
	{
		public static int rarity = 1;
		public static ItemType type = ItemType.RARE_UPGRADE;

		public const int SLOWAMMOUNT = 50;
		public const int LEVEL_ADD = 5;
		//public const float DURATION = 2.5f;
		public const float DURATION = 4f; //TODO until it refreshes normally again
		public const float DURATION_LEVEL_ADD = 0.5f;

		public CCAASlowUpgrade(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 5;
		}

		public override SkillEffect[] CreateAdditionalSkillEffects(Skill sk, SkillEffect[] effects)
		{
			if (sk.GetSkillId() == SkillId.CellShot)
			{
				CellShot skill = sk as CellShot;
				
				SkillEffect[] newEffects = new SkillEffect[1];
				newEffects[0] = new EffectSlow((AddValueByLevel(SLOWAMMOUNT, LEVEL_ADD) / 100f), AddValueByLevel(DURATION, DURATION_LEVEL_ADD));

				return newEffects;
			}

			return null;
		}

		protected override void InitInfo()
		{
			FileName = "ccaa_upgrade";
			TypeName = "Autoattack";
			VisibleName = "Slow Module";
			Description = "Every hit projectile slows down your enemy by " + AddValueByLevel(SLOWAMMOUNT, LEVEL_ADD) + "% for " + AddValueByLevel(DURATION, DURATION_LEVEL_ADD) + " seconds.";
		}
	}
}
