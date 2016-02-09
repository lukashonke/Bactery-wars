using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Skills;
using Assets.scripts.Skills.ActiveSkills;
using Assets.scripts.Skills.Base;
using Assets.scripts.Skills.SkillEffects;

namespace Assets.scripts.Upgrade
{
	/// <summary>
	/// allows dynamically changing of following things:
	/// - max hp, max mp, run speed, critical rate, critical dmg
	/// - 
	/// </summary>
	public abstract class AbstractUpgrade
	{
		public string Name { get; protected set; }
		public int Level { get; set; }
		private Character owner;
		public Character Owner
		{
			get { return owner; }
		}

		public AbstractUpgrade(int level)
		{
			Level = level;
		}

		public void SetOwner(Character ch)
		{
			owner = ch;
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

		public virtual void ModifySkillEffects(Skill sk, SkillEffect[] effects)
		{
			if (!(sk is ActiveSkill))
				return;

			ActiveSkill skill = (ActiveSkill) sk;
		}

		public virtual void ModifyRunSpeed(ref int runSpeed)
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
	}
}
