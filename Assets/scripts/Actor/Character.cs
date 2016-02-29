using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using Assets.scripts.Actor.MonsterClasses;
using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.Actor.Status;
using Assets.scripts.AI;
using Assets.scripts.Base;
using Assets.scripts.Mono;
using Assets.scripts.Mono.MapGenerator;
using Assets.scripts.Skills;
using Assets.scripts.Skills.Base;
using Assets.scripts.Skills.SkillEffects;
using Assets.scripts.Upgrade;
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

		public Inventory Inventory { get; private set; }

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
			OnLevelChange();
		}

		public virtual void OnLevelChange()
		{
			
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

	    public virtual void DoDie(Character killer=null, SkillId skillId=0)
	    {
		    if (killer != null)
		    {
				killer.OnKill(this, skillId);
		    }

		    OnDead(killer, skillId);

		    if (this is Monster)
		    {
			    MonsterTemplate mt = ((Monster) this).Template;
				mt.OnDie((Monster) this);
		    }

	        GetData().SetIsDead(true);
            WorldHolder.instance.activeMap.NotifyCharacterDied(this);
            DeleteMe();
	    }

		public virtual void OnKill(Character target, SkillId skillId)
		{
			foreach (AbstractUpgrade u in Inventory.ActiveUpgrades)
			{
				u.OnKill(target, skillId);
			}
		}

		public virtual void OnDead(Character killer, SkillId skillId)
		{
			//TODO perhaps add onDead on upgrades
		}

		public void DeleteMe()
		{
			if(AI != null)
			AI.StopAITask();

			if(Knownlist != null)
			Knownlist.Active = false;

			if(wallCheck != null)
				StopTask(wallCheck);

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
			Inventory = new Inventory(this, 10, 3);

			Knownlist.StartUpdating();

			if (AI == null)
				AI = InitAI();

			AI.StartAITask();

			CheckWalls();

			wallCheck = StartTask(WallCheckTask());
		}

		private Coroutine wallCheck;

		private IEnumerator WallCheckTask()
		{
			while (GetData().GetBody() != null)
			{
				if (Utils.IsNotAccessible(GetData().GetBody().transform.position))
				{
					if(this is Monster)
						DoDie();
					else if (this is Player)
					{
						if (!CheckWalls())
						{
							Message("You have been teleported at the start because you got stuck in the walls!");
							GetData().transform.position = WorldHolder.instance.GetStartPosition();
						}
					}
				}

				yield return new WaitForSeconds(5f);
			}
		}

		public bool CheckWalls()
		{
			int iteration = 0;
			if (Utils.IsNotAccessible(GetData().GetBody().transform.position))
			{
				//Debug.LogError("im in walls!, teleporting away");

				int minRange = 2;
				int maxRange = 4;
				int limit = 12;
				int mainLimit = 7;
				Vector3 currentPos = GetData().GetBody().transform.position;
				Vector3 newPos = currentPos;
				bool set = false;

				while (!set)
				{
					iteration++;
					if (iteration > 1000)
					{
						return true;
					}

					float randX = Random.Range(minRange, maxRange);
					float randY = Random.Range(minRange, maxRange);

					if (Random.Range(0, 2) == 0)
						randX *= -1;

					if (Random.Range(0, 2) == 0)
						randY *= -1;

					Vector3 v = new Vector3(currentPos.x + randX, currentPos.y + randY, 0);

					if (Utils.IsNotAccessible(v))
					{
						limit--;
						if (limit <= 0)
						{
							mainLimit--;

							limit = 12;

							if (mainLimit <= 0)
								break;

							maxRange = (int)(maxRange + maxRange);
						}

						continue;
					}

					newPos = v;
					set = true;
				}

				if (set)
				{
					Debug.DrawLine(currentPos, newPos, Color.blue, 10f);
					GetData().GetBody().transform.position = newPos;
					return true;
				}
				else
				{
					Debug.LogError("couldnt get character " + Name + " away from walls");
					return false;
				}
			}

			return true;
		}

		public AbstractData GetData()
		{
			return Data;
		}

		public void HitUpgrade(UpgradeScript upg)
		{
			if (upg.upgrade.CollectableByPlayer && !(this is Player))
				return;

			if (upg.upgrade == null)
			{
				Debug.LogError("null upgrade for " + upg.gameObject.name);
				return;
			}

			if (upg.upgrade.OnPickup(this))
			{
				upg.DeleteMe(true);
				Data.UpdateInventory(Inventory);
				UpdateStats();
				return;
			}

			if (upg.upgrade.GoesIntoBasestatSlot)
			{
				Message("You have absorbed " + upg.upgrade.VisibleName + "");
				Inventory.AddBasestatUpgrade(upg.upgrade);
				upg.DeleteMe(true);
				Data.UpdateInventory(Inventory);
				UpdateStats();
			}
			else if (AddUpgrade(upg.upgrade))
			{
				upg.DeleteMe(true);
				//EquipUpgrade(upg.upgrade);
			}
		}

		public bool AddUpgrade(AbstractUpgrade u)
		{
			if (Inventory.CanAdd(u))
			{
				u.SetOwner(this);
				Inventory.AddUpgrade(u);
				Data.UpdateInventory(Inventory);
				Message("You have picked up: " + u.VisibleName + "");
				return true;
			}

			return false;
		}

		public void RemoveUpgrade(AbstractUpgrade u)
		{
			u.SetOwner(null);
			Inventory.RemoveUpgrade(u);
			Data.UpdateInventory(Inventory);
			Message("Deleted " + u.VisibleName);
		}

		public void EquipUpgrade(AbstractUpgrade u)
		{
			if (!Inventory.EquipUpgrade(u))
				return;

			UpdateStats();

			Data.UpdateInventory(Inventory);
			Message("Equiped " + u.VisibleName);
		}

		public void UnequipUpgrade(AbstractUpgrade u, bool force=false)
		{
			Inventory.UnequipUpgrade(u, force);
			UpdateStats();

			Data.UpdateInventory(Inventory);
			Message("Unequiped " + u.VisibleName);
		}

		public void SwapUpgrade(AbstractUpgrade source, AbstractUpgrade target, int slot, int fromSlot, int toSlot)
		{
			Inventory.MoveUpgrade(source, fromSlot, toSlot, slot, target);
			UpdateStats();
			Data.UpdateInventory(Inventory);
		}

		public virtual void UpdateStats()
		{
			//TODO add support for this to monsters
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

		public void ProlongeEffectDuration(SkillEffect ef)
		{
			
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

		public bool HasEffectOfSkill(SkillId sk)
		{
			foreach (SkillEffect e in ActiveEffects)
			{
				if (e.SourceSkill == sk)
					return true;
			}

			return false;
		}

		public bool HasEffectAlready(Type efType)
		{
			foreach (SkillEffect e in ActiveEffects)
			{
				if (efType.Name.Equals(e.GetType().Name))
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
				Message("Skill is not yet available for use.", 2);
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

		public int CalculateDamage(int baseDamage, Character target, bool canCrit, out bool wasCrit)
		{
			baseDamage = (int) (baseDamage * Status.DamageOutputMul);
			baseDamage = (int) (baseDamage + Status.DamageOutputAdd);

			bool crit = canCrit && Random.Range(1, 1000) <= Status.CriticalRate;

			if (crit)
			{
				baseDamage = (int) (baseDamage*Status.CriticalDamageMul);
			}

			wasCrit = crit;

			return baseDamage;
		}

		public void SetIsWalking(bool walking)
		{
			Status.IsWalking = walking;
		}

		public void SetMoveSpeed(float speed)
		{
			Status.MoveSpeed = speed;
			GetData().SetMoveSpeed(speed);
		}

		public void SetShield(float shield)
		{
			Status.Shield = shield;
		}

		public void ReceiveHeal(Character source, int ammount, SkillId skillId = 0)
		{
			if (Status.IsDead)
				return;

			Status.ReceiveHeal(ammount);

			GetData().SetVisibleHp(Status.Hp);
		}

		public void ReceiveDamage(Character source, int damage, SkillId skillId=0, bool wasCrit=false)
		{
			if (Status.IsDead || damage <= 0)
				return;

			if (this is Player)
			{
				((Player)this).GetData().ui.DamageMessage(GetData().GetBody(), damage, Color.red);
			}

			Status.ReceiveDamage(damage);

			if (source != null)
			{
				source.OnGiveDamage(this, damage, skillId, wasCrit);
			}

			if (Status.IsDead)
			{
				DoDie(source, skillId);
			}

			GetData().SetVisibleHp(Status.Hp);

			AI.AddAggro(source, damage);
		}

		public void OnGiveDamage(Character target, int damage, SkillId skillId = 0, bool wasCrit=false)
		{
			if (this is Player)
			{
				if (wasCrit)
					((Player)this).GetData().ui.ObjectMessage(target.GetData().GetBody(), "*crit* " + damage, Color.green);
				else
					((Player)this).GetData().ui.DamageMessage(target.GetData().GetBody(), damage, Color.green);
			}

			foreach (AbstractUpgrade u in Inventory.ActiveUpgrades)
			{
				u.OnGiveDamage(target, damage, skillId);
			}
		}

		public void UpdateHp(int newHp)
		{
			Status.SetHp(newHp);
			GetData().SetVisibleHp(Status.Hp);
		}

		public void UpdateMp(int newMp)
		{
			Status.SetMp(newMp);
		}

		public void UpdateLevel(int newLevel)
		{
			Level = newLevel;
			GetData().SetVisibleLevel(Level);
		}

		public void HealMe()
		{
			UpdateHp(Status.MaxHp);
		}

		public void UpdateMaxHp(int newMaxHp)
		{
			Status.MaxHp = newMaxHp;
			GetData().SetVisibleMaxHp(Status.MaxHp);
		}

		public void UpdateMaxMp(int newMaxMp)
		{
			Status.MaxMp = newMaxMp;
			//TODO client side? 
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

		public bool CanAttack(Destroyable destr)
		{
			GameObject owner = destr.owner;

			if (owner == null)
				return true;

			Character ch = owner.GetChar();
			if (ch == null)
				return true;

			return CanAttack(ch);
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

		public void AddXp(int xp)
		{
			Status.AddXp(xp);

			if (Status.XP >= XpTable.GetXpForLevel(Level + 1))
			{
				LevelUp();
			}
		}

		private void LevelUp()
		{
			Status.XP -= XpTable.GetXpForLevel(Level + 1);
			SetLevel(Level + 1);
			Message("Level up! You are now level " + Level);
		}

		public virtual void Message(string s, int level=1)
		{
			Debug.Log("Message: " + s);
		}
	}
}
