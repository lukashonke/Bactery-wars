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
	public class ChargeRangeIncrease : EquippableItem
	{
		public static int rarity = 2;
		public static ItemType type = ItemType.CLASSIC_UPGRADE;
		public static bool enabled = true;

		private int temp;

		public ChargeRangeIncrease(int level)
			: base(level)
		{
			MaxLevel = 1;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			Charge skill = set.GetSkill(SkillId.Charge) as Charge;
			if (skill == null)
				return;

			temp = (int) (skill.range*0.75f);
			skill.range += temp;
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			Charge skill = set.GetSkill(SkillId.Charge) as Charge;
			if (skill == null)
				return;

			skill.range -= temp;
		}

		protected override void InitInfo()
		{
			FileName = "Charge_upgrade";
			TypeName = "Charge";
			VisibleName = "Range Module";
			Description = "Increases Charge jump range by 75%.";
		}
	}

	public class ChargeStunUpgrade : EquippableItem
	{
		public static int rarity = 1;
		public static ItemType type = ItemType.CLASSIC_UPGRADE;
		public static bool enabled = true;

		private const float DURATION = 5f;

		public ChargeStunUpgrade(int level)
			: base(level)
		{
			MaxLevel = 1;
		}

		public override SkillEffect[] CreateAdditionalSkillEffects(Skill sk, SkillEffect[] effects)
		{
			if (sk.GetSkillId() == SkillId.Charge)
			{
				Charge dg = sk as Charge;
				SkillEffect[] newEffects = new SkillEffect[1];
				newEffects[0] = new EffectStun(DURATION);

				return newEffects;
			}

			return null;
		}

		protected override void InitInfo()
		{
			FileName = "ColdCharge_upgrade";
			TypeName = "Cold Charge";
			VisibleName = "Stun Upgrade";
			Description = "Stuns the first enemy you hit while in Charge jump for 5 seconds.";
		}
	}

	public class ChargeAreaUpgrade : EquippableItem
	{
		public static int rarity = 1;
		public static ItemType type = ItemType.CLASSIC_UPGRADE;
		public static bool enabled = true;

		public ChargeAreaUpgrade(int level)
			: base(level)
		{
			MaxLevel = 1;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			Charge skill = set.GetSkill(SkillId.Charge) as Charge;
			if (skill == null)
				return;

			skill.areaDamage = true;
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			Charge skill = set.GetSkill(SkillId.Charge) as Charge;
			if (skill == null)
				return;

			skill.areaDamage = false;
		}

		protected override void InitInfo()
		{
			FileName = "ColdCharge_upgrade";
			TypeName = "Cold Charge";
			VisibleName = "Area Module";
			Description = "Charge will deal area damage."; //TODO description
		}
	}

	public class ChargeJumpbackUpgrade : EquippableItem
	{
		public static int rarity = 1;
		public static ItemType type = ItemType.CLASSIC_UPGRADE;
		public static bool enabled = true;

		public ChargeJumpbackUpgrade(int level)
			: base(level)
		{
			MaxLevel = 1;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			Charge skill = set.GetSkill(SkillId.Charge) as Charge;
			if (skill == null)
				return;

			skill.jumpBack = true;
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			Charge skill = set.GetSkill(SkillId.Charge) as Charge;
			if (skill == null)
				return;

			skill.jumpBack = false;
		}

		protected override void InitInfo()
		{
			FileName = "ColdCharge_upgrade";
			TypeName = "Cold Charge";
			VisibleName = "Jumpback Module";
			Description = "Charge will make u jump back after the first jump."; //TODO description
		}
	}

	public class ChargeRechargeUpgrade : EquippableItem
	{
		public static int rarity = 1;
		public static ItemType type = ItemType.EPIC_UPGRADE;
		public static bool enabled = true;

		public ChargeRechargeUpgrade(int level)
			: base(level)
		{
			MaxLevel = 1;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			Charge skill = set.GetSkill(SkillId.Charge) as Charge;
			if (skill == null)
				return;

			skill.rechargeOnKill = true;
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			Charge skill = set.GetSkill(SkillId.Charge) as Charge;
			if (skill == null)
				return;

			skill.rechargeOnKill = false;
		}

		protected override void InitInfo()
		{
			FileName = "ColdCharge_upgrade";
			TypeName = "Cold Charge";
			VisibleName = "Recharge Module";
			Description = "Charge recharges if you kill your target using this skill."; //TODO description
		}
	}

	public class ChargeAutoattackUpgrade : EquippableItem
	{
		public static int rarity = 1;
		public static ItemType type = ItemType.CLASSIC_UPGRADE;
		public static bool enabled = true;

		public ChargeAutoattackUpgrade(int level)
			: base(level)
		{
			MaxLevel = 1;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			Charge skill = set.GetSkill(SkillId.Charge) as Charge;
			if (skill == null)
				return;

			skill.autoattacksOnJump += 8;
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			Charge skill = set.GetSkill(SkillId.Charge) as Charge;
			if (skill == null)
				return;

			skill.autoattacksOnJump -= 8;
		}

		protected override void InitInfo()
		{
			FileName = "ColdCharge_upgrade";
			TypeName = "Cold Charge";
			VisibleName = "Autoattack Module";
			Description = "Charge will shoot 8 autoattacks."; //TODO description
		}
	}

	public class ChargeKillRechargeUpgrade : EquippableItem
	{
		public static int rarity = 1;
		public static ItemType type = ItemType.CLASSIC_UPGRADE;
		public static bool enabled = true;

		public ChargeKillRechargeUpgrade(int level)
			: base(level)
		{
			MaxLevel = 1;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			Charge skill = set.GetSkill(SkillId.Charge) as Charge;
			if (skill == null)
				return;

			skill.rechargeOnKill = true;
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			Charge skill = set.GetSkill(SkillId.Charge) as Charge;
			if (skill == null)
				return;

			skill.rechargeOnKill = false;
		}

		protected override void InitInfo()
		{
			FileName = "ColdCharge_upgrade";
			TypeName = "Cold Charge";
			VisibleName = "Maniac Module";
			Description = "Charge skill will have no reuse delay if you kill an enemy using it.";
		}
	}

	public class ChargeReuseUpgrade : EquippableItem
	{
		public static int rarity = 1;
		public static ItemType type = ItemType.CLASSIC_UPGRADE;
		public static bool enabled = true;

		private float temp;

		public ChargeReuseUpgrade(int level)
			: base(level)
		{
			MaxLevel = 1;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			Charge skill = set.GetSkill(SkillId.Charge) as Charge;
			if (skill == null)
				return;

			temp = skill.reuse * 0.3f;
			skill.reuse -= temp;
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			Charge skill = set.GetSkill(SkillId.Charge) as Charge;
			if (skill == null)
				return;

			skill.reuse += temp;
		}

		protected override void InitInfo()
		{
			FileName = "ColdCharge_upgrade";
			TypeName = "Cold Charge";
			VisibleName = "Reuse Module";
			Description = "Decreases Cold Charge reuse by 30%.";
		}
	}

	public class ChargeDoublejumpUpgrade : EquippableItem
	{
		public static int rarity = 1;
		public static ItemType type = ItemType.CLASSIC_UPGRADE;

		public ChargeDoublejumpUpgrade(int level)
			: base(level)
		{
			MaxLevel = 1;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			Charge skill = set.GetSkill(SkillId.Charge) as Charge;
			if (skill == null)
				return;

			skill.maxConsecutiveCharges += 1;
			skill.consecutiveTimelimit = 3f;
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			Charge skill = set.GetSkill(SkillId.Charge) as Charge;
			if (skill == null)
				return;

			skill.maxConsecutiveCharges -= 1;
			skill.consecutiveTimelimit -= 3f;
		}

		protected override void InitInfo()
		{
			FileName = "ColdCharge_upgrade";
			TypeName = "Cold Charge";
			VisibleName = "Doublejump Module";
			Description = "You can use Cold Charge twice in a row.";
		}
	}

	public class ChargeRangeUpgrade : EquippableItem
	{
		public static int rarity = 1;
		public static ItemType type = ItemType.EPIC_UPGRADE;

		public ChargeRangeUpgrade(int level)
			: base(level)
		{
			MaxLevel = 1;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			Charge skill = set.GetSkill(SkillId.Charge) as Charge;
			if (skill == null)
				return;

			//skill.infiniteRange = true;
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			Charge skill = set.GetSkill(SkillId.Charge) as Charge;
			if (skill == null)
				return;

			//skill.infiniteRange = false;
		}

		protected override void InitInfo()
		{
			FileName = "ColdCharge_upgrade";
			TypeName = "Cold Charge";
			VisibleName = "Range Module";
			Description = "You will spawn a trap"; //TODO description
		}
	}


	public class ChargeFirstHitDamageUpgrade : EquippableItem
	{
		public static int rarity = 1;
		public static ItemType type = ItemType.CLASSIC_UPGRADE;

		public const int DAMAGE = 20;
		public const float LEVEL_ADD = 4;

		public ChargeFirstHitDamageUpgrade(int level) : base(level)
		{
			RequiredClass = ClassId.CommonCold;

			MaxLevel = 5;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			Charge skill = set.GetSkill(SkillId.Charge) as Charge;
			if (skill == null)
				return;

			skill.firstEnemyHitDamage += (int) AddValueByLevel(DAMAGE, LEVEL_ADD);
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			Charge skill = set.GetSkill(SkillId.Charge) as Charge;
			if (skill == null)
				return;

			skill.firstEnemyHitDamage -= (int)AddValueByLevel(DAMAGE, LEVEL_ADD);
		}

		protected override void InitInfo()
		{
			FileName = "Charge_upgrade";
			TypeName = "Charge";
			VisibleName = "Hit Damage Module";
			Description = "First enemy hit by you during Charge jump will receive " + AddValueByLevel(DAMAGE, LEVEL_ADD) + " dmg.";
		}
	}

	public class ChargeHitDamageUpgrade : EquippableItem
	{
		public static int rarity = 2;
		public static ItemType type = ItemType.CLASSIC_UPGRADE;

		public const int DAMAGE = 15;
		public const float LEVEL_ADD = 4;

		public ChargeHitDamageUpgrade(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;

			MaxLevel = 5;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			Charge skill = set.GetSkill(SkillId.Charge) as Charge;
			if (skill == null)
				return;

			skill.hitEnemyDamage += (int)AddValueByLevel(DAMAGE, LEVEL_ADD);
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			Charge skill = set.GetSkill(SkillId.Charge) as Charge;
			if (skill == null)
				return;

			skill.hitEnemyDamage -= (int)AddValueByLevel(DAMAGE, LEVEL_ADD);
		}

		protected override void InitInfo()
		{
			FileName = "Charge_upgrade";
			TypeName = "Charge";
			VisibleName = "Hit Damage Module";
			Description = "All enemies hit by you during Charge jump will receive " + AddValueByLevel(DAMAGE, LEVEL_ADD) +" dmg.";
		}
	}

	public class ChargePenetrateThroughTargets : EquippableItem
	{
		public static int rarity = 1;
		public static ItemType type = ItemType.RARE_UPGRADE;

		public const int DAMAGE = 10;
		public const float LEVEL_ADD = 3;

		public ChargePenetrateThroughTargets(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;

			MaxLevel = 5;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			Charge skill = set.GetSkill(SkillId.Charge) as Charge;
			if (skill == null)
				return;

			skill.penetrateThroughTargets = true;
			skill.hitEnemyDamage += (int)AddValueByLevel(DAMAGE, LEVEL_ADD);
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			Charge skill = set.GetSkill(SkillId.Charge) as Charge;
			if (skill == null)
				return;

			skill.penetrateThroughTargets = false;
			skill.hitEnemyDamage -= (int)AddValueByLevel(DAMAGE, LEVEL_ADD);
		}

		protected override void InitInfo()
		{
			FileName = "Charge_upgrade";
			TypeName = "Charge";
			VisibleName = "Penetrate Module";
			Description = "You will jump through targets, dealing " + AddValueByLevel(DAMAGE, LEVEL_ADD) + " dmg to each.";
		}
	}

	public class ChargeCharges : EquippableItem
	{
		public static int rarity = 1;
		public static ItemType type = ItemType.RARE_UPGRADE;

		public ChargeCharges(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;

			MaxLevel = 5;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			Charge skill = set.GetSkill(SkillId.Charge) as Charge;
			if (skill == null)
				return;

			skill.maxConsecutiveCharges += 1;
			skill.consecutiveTimelimit = AddValueByLevel(3, 1f);
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			Charge skill = set.GetSkill(SkillId.Charge) as Charge;
			if (skill == null)
				return;

			skill.maxConsecutiveCharges -= 1;
			skill.consecutiveTimelimit -= AddValueByLevel(3, 1f);
		}

		protected override void InitInfo()
		{
			FileName = "Charge_upgrade";
			TypeName = "Charge";
			VisibleName = "Charges Module";
			Description = "Gives you two charges of Charge skill. The second charge must be triggered within " + AddValueByLevel(3, 1f) + " seconds from launching the first charge.";
		}
	}

	public class ChargeRechargeShotUpgrade : EquippableItem
	{
		public static int rarity = 1;
		public static ItemType type = ItemType.RARE_UPGRADE;

		public ChargeRechargeShotUpgrade(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			Charge skill = set.GetSkill(SkillId.Charge) as Charge;
			if (skill == null)
				return;

			skill.skillToBeRechargedOnThisUse = SkillId.SneezeShot;
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			Charge skill = set.GetSkill(SkillId.Charge) as Charge;
			if (skill == null)
				return;

			skill.skillToBeRechargedOnThisUse = 0;
		}

		protected override void InitInfo()
		{
			FileName = "Charge_upgrade";
			TypeName = "Charge";
			VisibleName = "Combo Module";
			Description = "After using Charge skill, skill Sneeze Shot is instantly recharged and available for use.";
		}
	}

	public class ChargeSpreadshotOnLand : EquippableItem
	{
		public static int rarity = 2;
		public static ItemType type = ItemType.CLASSIC_UPGRADE;

		public const int DAMAGE = 15;
		public const float LEVEL_ADD = 3f;

		public ChargeSpreadshotOnLand(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;

			MaxLevel = 5;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			Charge skill = set.GetSkill(SkillId.Charge) as Charge;
			if (skill == null)
				return;

			skill.spreadshotOnLand = true;
			skill.spreadshotDamage = (int) AddValueByLevel(DAMAGE, LEVEL_ADD);
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			Charge skill = set.GetSkill(SkillId.Charge) as Charge;
			if (skill == null)
				return;

			skill.spreadshotOnLand = false;
			skill.spreadshotDamage = 0;
		}

		protected override void InitInfo()
		{
			FileName = "Charge_upgrade";
			TypeName = "Charge";
			VisibleName = "Spreadshot Module";
			Description = "Shoots out 4 projectiles in all directions that deal " + AddValueByLevel(DAMAGE, LEVEL_ADD) + " to enemies.";
		}
	}
}
