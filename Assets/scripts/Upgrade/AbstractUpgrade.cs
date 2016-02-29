using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Actor.PlayerClasses.Base;
using Assets.scripts.Mono;
using Assets.scripts.Skills;
using Assets.scripts.Skills.ActiveSkills;
using Assets.scripts.Skills.Base;
using Assets.scripts.Skills.SkillEffects;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.scripts.Upgrade
{
	/// <summary>
	/// allows dynamically changing of following things:
	/// - max hp, max mp, run speed, critical rate, critical dmg
	/// - 
	/// </summary>
	public abstract class AbstractUpgrade
	{
		public string Name { get; protected set; }
		public string VisibleName { get; protected set; }
		public string Description { get; protected set; }
		public string Price { get; protected set; }
		public string AdditionalInfo { get; protected set; }
		public UpgradeType Type { get; protected set; }
		public ClassId RequiredClass { get; protected set; }

		public int CurrentProgress { get; set; }
		public int NeedForNextLevel { get; set; }

		public bool GoesIntoBasestatSlot { get; protected set; }

		public bool CollectableByPlayer { get; protected set; }

		public int Level { get; set; }
		public int MaxLevel { get; set; }
		private Character owner;
		public Character Owner
		{
			get { return owner; }
		}

		public Sprite MainSprite { get; protected set; }

		public AbstractUpgrade(int level, bool collectableByPlayer=true)
		{
			CollectableByPlayer = collectableByPlayer;
			Level = level;

			GoesIntoBasestatSlot = false;

			CurrentProgress = 0;
			NeedForNextLevel = 1;
			MaxLevel = 10;
			RequiredClass = 0; //TODO restrict drops only for the class that player currently has

			VisibleName = Name;
			Description = "No Description";
			Price = "No value";
			AdditionalInfo = null;
			Type = UpgradeType.CLASSIC;
		}

		public virtual AbstractUpgrade Init()
		{
			InitInfo();
			try
			{
				MainSprite = LoadSprite("lvl" + Level);
			}
			catch (Exception e)
			{
				//Debug.LogError(e.StackTrace);
				try
				{
					MainSprite = LoadSprite("lvl1");
				}
				catch (Exception)
				{
					//Debug.LogError(e.StackTrace);
					MainSprite = Resources.Load<Sprite>("Sprite/Upgrades/default");
				}
			}

			return this;
		}

		public void AddUpgradeProgress(AbstractUpgrade u)
		{
			if (Level == MaxLevel)
				return;

			CurrentProgress++;
			if (CurrentProgress >= NeedForNextLevel)
			{
				Remove();
				Level ++;
				Init();
				Apply();

				CurrentProgress = 0;

				NeedForNextLevel = (int) Math.Pow(2, Level);
			}
		}

		public AbstractUpgrade SetOwner(Character ch)
		{
			owner = ch;
			return this;
		}

		public GameObject SpawnGameObject(Vector3 pos)
		{
			GameObject o = new GameObject(VisibleName + " " + Level);
			SpriteRenderer r = o.AddComponent<SpriteRenderer>();
			r.sprite = MainSprite;
			o.AddComponent<BoxCollider2D>().isTrigger = true;
			o.AddComponent<UpgradeScript>().upgrade = this;
			o.transform.position = pos;

			GameObject bg = new GameObject("Background");
			r = bg.AddComponent<SpriteRenderer>();
			r.sprite = UpgradeTable.Instance.dropBg;
			r.sortingOrder = -1;
			bg.transform.parent = o.transform;
			bg.transform.localPosition = new Vector3(0, 0, 0);

			return o;
		}

		protected Sprite LoadSprite(string fileName)
		{
			Sprite o = Resources.Load<Sprite>("Sprite/Upgrades/" + Name + "/" + fileName);
			if(o == null)
				throw new NullReferenceException(fileName + " not found ");

			return o;
		}

		public float MulValueByLevel(int baseDamage, float levelMultiplier)
		{
			int add = (int) (baseDamage*(Level - 1)*levelMultiplier - baseDamage);
			if (add < 0)
				add = 0;
			return baseDamage + add;
		}

		/*public float AddValueByLevel(int baseDamage, float levelMultiplier, bool alsoNegative=false)
		{
			int add = (int)((Level - 1) * levelMultiplier);
			if (add < 0 && !alsoNegative)
				add = 0;
			return (baseDamage + add);
		}*/

		public float AddValueByLevel(float baseDamage, float levelMultiplier, bool alsoNegative=false)
		{
			float add = (Level - 1)*levelMultiplier;
			if (add < 0 && !alsoNegative)
				add = 0;
			return (baseDamage + add);
		}

		public void Apply()
		{
			ApplySkillChanges(Owner.Skills, Owner.MeleeSkill);
		}

		public void Remove()
		{
			RestoreSkillChanges(Owner.Skills, Owner.MeleeSkill);
		}

		public virtual void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
		}

		public virtual void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
		}

		public virtual SkillEffect[] CreateAdditionalSkillEffects(Skill sk, SkillEffect[] effects)
		{
			if (!(sk is ActiveSkill))
				return null;

			return null;
		}


		public virtual bool OnPickup(Character owner)
		{
			return false;
		}

		public virtual void OnKill(Character target, SkillId skillId)
		{
			
		}

		public virtual void OnGiveDamage(Character target, int damage, SkillId skillId)
		{
			
		}

		public virtual void ModifySkillEffects(Skill sk, SkillEffect[] effects)
		{
			if (!(sk is ActiveSkill))
				return;

			ActiveSkill skill = (ActiveSkill) sk;
		}

		public virtual void ModifyRunSpeed(ref float runSpeed)
		{
			
		}

		public virtual void ModifyDmgMul(ref float dmgMul)
		{
			
		}

		public virtual void ModifyDmgAdd(ref float dmgAdd)
		{

		}

		public virtual void ModifyMaxHp(ref int maxHp)
		{
			
		}

		public virtual void ModifyMaxMp(ref int maxMp)
		{
			
		}

		public virtual void ModifyCriticalRate(ref int critRate)
		{
			
		}

		public virtual void ModifyCriticalDmg(ref float critDmg)
		{
			
		}

		public virtual void ModifySkillCooldown(ActiveSkill sk, ref float cooldown)
		{
			
		}

		public virtual void ModifySkillReuse(ActiveSkill sk, ref float reuse)
		{

		}

		public virtual void ModifySkillCasttime(ActiveSkill sk, ref float casttime)
		{

		}

		public virtual void ModifyShield(ref float shield)
		{

		}

		protected abstract void InitInfo();

	}
}
