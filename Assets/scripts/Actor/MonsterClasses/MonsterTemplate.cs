using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.Actor.PlayerClasses.Base;
using Assets.scripts.Actor.Status;
using Assets.scripts.AI;
using Assets.scripts.Mono;
using Assets.scripts.Mono.MapGenerator;
using Assets.scripts.Skills;
using Assets.scripts.Skills.Base;
using UnityEngine;

namespace Assets.scripts.Actor.MonsterClasses
{
	public abstract class MonsterTemplate
	{
		public List<Skill> TemplateSkills { get; set; }
		public ActiveSkill MeleeSkill;

		public string Name { get; protected set; }

		public int MaxHp { get; protected set; }
		public int MaxMp { get; protected set; }
		public float MaxSpeed { get; protected set; }
		public float Shield { get; protected set; }
		public int CriticalRate { get; protected set; } // 1000 equals 100% to critical strike
		public float CriticalDamageMul { get; protected set; } // if critical strike, damage is multiplied by this value

		public int XpReward { get; protected set; }
		public float XpLevelMul { get; protected set; }

		public bool ShowNameInGame = false;

		public bool IsAggressive = false;
		public int AggressionRange = 5;

		public bool AlertsAllies = false;

		public bool RambleAround = false;
		public int RambleAroundMaxDist = 4;

		protected MonsterTemplate()
		{
			TemplateSkills = new List<Skill>();

			InitDefaultStats();

			Init();

			Name = Enum.GetName(typeof (MonsterId), GetMonsterId());
		}

		protected void InitDefaultStats()
		{
			MaxHp = 50;
			MaxMp = 50;
			MaxSpeed = 10;
			Shield = 1.0f;
			CriticalRate = 0;
			CriticalDamageMul = 2f;
			XpReward = 1;
			XpLevelMul = 0.5f; // 50% more XP per each next level
		}

		public virtual int GetXp(Monster m)
		{
			float lvlBonus = (m.Level-1)*0.5f*XpReward;
			if (lvlBonus < 0)
				lvlBonus = 0;

			return (int) (XpReward  + lvlBonus);
		}

		protected virtual void Init()
		{
			AddSkillsToTemplate();
		}

		public virtual void InitSkillsOnMonster(SkillSet set, ActiveSkill meleeSkill, int level)
		{

		}

		public virtual void OnDie(Monster m)
		{
			
		}

		public virtual void InitMonsterStats(Monster m, int level)
		{
			//Debug.Log("level is " + level);
			//status.MaxHp = status.MaxHp * level;
			//m.Status.SetHp(status.MaxHp);
		}

		protected virtual void SetMeleeAttackSkill(ActiveSkill skill)
		{
			MeleeSkill = skill;
		}

		protected abstract void AddSkillsToTemplate();
		public abstract MonsterAI CreateAI(Character ch);

		public virtual void OnTalkTo(Character source)
		{
			
		}

		public abstract GroupTemplate GetGroupTemplate();
		public abstract MonsterId GetMonsterId();
	}
}
