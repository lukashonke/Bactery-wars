// Copyright (c) 2015, Lukas Honke
// ========================
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
using Assets.scripts.Mono.ObjectData;
using Assets.scripts.Skills;
using Assets.scripts.Skills.Base;
using UnityEngine;

namespace Assets.scripts.Actor.MonsterClasses
{
	public abstract class MonsterTemplate
	{
		public List<Skill> TemplateSkills { get; set; }
		public ActiveSkill MeleeSkill;

		public string Name { get; set; }

		public int TargetRotationSpeed { get; set; }

		public int MaxHp { get; set; }
		public int MaxMp { get; set; }
		public float MaxSpeed { get; set; }
		public float RotationSpeed { get; set; }
		public float Shield { get; set; }
		public int CriticalRate { get; set; } // 1000 equals 100% to critical strike
		public float CriticalDamageMul { get; set; } // if critical strike, damage is multiplied by this value

		public int HpLevelScale { get; set; }

		public int XpReward { get; set; }
		public float XpLevelMul { get; set; }

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
			RotationSpeed = 25;
			Shield = 1.0f;
			CriticalRate = 0;
			CriticalDamageMul = 2f;
			XpReward = 1;
			XpLevelMul = 0.5f; // 50% more XP per each next level
			TargetRotationSpeed = 10;

			HpLevelScale = 0;
		}

		public virtual int GetXp(Monster m)
		{
			float lvlBonus = (m.Level-1)*0.5f*XpReward;
			if (lvlBonus < 0)
				lvlBonus = 0;

			return (int) (XpReward  + lvlBonus);
		}

		public virtual void Init()
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
			m.Status.MaxHp = Scale(m.Status.MaxHp, HpLevelScale, level);
			m.Status.SetHp(m.Status.MaxHp);
		}

		public virtual void InitAppearanceData(Monster m, EnemyData data)
		{
			
		}

		public virtual void OnAfterSpawned(Monster m)
		{
			
		}

		protected virtual void SetMeleeAttackSkill(ActiveSkill skill)
		{
			MeleeSkill = skill;
		}

		public abstract void AddSkillsToTemplate();
		public abstract MonsterAI CreateAI(Character ch);

		public virtual void OnTalkTo(Character source)
		{
			
		}

		public abstract GroupTemplate GetGroupTemplate();
		public abstract MonsterId GetMonsterId();

		public virtual string GetMonsterTypeName()
		{
			return GetMonsterId().ToString();
		}

		public virtual string GetFolderName()
		{
			return GetMonsterId().ToString();
		}

		protected int Scale(int defaultVal, int addPerLevel, int level)
		{
			return defaultVal + ((level - 1)*addPerLevel);
		}

		protected float Scale(float defaultVal, float addPerLevel, int level)
		{
			return defaultVal + ((level - 1) * addPerLevel);
		}

		/// <param name="mulPerLevel">0.3 --- +30% per level</param>
		protected float ScalePercent(int defaultVal, float mulPerLevel, int level)
		{
			return defaultVal + defaultVal*((level - 1)*mulPerLevel);
		}
	}
}
