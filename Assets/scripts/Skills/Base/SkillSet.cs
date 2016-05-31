using System;
using System.Collections.Generic;

namespace Assets.scripts.Skills.Base
{
	/*
		Trida slouzi k ulozeni setu skillů jednoho Characteru (hrac nebo jiny objekt)
		Obsahuje informace o pouzivani (posledni pouziti, cas ktery zbyva do dalsiho mozneho pouziti daneho skillu, atd.)
	*/
	public class SkillSet
	{
		public List<Skill> Skills { get; private set; }

		public SkillSet()
		{
			Skills = new List<Skill>();
		}

		public void AddSkill(Skill sk)
		{
			Skills.Add(sk);
		}

		public void RemoveSkill(Skill sk)
		{
			Skills.Remove(sk);
		}

		public Skill GetSkill(int order)
		{
			if(order < Skills.Count)
				return Skills[order];
			return null;
		}

		public Skill GetSkill(SkillId id)
		{
			foreach (Skill s in Skills)
			{
				if (s.GetSkillId() == id)
					return s;
			}

			return null;
		}

		public int GetSkillSlot(string name)
		{
			int i = 0;
			foreach (Skill s in Skills)
			{
				if (s.GetName().Equals(name, StringComparison.InvariantCultureIgnoreCase))
					return i;
				i++;
			}
			return 0;
		}

		public bool HasSkill(SkillId id)
		{
			foreach (Skill s in Skills)
			{
				if (s.GetSkillId() == id)
					return true;
			}

			return false;
		}
	}
}
