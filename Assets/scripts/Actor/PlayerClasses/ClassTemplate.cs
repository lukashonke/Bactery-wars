using System.Collections.Generic;
using Assets.scripts.Actor.PlayerClasses.Base;
using Assets.scripts.Skills;
using Assets.scripts.Skills.Base;

namespace Assets.scripts.Actor.PlayerClasses
{
	/*
		Hardcoded templates
	*/
	public abstract class ClassTemplate
	{
		public List<Skill> TemplateSkills { get; set; }
		public ActiveSkill MeleeSkill;

		public int MaxHp { get; protected set; }
		public int MaxMp { get; protected set; }
		public float MaxSpeed { get; protected set; }
		public float Shield { get; protected set; }
		public int CriticalRate { get; protected set; } // 1000 equals 100% to critical strike
		public float CriticalDamageMul { get; protected set; } // if critical strike, damage is multiplied by this value
		public float DamageMul { get; protected set; }
		public float DamageAdd { get; protected set; }

		public int InventoryCapacity { get; protected set; }
		public int ActiveUpgradesCapacity { get; protected set; }

		protected ClassTemplate()
		{
			TemplateSkills = new List<Skill>();

			InitDefaultStats();

			Init();
		}

		protected void InitDefaultStats()
		{
			MaxHp = 50;
			MaxMp = 50;
			MaxSpeed = 10;
			Shield = 1.0f;
			CriticalRate = 0;
			CriticalDamageMul = 2f;
			DamageMul = 1f;
			DamageAdd = 0;

			InventoryCapacity = 10;
			ActiveUpgradesCapacity = 3;
		}

		protected virtual void Init()
		{
			AddSkillsToTemplate();
		}

		public virtual void InitSkillsOnPlayer(SkillSet set, ActiveSkill meleeSkill)
		{

		}

		protected virtual void SetMeleeAttackSkill(ActiveSkill skill)
		{
			MeleeSkill = skill;
		}

		protected abstract void AddSkillsToTemplate();
		public abstract ClassId GetClassId();
	}
}
