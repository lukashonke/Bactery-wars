// Copyright (c) 2015, Lukas Honke
// ========================
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Assets.scripts.Actor.PlayerClasses;
using Assets.scripts.Actor.Status;
using Assets.scripts.AI;
using Assets.scripts.Base;
using Assets.scripts.Mono;
using Assets.scripts.Mono.ObjectData;
using Assets.scripts.Skills;
using Assets.scripts.Skills.Base;
using Assets.scripts.Upgrade;
using Assets.scripts.Upgrade.Classic;
using UnityEngine;

namespace Assets.scripts.Actor
{
	/// <summary>
	/// Class that holds information about the player (skills, class, stats, status - hp, speed, etc.)
	/// </summary>
	public class Player : Character
	{
		public ClassTemplate Template { get; set; }
		public int DnaPoints { get; private set; }
		public int UpgradePoints { get; set; }

		public List<Skill> AvailableSkills { get; private set; }
		public List<Skill> AvailableAutoattacks { get; private set; }

		public Dictionary<int, int> SkillSlotLevels { get; private set; }

		public Stash ItemStash { get; private set; }

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

		public override void OnLevelChange()
		{
			if (Level >= 10)
			{
				Inventory.ActiveCapacity = 4;
			}
			else if (Level >= 5)
			{
				Inventory.ActiveCapacity = 3;
			}

			if(Inventory != null && GetData() != null && GetData().ui != null)
				GetData().ui.UpdateInventory(Inventory);
		}

		public override void DoDie(Character killer = null, SkillId skillId = SkillId.SkillTemplate)
		{
			base.DoDie(killer, skillId);

			GameSystem.Instance.Controller.PlayerDied();
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
			ItemStash = new Stash(this, 50);

			AvailableSkills = new List<Skill>();
			AvailableAutoattacks = new List<Skill>();
			SkillSlotLevels = new Dictionary<int, int>();
			UpgradePoints = 100;

			for (int ii = 0; ii < 9; ii++)
			{
				SkillSlotLevels.Add(ii, 1);
			}

			//TODO temp reseni, prida vsechny skilly
			foreach (SkillId id in Enum.GetValues(typeof(SkillId)))
			{
				Skill sk = SkillTable.Instance.GetSkill(id);
				if (sk == null)
					continue;
				if (sk.AvailableToPlayer)
				{
					AvailableSkills.Add(sk);
				}

				if (sk.AvailableToDeveloper && GameSession.getAllSkills)
				{
					AvailableSkills.Add(sk);
				}

				if(sk.AvailableToPlayerAsAutoattack)
					AvailableAutoattacks.Add(sk);
			}

			int i = 0;
			foreach(Skill templateSkill in Template.TemplateSkills)
			{
				// vytvorit novy objekt skillu
				Skill newSkill = SkillTable.Instance.CreateSkill(templateSkill.GetSkillId());
				newSkill.SetOwner(this);
				newSkill.InitIcon();

				//newSkill.IsLocked = true;

				Skills.AddSkill(newSkill);

				i++;
				//Debug.Log("adding skill to " + i + ": " + newSkill.Name);
			}

			if (Template.MeleeSkill != null)
			{
				Skill newSkill = SkillTable.Instance.CreateSkill(Template.MeleeSkill.GetSkillId());
				newSkill.SetOwner(this);
				newSkill.InitIcon();

				MeleeSkill = (ActiveSkill) newSkill;
			}

			Template.InitSkillsOnPlayer(Skills, MeleeSkill);

			Inventory.ActiveCapacity = Template.ActiveUpgradesCapacity;
			Inventory.Capacity = Template.InventoryCapacity;

			Inventory.BasestatUpgrades.Add(new HpUpgradeAdd(0).Init().SetOwner(this) as EquippableItem);
			Inventory.BasestatUpgrades.Add(new SpeedUpgrade(0).Init().SetOwner(this) as EquippableItem);
			Inventory.BasestatUpgrades.Add(new DamageUpgrade(0).Init().SetOwner(this) as EquippableItem);
			Inventory.BasestatUpgrades.Add(new ShieldUpgrade(0).Init().SetOwner(this) as EquippableItem);
			Inventory.BasestatUpgrades.Add(new CriticalRateUpgrade(0).Init().SetOwner(this) as EquippableItem);

			Data.UpdateInventory(Inventory);
			Inventory.LoadUpgrades();
			UpdateStats();
		}

