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
	public class SneezeShotThreeProjectiles : EquippableItem
	{
		public static int rarity = 1;
		public static ItemType type = ItemType.CLASSIC;

		public SneezeShotThreeProjectiles(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 1;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			SneezeShot skill = set.GetSkill(SkillId.SneezeShot) as SneezeShot;
			if (skill == null)
				return;

			skill.projectilesCount += 1;
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			SneezeShot skill = set.GetSkill(SkillId.SneezeShot) as SneezeShot;
			if (skill == null)
				return;

			skill.projectilesCount -= 1;
		}

		protected override void InitInfo()
		{
			FileName = "sneezeshot_upgrade";
			TypeName = "Sneeze Shot";
			VisibleName = "Projectile Module";
			Description = "Sneeze Shot fires one extra projectile.";
		}
	}

	public class SneezeShotDamageUpgrade : EquippableItem
	{
		public static int rarity = 2;
		public static ItemType type = ItemType.CLASSIC;

		public const int DAMAGE = 10;
		public const float LEVEL_ADD = 2;

		public SneezeShotDamageUpgrade(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 5;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			SneezeShot skill = set.GetSkill(SkillId.SneezeShot) as SneezeShot;
			if (skill == null)
				return;

			skill.baseDamage += (int) AddValueByLevel(DAMAGE, LEVEL_ADD);
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			SneezeShot skill = set.GetSkill(SkillId.SneezeShot) as SneezeShot;
			if (skill == null)
				return;

			skill.baseDamage -= (int)AddValueByLevel(DAMAGE, LEVEL_ADD);
		}

		protected override void InitInfo()
		{
			FileName = "sneezeshot_upgrade";
			TypeName = "Sneeze Shot";
			VisibleName = "Damage Module";
			Description = "Sneeze Shot projectile deals " + (int)AddValueByLevel(DAMAGE, LEVEL_ADD) + " extra damage.";
		}
	}

	public class SneezeShotAimedProjectiles : EquippableItem
	{
		public static int rarity = 1;
		public static ItemType type = ItemType.CLASSIC;

		public SneezeShotAimedProjectiles(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 1;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			SneezeShot skill = set.GetSkill(SkillId.SneezeShot) as SneezeShot;
			if (skill == null)
				return;

			skill.projectilesAim = true;
			skill.aimArea = 10;
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			SneezeShot skill = set.GetSkill(SkillId.SneezeShot) as SneezeShot;
			if (skill == null)
				return;

			skill.projectilesAim = false;
			skill.aimArea = 20;
		}

		protected override void InitInfo()
		{
			FileName = "sneezeshot_upgrade";
			TypeName = "Sneeze Shot";
			VisibleName = "Aim Module";
			Description = "Projectiles automatically scan area in front of them to adjust their path to hit enemies.";
		}
	}

	public class SneezeShotHomingMissile : EquippableItem
	{
		public static int rarity = 2;
		public static ItemType type = ItemType.CLASSIC;

		private int temp;
		private int temp2;

		public SneezeShotHomingMissile(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 1;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			SneezeShot skill = set.GetSkill(SkillId.SneezeShot) as SneezeShot;
			if (skill == null)
				return;

			temp = skill.projectilesCount;
			temp2 = skill.range;
			skill.range *= 3;
			skill.projectilesCount = 1;
			skill.projectilesAim = true;
			skill.aimArea = skill.range;
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			SneezeShot skill = set.GetSkill(SkillId.SneezeShot) as SneezeShot;
			if (skill == null)
				return;

			skill.projectilesCount = temp;
			skill.range = temp2;
			skill.projectilesAim = false;
			skill.aimArea = 0;
		}

		protected override void InitInfo()
		{
			FileName = "sneezeshot_upgrade";
			TypeName = "Sneeze Shot";
			VisibleName = "Missile Module";
			Description = "Sneeze Shot fires one projectile that is guided to its target. Range is tripled.";
		}
	}

	public class SneezeShotPoisonUpgrade : EquippableItem
	{
		public static int rarity = 2;
		public static ItemType type = ItemType.CLASSIC;

		public const int DAMAGE = 5;
		public const float LEVEL_ADD = 1f;

		public const int SLOW_AMMOUNT_PERCENT = 30;
		public const float SLOW_LEVEL_ADD = 6f;

		public SneezeShotPoisonUpgrade(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 5;
		}

		public override SkillEffect[] CreateAdditionalSkillEffects(Skill sk, SkillEffect[] effects)
		{
			if (sk.GetSkillId() == SkillId.SneezeShot)
			{
				SneezeShot skill = sk as SneezeShot;

				SkillEffect[] newEffects = new SkillEffect[2];
				newEffects[0] = new EffectSlow((AddValueByLevel(SLOW_AMMOUNT_PERCENT, SLOW_LEVEL_ADD) / 100f), 5);
				newEffects[1] = new EffectDamageOverTime((int) AddValueByLevel(DAMAGE, LEVEL_ADD), 5, 1, false);

				return newEffects;
			}

			return null;
		}

		protected override void InitInfo()
		{
			FileName = "sneezeshot_upgrade";
			TypeName = "Sneeze Shot";
			VisibleName = "Poison Module";
			Description = "Sneeze Shot gives your target additional " + AddValueByLevel(DAMAGE, LEVEL_ADD) + " dmg/sec for 5 seconds and slows him by " + AddValueByLevel(SLOW_AMMOUNT_PERCENT, SLOW_LEVEL_ADD) + "% for 5 seconds.";
		}
	}

	public class SneezeShotPenetrateUpgrade : EquippableItem
	{
		public static int rarity = 1;
		public static ItemType type = ItemType.RARE;

		public SneezeShotPenetrateUpgrade(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 1;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			SneezeShot skill = set.GetSkill(SkillId.SneezeShot) as SneezeShot;
			if (skill == null)
				return;

			skill.penetrateTargets = true;
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			SneezeShot skill = set.GetSkill(SkillId.SneezeShot) as SneezeShot;
			if (skill == null)
				return;

			skill.penetrateTargets = false;
		}

		protected override void InitInfo()
		{
			FileName = "sneezeshot_upgrade";
			TypeName = "Sneeze Shot";
			VisibleName = "Penetration Module";
			Description = "Sneeze Shot penetrates through your targets to hit multiple enemies.";
		}
	}

	public class SneezeShotPushUpgrade : EquippableItem
	{
		public static int rarity = 1;
		public static ItemType type = ItemType.CLASSIC;

		public SneezeShotPushUpgrade(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 1;
		}

		public override SkillEffect[] CreateAdditionalSkillEffects(Skill sk, SkillEffect[] effects)
		{
			if (sk.GetSkillId() == SkillId.SneezeShot)
			{
				SneezeShot skill = sk as SneezeShot;

				SkillEffect[] newEffects = new SkillEffect[2];
				newEffects[0] = new EffectPushaway(50);
				newEffects[1] = new EffectStun(1f);

				return newEffects;
			}

			return null;
		}

		protected override void InitInfo()
		{
			FileName = "sneezeshot_upgrade";
			TypeName = "Sneeze Shot";
			VisibleName = "Push Module";
			Description = "Enemies hit by Sneeze Shot will be pushed away by the projectile.";
		}
	}

	public class SneezeShotExplodeUpgrade : EquippableItem //TODO effect 
	{
		public static int rarity = 2;
		public static ItemType type = ItemType.CLASSIC;

		public const float RADIUS = 5;
		public const float RADIUS_LEVEL_ADD = 0.6f;

		public const float DAMAGE = 20;
		public const float DAMAGE_LEVEL_ADD = 3;

		private int temp;

		public SneezeShotExplodeUpgrade(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 5;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			SneezeShot skill = set.GetSkill(SkillId.SneezeShot) as SneezeShot;
			if (skill == null)
				return;

			temp = skill.projectilesCount;
			skill.projectilesCount = 1;
			skill.explodeEffect = true;
			skill.DisableOriginalEffects();
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			SneezeShot skill = set.GetSkill(SkillId.SneezeShot) as SneezeShot;
			if (skill == null)
				return;

			skill.projectilesCount = temp;
			skill.explodeEffect = false;
			skill.EnableOriginalEffects();
		}

		public override SkillEffect[] CreateAdditionalSkillEffects(Skill sk, SkillEffect[] effects)
		{
			if (sk.GetSkillId() == SkillId.SneezeShot)
			{
				SkillEffect[] newEffects = new SkillEffect[1];
				newEffects[0] = new EffectAreaDamage((int) AddValueByLevel(DAMAGE, DAMAGE_LEVEL_ADD), 0, AddValueByLevel(RADIUS, RADIUS_LEVEL_ADD));

				return newEffects;
			}

			return null;
		}

		protected override void InitInfo()
		{
			FileName = "sneezeshot_upgrade";
			TypeName = "Sneeze Shot";
			VisibleName = "Push Module";
			Description = "Sneeze Shot fires only one projectile which explodes on impact and deals " + AddValueByLevel(DAMAGE, DAMAGE_LEVEL_ADD) + " area damage in radius of " + AddValueByLevel(RADIUS, RADIUS_LEVEL_ADD) + ".";
		}
	}

	public class SneezeShotDoubleExploderUpgrade : EquippableItem //TODO effect 
	{
		public static int rarity = 1;
		public static ItemType type = ItemType.EPIC;

		public const float RADIUS = 5;
		public const float RADIUS_LEVEL_ADD = 0.6f;

		private int temp;

		public SneezeShotDoubleExploderUpgrade(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 5;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			SneezeShot skill = set.GetSkill(SkillId.SneezeShot) as SneezeShot;
			if (skill == null)
				return;

			temp = skill.projectilesCount;
			skill.projectilesCount = 2;
			skill.explodeEffect = true;
			skill.randomAngle = 35;
			skill.DisableOriginalEffects();
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			SneezeShot skill = set.GetSkill(SkillId.SneezeShot) as SneezeShot;
			if (skill == null)
				return;

			skill.projectilesCount = temp;
			skill.explodeEffect = false;
			skill.randomAngle = 0;
			skill.EnableOriginalEffects();
		}

		public override SkillEffect[] CreateAdditionalSkillEffects(Skill sk, SkillEffect[] effects)
		{
			if (sk.GetSkillId() == SkillId.SneezeShot)
			{
				SneezeShot skill = sk as SneezeShot;

				SkillEffect[] newEffects = new SkillEffect[1];
				newEffects[0] = new EffectAreaDamage(skill.baseDamage, 0, AddValueByLevel(RADIUS, RADIUS_LEVEL_ADD));

				return newEffects;
			}

			return null;
		}

		protected override void InitInfo()
		{
			FileName = "sneezeshot_upgrade";
			TypeName = "Sneeze Shot";
			VisibleName = "Explode Module";
			Description = "Sneeze Shot fires 2 projectiles in random direction in front of you, projectiles explode on contact and deal area damage of same strength as the skill damage.";
		}
	}

	public class SneezeShotTripleExploderUpgrade : EquippableItem //TODO effect 
	{
		public static int rarity = 2;
		public static ItemType type = ItemType.EPIC;

		public const float RADIUS = 5;
		public const float RADIUS_LEVEL_ADD = 0.6f;

		private int temp;

		public SneezeShotTripleExploderUpgrade(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 5;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			SneezeShot skill = set.GetSkill(SkillId.SneezeShot) as SneezeShot;
			if (skill == null)
				return;

			temp = skill.projectilesCount;
			skill.projectilesCount = 3;
			skill.explodeEffect = true;
			skill.randomAngle = 35;
			skill.DisableOriginalEffects();
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			SneezeShot skill = set.GetSkill(SkillId.SneezeShot) as SneezeShot;
			if (skill == null)
				return;

			skill.projectilesCount = temp;
			skill.explodeEffect = false;
			skill.randomAngle = 0;
			skill.EnableOriginalEffects();
		}

		public override SkillEffect[] CreateAdditionalSkillEffects(Skill sk, SkillEffect[] effects)
		{
			if (sk.GetSkillId() == SkillId.SneezeShot)
			{
				SneezeShot skill = sk as SneezeShot;

				SkillEffect[] newEffects = new SkillEffect[1];
				newEffects[0] = new EffectAreaDamage(skill.baseDamage, 0, AddValueByLevel(RADIUS, RADIUS_LEVEL_ADD));

				return newEffects;
			}

			return null;
		}

		protected override void InitInfo()
		{
			FileName = "sneezeshot_upgrade";
			TypeName = "Sneeze Shot";
			VisibleName = "Explode Module";
			Description = "Sneeze Shot fires 2 projectiles in random direction in front of you, projectiles explode on contact and deal area damage with strength same as the skill damage.";
		}
	}

	public class SneezeShotMissilesUpgrade : EquippableItem //TODO effect 
	{
		public static int rarity = 2;
		public static ItemType type = ItemType.EPIC;

		public const int COUNT = 6;
		public const float COUNT_ADD = 1f;

		public const int DAMAGE = 10;

		private int temp, temp2;

		public SneezeShotMissilesUpgrade(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 5;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			SneezeShot skill = set.GetSkill(SkillId.SneezeShot) as SneezeShot;
			if (skill == null)
				return;

			temp = skill.projectilesCount;
			temp2 = skill.baseDamage;

			skill.projectilesCount = (int) AddValueByLevel(COUNT, COUNT_ADD);
			skill.randomAngle = 45;
			skill.selectTargetOnLaunch = true;
			skill.baseDamage = DAMAGE;
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			SneezeShot skill = set.GetSkill(SkillId.SneezeShot) as SneezeShot;
			if (skill == null)
				return;

			skill.projectilesCount = temp;
			skill.baseDamage = temp2;
			skill.randomAngle = 0;
			skill.selectTargetOnLaunch = false;
		}

		protected override void InitInfo()
		{
			FileName = "sneezeshot_upgrade";
			TypeName = "Sneeze Shot";
			VisibleName = "Missile Volley Module";
			Description = "Sneeze Shot fires " + AddValueByLevel(COUNT, COUNT_ADD) + " projectiles at random angle that guide themselves to targets. Damage is set to 10.";
		}
	}

	public class SneezeShotPenetrateAimUpgrade : EquippableItem //TODO effect 
	{
		public static int rarity = 2;
		public static ItemType type = ItemType.RARE;

		public const int AFTER_DMG_AMOUNT = 50;
		public const int AFTER_DMG_LEVEL_ADD = 10;

		private int temp, temp2;
		private float temp3;

		public SneezeShotPenetrateAimUpgrade(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 5;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			SneezeShot skill = set.GetSkill(SkillId.SneezeShot) as SneezeShot;
			if (skill == null)
				return;

			temp = skill.secondDamage;
			temp2 = skill.aimArea;
			temp3 = skill.interpolAdd;

			skill.penetrateTargets = true;
			skill.maxPenetratedTargets += 2;
			skill.aimArea = skill.range;
			skill.interpolAdd = 1f;
			skill.navigateAfterPenetration = true;
			skill.secondDamage = (int) (skill.baseDamage*AddValueByLevel(AFTER_DMG_AMOUNT, AFTER_DMG_LEVEL_ADD)/100f);
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			SneezeShot skill = set.GetSkill(SkillId.SneezeShot) as SneezeShot;
			if (skill == null)
				return;

			skill.aimArea = temp2;
			skill.secondDamage = temp;
			skill.interpolAdd = temp3;
			skill.penetrateTargets = false;
			skill.maxPenetratedTargets -= 2;
			skill.navigateAfterPenetration = false;
		}

		protected override void InitInfo()
		{
			FileName = "sneezeshot_upgrade";
			TypeName = "Sneeze Shot";
			VisibleName = "Penetrate Module";
			Description = "Sneeze Shot projectiles will penetrate the first target they hit and then choose another target and guide themselves to them. Second target hit will receive " + AddValueByLevel(AFTER_DMG_AMOUNT, AFTER_DMG_LEVEL_ADD) + "% of the skill damage.";
		}
	}

	public class SneezeShotPenetrateMissilePenetratorUpgrade : EquippableItem //TODO effect 
	{
		public static int rarity = 2;
		public static ItemType type = ItemType.RARE;

		private int temp, temp2, temp4;
		private float temp3;

		public SneezeShotPenetrateMissilePenetratorUpgrade(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 1;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			SneezeShot skill = set.GetSkill(SkillId.SneezeShot) as SneezeShot;
			if (skill == null)
				return;

			temp = skill.range;
			temp2 = skill.aimArea;
			temp3 = skill.interpolAdd;
			temp4 = skill.projectilesCount;

			skill.penetrateTargets = true;
			skill.range = skill.range*2;
			skill.projectilesCount = 1;
			skill.maxPenetratedTargets += 4;
			skill.aimArea = skill.range;
			skill.interpolAdd = 1f;
			skill.navigateAfterPenetration = true;
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			SneezeShot skill = set.GetSkill(SkillId.SneezeShot) as SneezeShot;
			if (skill == null)
				return;

			skill.range = temp;
			skill.aimArea = temp2;
			skill.interpolAdd = temp3;
			skill.projectilesCount = temp4;
			skill.penetrateTargets = false;
			skill.maxPenetratedTargets -= 4;
			skill.navigateAfterPenetration = false;
		}

		protected override void InitInfo()
		{
			FileName = "sneezeshot_upgrade";
			TypeName = "Sneeze Shot";
			VisibleName = "Penetrate Module";
			Description = "Sneeze Shot will fire only 1 projectile that will penetrate through up to 4 targets. The projectile will guide itself to a new target after hitting something. Skill range is doubled.";
		}
	}

	public class SneezeShotZeroReuseUpgrade : EquippableItem //TODO effect 
	{
		public static int rarity = 2;
		public static ItemType type = ItemType.CLASSIC;

		public const int CHANCE = 25;
		public const int CHANCE_LEVEL_ADD = 2;

		public SneezeShotZeroReuseUpgrade(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 5;
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			SneezeShot skill = set.GetSkill(SkillId.SneezeShot) as SneezeShot;
			if (skill == null)
				return;

			skill.nullReuseChance = (int) AddValueByLevel(CHANCE, CHANCE_LEVEL_ADD);
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			SneezeShot skill = set.GetSkill(SkillId.SneezeShot) as SneezeShot;
			if (skill == null)
				return;

			skill.nullReuseChance = 0;
		}

		protected override void InitInfo()
		{
			FileName = "sneezeshot_upgrade";
			TypeName = "Sneeze Shot";
			VisibleName = "Reuse Module";
			Description = "Sneeze Shot will have a " + AddValueByLevel(CHANCE, CHANCE_LEVEL_ADD) + "% chance that the skill will be immediately available for use after casting (= chance for zero reuse).";
		}
	}

	public class SneezeShotWeakenUpgrade : EquippableItem //TODO effect 
	{
		public static int rarity = 2;
		public static ItemType type = ItemType.CLASSIC;

		public const float DURATION = 5;
		public const float AMOUNT_LEVEL_ADD = 1f;

		public SneezeShotWeakenUpgrade(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 5;
		}

		public override SkillEffect[] CreateAdditionalSkillEffects(Skill sk, SkillEffect[] effects)
		{
			if (sk.GetSkillId() == SkillId.SneezeShot)
			{
				SkillEffect[] newEffects = new SkillEffect[1];
				newEffects[0] = new EffectShield(-0.5f, AddValueByLevel(DURATION, AMOUNT_LEVEL_ADD));

				return newEffects;
			}

			return null;
		}

		protected override void InitInfo()
		{
			FileName = "sneezeshot_upgrade";
			TypeName = "Sneeze Shot";
			VisibleName = "Reuse Module";
			Description = "Sneeze Shot will reduce the shield protection of hit enemies by 50% for " + AddValueByLevel(DURATION, AMOUNT_LEVEL_ADD) + " seconds.";
		}
	}

	public class SneezeShotDieExplodeUpgrade : EquippableItem //TODO effect 
	{
		public static int rarity = 2;
		public static ItemType type = ItemType.CLASSIC;

		public const float DAMAGE = 30;
		public const float AMOUNT_LEVEL_ADD = 5;

		public const float RANGE = 5;
		public const float RANGE_LEVEL_ADD = 0.5f;

		public SneezeShotDieExplodeUpgrade(int level)
			: base(level)
		{
			RequiredClass = ClassId.CommonCold;
			MaxLevel = 5;
		}

		public override void OnKill(Character target, SkillId skillId)
		{
			if (target != null && skillId == SkillId.SneezeShot)
			{
				SneezeShot ss = Owner.Skills.GetSkill(skillId) as SneezeShot;

				GameObject explosion = ss.CreateParticleEffect("Explosion", false, target.GetData().GetBody().transform.position);
				explosion.GetComponent<ParticleSystem>().Play();
				Object.Destroy(explosion, 2f);

				EffectAuraDamage ef = new EffectAuraDamage((int) AddValueByLevel(DAMAGE, AMOUNT_LEVEL_ADD), 0, AddValueByLevel(RANGE, RANGE_LEVEL_ADD));
				ef.Source = Owner;
				ef.attackTeam = target.Team;

				ef.ApplyEffect(target, null);
			}
		}

		protected override void InitInfo()
		{
			FileName = "sneezeshot_upgrade";
			TypeName = "Sneeze Shot";
			VisibleName = "Explosion Module";
			Description = "All enemies killed using Sneeze Shot will explode, dealing " + AddValueByLevel(DAMAGE, AMOUNT_LEVEL_ADD) + " damage to all nearby enemies in range of " + AddValueByLevel(RANGE, RANGE_LEVEL_ADD) + ".";
		}
	}
}
