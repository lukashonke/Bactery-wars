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
	public class CCAADamageUpgrade : AbstractUpgrade
	{
		public const int DAMAGE = 2;
		public const float LEVEL_MUL = 1f;

		public CCAADamageUpgrade(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 4;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			CommonColdAutoattack skill = melee as CommonColdAutoattack;
			if (skill == null)
				return;

			skill.baseDamage += (int)AddValueByLevel(DAMAGE, LEVEL_MUL);
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			CommonColdAutoattack skill = melee as CommonColdAutoattack;
			if (skill == null)
				return;

			skill.baseDamage -= (int)AddValueByLevel(DAMAGE, LEVEL_MUL);
		}

		protected override void InitInfo()
		{
			Name = "ccaa_upgrade";
			VisibleName = "Autoattack Damage Upgrade";
			Description = "Increase your autoattack damage by " + AddValueByLevel(DAMAGE, LEVEL_MUL) + ".";
		}
	}

	public class CCAARangeUpgrade : AbstractUpgrade
	{
		public const int VALUE = 3;
		public const float LEVEL_MUL = 1f;

		public CCAARangeUpgrade(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 4;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			CommonColdAutoattack skill = melee as CommonColdAutoattack;
			if (skill == null)
				return;

			skill.range += (int)AddValueByLevel(VALUE, LEVEL_MUL);
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			CommonColdAutoattack skill = melee as CommonColdAutoattack;
			if (skill == null)
				return;

			skill.range -= (int)AddValueByLevel(VALUE, LEVEL_MUL);
		}

		protected override void InitInfo()
		{
			Name = "ccaa_upgrade";
			VisibleName = "Autoattack Range Upgrade";
			Description = "Increase your autoattack range by " + AddValueByLevel(VALUE, LEVEL_MUL) + ".";
		}
	}

	public class CCAAReuseUpgrade : AbstractUpgrade
	{
		public const int ANGLE = 10;
		private float val;

		public CCAAReuseUpgrade(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 1;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			CommonColdAutoattack skill = melee as CommonColdAutoattack;
			if (skill == null)
				return;

			val = skill.reuse/2;
			skill.reuse -= val;
			skill.deviationAngle = ANGLE;
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			CommonColdAutoattack skill = melee as CommonColdAutoattack;
			if (skill == null)
				return;

			skill.reuse += val;
			skill.deviationAngle = 0;
		}

		protected override void InitInfo()
		{
			Name = "ccaa_upgrade";
			VisibleName = "Autoattack Reuse Upgrade";
			Description = "Decreases your autoattack reuse by 50%, but adds a random angle (up to " + ANGLE + " degrees) to shooting.";
		}
	}

	public class CCAADoubleattackChanceUpgrade : AbstractUpgrade
	{
		public const int CHANCE = 30;
		public const int LEVEL_ADD = 5;

		public CCAADoubleattackChanceUpgrade(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 6;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			CommonColdAutoattack skill = melee as CommonColdAutoattack;
			if (skill == null)
				return;

			skill.doubleAttackChance += (int)AddValueByLevel(CHANCE, LEVEL_ADD);
			skill.doubleAttackProjectileCount += 1;
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			CommonColdAutoattack skill = melee as CommonColdAutoattack;
			if (skill == null)
				return;

			skill.doubleAttackChance -= (int)AddValueByLevel(CHANCE, LEVEL_ADD);
			skill.doubleAttackProjectileCount -= 1;
		}

		protected override void InitInfo()
		{
			Name = "ccaa_upgrade";
			VisibleName = "Autoattack Doubleattack Upgrade";
			Description = "Adds a chance of " + AddValueByLevel(CHANCE, LEVEL_ADD) + " that your autoattack will fire 2 projectiles.";
		}
	}

	public class CCAADoubleattackUpgrade : AbstractUpgrade
	{
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
			CommonColdAutoattack skill = melee as CommonColdAutoattack;
			if (skill == null)
				return;

			skill.doubleAttackChance += 100;

			temp = skill.reuse*(AddValueByLevel(REUSE_INCREASE, LEVEL_ADD, true)/100f);
			skill.reuse = skill.reuse + temp;
			skill.doubleAttackProjectileCount += 1;
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			CommonColdAutoattack skill = melee as CommonColdAutoattack;
			if (skill == null)
				return;

			skill.doubleAttackChance -= 100;
			skill.doubleAttackProjectileCount -= 1;
			skill.reuse = skill.reuse - temp;
		}

		protected override void InitInfo()
		{
			Name = "ccaa_upgrade";
			VisibleName = "Autoattack Doubleattack Upgrade";
			Description = "Autoattack fires two projectiles, but also increases reuse by " + AddValueByLevel(REUSE_INCREASE, LEVEL_ADD, true) + " percent.";
		}
	}

	public class CCAATripleshotUpgrade : AbstractUpgrade
	{
		public const int REUSE_INCREASE = 75;
		public const int LEVEL_ADD = -4;

		private float temp;

		public CCAATripleshotUpgrade(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 5;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			CommonColdAutoattack skill = melee as CommonColdAutoattack;
			if (skill == null)
				return;

			skill.doubleAttackChance += 100;

			temp = skill.reuse * (AddValueByLevel(REUSE_INCREASE, LEVEL_ADD, true) / 100f);
			skill.doubleAttackProjectileCount += 2;
			skill.reuse = skill.reuse + temp;
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			CommonColdAutoattack skill = melee as CommonColdAutoattack;
			if (skill == null)
				return;

			skill.doubleAttackChance -= 100;
			skill.doubleAttackProjectileCount -= 2;
			skill.reuse = skill.reuse - temp;
		}

		protected override void InitInfo()
		{
			Name = "ccaa_upgrade";
			VisibleName = "Autoattack Tripleshot Upgrade";
			Description = "Autoattack fires 3 projectiles, but also increases reuse by " + AddValueByLevel(REUSE_INCREASE, LEVEL_ADD, true) + " percent.";
		}
	}

	public class CCAAQuatroshotUpgrade : AbstractUpgrade
	{
		public const int REUSE_INCREASE = 100;
		public const int LEVEL_ADD = -4;

		private float temp;

		public CCAAQuatroshotUpgrade(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 5;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			CommonColdAutoattack skill = melee as CommonColdAutoattack;
			if (skill == null)
				return;

			skill.doubleAttackChance += 100;

			temp = skill.reuse * (AddValueByLevel(REUSE_INCREASE, LEVEL_ADD, true) / 100f);
			skill.doubleAttackProjectileCount += 3;
			skill.reuse = skill.reuse + temp;
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			CommonColdAutoattack skill = melee as CommonColdAutoattack;
			if (skill == null)
				return;

			skill.doubleAttackChance -= 100;
			skill.doubleAttackProjectileCount -= 3;
			skill.reuse = skill.reuse - temp;
		}

		protected override void InitInfo()
		{
			Name = "ccaa_upgrade";
			VisibleName = "Autoattack Quatroshot Upgrade";
			Description = "Autoattack fires 4 projectiles, but also increases reuse by " + AddValueByLevel(REUSE_INCREASE, LEVEL_ADD, true) + " percent.";
		}
	}

	public class CCAAShotgunUpgrade : AbstractUpgrade
	{
		public const int PROJECTILE_COUNT = 3;

		public CCAAShotgunUpgrade(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 1;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			CommonColdAutoattack skill = melee as CommonColdAutoattack;
			if (skill == null)
				return;

			skill.shotgunChance += 100;
			skill.shotgunProjectilesCount += PROJECTILE_COUNT;
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			CommonColdAutoattack skill = melee as CommonColdAutoattack;
			if (skill == null)
				return;

			skill.shotgunChance -= 100;
			skill.shotgunProjectilesCount -= PROJECTILE_COUNT;
		}

		protected override void InitInfo()
		{
			Name = "ccaa_upgrade";
			VisibleName = "Autoattack Shotgun Upgrade";
			Description = "Every autoattack will fire " + PROJECTILE_COUNT + " projectiles in shotgun-like style.";
		}
	}

	public class CCAAShotgunChanceUpgrade : AbstractUpgrade
	{
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
			CommonColdAutoattack skill = melee as CommonColdAutoattack;
			if (skill == null)
				return;

			skill.consecutiveShotgunCounter += CONSECUTIVE_COUNTER;
			skill.shotgunProjectilesCount += PROJECTILE_COUNT;
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			CommonColdAutoattack skill = melee as CommonColdAutoattack;
			if (skill == null)
				return;

			skill.consecutiveShotgunCounter -= CONSECUTIVE_COUNTER;
			skill.shotgunProjectilesCount -= PROJECTILE_COUNT;
		}

		protected override void InitInfo()
		{
			Name = "ccaa_upgrade";
			VisibleName = "Autoattack Shotgun Upgrade";
			Description = "Every " + CONSECUTIVE_COUNTER + "th autoattack will fire " + PROJECTILE_COUNT + " projectiles in shotgun-like style.";
		}
	}

	public class CCAASpreadshootUpgrade : AbstractUpgrade
	{
		public CCAASpreadshootUpgrade(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 1;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			CommonColdAutoattack skill = melee as CommonColdAutoattack;
			if (skill == null)
				return;

			skill.toAllDirections = true;
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			CommonColdAutoattack skill = melee as CommonColdAutoattack;
			if (skill == null)
				return;

			skill.toAllDirections = false;
		}

		protected override void InitInfo()
		{
			Name = "ccaa_upgrade";
			VisibleName = "Autoattack Spreadshot Upgrade";
			Description = "Fires projectiles into all four directions (forward, right, left and backwards)";
		}
	}

	public class CCAAThunderUpgrade : AbstractUpgrade
	{
		public CCAAThunderUpgrade(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 1;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			CommonColdAutoattack skill = melee as CommonColdAutoattack;
			if (skill == null)
				return;

			skill.thunder = true;
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			CommonColdAutoattack skill = melee as CommonColdAutoattack;
			if (skill == null)
				return;

			skill.thunder = false;
		}

		protected override void InitInfo()
		{
			Name = "ccaa_upgrade";
			VisibleName = "Autoattack Thunder Upgrade";
			Description = "Fires projectiles into all four directions (forward, right, left and backwards)";
		}
	}

	public class CCAADamageForRangeUpgrade : AbstractUpgrade
	{
		public const int VALUE = 50;
		private float temp1, temp2;

		public CCAADamageForRangeUpgrade(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 1;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			CommonColdAutoattack skill = melee as CommonColdAutoattack;
			if (skill == null)
				return;

			temp1 = skill.range/2f;
			temp2 = skill.baseDamage/2f;

			skill.range -= (int)temp1;
			skill.baseDamage += (int)temp2;
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			CommonColdAutoattack skill = melee as CommonColdAutoattack;
			if (skill == null)
				return;

			skill.range += (int)temp1;
			skill.baseDamage -= (int)temp2;
		}

		protected override void InitInfo()
		{
			Name = "ccaa_upgrade";
			VisibleName = "Autoattack Damage Upgrade";
			Description = "Decreases autoattack range by 50% and increases damage by 50%.";
		}
	}

	public class CCAAForceUpgrade : AbstractUpgrade
	{
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
			CommonColdAutoattack skill = melee as CommonColdAutoattack;
			if (skill == null)
				return;

			temp = skill.projectileForce;
			temp2 = skill.rangeCheckFrequency;

			skill.projectileForce *= 2;
			skill.rangeCheckFrequency = 0.01f;
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			CommonColdAutoattack skill = melee as CommonColdAutoattack;
			if (skill == null)
				return;

			skill.projectileForce = temp;
			skill.rangeCheckFrequency = temp2;
		}

		protected override void InitInfo()
		{
			Name = "ccaa_upgrade";
			VisibleName = "Autoattack Force Upgrade";
			Description = "Doubles the speed of autoattack projectiles.";
		}
	}

	public class CCAASlowUpgrade : AbstractUpgrade //TODO bugged - doesnt refresh counter
	{
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
			if (sk.GetSkillId() == SkillId.CommonColdAutoattack)
			{
				CommonColdAutoattack skill = sk as CommonColdAutoattack;
				
				SkillEffect[] newEffects = new SkillEffect[1];
				newEffects[0] = new EffectSlow((AddValueByLevel(SLOWAMMOUNT, LEVEL_ADD) / 100f), AddValueByLevel(DURATION, DURATION_LEVEL_ADD));

				return newEffects;
			}

			return null;
		}

		protected override void InitInfo()
		{
			Name = "ccaa_upgrade";
			VisibleName = "Autoattack Slow Upgrade";
			Description = "Every hit projectile slows down your enemy by " + AddValueByLevel(SLOWAMMOUNT, LEVEL_ADD) + "% for " + AddValueByLevel(DURATION, DURATION_LEVEL_ADD) + " seconds.";
		}
	}
}
