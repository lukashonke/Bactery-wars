using System;
using Assets.scripts.Actor;
using Assets.scripts.Mono;
using UnityEngine;

namespace Assets.scripts.Skills
{
	/*
		Sablona pro skill
	*/
	public abstract class Skill
	{
		public int Id { get; private set; }
		public string Name { get; private set; }

		public Character Owner { get; private set; }

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

		public Skill(string name, int id)
		{
			Id = id;
			Name = name;

			// nastavit defaultni parametry
			MaxLevel = 1;
		}

		public void SetOwner(Character ch)
		{
			if (Owner != null)
			{
				Debug.LogError("Error : Skill ID " + Id + " uz majitele ma - " + Owner.Name);
				return;
			}

			Owner = ch;

			// upozorni skill ze byl prirazen k majiteli
			SkillAdded();
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

		// vytvori novou kopii sama sebe
		public abstract Skill Instantiate();

		public abstract void SkillAdded();

		public abstract bool CanUse();
		public abstract void SetReuseTimer();
		public abstract bool IsActive();
		public abstract bool IsBeingCasted();
		public abstract void Start();
		public abstract void AbortCast();
		public abstract void End();
	}
}
