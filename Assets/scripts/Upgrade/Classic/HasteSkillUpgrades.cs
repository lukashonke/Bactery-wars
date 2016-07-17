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

namespace Assets.scripts.Upgrade.Classic
{
	public class HasteUsagesIncrease : EquippableItem
	{
		public static int rarity = 2;
		public static ItemType type = ItemType.CLASSIC_UPGRADE;
		public static bool enabled = true;

		private int temp;

		public HasteUsagesIncrease(int level)
			: base(level)
		{
			MaxLevel = 1;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			Haste skill = set.GetSkill(SkillId.Haste) as Haste;
			if (skill == null)
				return;

			skill.count += 3;
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			Haste skill = set.GetSkill(SkillId.Haste) as Haste;
			if (skill == null)
				return;

			skill.count -= 3; ;
		}

		protected override void InitInfo()
		{
			FileName = "Haste_upgrade";
			TypeName = "Haste";
			VisibleName = "Usages Module";
			Description = "Dash skill will apply its bonus for 8 autoattacks.";
		}
	}

	public class HastePushbackUpgrade : EquippableItem
	{
		public static int rarity = 1;
		public static ItemType type = ItemType.CLASSIC_UPGRADE;
		public static bool enabled = true;

		public HastePushbackUpgrade(int level)
			: base(level)
		{
			MaxLevel = 1;
		}

		public override SkillEffect[] CreateAdditionalSkillEffects(Skill sk, SkillEffect[] effects)
		{
			ActiveSkill aa = Owner.MeleeSkill;

			if (aa != null && sk.GetSkillId() == aa.GetSkillId())
			{
				if (sk.Owner.HasEffectOfSkill(SkillId.Haste))
				{
					SkillEffect[] ef = new SkillEffect[1];
					ef[0] = new EffectPushaway(30);
					return ef;
				}
			}

			return null;
		}

		protected override void InitInfo()
		{
			FileName = "ColdHaste_upgrade";
			TypeName = "Cold Haste";
			VisibleName = "Force Module";
			Description = "Haste will give your autoattacks a pushaway effect.";
		}
	}

	public class HasteReuseModule : EquippableItem
	{
		public static int rarity = 1;
		public static ItemType type = ItemType.EPIC_UPGRADE;
		public static bool enabled = true;

		private const int POWER = 20;

		public HasteReuseModule(int level)
			: base(level)
		{
			MaxLevel = 1;
		}

		public override void OnGiveDamage(Character target, int damage, SkillId skillId)
		{
			ActiveSkill aa = Owner.MeleeSkill;

			if (target != null && aa != null && skillId == aa.GetSkillId() && damage > 0)
			{
				if (Owner.HasEffectOfSkill(SkillId.Haste))
				{
					int drainAmmount = (int)(damage * POWER / 100f);

					if(drainAmmount > 0)
						Owner.ReceiveHeal(target, drainAmmount, skillId);
				}
			}
		}

		protected override void InitInfo()
		{
			FileName = "cellfury_upgrade";
			TypeName = "Cell Fury";
			VisibleName = "Drain Module";
			Description = "Autoattacks buffed by Haste will return " + POWER + "% of damage you deal to you as HP.";
		}
	}

	public class HasteShieldUpgrade : EquippableItem
	{
		public static int rarity = 1;
		public static ItemType type = ItemType.CLASSIC_UPGRADE;
		public static bool enabled = true;

		public HasteShieldUpgrade(int level)
			: base(level)
		{
			MaxLevel = 1;
		}

		public override SkillEffect[] CreateAdditionalSkillEffects(Skill sk, SkillEffect[] effects)
		{
			ActiveSkill aa = Owner.MeleeSkill;

			if (aa != null && sk.GetSkillId() == aa.GetSkillId())
			{
				if (sk.Owner.HasEffectOfSkill(SkillId.Haste))
				{
					SkillEffect[] ef = new SkillEffect[1];
					ef[0] = new EffectShield(1.0f, 1f).TargetOwner();
					return ef;
				}
			}

			return null;
		}

		protected override void InitInfo()
		{
			FileName = "Haste_upgrade";
			TypeName = "Haste";
			VisibleName = "Shield Module";
			Description = "Every autoattack buffed using Haste skill will give you a one second invulnerability to all received damage.";
		}
	}

}
