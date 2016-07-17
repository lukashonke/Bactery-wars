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
	public class CoughBulletPenetrateUpgrade : EquippableItem
	{
		public static int rarity = 1;
		public static ItemType type = ItemType.CLASSIC_UPGRADE;
		public static bool enabled = true;

		public CoughBulletPenetrateUpgrade(int level)
			: base(level)
		{
			MaxLevel = 1;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			CoughBullet skill = set.GetSkill(SkillId.CoughBullet) as CoughBullet;
			if (skill == null)
				return;

			skill.maxPenetratedTargets += 1;
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			CoughBullet skill = set.GetSkill(SkillId.CoughBullet) as CoughBullet;
			if (skill == null)
				return;

			skill.maxPenetratedTargets -= 1;
		}

		protected override void InitInfo()
		{
			FileName = "CoughBullet_upgrade";
			TypeName = "Cough Bullet";
			VisibleName = "Penetration Upgrade";
			Description = "Cough Bullet will penetrate 1 more target.";
		}
	}

	public class CoughBulletStunUpgrade : EquippableItem
	{
		public static int rarity = 1;
		public static ItemType type = ItemType.CLASSIC_UPGRADE;
		public static bool enabled = true;

		public CoughBulletStunUpgrade(int level)
			: base(level)
		{
			MaxLevel = 1;
		}

		public override SkillEffect[] CreateAdditionalSkillEffects(Skill sk, SkillEffect[] effects)
		{
			if (sk.GetSkillId() == SkillId.CoughBullet)
			{
				SkillEffect[] newEffects = new SkillEffect[1];
				//newEffects[0] = new EffectPushaway(50);
				newEffects[0] = new EffectStun(3f);

				return newEffects;
			}

			return null;
		}

		protected override void InitInfo()
		{
			FileName = "CoughBullet_upgrade";
			TypeName = "Cough Bullet";
			VisibleName = "Stun Upgrade";
			Description = "Enemies hit by Cough Bullet will be stunned.";
		}
	}

	public class CoughBulletReturnUpgrade : EquippableItem
	{
		public static int rarity = 1;
		public static ItemType type = ItemType.CLASSIC_UPGRADE;
		public static bool enabled = true;

		public CoughBulletReturnUpgrade(int level)
			: base(level)
		{
			MaxLevel = 1;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			CoughBullet skill = set.GetSkill(SkillId.CoughBullet) as CoughBullet;
			if (skill == null)
				return;

			skill.returnBack = true;
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			CoughBullet skill = set.GetSkill(SkillId.CoughBullet) as CoughBullet;
			if (skill == null)
				return;

			skill.returnBack = true;
		}

		protected override void InitInfo()
		{
			FileName = "CoughBullet_upgrade";
			TypeName = "Cough Bullet";
			VisibleName = "Projectile Module";
			Description = "Cough Shot projectile will return back to player instead of being destroyed, following the same route.";
		}
	}

	public class CoughBulletReuseUpgrade : EquippableItem
	{
		public static int rarity = 1;
		public static ItemType type = ItemType.CLASSIC_UPGRADE;
		public static bool enabled = true;

		private float temp;

		public CoughBulletReuseUpgrade(int level)
			: base(level)
		{
			MaxLevel = 1;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			CoughBullet skill = set.GetSkill(SkillId.CoughBullet) as CoughBullet;
			if (skill == null)
				return;

			temp = skill.reuse*0.3f;
			skill.reuse -= temp;
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			CoughBullet skill = set.GetSkill(SkillId.CoughBullet) as CoughBullet;
			if (skill == null)
				return;

			skill.reuse += temp;
		}

		protected override void InitInfo()
		{
			FileName = "CoughBullet_upgrade";
			TypeName = "Cough Bullet";
			VisibleName = "Reuse Module";
			Description = "Decreases the reuse rate of Cough Bullet by 30%.";
		}
	}

	public class CoughBulletChainUpgrade : EquippableItem
	{
		public static int rarity = 1;
		public static ItemType type = ItemType.EPIC_UPGRADE;
		public static bool enabled = true;

		public CoughBulletChainUpgrade(int level)
			: base(level)
		{
			MaxLevel = 1;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			CoughBullet skill = set.GetSkill(SkillId.CoughBullet) as CoughBullet;
			if (skill == null)
				return;

			skill.chainEffect = true;
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			CoughBullet skill = set.GetSkill(SkillId.CoughBullet) as CoughBullet;
			if (skill == null)
				return;

			skill.chainEffect = false;
		}

		protected override void InitInfo()
		{
			FileName = "CoughBullet_upgrade";
			TypeName = "Cough Bullet";
			VisibleName = "Doubleshot Module";
			Description = "Upon hiting an enemy with Cold Shot projectile, a new sub-projectile will be created (chain effect).";
		}
	}
}
