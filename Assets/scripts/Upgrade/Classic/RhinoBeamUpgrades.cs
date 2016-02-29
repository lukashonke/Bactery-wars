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
	public class RhinobeamDurationUpgrade : AbstractUpgrade
	{
		public static int rarity = 1;
		public static UpgradeType type = UpgradeType.CLASSIC;

		public const int POWER = 50;
		public const float LEVEL_ADD = 10;

		private float temp;

		public RhinobeamDurationUpgrade(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 5;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			RhinoBeam skill = set.GetSkill(SkillId.RhinoBeam) as RhinoBeam;
			if (skill == null)
				return;

			temp = skill.coolDown;
			skill.coolDown = (int) (skill.coolDown * (AddValueByLevel(POWER, LEVEL_ADD)/100f+1));
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			RhinoBeam skill = set.GetSkill(SkillId.RhinoBeam) as RhinoBeam;
			if (skill == null)
				return;

			skill.coolDown = temp;
		}

		protected override void InitInfo()
		{
			Name = "rhinobeam_upgrade";
			VisibleName = "Rhino Beam Duration Upgrade";
			Description = "Rhino Beam maximum active time is increased by " + AddValueByLevel(POWER, LEVEL_ADD) + "%.";
		}
	}

	public class RhinobeamWidthUpgrade : AbstractUpgrade
	{
		public static int rarity = 1;
		public static UpgradeType type = UpgradeType.CLASSIC;

		public const float WIDTH = 2;
		public const float LEVEL_ADD = 0.1f;

		private float temp;

		public RhinobeamWidthUpgrade(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 5;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			RhinoBeam skill = set.GetSkill(SkillId.RhinoBeam) as RhinoBeam;
			if (skill == null)
				return;

			temp = skill.width;
			skill.width = AddValueByLevel(WIDTH, LEVEL_ADD);
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			RhinoBeam skill = set.GetSkill(SkillId.RhinoBeam) as RhinoBeam;
			if (skill == null)
				return;

			skill.width = temp;
		}

		protected override void InitInfo()
		{
			Name = "rhinobeam_upgrade";
			VisibleName = "Rhino Beam Width Upgrade";
			Description = "Rhino Beam's ray width is set to " + AddValueByLevel(WIDTH, LEVEL_ADD) + " (without upgrade it is 1).";
		}
	}

	public class RhinobeamReuseUpgrade : AbstractUpgrade
	{
		public static int rarity = 2;
		public static UpgradeType type = UpgradeType.CLASSIC;

		public const int POWER = 25;
		public const float LEVEL_ADD = 2;

		private float temp;

		public RhinobeamReuseUpgrade(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 5;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			RhinoBeam skill = set.GetSkill(SkillId.RhinoBeam) as RhinoBeam;
			if (skill == null)
				return;

			temp = skill.reuse;
			skill.reuse = (int)(skill.reuse * (1-(AddValueByLevel(POWER, LEVEL_ADD) / 100f)));
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			RhinoBeam skill = set.GetSkill(SkillId.RhinoBeam) as RhinoBeam;
			if (skill == null)
				return;

			skill.reuse = temp;
		}

		protected override void InitInfo()
		{
			Name = "rhinobeam_upgrade";
			VisibleName = "Rhino Beam Reuse Upgrade";
			Description = "Rhino Beam reuse is decreased by " + AddValueByLevel(POWER, LEVEL_ADD) + "%.";
		}
	}

	public class RhinobeamEpicReuseUpgrade : AbstractUpgrade
	{
		public static int rarity = 1;
		public static UpgradeType type = UpgradeType.EPIC;

		public const int POWER = 25;
		public const float LEVEL_ADD = 2;

		private float temp;

		public RhinobeamEpicReuseUpgrade(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 5;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			RhinoBeam skill = set.GetSkill(SkillId.RhinoBeam) as RhinoBeam;
			if (skill == null)
				return;

			temp = skill.reuse;
			skill.reuse = (int)(skill.reuse * (1 - (AddValueByLevel(POWER, LEVEL_ADD) / 100f)));
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			RhinoBeam skill = set.GetSkill(SkillId.RhinoBeam) as RhinoBeam;
			if (skill == null)
				return;

			skill.reuse = temp;
		}

		protected override void InitInfo()
		{
			Name = "rhinobeam_upgrade";
			VisibleName = "Rhino Beam Reuse Upgrade";
			Description = "Rhino Beam reuse is decreased by " + AddValueByLevel(POWER, LEVEL_ADD) + "%.";
		}
	}

	public class RhinobeamDamageUpgrade : AbstractUpgrade
	{
		public static int rarity = 2;
		public static UpgradeType type = UpgradeType.RARE;

		public const int POWER = 50;
		public const float LEVEL_ADD = 10;

		public const int COOLDOWN_DOWN = 50;

		private float temp;
		private int temp2;

		public RhinobeamDamageUpgrade(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 5;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			RhinoBeam skill = set.GetSkill(SkillId.RhinoBeam) as RhinoBeam;
			if (skill == null)
				return;

			temp = skill.coolDown;
			temp2 = skill.baseDamage;
			skill.coolDown = skill.coolDown*COOLDOWN_DOWN/100f;
			skill.baseDamage = (int) (skill.baseDamage*(AddValueByLevel(POWER, LEVEL_ADD)/100f + 1f));
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			RhinoBeam skill = set.GetSkill(SkillId.RhinoBeam) as RhinoBeam;
			if (skill == null)
				return;

			skill.coolDown = temp;
			skill.baseDamage = temp2;
		}

		protected override void InitInfo()
		{
			Name = "rhinobeam_upgrade";
			VisibleName = "Rhino Beam Damage Upgrade";
			Description = "Rhino Beam damage is increased by " + AddValueByLevel(POWER, LEVEL_ADD) + "%, the max. duration of the ray is decreased by " + COOLDOWN_DOWN + "%.";
		}
	}

	public class RhinobeamStunUpgrade : AbstractUpgrade
	{
		public static int rarity = 1;
		public static UpgradeType type = UpgradeType.EPIC;

		public const float DURATION = 4;
		public const float LEVEL_ADD = 0.5f;

		public RhinobeamStunUpgrade(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 5;
		}

		public override SkillEffect[] CreateAdditionalSkillEffects(Skill sk, SkillEffect[] effects)
		{
			if (sk.GetSkillId() == SkillId.RhinoBeam)
			{
				SkillEffect[] newEffects = new SkillEffect[1];
				newEffects[0] = new EffectStun(AddValueByLevel(DURATION, LEVEL_ADD));

				return newEffects;
			}

			return null;
		}

		protected override void InitInfo()
		{
			Name = "rhinobeam_upgrade";
			VisibleName = "Rhino Beam Stun Upgrade";
			Description = "Rhino Beam enemies hit will be stunned for " + AddValueByLevel(DURATION, LEVEL_ADD) + " seconds.";
		}
	}

	public class RhinobeamPushUpgrade : AbstractUpgrade
	{
		public static int rarity = 2;
		public static UpgradeType type = UpgradeType.RARE;

		public const float POWER = 5;
		public const float LEVEL_ADD = 1;

		private int temp;

		public RhinobeamPushUpgrade(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 5;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			RhinoBeam skill = set.GetSkill(SkillId.RhinoBeam) as RhinoBeam;
			if (skill == null)
				return;

			temp = skill.baseDamage;
			skill.baseDamage = (int)(skill.baseDamage * 4f);
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			RhinoBeam skill = set.GetSkill(SkillId.RhinoBeam) as RhinoBeam;
			if (skill == null)
				return;

			skill.baseDamage = temp;
		}

		public override SkillEffect[] CreateAdditionalSkillEffects(Skill sk, SkillEffect[] effects)
		{
			if (sk.GetSkillId() == SkillId.RhinoBeam)
			{
				SkillEffect[] newEffects = new SkillEffect[1];
				newEffects[0] = new EffectPushaway((int) AddValueByLevel(POWER, LEVEL_ADD));

				return newEffects;
			}

			return null;
		}

		protected override void InitInfo()
		{
			Name = "rhinobeam_upgrade";
			VisibleName = "Rhino Beam Push Upgrade";
			Description = "Rhino Beam enemies hit will be pushed away with force of " + AddValueByLevel(POWER, LEVEL_ADD) + ". Damage is increased to 400% (x4).";
		}
	}

	public class RhinobeamRotationUpgrade : AbstractUpgrade
	{
		public static int rarity = 2;
		public static UpgradeType type = UpgradeType.CLASSIC;

		private float temp;

		public RhinobeamRotationUpgrade(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 1;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			RhinoBeam skill = set.GetSkill(SkillId.RhinoBeam) as RhinoBeam;
			if (skill == null)
				return;

			temp = skill.rotateSpeed;
			skill.rotateSpeed = 100f;
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			RhinoBeam skill = set.GetSkill(SkillId.RhinoBeam) as RhinoBeam;
			if (skill == null)
				return;

			skill.rotateSpeed = temp;
		}

		protected override void InitInfo()
		{
			Name = "rhinobeam_upgrade";
			VisibleName = "Rhino Beam Rotation Upgrade";
			Description = "Rhino Beam direction rotation/shift will be instant.";
		}
	}
}
