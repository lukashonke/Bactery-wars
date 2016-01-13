using System;
using System.Collections.Generic;
using Assets.scripts.Actor;
using Assets.scripts.Mono;
using Assets.scripts.Mono.ObjectData;
using Assets.scripts.Skills.Base;
using Assets.scripts.Skills.SkillEffects;
using UnityEngine;

namespace Assets.scripts.Skills
{
	/*
		Sablona pro skill
	*/
	public abstract class Skill
	{
		private string name;

		public Character Owner { get; private set; }

		public List<SkillTraits> Traits { get; private set; }

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

		public Skill()
		{
			// nastavit defaultni parametry
			MaxLevel = 1;

			Traits = new List<SkillTraits>();
		}

		public void Init()
		{
			name = Enum.GetName(typeof (SkillId), GetSkillId());

			InitTraits();
			InitDynamicTraits();
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

		/// <summary>
		/// Applies all SkillEffects of this skill to the target
		/// </summary>
		/// <param name="source">who casted the skill (usually the Owner of this skill)</param>
		/// <param name="target">who receives the effects</param>
		protected void ApplyEffects(Character source, GameObject target, bool allowStackingSameEffect=false)
		{
			SkillEffect[] efs = CreateEffects();

			if (efs != null)
			{
				foreach (SkillEffect ef in efs)
				{
					ef.Source = source;

					if (!allowStackingSameEffect && !(ef is EffectDamage))
					{
						Character targetCh = target.GetChar();

						if (targetCh != null && targetCh.HasEffectAlready(ef))
						{
							Debug.Log(" ** not applying again " + ef.GetType().Name);
							continue;
						}
					}

					Debug.Log("applying " + ef.GetType().Name);

					ef.ApplyEffect(source, target);
				}
			}
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
		public abstract SkillEffect[] CreateEffects();
		public abstract void InitTraits();
		protected abstract void InitDynamicTraits();

		public abstract void SkillAdded();

		public abstract bool CanUse();
		public abstract void SetReuseTimer();
		public abstract bool IsActive();
		public abstract bool IsBeingCasted();
		public abstract bool IsBeingConfirmed();
		public abstract void Start();
		public abstract void AbortCast();
		public abstract void End();
	}
}
