﻿// Copyright (c) 2015, Lukas Honke
// ========================
using System;
using System.Collections.Generic;
using Assets.scripts.Actor;
using Assets.scripts.Mono;
using Assets.scripts.Mono.ObjectData;
using Assets.scripts.Skills.Base;
using Assets.scripts.Skills.SkillEffects;
using Assets.scripts.Upgrade;
using UnityEngine;

namespace Assets.scripts.Skills
{
	/// <summary>
	/// Represents a skill that can either be passive or active
	/// </summary>
	public abstract class Skill
	{
		private string name;

		public bool IsLocked { get; set; }
		public bool AvailableToPlayer { get; set; }
		public bool AvailableToPlayerAsAutoattack { get; set; }
		public bool AvailableToDeveloper { get; set; }

		public int RequiredSlotLevel { get; set; }

		public Character Owner { get; private set; }

		public List<SkillTraits> Traits { get; private set; }

		public Sprite Icon { get; set; }

		protected List<SkillEffect> additionalEffects;
		protected bool originalEffectsDisabled = false;

		private int level;
		public int Level
		{
			get
			{
				return level;
			}
			set
			{
				if (level < 1 || level > MaxLevel)
					return;
				level = value;
			}
		}

		public int MaxLevel { get; set; }

		public abstract SkillId GetSkillId();

		public string GetName()
		{
			return name;
		}

		public abstract string GetVisibleName();

		public virtual string GetDescription()
		{
			return null;
		}

		public Skill()
		{
			// nastavit defaultni parametry
			MaxLevel = 1;

			Traits = new List<SkillTraits>();

			AvailableToPlayer = false;
			AvailableToPlayerAsAutoattack = false;

			RequiredSlotLevel = 1;
		}

		public void Init()
		{
			name = Enum.GetName(typeof (SkillId), GetSkillId());

			try
			{
				Icon = LoadResourceSprite("skill", GetName(), "Icon");
			}
			catch (Exception)
			{
				Icon = Resources.Load<Sprite>("Sprite/ui/icons/default_skill");
			}

			InitTraits();
			InitDynamicTraits();
		}

		private Sprite LoadResourceSprite(string type, string resourceFolderName, string fileName)
		{
			Sprite go = Resources.Load<Sprite>("Prefabs/" + type + "/" + resourceFolderName + "/" + fileName);

			if (go == null)
				throw new NullReferenceException("Prefabs/" + type + "/" + resourceFolderName + "/" + fileName + " !");

			return go;
		}

		public void InitIcon()
		{
			try
			{
				Icon = GetOwnerData().LoadResourceSprite("skill", GetName(), "Icon");
			}
			catch (Exception)
			{
				Icon = Resources.Load<Sprite>("Sprite/ui/icons/default_skill");
			}
		}

		public Skill AddTrait(SkillTraits t)
		{
			if(!Traits.Contains(t))
				Traits.Add(t);

			return this;
		}

		public bool HasTrait(SkillTraits t)
		{
			return Traits.Contains(t);
		}

		public void SetOwner(Character ch)
		{
			if (Owner != null)
			{
				Debug.LogError("Error : Skill ID " + GetName() + " uz majitele ma - " + Owner.Name);
				return;
			}

			Owner = ch;

			// upozorni skill ze byl prirazen k majiteli
			SkillAdded();
		}

		protected void ApplyEffect(Character source, GameObject target, SkillEffect ef, bool allowStackingSameEffect = false)
		{
			foreach (EquippableItem u in Owner.Inventory.ActiveUpgrades)
			{
				u.ModifySkillEffects(this, new SkillEffect[] { ef });
			}

			ef.Source = source;
			ef.SourceSkill = GetSkillId();
			ef.SourceSkillObject = this;

			if (!allowStackingSameEffect && !(ef is EffectDamage))
			{
				Character targetCh = target.GetChar();

				if (targetCh != null && targetCh.HasEffectAlready(ef))
				{
					SkillEffect oldEf = targetCh.GetCopyEffect(ef);

					if(oldEf != null)
						targetCh.RemoveEffect(oldEf);
					else return;
				}
			}

			ef.ApplyEffect(source, target);
		}

