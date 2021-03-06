﻿// Copyright (c) 2015, Lukas Honke
// ========================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Skills;
using Assets.scripts.Skills.SkillEffects;
using UnityEngine;

namespace Assets.scripts.Actor.Status
{
	/*
		Trida slouzi k ulozeni stavovych informaci charakteru:
		(pocet HP=zivota, MP, maximalni mozna momentalni rychlost pohybu, prokleti, atd.)
	*/
	public abstract class CharStatus
	{
		public bool IsDead { get; private set; }

		public int Hp { get; private set; }
		public int Mp { get; private set; }
		public int MaxHp { get; set; }
		public int MaxMp { get; set; }
		public float MoveSpeed { get; set; }
		public float RotationSpeed { get; set; }

		public float Shield { get; set; }
		public int CriticalRate { get; set; } // 1000 equals 100% to critical strike
		public float CriticalDamageMul { get; set; } // if critical strike, damage is multiplied by this value

		public float DamageOutputMul { get; set; }
		public float DamageOutputAdd { get; set; }

		public List<Skill> ActiveSkills { get; private set; }

		public int XP { get; set; }

		public bool IsWalking = false;

		public bool CanCastSkills { get; set; }

		protected CharStatus(bool isDead, int hp, int mp, int maxHp, int maxMp, float moveSpeed, float rotationSpeed, float shield, int criticalRate, float criticalDamageMul, float damageMul, float damageAdd)
		{
			IsDead = isDead;
			Hp = hp;
			Mp = mp;
			MaxHp = maxHp;
			MaxMp = maxMp;
			MoveSpeed = moveSpeed;
			RotationSpeed = rotationSpeed;

			DamageOutputAdd = damageAdd;
			DamageOutputMul = damageMul;

			Shield = shield;

			CriticalRate = criticalRate;
			CriticalDamageMul = criticalDamageMul;

			ActiveSkills = new List<Skill>();
			CanCastSkills = true;
		}

		public void AddXp(int xp)
		{
			XP += xp;
		}

		public void ReceiveHeal(int heal)
		{
			SetHp(Hp + heal);
		}

		public void ReceiveDamage(int dmg)
		{
			//int shieldReduction = (int) (dmg*Shield - dmg);
			//dmg -= shieldReduction;

			Hp -= dmg;

			if (Hp < 0)
				Hp = 0;

			if (Hp == 0)
				DoDie();
		}

		public void SetHp(int newHp)
		{
			if (newHp > MaxHp)
				newHp = MaxHp;

			Hp = newHp;
		}

		public void SetMp(int newMp)
		{
			if (newMp > MaxMp)
				newMp = MaxMp;

			Mp = newMp;
		}

		public void SetSpeed(float newSpeed)
		{
			MoveSpeed = newSpeed;
		}

		public bool HasMana(int mp)
		{
			return Mp >= mp;
		}

		public bool HasHp(int hp)
		{
			return Hp >= hp;
		}

		public void DrainMana(int mp)
		{
			if (Mp < mp)
			{
				Mp = 0;
			}
			else
			{
				Mp -= mp;
			}
		}

		private void DoDie()
		{
			IsDead = true;
		}

		public bool IsCasting()
		{
			return ActiveSkills.Count > 0;
		}

		public bool Stunned { get; set; }

		public bool IsStunned() // TODO stun finish
		{
			return Stunned;
		}

		public bool IsImmobilized()
		{
			if (IsStunned())
				return true;

			// rooted checks

			return false;
		}

		public void Revive()
		{
			IsDead = false;
		}
	}
}
