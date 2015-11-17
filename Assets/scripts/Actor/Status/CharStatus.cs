using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Skills;
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
		public int MoveSpeed { get; set; }

		public List<Skill> ActiveSkills { get; private set; }

		protected CharStatus(bool isDead, int hp, int mp)
		{
			IsDead = isDead;
			Hp = hp;
			Mp = mp;
			MoveSpeed = 5;
			ActiveSkills = new List<Skill>();
		}

		public void ReceiveDamage(int dmg)
		{
			Hp -= dmg;

			if (Hp < 0)
				Hp = 0;

			if (Hp == 0)
				DoDie();
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
	}
}
