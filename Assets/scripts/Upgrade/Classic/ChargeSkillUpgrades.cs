// Copyright (c) 2015, Lukas Honke
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
	public class ChargeSkillRangeIncrease : EquippableItem
	{
		public static int rarity = 2;
		public static ItemType type = ItemType.CLASSIC_UPGRADE;
		public static bool enabled = true;

		private int temp;

		public ChargeSkillRangeIncrease(int level)
			: base(level)
		{
			MaxLevel = 1;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			ChargeSkill skill = set.GetSkill(SkillId.ChargeSkill) as ChargeSkill;
			if (skill == null)
				return;

			skill.uses += 1;
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			ChargeSkill skill = set.GetSkill(SkillId.ChargeSkill) as ChargeSkill;
			if (skill == null)
				return;

			skill.uses -= 1; ;
		}

		protected override void InitInfo()
		{
			FileName = "ChargeSkill_upgrade";
			TypeName = "ChargeSkill";
			VisibleName = "Charge Module";
			Description = "Charge Skill will charge two skills, not one.";
		}
	}

	public class ChargeSkillStunUpgrade : EquippableItem
	{
		public static int rarity = 1;
		public static ItemType type = ItemType.CLASSIC_UPGRADE;
		public static bool enabled = true;

		public ChargeSkillStunUpgrade(int level)
			: base(level)
		{
			MaxLevel = 1;
		}

		public override SkillEffect[] CreateAdditionalSkillEffects(Skill sk, SkillEffect[] effects)
		{
			if (sk.GetSkillId() == SkillId.ChargeSkill)
			{
				ChargeSkill dg = sk as ChargeSkill;
				SkillEffect[] newEffects = new SkillEffect[1];
				newEffects[0] = new EffectDash(0.3f, 0);

				return newEffects;
			}

			return null;
		}

		protected override void InitInfo()
		{
			FileName = "ColdChargeSkill_upgrade";
			TypeName = "Cold ChargeSkill";
			VisibleName = "Speed Module";
			Description = "Being charged with Charge Skill will grant you +30% movement speed.";
		}
	}

	public class ChargeSkillReuseModule : EquippableItem
	{
		public static int rarity = 2;
		public static ItemType type = ItemType.EPIC_UPGRADE;
		public static bool enabled = true;

		private int temp;

		public ChargeSkillReuseModule(int level)
			: base(level)
		{
			MaxLevel = 1;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			ChargeSkill skill = set.GetSkill(SkillId.ChargeSkill) as ChargeSkill;
			if (skill == null)
				return;

			skill.resetSkillSlotId = 0;
			skill.resetSkillMaxReuse = 10;
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			ChargeSkill skill = set.GetSkill(SkillId.ChargeSkill) as ChargeSkill;
			if (skill == null)
				return;

			skill.resetSkillSlotId = -1;
		}

		protected override void InitInfo()
		{
			FileName = "ChargeSkill_upgrade";
			TypeName = "ChargeSkill";
			VisibleName = "Reuse Module";
			Description = "Upon using Charge Skill, your skill in the first slot will be automatically reactivated and available for use. Only works if the skill in the first slot has reuse delay lower than 10 seconds.";
		}
	}

	public class ChargeSkillShieldUpgrade : EquippableItem
	{
		public static int rarity = 2;
		public static ItemType type = ItemType.EPIC_UPGRADE;
		public static bool enabled = true;

		public ChargeSkillShieldUpgrade(int level)
			: base(level)
		{
			MaxLevel = 1;
		}

		public override SkillEffect[] CreateAdditionalSkillEffects(Skill sk, SkillEffect[] effects)
		{
			if (sk.GetSkillId() == SkillId.ChargeSkill)
			{
				ChargeSkill dg = sk as ChargeSkill;
				SkillEffect[] newEffects = new SkillEffect[1];
				newEffects[0] = new EffectForceShield(1, 0);

				return newEffects;
			}

			return null;
		}

		protected override void InitInfo()
		{
			FileName = "ChargeSkill_upgrade";
			TypeName = "ChargeSkill";
			VisibleName = "Shield Module";
			Description = "Charge Skill will grant you a force shield that will protect you from damage. The shield gets destroyed if you receive damage.";
		}
	}

}
