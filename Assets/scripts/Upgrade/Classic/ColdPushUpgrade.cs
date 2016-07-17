// Copyright (c) 2015, Lukas Honke
// ========================
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
	public class ColdPushForceUpgrade : EquippableItem
	{
		public static int rarity = 1;
		public static ItemType type = ItemType.EPIC_UPGRADE;
		public static bool enabled = true;

		public const int POWER = 100;

		private int temp;

		public ColdPushForceUpgrade(int level)
			: base(level)
		{
			MaxLevel = 1;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			ColdPush skill = set.GetSkill(SkillId.ColdPush) as ColdPush;
			if (skill == null)
				return;

			temp = skill.pushbackForce;
			skill.pushbackForce = (int) (skill.pushbackForce * (POWER/100f+1));
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
			FileName = "coldpush_upgrade";
			TypeName = "Cold Push";
			VisibleName = "Force Module";
			Description = "The strength of Cold Push effect is increased by " + POWER + "%.";
		}
	}

	public class ColdPushAngleUpgrade : EquippableItem
	{
		public static int rarity = 1;
		public static ItemType type = ItemType.CLASSIC_UPGRADE;
		public static bool enabled = true;

		private int temp;

		public ColdPushAngleUpgrade(int level)
			: base(level)
		{
			MaxLevel = 1;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			ColdPush skill = set.GetSkill(SkillId.ColdPush) as ColdPush;
			if (skill == null)
				return;

			temp = skill.angle;
			skill.angle = 360;
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
			FileName = "coldpush_upgrade";
			TypeName = "Cold Push";
			VisibleName = "Force Module";
			Description = "Cold Push effect angle is set to 360 degrees.";
		}
	}

	public class ColdPushStunUpgrade : EquippableItem
	{
		public static int rarity = 1;
		public static ItemType type = ItemType.CLASSIC_UPGRADE;
		public static bool enabled = true;

		public const float DURATION = 2f;

		private float temp;

		public ColdPushStunUpgrade(int level)
			: base(level)
		{
			MaxLevel = 1;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			ColdPush skill = set.GetSkill(SkillId.ColdPush) as ColdPush;
			if (skill == null)
				return;

			temp = skill.stunDuration;
			skill.stunDuration *= DURATION;
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			ColdPush skill = set.GetSkill(SkillId.ColdPush) as ColdPush;
			if (skill == null)
				return;

			skill.stunDuration = temp;
		}

		protected override void InitInfo()
		{
			FileName = "coldpush_upgrade";
			TypeName = "Cold Push";
			VisibleName = " Stun Module";
			Description = "Stun duration of Cold Push will be doubled.";
		}
	}

	public class ColdPushReuseUpgrade : EquippableItem
	{
		public static int rarity = 1;
		public static ItemType type = ItemType.CLASSIC_UPGRADE;
		public static bool enabled = true;

		private float temp;

		public ColdPushReuseUpgrade(int level)
			: base(level)
		{
			MaxLevel = 1;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			ColdPush skill = set.GetSkill(SkillId.ColdPush) as ColdPush;
			if (skill == null)
				return;

			temp = skill.reuse * 0.3f;
			skill.reuse -= temp;
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			ColdPush skill = set.GetSkill(SkillId.ColdPush) as ColdPush;
			if (skill == null)
				return;

			skill.reuse += temp;
		}

		protected override void InitInfo()
		{
			FileName = "ColdPush_upgrade";
			TypeName = "Cold Push";
			VisibleName = "Reuse Module";
			Description = "Decreases the reuse rate of Cold Push by 30%.";
		}
	}

	public class ColdPushRangeUpgrade : EquippableItem
	{
		public static int rarity = 1;
		public static ItemType type = ItemType.RARE_UPGRADE;

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
			FileName = "coldpush_upgrade";
			TypeName = "Cold Push";
			VisibleName = "Force Module";
			Description = "Cold Push has increased range by " + AddValueByLevel(POWER, LEVEL_ADD) + "%.";
		}
	}

	public class ColdPushSlowUpgrade : EquippableItem
	{
		public static int rarity = 2;
		public static ItemType type = ItemType.CLASSIC_UPGRADE;

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
			FileName = "coldpush_upgrade";
			TypeName = "Cold Push";
			VisibleName = "Stun Module";
			Description = "Targets hit by Cold Push will be stunned for " + AddValueByLevel(DURATION, DURATION_LEVEL_ADD) + " seconds, but the reuse of the skill is doubled.";
		}
	}
}
