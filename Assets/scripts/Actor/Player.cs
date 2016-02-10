﻿using System.Collections;
using Assets.scripts.Actor.PlayerClasses;
using Assets.scripts.Actor.Status;
using Assets.scripts.AI;
using Assets.scripts.Base;
using Assets.scripts.Mono;
using Assets.scripts.Mono.ObjectData;
using Assets.scripts.Skills;
using Assets.scripts.Skills.Base;
using Assets.scripts.Upgrade;
using UnityEngine;

namespace Assets.scripts.Actor
{
	/// <summary>
	/// Datova trida reprezentujici informace o jednom hraci (jeho skilly, classu, stav (pocet HP, atd.))
	/// </summary>
	public class Player : Character
	{
		public ClassTemplate Template { get; set; }

		public Player(string name, PlayerData dataObject, ClassTemplate template) : base(name)
		{
			Data = dataObject;

			Template = template;
		}

		public Player(string name, PlayerData dataObject, ClassTemplate template, AbstractAI ai) : base(name, ai)
		{
			Data = dataObject;

			Template = template;
		}

		public new PlayerData GetData()
		{
			return (PlayerData) Data;
		}

		protected override AbstractAI InitAI()
		{
			return new PlayerAI(this);
		}

		/// <summary>
		/// Inicializuje sablonu hrace
		/// </summary>
		public void InitTemplate()
		{
			int i = 0;
			foreach(Skill templateSkill in Template.TemplateSkills)
			{
				// vytvorit novy objekt skillu
				Skill newSkill = SkillTable.Instance.CreateSkill(templateSkill.GetSkillId());
				newSkill.SetOwner(this);

				Skills.AddSkill(newSkill);

				i++;
				//Debug.Log("adding skill to " + i + ": " + newSkill.Name);
			}

			if (Template.MeleeSkill != null)
			{
				Skill newSkill = SkillTable.Instance.CreateSkill(Template.MeleeSkill.GetSkillId());
				newSkill.SetOwner(this);

				MeleeSkill = (ActiveSkill) newSkill;
			}

			Template.InitSkillsOnPlayer(Skills, MeleeSkill);

			Inventory.ActiveCapacity = Template.ActiveUpgradesCapacity;
			Inventory.Capacity = Template.InventoryCapacity;

			Inventory.LoadUpgrades();
			UpdateStats();
		}

		public override void UpdateStats()
		{
			int tmpMaxHp = Template.MaxHp;
			int tmpMaxMp = Template.MaxMp;
			int tmpCritRate = Template.CriticalRate;
			float tmpCritDmg = Template.CriticalDamageMul;
			int tmpRunSpeed = Template.MaxSpeed;

			foreach (AbstractUpgrade u in Inventory.ActiveUpgrades)
			{
				u.ModifyMaxHp(ref tmpMaxHp);
				u.ModifyMaxMp(ref tmpMaxMp);
				u.ModifyCriticalRate(ref tmpCritRate);
				u.ModifyCriticalDmg(ref tmpCritDmg);
				u.ModifyRunSpeed(ref tmpRunSpeed);
			}

			UpdateMaxHp(tmpMaxHp);
			if (Status.Hp > Status.MaxHp)
				UpdateHp(Status.MaxHp);

			UpdateMaxMp(tmpMaxMp);
			if (Status.Mp > Status.MaxMp)
				UpdateMp(Status.MaxMp);

			Status.CriticalRate = tmpCritRate;
			Status.CriticalDamageMul = tmpCritDmg;
			SetMoveSpeed(tmpRunSpeed);

			Data.UpdateStats();
		}

		/// <summary>
		/// Probiha kazdy snimek hry
		/// </summary>
		public override void OnUpdate()
		{
			
		}

		/// <summary>
		/// Inicializuje status hrace (HP, max. rychlost, atd.)
		/// </summary>
		protected override CharStatus InitStatus()
		{
			CharStatus st = new PlayerStatus(false, Template.MaxHp, Template.MaxMp, Template); //TODO
			GetData().SetVisibleHp(st.Hp);
			GetData().SetVisibleMaxHp(st.MaxHp);
			GetData().SetMoveSpeed(st.MoveSpeed);

			return st;
		}

		/// <summary>
		/// Inicializuje skillset hrace
		/// </summary>
		protected override SkillSet InitSkillSet()
		{
			SkillSet set = new SkillSet();
			return set;
		}

		/// <summary>
		/// Vytvori novy Task (vyuziva Unity Coroutiny)
		/// Task je ukol ktery muze probihat rozlozeny mezi nekolik snimku hry
		/// (prubeh metody se muze na urcitou dobu pozastavit a provest az v jinem, popr. hned nasledujicim snimku)
		/// </summary>
		public override Coroutine StartTask(IEnumerator skillTask)
		{
			return GetData().StartCoroutine(skillTask);
		}

		/// <summary>
		/// Predcasne ukonci Task
		/// </summary>
		/// <param name="c"></param>
		public override void StopTask(Coroutine c)
		{
			if(c != null)
				GetData().StopCoroutine(c);
		}

		public override void StopTask(IEnumerator t)
		{
			GetData().StopCoroutine(t);
		}

		public override bool IsInteractable()
		{
			return false;
		}
	}
}
