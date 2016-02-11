using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Skills;
using Assets.scripts.Skills.ActiveSkills;
using Assets.scripts.Skills.Base;

namespace Assets.scripts.Upgrade.Classic
{
	public class DodgeFirstHitDamageUpgrade : AbstractUpgrade
	{
		public const int DAMAGE = 20;
		public const float LEVEL_MUL = 1.2f;

		public DodgeFirstHitDamageUpgrade(int level) : base(level)
		{
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			Dodge skill = set.GetSkill(SkillId.Dodge) as Dodge;
			if (skill == null)
				return;

			skill.firstEnemyHitDamage += MulValueByLevel(DAMAGE, LEVEL_MUL);
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			Dodge skill = set.GetSkill(SkillId.Dodge) as Dodge;
			if (skill == null)
				return;

			skill.firstEnemyHitDamage -= MulValueByLevel(DAMAGE, LEVEL_MUL);
		}

		protected override void InitInfo()
		{
			Name = "dodge_upgrade";
			VisibleName = "Dodge Hit Damage Upgrade";
			Description = "First enemy hit by you during Dodge jump will receive " + MulValueByLevel(DAMAGE, LEVEL_MUL) + " dmg.";
		}
	}

	public class DodgeHitDamageUpgrade : AbstractUpgrade
	{
		public const int DAMAGE = 10;
		public const float LEVEL_MUL = 1.2f;

		public DodgeHitDamageUpgrade(int level)
			: base(level)
		{
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			Dodge skill = set.GetSkill(SkillId.Dodge) as Dodge;
			if (skill == null)
				return;

			skill.hitEnemyDamage += MulValueByLevel(DAMAGE, LEVEL_MUL);
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			Dodge skill = set.GetSkill(SkillId.Dodge) as Dodge;
			if (skill == null)
				return;

			skill.hitEnemyDamage -= MulValueByLevel(DAMAGE, LEVEL_MUL);
		}

		protected override void InitInfo()
		{
			Name = "dodge_upgrade";
			VisibleName = "Dodge Hit Damage Upgrade";
			Description = "All enemies hit by you during Dodge jump will receive " + MulValueByLevel(DAMAGE, LEVEL_MUL) +" dmg.";
		}
	}

	public class DodgePenetrateThroughTargets : AbstractUpgrade
	{
		public const int DAMAGE = 10;
		public const float LEVEL_MUL = 1.2f;

		public DodgePenetrateThroughTargets(int level)
			: base(level)
		{
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			Dodge skill = set.GetSkill(SkillId.Dodge) as Dodge;
			if (skill == null)
				return;

			skill.penetrateThroughTargets = true;
			skill.hitEnemyDamage += MulValueByLevel(DAMAGE, LEVEL_MUL);
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			Dodge skill = set.GetSkill(SkillId.Dodge) as Dodge;
			if (skill == null)
				return;

			skill.penetrateThroughTargets = false;
			skill.hitEnemyDamage -= MulValueByLevel(DAMAGE, LEVEL_MUL);
		}

		protected override void InitInfo()
		{
			Name = "dodge_upgrade";
			VisibleName = "Dodge Penetrate Upgrade";
			Description = "You will jump through targets, dealing " + MulValueByLevel(DAMAGE, LEVEL_MUL) + " dmg to each.";
		}
	}

	public class DodgeRangeIncrease : AbstractUpgrade
	{
		public const int VALUE = 5;
		public const float LEVEL_ADD = 1f;

		public DodgeRangeIncrease(int level)
			: base(level)
		{
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			Dodge skill = set.GetSkill(SkillId.Dodge) as Dodge;
			if (skill == null)
				return;

			skill.range += AddValueByLevel(VALUE, LEVEL_ADD);
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			Dodge skill = set.GetSkill(SkillId.Dodge) as Dodge;
			if (skill == null)
				return;

			skill.range -= AddValueByLevel(VALUE, LEVEL_ADD);
		}

		protected override void InitInfo()
		{
			Name = "dodge_upgrade";
			VisibleName = "Dodge Range Upgrade";
			Description = "Increases Dodge range by " + AddValueByLevel(VALUE, LEVEL_ADD) + " points (default is 10).";
		}
	}
}
