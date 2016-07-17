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
using UnityEngine.Assertions.Comparers;
using Object = UnityEngine.Object;

namespace Assets.scripts.Upgrade.Classic
{
	public class PoisonShotDebuffUpgrade : EquippableItem
	{
		public static int rarity = 1;
		public static ItemType type = ItemType.CLASSIC_UPGRADE;
		public static bool enabled = true;

		public PoisonShotDebuffUpgrade(int level)
			: base(level)
		{
			MaxLevel = 1;
		}

		public override SkillEffect[] CreateAdditionalSkillEffects(Skill sk, SkillEffect[] effects)
		{
			if (sk.GetSkillId() == SkillId.PoisonShot)
			{
				PoisonShot ps = sk as PoisonShot;
				SkillEffect[] newEffects = new SkillEffect[1];
				newEffects[0] = new EffectShield(-0.5f, ps.duration);

				return newEffects;
			}

			return null;
		}

		protected override void InitInfo()
		{
			FileName = "PoisonShot_upgrade";
			TypeName = "Poison Shot";
			VisibleName = "Debuff Upgrade";
			Description = "Enemies hit by Poison Shot will be 50% weaker to all damage, including this poisons.";
		}
	}

	public class PoisonShotExplodeUpgrade : EquippableItem
	{
		public static int rarity = 1;
		public static ItemType type = ItemType.CLASSIC_UPGRADE;
		public static bool enabled = true;

		private const int DAMAGE = 10;
		private const int AREA = 8;

		public PoisonShotExplodeUpgrade(int level)
			: base(level)
		{
			MaxLevel = 1;
		}

		public override SkillEffect[] CreateAdditionalSkillEffects(Skill sk, SkillEffect[] effects)
		{
			if (sk.GetSkillId() == SkillId.PoisonShot)
			{
				PoisonShot ps = sk as PoisonShot;
				SkillEffect[] newEffects = new SkillEffect[1];
				newEffects[0] = new EffectExplodeOnDie(ps.duration, DAMAGE, AREA);

				return newEffects;
			}

			return null;
		}

		protected override void InitInfo()
		{
			FileName = "PoisonShot_upgrade";
			TypeName = "Poison Shot";
			VisibleName = "Explode Upgrade";
			Description = "Enemies infected by Poison Shot will explode on death, dealing damage to all surrounding monsters.";
		}
	}

	public class PoisonShotAreaUpgrade : EquippableItem
	{
		public static int rarity = 1;
		public static ItemType type = ItemType.CLASSIC_UPGRADE;
		public static bool enabled = true;

		public PoisonShotAreaUpgrade(int level)
			: base(level)
		{
			MaxLevel = 1;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			PoisonShot skill = set.GetSkill(SkillId.PoisonShot) as PoisonShot;
			if (skill == null)
				return;

			skill.areaEffect = true;
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			PoisonShot skill = set.GetSkill(SkillId.PoisonShot) as PoisonShot;
			if (skill == null)
				return;

			skill.areaEffect = false;
		}

		protected override void InitInfo()
		{
			FileName = "PoisonShot_upgrade";
			TypeName = "Poison Shot";
			VisibleName = "Area Upgrade";
			Description = "Poison Shot will poison enemies in area around the projectile hit.";
		}
	}

	public class PoisonShotReuseUpgrade : EquippableItem
	{
		public static int rarity = 1;
		public static ItemType type = ItemType.CLASSIC_UPGRADE;
		public static bool enabled = true;

		private float temp;

		public PoisonShotReuseUpgrade(int level)
			: base(level)
		{
			MaxLevel = 1;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			PoisonShot skill = set.GetSkill(SkillId.PoisonShot) as PoisonShot;
			if (skill == null)
				return;

			temp = skill.reuse * 0.3f;
			skill.reuse -= temp;
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			PoisonShot skill = set.GetSkill(SkillId.PoisonShot) as PoisonShot;
			if (skill == null)
				return;

			skill.reuse += temp;
		}

		protected override void InitInfo()
		{
			FileName = "PoisonShot_upgrade";
			TypeName = "Poison Shot";
			VisibleName = "Reuse Module";
			Description = "Decreases the reuse rate of Poison Shot by 30%.";
		}
	}

	public class PoisonShotSpreadUpgrade : EquippableItem
	{
		public static int rarity = 1;
		public static ItemType type = ItemType.EPIC_UPGRADE;
		public static bool enabled = true;

		private const int AREA = 7;

		public PoisonShotSpreadUpgrade(int level)
			: base(level)
		{
			MaxLevel = 1;
		}

		public override SkillEffect[] CreateAdditionalSkillEffects(Skill sk, SkillEffect[] effects)
		{
			if (sk.GetSkillId() == SkillId.PoisonShot)
			{
				PoisonShot ps = sk as PoisonShot;
				SkillEffect[] newEffects = new SkillEffect[1];
				//newEffects[0] = new EffectPushaway(50);
				newEffects[0] = new EffectReproduceOnDie(ps.duration, AREA);

				return newEffects;
			}

			return null;
		}

		protected override void InitInfo()
		{
			FileName = "PoisonShot_upgrade";
			TypeName = "Poison Shot";
			VisibleName = "Spread Upgrade";
			Description = "Enemies killed while poisoned by Poison Shot will spread the effect to nearby enemies.";
		}
	}

	public class PoisonShotChargeUpgrade : EquippableItem
	{
		public static int rarity = 1;
		public static ItemType type = ItemType.EPIC_UPGRADE;
		public static bool enabled = true;

		public PoisonShotChargeUpgrade(int level)
			: base(level)
		{
			MaxLevel = 1;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			PoisonShot skill = set.GetSkill(SkillId.PoisonShot) as PoisonShot;
			if (skill == null)
				return;

			skill.maxConsecutiveCharges += 1;
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			PoisonShot skill = set.GetSkill(SkillId.PoisonShot) as PoisonShot;
			if (skill == null)
				return;

			skill.maxConsecutiveCharges -= 1;
		}

		protected override void InitInfo()
		{
			FileName = "PoisonShot_upgrade";
			TypeName = "Poison Shot";
			VisibleName = "Charge Module";
			Description = "You can cast Poison Shot twice quickly one after another before the skill starts reuse state.";
		}
	}
}
