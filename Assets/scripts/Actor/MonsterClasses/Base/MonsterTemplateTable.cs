using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor.PlayerClasses;
using Assets.scripts.Actor.PlayerClasses.Base;

namespace Assets.scripts.Actor.MonsterClasses.Base
{
	public class MonsterTemplateTable
	{
		// singleton
		private static MonsterTemplateTable instance = null;
		public static MonsterTemplateTable Instance
		{
			get
			{
				if (instance == null)
					instance = new MonsterTemplateTable();

				return instance;
			}
		}

		private Dictionary<MonsterId, MonsterTemplate> types;

		public MonsterTemplateTable()
		{
			types = new Dictionary<MonsterId, MonsterTemplate>();

			Init();
		}

		// Initialize all possible classes here
		private void Init()
		{
			AddType(new WhiteCellTemplate(MonsterId.TestMonster));
			AddType(new LeukocyteMelee(MonsterId.Leukocyte_melee));
			AddType(new LeukocyteRanged(MonsterId.Leukocyte_ranged));
		}

		public void AddType(MonsterTemplate t)
		{
			types.Add(t.MonsterId, t);
		}

		public MonsterTemplate GetType(MonsterId type)
		{
			MonsterTemplate tObject;
            types.TryGetValue(type, out tObject);
			return tObject;
		}
	}
}
