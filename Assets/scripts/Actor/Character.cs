using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor.Status;
using Assets.scripts.AI;
using Assets.scripts.Base;
using Assets.scripts.Mono;
using Assets.scripts.Skills;
using Assets.scripts.Skills.Base;
using UnityEngine;

namespace Assets.scripts.Actor
{
	public abstract class Character : Entity
	{
		/// <summary>
		/// Status characteru (HP, MP, atd.)
		/// </summary>
		public CharStatus Status { get; private set; }

		/// <summary>
		/// Skillset characteru
		/// </summary>
		public SkillSet Skills { get; set; }
		public ActiveSkill MeleeSkill { get; set; }
		public Knownlist Knownlist { get; private set; }
		public AbstractAI AI { get; private set; }

		public AbstractData Data { get; set; }

		public int Team { get; set; }

		protected Character(string name) : base(name)
		{
        }

		public void Init()
		{
			Knownlist = new Knownlist(this);
			Status = InitStatus();
			Skills = InitSkillSet();

			Knownlist.StartUpdating();

			AI = InitAI();
			AI.StartAITask();
		}

		public AbstractData GetData()
		{
			return Data;
		}

		protected abstract AbstractAI InitAI();
		protected abstract CharStatus InitStatus();
		protected abstract SkillSet InitSkillSet();

		public override void OnUpdate()
		{
			
		}

		/// <summary>
		/// Spusti kouzleni skillu
		/// </summary>
		/// <param name="skill"></param>
		public void CastSkill(Skill skill)
		{
			// skill is passive - cant cast it
			if (skill is PassiveSkill)
				return;

			// reuse check
			if (!skill.CanUse())
			{
				Debug.Log("skill cannot be used again yet");
				return;
			}

			skill.Start();
		}

		public void NotifyCastingModeChange()
		{
			GetData().IsCasting = Status.IsCasting();
		}

		/// <summary>
		/// Prerusi kouzleni vsech aktivnich skillů
		/// </summary>
		public void BreakCasting()
		{
			if (this is Player)
			{
				if (((Player)this).GetData().ActiveConfirmationSkill != null)
				{
					((Player)this).GetData().ActiveConfirmationSkill.AbortCast();
				}
			}

			if (!Status.IsCasting())
				return;

			Skill sk;
			for(int i = 0; i < Status.ActiveSkills.Count; i++)
			{
				sk = Status.ActiveSkills[i];

				if (sk.IsBeingCasted())
					sk.AbortCast();
			}

			Debug.Log("break done");
		}

		public void ReceiveDamage(Character source, int damage)
		{
			Status.ReceiveDamage(damage);

			if (Status.IsDead)
			{
				GetData().SetIsDead(true);
			}

			GetData().SetVisibleHp(Status.Hp);

			AI.AddAggro(source, damage);
		}

		public ActiveSkill GetMeleeAttackSkill()
		{
			return MeleeSkill;
		}

		/// <summary>
		/// Vytvori novy Task (vyuziva Unity Coroutiny)
		/// Task je ukol ktery muze probihat rozlozeny mezi nekolik snimku hry
		/// (prubeh metody se muze na urcitou dobu pozastavit a provest az v jinem, popr. hned nasledujicim snimku)
		/// </summary>
		public virtual Coroutine StartTask(IEnumerator skillTask)
		{
			return GameSystem.Instance.StartTask(skillTask);
		}

		/// <summary>
		/// Predcasne ukonci Task
		/// </summary>
		/// <param name="c"></param>
		public virtual void StopTask(Coroutine c)
		{
			GameSystem.Instance.StopTask(c);
		}

		public virtual void StopTask(IEnumerator t)
		{
			GameSystem.Instance.StopTask(t);
		}

		public bool CanAttack(Character targetCh)
		{
			return Team != targetCh.Team;
		}

		public bool CanAutoAttack(Character ch)
		{
			if (this is Monster)
			{
				//TODO add isAggressive params

				return CanAttack(ch);
			}

			return false;
		}
	}
}
