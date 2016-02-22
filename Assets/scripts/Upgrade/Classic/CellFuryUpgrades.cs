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
	public class CellFuryRangeUpgrade : AbstractUpgrade
	{
		public const int RANGE = 100;
		public const float LEVEL_ADD = 10;

		private float temp;

		public CellFuryRangeUpgrade(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 5;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			CellFury skill = set.GetSkill(SkillId.CellFury) as CellFury;
			if (skill == null)
				return;

			temp = skill.rangeBoost;
			skill.rangeBoost += (AddValueByLevel(RANGE, LEVEL_ADD)/100f + 1f);
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			CellFury skill = set.GetSkill(SkillId.CellFury) as CellFury;
			if (skill == null)
				return;

			skill.rangeBoost = temp;
		}

		protected override void InitInfo()
		{
			Name = "sneezeshot_upgrade";
			VisibleName = "Cell Fury Range Upgrade";
			Description = "Your autoattack has range increased by " + AddValueByLevel(RANGE, LEVEL_ADD) + "%.";
		}
	}

	public class CellFuryPushUpgrade : AbstractUpgrade
	{
		public const int POWER = 25;
		public const float LEVEL_ADD = 5;

		private float temp;

		public CellFuryPushUpgrade(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 5;
		}

		public override SkillEffect[] CreateAdditionalSkillEffects(Skill sk, SkillEffect[] effects)
		{
			if (sk.GetSkillId() == SkillId.CommonColdAutoattack)
			{
				if (sk.Owner.HasEffectOfSkill(SkillId.CellFury))
				{
					SkillEffect[] ef = new SkillEffect[1];
					ef[0] = new EffectPushaway((int) AddValueByLevel(POWER, LEVEL_ADD));
					return ef;
				}
			}

			return null;
		}

		protected override void InitInfo()
		{
			Name = "sneezeshot_upgrade";
			VisibleName = "Cell Fury Push Upgrade";
			Description = "Your autoattack will push enemies away with force of " + AddValueByLevel(POWER, LEVEL_ADD) + ".";
		}
	}

	public class CellFuryReuseUpgrade : AbstractUpgrade
	{
		public const int REUSE = 25;
		public const float LEVEL_ADD = 4;

		private float temp;

		public CellFuryReuseUpgrade(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 5;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			CellFury skill = set.GetSkill(SkillId.CellFury) as CellFury;
			if (skill == null)
				return;

			temp = skill.reuse;
			skill.reuse = skill.reuse*(1 - AddValueByLevel(REUSE, LEVEL_ADD)/100f);
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			CellFury skill = set.GetSkill(SkillId.CellFury) as CellFury;
			if (skill == null)
				return;

			skill.reuse = temp;
		}

		protected override void InitInfo()
		{
			Name = "sneezeshot_upgrade";
			VisibleName = "Cell Fury Reuse Upgrade";
			Description = "The reuse of Cell Fury is decreased by " + AddValueByLevel(REUSE, LEVEL_ADD) + "%.";
		}
	}

	public class CellFuryNullReuseUpgrade : AbstractUpgrade
	{
		public const int CHANCE = 10;
		public const float LEVEL_ADD = 2;

		private int temp;

		public CellFuryNullReuseUpgrade(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 5;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			CellFury skill = set.GetSkill(SkillId.CellFury) as CellFury;
			if (skill == null)
				return;

			temp = skill.nullReuseChance;
			skill.nullReuseChance = (int) AddValueByLevel(CHANCE, LEVEL_ADD);
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			CellFury skill = set.GetSkill(SkillId.CellFury) as CellFury;
			if (skill == null)
				return;

			skill.nullReuseChance = temp;
		}

		protected override void InitInfo()
		{
			Name = "sneezeshot_upgrade";
			VisibleName = "Cell Fury Reuse Upgrade";
			Description = "There is a " + AddValueByLevel(CHANCE, LEVEL_ADD) + "% chance that Cell Fury skill will have no reuse (it will be available immediatelly after you cast it).";
		}
	}

	public class CellFuryShieldUpgrade : AbstractUpgrade
	{
		public const int POWER = 50;
		public const float LEVEL_ADD = 10;

		private float temp;

		public CellFuryShieldUpgrade(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 5;
		}

		public override SkillEffect[] CreateAdditionalSkillEffects(Skill sk, SkillEffect[] effects)
		{
			if (sk.GetSkillId() == SkillId.CellFury)
			{
				CellFury cf = sk as CellFury;

				SkillEffect[] ef = new SkillEffect[1];
				ef[0] = new EffectShield(AddValueByLevel(POWER, LEVEL_ADD) / 100f, cf.duration);
				return ef;
			}

			return null;
		}

		protected override void InitInfo()
		{
			Name = "sneezeshot_upgrade";
			VisibleName = "Cell Fury Shield Upgrade";
			Description = "When Cell Fury is active, your shield will be " + AddValueByLevel(POWER, LEVEL_ADD) + "% stronger.";
		}
	}

	public class CellFurySpeedUpgrade : AbstractUpgrade
	{
		public const int POWER = 50;
		public const float LEVEL_ADD = 4;

		public CellFurySpeedUpgrade(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 5;
		}

		public override SkillEffect[] CreateAdditionalSkillEffects(Skill sk, SkillEffect[] effects)
		{
			if (sk.GetSkillId() == SkillId.CellFury)
			{
				CellFury cf = sk as CellFury;

				SkillEffect[] ef = new SkillEffect[1];
				ef[0] = new EffectDash(AddValueByLevel(POWER, LEVEL_ADD) / 100f, cf.duration);
				return ef;
			}

			return null;
		}

		protected override void InitInfo()
		{
			Name = "sneezeshot_upgrade";
			VisibleName = "Cell Fury Speed Upgrade";
			Description = "When Cell Fury is active, your movement speed is increased by " + AddValueByLevel(POWER, LEVEL_ADD) + "%.";
		}
	}

	public class CellFuryDrainUpgrade : AbstractUpgrade
	{
		public const int POWER = 50;
		public const float LEVEL_ADD = 4;

		public CellFuryDrainUpgrade(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 5;
		}

		public override void OnGiveDamage(Character target, int damage, SkillId skillId)
		{
			if (target != null && skillId == SkillId.CommonColdAutoattack && damage > 0)
			{
				if (Owner.HasEffectOfSkill(SkillId.CellFury))
				{
					int drainAmmount = (int) (damage*(AddValueByLevel(POWER, LEVEL_ADD)/100f));
					Owner.ReceiveHeal(target, drainAmmount, skillId);
				}
			}
		}

		protected override void InitInfo()
		{
			Name = "sneezeshot_upgrade";
			VisibleName = "Cell Fury Speed Upgrade";
			Description = "When Cell Fury is active, your movement speed is increased by " + AddValueByLevel(POWER, LEVEL_ADD) + "%.";
		}
	}
}
