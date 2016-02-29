using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Actor.PlayerClasses.Base;
using Assets.scripts.Skills;
using Assets.scripts.Skills.ActiveSkills;
using Assets.scripts.Skills.Base;
using Assets.scripts.Skills.SkillEffects;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.scripts.Upgrade.Classic
{
	public class ColdPushForceUpgrade : AbstractUpgrade
	{
		public static int rarity = 2;
		public static UpgradeType type = UpgradeType.CLASSIC;

		public const int POWER = 50;
		public const float LEVEL_ADD = 5;

		private int temp;

		public ColdPushForceUpgrade(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 5;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			ColdPush skill = set.GetSkill(SkillId.ColdPush) as ColdPush;
			if (skill == null)
				return;

			temp = skill.pushbackForce;
			skill.pushbackForce = (int) (skill.pushbackForce * (AddValueByLevel(POWER, LEVEL_ADD)/100f+1));
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			ColdPush skill = set.GetSkill(SkillId.ColdPush) as ColdPush;
			if (skill == null)
				return;

			skill.pushbackForce = temp;
		}

		protected override void InitInfo()
		{
			Name = "coldpush_upgrade";
			VisibleName = "Cold Push Force Upgrade";
			Description = "Cold Push pushes enemies further because it has increased force by " + AddValueByLevel(POWER, LEVEL_ADD) + "%.";
		}
	}

	public class ColdPushRangeUpgrade : AbstractUpgrade
	{
		public static int rarity = 1;
		public static UpgradeType type = UpgradeType.RARE;

		public const int POWER = 50;
		public const float LEVEL_ADD = 5;

		private int temp;

		public ColdPushRangeUpgrade(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 5;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			ColdPush skill = set.GetSkill(SkillId.ColdPush) as ColdPush;
			if (skill == null)
				return;

			temp = skill.range;
			skill.range = (int)(skill.range * (AddValueByLevel(POWER, LEVEL_ADD) / 100f + 1));
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			ColdPush skill = set.GetSkill(SkillId.ColdPush) as ColdPush;
			if (skill == null)
				return;

			skill.range = temp;
		}

		protected override void InitInfo()
		{
			Name = "coldpush_upgrade";
			VisibleName = "Cold Push Force Upgrade";
			Description = "Cold Push has increased range by " + AddValueByLevel(POWER, LEVEL_ADD) + "%.";
		}
	}

	public class ColdPushAngleUpgrade : AbstractUpgrade
	{
		public static int rarity = 2;
		public static UpgradeType type = UpgradeType.CLASSIC;

		private int temp;

		public ColdPushAngleUpgrade(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 1;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			ColdPush skill = set.GetSkill(SkillId.ColdPush) as ColdPush;
			if (skill == null)
				return;

			temp = skill.angle;
			skill.angle = 180;
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			ColdPush skill = set.GetSkill(SkillId.ColdPush) as ColdPush;
			if (skill == null)
				return;

			skill.angle = temp;
		}

		protected override void InitInfo()
		{
			Name = "coldpush_upgrade";
			VisibleName = "Cold Push Force Upgrade";
			Description = "Cold Push effect angle is set to 180 degrees from in front of you.";
		}
	}

	public class ColdPushSlowUpgrade : AbstractUpgrade
	{
		public static int rarity = 2;
		public static UpgradeType type = UpgradeType.CLASSIC;

		public const float DURATION = 5f;
		public const float DURATION_LEVEL_ADD = 1f;

		private float temp;
		private float temp2;

		public ColdPushSlowUpgrade(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 5;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			ColdPush skill = set.GetSkill(SkillId.ColdPush) as ColdPush;
			if (skill == null)
				return;

			temp = skill.stunDuration;
			temp2 = skill.reuse;

			skill.stunDuration = AddValueByLevel(DURATION, DURATION_LEVEL_ADD);
			skill.reuse *= 2;
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			ColdPush skill = set.GetSkill(SkillId.ColdPush) as ColdPush;
			if (skill == null)
				return;

			skill.stunDuration = temp;
			skill.reuse = temp2;
		}

		protected override void InitInfo()
		{
			Name = "coldpush_upgrade";
			VisibleName = "Cold Push Stun Upgrade";
			Description = "Targets hit by Cold Push will be stunned for " + AddValueByLevel(DURATION, DURATION_LEVEL_ADD) + " seconds, but the reuse of the skill is doubled.";
		}
	}

	public class ColdPushStunUpgrade : AbstractUpgrade //TODO not done
	{
		public static int rarity = 10;
		public static UpgradeType type = UpgradeType.EPIC;

		public const int SLOWAMMOUNT = 50;
		public const int LEVEL_ADD = 4;

		public const float DURATION = 4f;
		public const float DURATION_LEVEL_ADD = 0.4f;

		public ColdPushStunUpgrade(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 5;
		}

		public override SkillEffect[] CreateAdditionalSkillEffects(Skill sk, SkillEffect[] effects)
		{
			if (sk.GetSkillId() == SkillId.ColdPush)
			{
				ColdPush skill = sk as ColdPush;

				SkillEffect[] newEffects = new SkillEffect[1];
				newEffects[0] = new EffectSlow((AddValueByLevel(SLOWAMMOUNT, LEVEL_ADD) / 100f), AddValueByLevel(DURATION, DURATION_LEVEL_ADD));

				return newEffects;
			}

			return null;
		}

		protected override void InitInfo()
		{
			Name = "coldpush_upgrade";
			VisibleName = "Cold Push Slow Upgrade";
			Description = "UNFINISHED Targets hit with Cold Push skill will be stunned down by " + AddValueByLevel(SLOWAMMOUNT, LEVEL_ADD) + "% for " + AddValueByLevel(DURATION, DURATION_LEVEL_ADD) + " seconds.";
		}
	}
}
