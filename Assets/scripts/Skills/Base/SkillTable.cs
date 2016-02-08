using System;
using System.Collections.Generic;
using Assets.scripts.Skills.ActiveSkills;
using UnityEngine;

namespace Assets.scripts.Skills.Base
{
	/*
		Trida slouzi k ulozeni seznamu skillu a jejich vytvareni pro hrace
	*/
	public class SkillTable
	{
		// singleton
		private static SkillTable instance = null;
		public static SkillTable Instance
		{
			get
			{
				if (instance == null)
					instance = new SkillTable();

				return instance;
			}
		}

		// seznam skillu ve hre podle ID
		private Dictionary<SkillId, Skill> skills;
		
		public SkillTable()
		{
			skills = new Dictionary<SkillId, Skill>();
			Load();
		}

		public Skill GetSkill(SkillId id)
		{
			Skill sk;
			if (!skills.TryGetValue(id, out sk))
				Debug.LogWarning("Could not find skill ID " + Enum.GetName(typeof(SkillId), id));

			return sk;
		}

		public Skill CreateSkill(SkillId id)
		{
			Skill sk = GetSkill(id);

			// create a new copy of a skill
			// this is important!
			Skill newSkill = sk.Instantiate();
			newSkill.Init();

			return newSkill;
		}

		private void AddSkill(Skill sk)
		{
			skills.Add(sk.GetSkillId(), sk);
		}

		private void RemoveSkill(Skill sk)
		{
			skills.Remove(sk.GetSkillId());
		}

		// Load all skills ingame
		// TODO unhardcode: dynamically load all Skill derivates from a folder
		private void Load()
		{
			Skill skill;

			List<Type> types = Utils.GetTypesInNamespace("Assets.scripts.Skills.ActiveSkills", true, typeof(ActiveSkill));
			foreach (Type type in types)
			{
				skill = Activator.CreateInstance(type) as Skill;
				AddSkill(skill);
			}

			Debug.Log("Loaded " + types.Count + " active skills");

			types = Utils.GetTypesInNamespace("Assets.scripts.Skills.PassiveSkills", true, typeof(PassiveSkill));
			foreach (Type type in types)
			{
				skill = Activator.CreateInstance(type) as Skill;
				AddSkill(skill);
			}

			Debug.Log("Loaded " + types.Count + " passive skills");

			/*skill = new SkillTemplate("Skill Template", 1);
			AddSkill(skill);

			skill = new SkillTestProjectile("Test Projectile", 2);
			AddSkill(skill);

			skill = new SkillTestProjectileTriple("Test Projectile Triple", 3);
			AddSkill(skill);

			skill = new SkillTestProjectileAllAround("Test Projectile Aura", 4);
			AddSkill(skill);

			skill = new JumpShort("Short Jump", 5);
			AddSkill(skill);

			skill = new ChainSkill("Chain Skill", 6);
			AddSkill(skill);

			skill = new SkillAreaExplode("Bomb Skill", 7);
			AddSkill(skill);

			skill = new MissileProjectile("Missile Projectile", 8);
			AddSkill(skill);

			skill = new ChainedProjectile("Chained Projectile", 9);
			AddSkill(skill);

			skill = new MeleeAttack("Melee Attack", 10);
			AddSkill(skill);*/
		}
	}
}
