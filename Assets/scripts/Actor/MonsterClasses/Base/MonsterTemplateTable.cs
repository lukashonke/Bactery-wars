using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Assets.scripts.Actor.PlayerClasses;
using Assets.scripts.Actor.PlayerClasses.Base;
using UnityEngine;

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
			MonsterTemplate t;
			List<Type> types = Utils.GetTypesInNamespace("Assets.scripts.Actor.MonsterClasses.Monsters", true, typeof(MonsterTemplate));

			foreach (Type type in types)
			{
				t = Activator.CreateInstance(type) as MonsterTemplate;
				AddType(t);
			}

			Debug.Log("Loaded " + types.Count + " monster classes.");

			types = Utils.GetTypesInNamespace("Assets.scripts.Actor.MonsterClasses.Boss", true, typeof(BossTemplate));

			foreach (Type type in types)
			{
				t = Activator.CreateInstance(type) as BossTemplate;
				AddType(t);
			}

			Debug.Log("Loaded " + types.Count + " boss classes.");
		}

		public void AddType(MonsterTemplate t)
		{
			try
			{
				types.Add(t.GetMonsterId(), t);
			}
			catch (Exception)
			{
				Debug.LogError("type " + t.GetMonsterId() + " already exists! ( " + t.GetType().Name + ")");
			}
		}

		public MonsterTemplate GetType(MonsterId type)
		{
			MonsterTemplate tObject;
            types.TryGetValue(type, out tObject);
			return tObject;
		}
	}
}