		public int GetSlotUpgradePrice(int slot, int level)
		{
			return 10;
		}

		public int GetStatUpgradePrice(int level)
		{
			return 10;
		}

		public void LevelUpSlot(int slot)
		{
			int val = SkillSlotLevels[slot];
			int price = GetSlotUpgradePrice(slot, val);

			if (UpgradePoints >= price)
			{
				UpgradePoints -= price;
				val ++;
				SkillSlotLevels[slot] = val;
			}
		}

		public void SwapSkills(Skill firstSkill, string targetSkillName)
		{
			int fromIndex = GetSkillIndex(firstSkill.GetName());
			int toIndex = GetSkillIndex(targetSkillName);

			int firstLevel = SkillSlotLevels[fromIndex];
			int secondLevel = SkillSlotLevels[toIndex];

			DeactivateSkill(firstSkill.GetName());
			DeactivateSkill(targetSkillName);

			SkillSlotLevels[fromIndex] = secondLevel;
			SkillSlotLevels[toIndex] = firstLevel;

			if (fromIndex > toIndex)
			{
				ActivateSkill(firstSkill, toIndex);
				Skill sk = SkillTable.Instance.GetSkill((SkillId)Enum.Parse(typeof(SkillId), targetSkillName));

				ActivateSkill(sk, fromIndex);
			}
			else
			{
				Skill sk = SkillTable.Instance.GetSkill((SkillId)Enum.Parse(typeof(SkillId), targetSkillName));
				ActivateSkill(sk, fromIndex);

				ActivateSkill(firstSkill, toIndex);
			}
		}

		public int DeactivateSkill(string skillName)
		{
			int index = 0;
			foreach (Skill sk in Skills.Skills.ToArray())
			{
				if (sk.GetName() == skillName)
				{
					Skills.Skills.RemoveAt(index);
					break;
				}
				index++;
			}

			return index;
		}

		public void DeactivateSkill(int index)
		{
			Skills.Skills.RemoveAt(index);
		}

		public int GetSkillIndex(string skillName)
		{
			int index = 0;
			foreach (Skill sk in Skills.Skills.ToArray())
			{
				if (sk.GetName() == skillName)
				{
					break;
				}
				index++;
			}

			return index;
		}

		public void SelectAutoattack(Skill skill)
		{
			if (skill is ActiveSkill)
			{
				Skill newSkill = SkillTable.Instance.CreateSkill(skill.GetSkillId());
				newSkill.SetOwner(this);
				newSkill.InitIcon();

				MeleeSkill = (ActiveSkill)newSkill;
			}
		}

		public void TutorialActivateSkill(int level)
		{
			Skill sk = null;
			switch (level)
			{
				case 0:
					sk = SkillTable.Instance.GetSkill(SkillId.SneezeShot);
					break;
				case 1:
					sk = SkillTable.Instance.GetSkill(SkillId.Charge);
					break;
				case 2:
					sk = SkillTable.Instance.GetSkill(SkillId.ColdPush);
					break;
				case 3:
					sk = SkillTable.Instance.GetSkill(SkillId.Haste);
					break;
				case 4:
					sk = SkillTable.Instance.GetSkill(SkillId.RhinoBeam);
					break;
			}

			if (sk != null)
			{
				ActivateSkill(sk, level);
				GetData().ui.UpdateSkillTimers();
			}
		}

