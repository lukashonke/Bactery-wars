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
			return Skills[order];
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
	}
}
