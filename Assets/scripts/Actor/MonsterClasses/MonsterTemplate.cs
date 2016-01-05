﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.Actor.PlayerClasses.Base;
using Assets.scripts.AI;
using Assets.scripts.Mono;
using Assets.scripts.Mono.MapGenerator;
using Assets.scripts.Skills;
using UnityEngine.iOS;

namespace Assets.scripts.Actor.MonsterClasses
{
	public abstract class MonsterTemplate
	{
		public List<Skill> TemplateSkills { get; set; }
		public ActiveSkill MeleeSkill;

		public int MaxHp { get; protected set; }
		public int MaxMp { get; protected set; }
		public int MaxSpeed { get; protected set; }
		public float Shield { get; protected set; }
		public int CriticalRate { get; protected set; } // 1000 equals 100% to critical strike
		public float CriticalDamageMul { get; protected set; } // if critical strike, damage is multiplied by this value

		protected MonsterTemplate()
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
		}

		protected virtual void Init()
		{
			AddSkills();
		}

		protected virtual void SetMeleeAttackSkill(ActiveSkill skill)
		{
			MeleeSkill = skill;
		}

		protected abstract void AddSkills();
		public abstract MonsterAI CreateAI(Character ch);

		public virtual void OnTalkTo(Character source)
		{
			
		}

		public abstract GroupTemplate GetGroupTemplate();
		public abstract MonsterId GetMonsterId();
	}
}