		public void ActivateSkill(Skill skill, int targetSlot)
		{
			if (Skills.HasSkill(skill.GetSkillId()))
				return;

			if (targetSlot <= 5 && targetSlot >= 0)
			{
				int slotLevel = SkillSlotLevels[targetSlot];
				if (skill.RequiredSlotLevel > slotLevel)
				{
					Message("This skill requires slot of level " + skill.RequiredSlotLevel + ".");
					return;
				}

				Skill newSkill = SkillTable.Instance.CreateSkill(skill.GetSkillId());
				newSkill.SetOwner(this);
				newSkill.IsLocked = false;
				newSkill.InitIcon();

				bool set = false;
				for (int i = 0; i < 10; i++)
				{
					if (i == targetSlot)
					{
						Skills.Skills.Insert(i, newSkill);
						set = true;
						break;
					}
				}

				if (!set)
				{
					Skills.Skills.Add(newSkill);
				}
			}
		}

		public void UnlockSkill(int order, bool msg)
		{
			int i = 0;
			foreach (Skill sk in Skills.Skills)
			{
				if (i == order)
				{
					sk.IsLocked = false;

					if (sk is ActiveSkill)
					{
						((ActiveSkill)sk).LastUsed = -1000f;
						Data.SetSkillReuseTimer(sk as ActiveSkill, false);
					}

					if(msg)
						Message("You have unlocked skill " + sk.GetVisibleName() + ".");
				}
				i++;
			}
		}

		public override void UpdateStats()
		{
			int tmpMaxHp = Template.MaxHp;
			int tmpMaxMp = Template.MaxMp;
			int tmpCritRate = Template.CriticalRate;
			float tmpCritDmg = Template.CriticalDamageMul;
			float tmpRunSpeed = Template.MaxSpeed;
			float tmpDmgMul = Template.DamageMul;
			float tmpDmgAdd = Template.DamageAdd;
			float tmpShield = Template.Shield;

			foreach (EquippableItem u in Inventory.ActiveUpgrades)
			{
				u.ModifyMaxHp(ref tmpMaxHp);
				u.ModifyMaxMp(ref tmpMaxMp);
				u.ModifyCriticalRate(ref tmpCritRate);
				u.ModifyCriticalDmg(ref tmpCritDmg);
				u.ModifyRunSpeed(ref tmpRunSpeed);
				u.ModifyDmgMul(ref tmpDmgMul);
				u.ModifyDmgAdd(ref tmpDmgAdd);
				u.ModifyShield(ref tmpShield);
			}

			foreach (EquippableItem u in Inventory.BasestatUpgrades)
			{
				u.ModifyMaxHp(ref tmpMaxHp);
				u.ModifyMaxMp(ref tmpMaxMp);
				u.ModifyCriticalRate(ref tmpCritRate);
				u.ModifyCriticalDmg(ref tmpCritDmg);
				u.ModifyRunSpeed(ref tmpRunSpeed);
				u.ModifyDmgMul(ref tmpDmgMul);
				u.ModifyDmgAdd(ref tmpDmgAdd);
				u.ModifyShield(ref tmpShield);
			}

			UpdateMaxHp(tmpMaxHp);
			if (Status.Hp > Status.MaxHp)
				UpdateHp(Status.MaxHp);

			UpdateMaxMp(tmpMaxMp);
			if (Status.Mp > Status.MaxMp)
				UpdateMp(Status.MaxMp);

			Status.CriticalRate = tmpCritRate;
			Status.CriticalDamageMul = tmpCritDmg;
			Status.DamageOutputMul = tmpDmgMul;
			Status.DamageOutputAdd = tmpDmgAdd;
			Status.Shield = tmpShield;
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

		public override void Message(string s, int level=1)
		{
			GetData().ui.ScreenMessage(s, level);
		}

		public void AddDnaPoints(int n)
		{
			DnaPoints += n;
		}

		public bool ReduceDnaPoints(int n)
		{
			if (DnaPoints >= n)
			{
				DnaPoints -= n;
				return true;
			}
			return false;
		}
	}
}
