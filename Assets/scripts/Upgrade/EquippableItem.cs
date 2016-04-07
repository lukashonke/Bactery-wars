using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Actor.PlayerClasses.Base;
using Assets.scripts.Mono;
using Assets.scripts.Skills;
using Assets.scripts.Skills.ActiveSkills;
using Assets.scripts.Skills.Base;
using Assets.scripts.Skills.SkillEffects;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.scripts.Upgrade
{
	/// <summary>
	/// allows dynamically changing of following things:
	/// - max hp, max mp, run speed, critical rate, critical dmg
	/// - 
	/// </summary>
	public abstract class EquippableItem : InventoryItem
	{
		public ClassId RequiredClass { get; protected set; }

		public int CurrentProgress { get; set; }
		public int NeedForNextLevel { get; set; }

		public bool GoesIntoBasestatSlot { get; protected set; }

		public EquippableItem(int level, bool collectableByPlayer=true) : base(level)
		{
			CollectableByPlayer = collectableByPlayer;

			GoesIntoBasestatSlot = false;

			CurrentProgress = 0;
			NeedForNextLevel = 1;
			MaxLevel = 10;
			RequiredClass = 0; //TODO restrict drops only for the class that player currently has

			VisibleName = FileName;
			TypeName = "Stat";
			Description = "No Description";
			Price = "No value";
			AdditionalInfo = null;
		}

		public void AddUpgradeProgress(EquippableItem u)
		{
			if (Level == MaxLevel)
				return;

			CurrentProgress++;
			if (CurrentProgress >= NeedForNextLevel)
			{
				Remove();
				Level ++;
				Init();
				Apply();

				CurrentProgress = 0;

				//NeedForNextLevel = (int) Math.Pow(2, Level);
				NeedForNextLevel = 1;
			}
		}

		public float MulValueByLevel(int baseDamage, float levelMultiplier)
		{
			int add = (int) (baseDamage*(Level - 1)*levelMultiplier - baseDamage);
			if (add < 0)
				add = 0;
			return baseDamage + add;
		}

		/*public float AddValueByLevel(int baseDamage, float levelMultiplier, bool alsoNegative=false)
		{
			int add = (int)((Level - 1) * levelMultiplier);
			if (add < 0 && !alsoNegative)
				add = 0;
			return (baseDamage + add);
		}*/

		public float AddValueByLevel(float baseDamage, float levelMultiplier, bool alsoNegative=false)
		{
			float add = (Level - 1)*levelMultiplier;
			if (add < 0 && !alsoNegative)
				add = 0;
			return (baseDamage + add);
		}

		public void Apply()
		{
			ApplySkillChanges(Owner.Skills, Owner.MeleeSkill);
		}

		public void Remove()
		{
			RestoreSkillChanges(Owner.Skills, Owner.MeleeSkill);
		}

		public virtual void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
		}

		public virtual void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
		}

		public virtual SkillEffect[] CreateAdditionalSkillEffects(Skill sk, SkillEffect[] effects)
		{
			if (!(sk is ActiveSkill))
				return null;

			return null;
		}

		public virtual void OnKill(Character target, SkillId skillId)
		{
			
		}

		public virtual void OnGiveDamage(Character target, int damage, SkillId skillId)
		{
			
		}

		public virtual void ModifySkillEffects(Skill sk, SkillEffect[] effects)
		{
			if (!(sk is ActiveSkill))
				return;

			ActiveSkill skill = (ActiveSkill) sk;
		}

		public virtual void ModifyRunSpeed(ref float runSpeed)
		{
			
		}

		public virtual void ModifyDmgMul(ref float dmgMul)
		{
			
		}

		public virtual void ModifyDmgAdd(ref float dmgAdd)
		{

		}

		public virtual void ModifyMaxHp(ref int maxHp)
		{
			
		}

		public virtual void ModifyMaxMp(ref int maxMp)
		{
			
		}

		public virtual void ModifyCriticalRate(ref int critRate)
		{
			
		}

		public virtual void ModifyCriticalDmg(ref float critDmg)
		{
			
		}

		public virtual void ModifySkillCooldown(ActiveSkill sk, ref float cooldown)
		{
			
		}

		public virtual void ModifySkillReuse(ActiveSkill sk, ref float reuse)
		{

		}

		public virtual void ModifySkillCasttime(ActiveSkill sk, ref float casttime)
		{

		}

		public virtual void ModifyShield(ref float shield)
		{

		}

		public new EquippableItem SetOwner(Character ch)
		{
			base.SetOwner(ch);
			return this;
		}
	}
}