		/// <summary>
		/// Applies all SkillEffects of this skill to the target
		/// </summary>
		/// <param name="source">who casted the skill (usually the Owner of this skill)</param>
		/// <param name="target">who receives the effects</param>
		public void ApplyEffects(Character source, GameObject target, bool allowStackingSameEffect=false, int param=0)
		{
			// add new effect from templates // TOOD vytvorit kopii efektu! 
			if (additionalEffects != null)
			{
				foreach (SkillEffect ef in additionalEffects)
				{
					SkillEffect newEf = ef.Clone() as SkillEffect;

					newEf.Source = source;
					newEf.SourceSkill = GetSkillId();
					newEf.SourceSkillObject = this;

					if (!allowStackingSameEffect && !(newEf is EffectDamage))
					{
						Character targetCh = target.GetChar();

						if (targetCh != null && targetCh.HasEffectAlready(newEf))
						{
							SkillEffect oldEf = targetCh.GetCopyEffect(newEf);

							if (oldEf != null)
								targetCh.RemoveEffect(oldEf);
							else continue;
						}
					}

					newEf.ApplyEffect(source, target);
				}
			}

			SkillEffect[] efs = CreateEffects(param);

			// add new effects from ugprades
			foreach (EquippableItem u in Owner.Inventory.ActiveUpgrades)
			{
				SkillEffect[] newEffects = u.CreateAdditionalSkillEffects(this, efs);

				if (newEffects != null)
				{
					foreach (SkillEffect ef in newEffects)
					{
						ef.Source = source;
						ef.SourceSkill = GetSkillId();
						ef.SourceSkillObject = this;

						if (!allowStackingSameEffect && !(ef is EffectDamage))
						{
							Character targetCh = target.GetChar();
							if (targetCh != null && targetCh.HasEffectAlready(ef))
							{
								SkillEffect oldEf = targetCh.GetCopyEffect(ef);

								if (oldEf != null)
									targetCh.RemoveEffect(oldEf);
								else continue;
							}
						}

						ef.ApplyEffect(source, target);
					}
				}
			}

			foreach (EquippableItem u in Owner.Inventory.ActiveUpgrades)
			{
				u.ModifySkillEffects(this, efs);
			}

			if (efs != null && !originalEffectsDisabled)
			{
				foreach (SkillEffect ef in efs)
				{
					ef.Source = source;
					ef.SourceSkill = GetSkillId();
					ef.SourceSkillObject = this;

					if (!allowStackingSameEffect && !(ef is EffectDamage))
					{
						Character targetCh = target.GetChar();

						if (targetCh != null && targetCh.HasEffectAlready(ef))
						{
							SkillEffect oldEf = targetCh.GetCopyEffect(ef);

							if (oldEf != null)
								targetCh.RemoveEffect(oldEf);
							else continue;
						}
					}

					ef.ApplyEffect(source, target);
				}
			}
		}

		public void DisableOriginalEffects()
		{
			originalEffectsDisabled = true;
		}

		public void EnableOriginalEffects()
		{
			originalEffectsDisabled = false;
		}

		public void AddAdditionalEffect(SkillEffect e)
		{
			if (additionalEffects == null)
				additionalEffects = new List<SkillEffect>();

			additionalEffects.Add(e);
		}

		public AbstractData GetOwnerData()
		{
			return Owner.GetData();
		}

		/// <summary>
		/// works only if the owner is a Player
		/// </summary>
		public PlayerData GetPlayerData()
		{
			if (Owner is Player)
			{
				return ((Player) Owner).GetData();
			}

			return null;
		}

		public static Character GetCharacterFromObject(GameObject obj)
		{
			AbstractData data = obj.GetComponentInParent<AbstractData>();

			if (data == null)
				return null;

			Character targetCh = data.GetOwner();

			return targetCh;
		}

		// vytvori novou kopii sama sebe
		public abstract Skill Instantiate();
		public abstract SkillEffect[] CreateEffects(int param);
		public abstract void InitTraits();
		protected abstract void InitDynamicTraits();

		public abstract void SkillAdded();

		public abstract bool CanUse(bool beingCast=true);
		public abstract void SetReuseTimer();
		public abstract bool IsActive();
		public abstract bool IsBeingCasted();
		public abstract bool IsBeingConfirmed();
		public abstract void Start();
		public abstract void AbortCast();
		public abstract void End();

		public abstract string GetBaseInfo();

		public virtual void NotifyAnotherSkillCastStart(Skill sk)
		{
			
		}

		public virtual void NotifyAnotherSkillCastEnd(Skill sk)
		{

		}

		public virtual void NotifyEffectRemoved(SkillEffect eff)
		{
			
		}

		public virtual void NotifyCharacterDied()
		{
			
		}
	}
}
