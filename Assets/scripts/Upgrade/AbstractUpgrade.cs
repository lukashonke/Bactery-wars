using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
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

		public bool CollectableByPlayer { get; protected set; }

		public int Level { get; set; }
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

			VisibleName = Name;
			Description = "No Description";
			Price = "No value";
			AdditionalInfo = "some info";
			Type = UpgradeType.CLASSIC;
		}

		public virtual void Init()
		{
			try
			{
				MainSprite = LoadSprite("lvl" + Level + ".png");
			}
			catch (Exception)
			{
				try
				{
					MainSprite = LoadSprite("lvl1.png");
				}
				catch (Exception)
				{
					MainSprite = Resources.Load<Sprite>("Sprite/Upgrades/default");
				}
			}
		}

		public void SetOwner(Character ch)
		{
			owner = ch;
		}

		public GameObject SpawnGameObject(Vector3 pos)
		{
			GameObject o = new GameObject(VisibleName + " " + Level);
			SpriteRenderer r = o.AddComponent<SpriteRenderer>();
			r.sprite = MainSprite;
			o.AddComponent<BoxCollider2D>().isTrigger = true;
			o.AddComponent<UpgradeScript>().upgrade = this;
			o.transform.position = pos;
			return o;
		}

		protected Sprite LoadSprite(string fileName)
		{
			Sprite o = Resources.Load<Sprite>("Sprite/Upgrades/" + Name + "/" + fileName);
			if(o == null)
				throw new NullReferenceException(fileName + " not found ");

			return o;
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

		public virtual void ModifySkillEffects(Skill sk, SkillEffect[] effects)
		{
			if (!(sk is ActiveSkill))
				return;

			ActiveSkill skill = (ActiveSkill) sk;
		}

		public virtual void ModifyRunSpeed(ref float runSpeed)
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
	}
}
