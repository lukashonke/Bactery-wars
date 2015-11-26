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
		private Dictionary<int, Skill> skills;
		
		public SkillTable()
		{
			skills = new Dictionary<int, Skill>();
			Load();
		}

		public Skill GetSkill(int id)
		{
			Skill sk;
			if (!skills.TryGetValue(id, out sk))
				Debug.LogWarning("Could not find skill ID " + id);

			return sk;
		}

		public Skill CreateSkill(int id)
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
			skills.Add(sk.Id, sk);
		}

		private void RemoveSkill(Skill sk)
		{
			skills.Remove(sk.Id);
		}

		// Load all skills ingame
		// TODO unhardcode: dynamically load all Skill derivates from a folder
		private void Load()
		{
			Skill skill;

			skill = new SkillTemplate("Skill Template", 1);
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
		}
	}
}
