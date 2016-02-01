using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.Actor.Status;
using Assets.scripts.AI;
using Assets.scripts.Base;
using Assets.scripts.Mono;
using Assets.scripts.Mono.MapGenerator;
using Assets.scripts.Skills;
using Assets.scripts.Skills.Base;
using Assets.scripts.Skills.SkillEffects;
using UnityEngine;
using Random = UnityEngine.Random;

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

		public List<Monster> summons; 

		public List<SkillEffect> ActiveEffects { get; private set; }
		private Coroutine effectUpdateTask;

		public AbstractData Data { get; set; }

		public int Level = 1;
		public int Team { get; set; }

		public void SetLevel(int level)
		{
			Level = level;
			Data.SetVisibleLevel(level);
		}

		protected Character(string name) : base(name)
		{
			Level = 1;
		}

		protected Character(string name, AbstractAI ai) : base(name)
		{
			Level = 1;
			AI = ai;
		}

	    public void DoDie()
	    {
	        GetData().SetIsDead(true);
            WorldHolder.instance.activeMap.NotifyCharacterDied(this);
            DeleteMe();
	    }

		public void DeleteMe()
		{
			if(AI != null)
			AI.StopAITask();

			if(Knownlist != null)
			Knownlist.Active = false;

			try
			{
				foreach (Skill sk in Skills.Skills)
				{
					if (sk is ActiveSkill)
					{
						if (((ActiveSkill)sk).IsActive() || ((ActiveSkill)sk).IsBeingCasted())
						{
							((ActiveSkill)sk).AbortCast();
						}
					}
				}
			}
			catch (Exception)
			{
			}

			if (this is Monster)
			{
				if (((Monster) this).GetMaster() != null)
				{
					((Monster) this).GetMaster().RemoveSummon((Monster) this);
				}
			}

			RemoveAllSummons();
		}

		/// <summary>
		/// zavolano hned po vytvoreni Characteru (hned po zavolani konstruktorů)
		/// </summary>
		public void Init()
		{
			Knownlist = new Knownlist(this);
			ActiveEffects = new List<SkillEffect>();
			Status = InitStatus();
			Skills = InitSkillSet();
			summons = new List<Monster>();

			Knownlist.StartUpdating();

			if (AI == null)
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

		public void AddEffect(SkillEffect ef, float duration)
		{
			if(ActiveEffects.Count > 5)
				return;

			ActiveEffects.Add(ef);

			if(duration > 0)
				StartTask(CancelEffect(ef, duration)); // cancel this effect in 'duration'

			StartEffectUpdate();
		}

		public bool HasEffectAlready(SkillEffect ef)
		{
			if (ef.Source == null)
				return false;

			foreach (SkillEffect e in ActiveEffects)
			{
				if (ef.GetType().Name.Equals(e.GetType().Name) && ef.Source.Equals(e.Source))
					return true;
			}

			return false;
		}

		public void RemoveEffect(SkillEffect ef)
		{
			ef.OnRemove();
			ActiveEffects.Remove(ef);

			if (ActiveEffects.Count == 0)
			{
				StopEffectUpdate();
			}
		}

		private void StartEffectUpdate()
		{
			if (effectUpdateTask == null)
			{
				effectUpdateTask = StartTask(UpdateEffects());
			}
		}

		private void StopEffectUpdate()
		{
			if (effectUpdateTask != null)
			{
				StopTask(effectUpdateTask);
				effectUpdateTask = null;
			}
		}

		private IEnumerator UpdateEffects()
		{
			while (ActiveEffects.Count > 0)
			{
				float currentTime = Time.time;

				for(int i = 0; i < ActiveEffects.Count; i++)
				{
					SkillEffect ef = ActiveEffects[i];

					if (ef == null || ef.period <= 0)
						continue;

					if (ef.lastUpdateTime + ef.period <= currentTime)
					{
						// effekt ma urcity pocet opakovani, pote se zrusi
						if (ef.count != -1)
						{
							if (ef.count > 0)
							{
								ef.count --;
								ef.Update();
								ef.lastUpdateTime = currentTime;
							}
							else
							{
								RemoveEffect(ef);
								break;
							}
						}
						else // opakuje se dokud nevyprsi duration
						{
							ef.Update();
							ef.lastUpdateTime = currentTime;
						}
					}
				}

				yield return new WaitForSeconds(0.5f);
			}
		}

		private IEnumerator CancelEffect(SkillEffect ef, float duration)
		{
			yield return new WaitForSeconds(duration);
			if(ef != null)
				RemoveEffect(ef);
		}

		public bool CanCastSkill(Skill skill)
		{
			if (Status.IsDead || Status.IsStunned()) 
				return false;

			if (skill is ActiveSkill)
			{
				ActiveSkill s = (ActiveSkill)skill;

				foreach (Skill sk in Skills.Skills)
				{
					if (sk is ActiveSkill && ((ActiveSkill)sk).IsActive()) //TODO check for can be casted simultaneously
					{
						if (!s.canBeCastSimultaneously)
						{
							return false;
						}
					}
				}
			}

			return true;
		}

		/// <summary>
		/// Spusti kouzleni skillu
		/// </summary>
		/// <param name="skill"></param>
		public void CastSkill(Skill skill)
		{
			if (!CanCastSkill(skill))
				return;

			// skill is passive - cant cast it
			if (skill is PassiveSkill)
				return;

			// reuse check
			if (!skill.CanUse())
			{
				Debug.Log("skill cannot be used again yet");
				return;
			}

			if(skill is ActiveSkill)
				((ActiveSkill)skill).Start(GetData().Target);
			else
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

				if (sk.IsBeingCasted() || sk.IsActive())
					sk.AbortCast();
			}

			Debug.Log("break done");
		}

		public int CalculateDamage(int baseDamage, Character target, bool canCrit)
		{
			bool crit = canCrit && Random.Range(1, 1000) <= Status.CriticalRate;

			if (crit)
			{
				baseDamage = (int) (baseDamage*Status.CriticalDamageMul);
			}

			return baseDamage;
		}

		public void SetMoveSpeed(int speed)
		{
			Status.MoveSpeed = speed;
			GetData().SetMoveSpeed(speed);
		}

		public void ReceiveDamage(Character source, int damage)
		{
			if (Status.IsDead)
				return;

			Status.ReceiveDamage(damage);

			if (Status.IsDead)
			{
				DoDie();
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
			if (targetCh.IsInteractable())
				return false;

			return Team != targetCh.Team;
		}

		public bool CanAutoAttack(Character ch)
		{
			return CanAttack(ch);
		}

		public void OnAttack(Character target)
		{
			if (HasSummons())
			{
				foreach (Monster summon in summons)
				{
					if (summon.Status.IsDead)
						continue;

					summon.MasterAttacked(target);
				}
			}	

			target.OnAttacked(this);		
		}

		public void OnAttacked(Character attacker)
		{
			if (HasSummons())
			{
				foreach (Monster summon in summons)
				{
					if (summon.Status.IsDead)
						continue;

					summon.MasterIsAttacked(attacker);
				}
			}
		}

		public void ForceStopAttack()
		{
			if (HasSummons())
			{
				foreach (Monster summon in summons)
				{
					if (summon.Status.IsDead)
						continue;

					summon.MasterForcedStopAttack();
				}
			}
		}

		public void ChangeAI(AbstractAI ai)
		{
			if (AI != null)
			{
				AI.StopAITask();
			}

			AI = ai;
			AI.StartAITask();
		}

		public virtual bool IsInteractable()
		{
			return true;
		}

		private void RemoveAllSummons()
		{
			for (int i = 0; i < summons.Count; i++)
			{
				Monster m = summons[i];

				if (m != null)
				{
					m.DoDie();
				}
			}
		}

		public void AddSummon(Monster m)
		{
			summons.Add(m);
			m.SetMaster(this);
		}

		public void RemoveSummon(Monster m)
		{
			summons.Remove(m);
		}

		public bool HasSummons()
		{
			return summons.Count > 0;
		}
	}
}
