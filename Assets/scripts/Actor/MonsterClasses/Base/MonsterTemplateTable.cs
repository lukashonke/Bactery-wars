using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
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

		private List<MonsterTemplate> customTypes;  

		public MonsterTemplateTable()
		{
			types = new Dictionary<MonsterId, MonsterTemplate>();

			customTypes = new List<MonsterTemplate>();

			Init();

			Load();
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

		public MonsterTemplate GetType(string typeName)
		{
			if (Enum.IsDefined(typeof (MonsterId), typeName))
			{
				MonsterId id = (MonsterId) Enum.Parse(typeof (MonsterId), typeName);
				return GetType(id);
			}
			else
			{
				return GetCustomTemplate(typeName);
			}
		}

		public MonsterTemplate GetType(MonsterId type)
		{
			if (type == MonsterId.CustomMonster)
			{
				throw new ArgumentException("nelze zavolat gettype na MonsterId.CustomMonster, pouzit metodu s parametrem pro string");
			}

			MonsterTemplate tObject;
            types.TryGetValue(type, out tObject);
			return tObject;
		}

		public MonsterTemplate GetCustomTemplate(string name)
		{
			foreach(MonsterTemplate templ in customTypes)
			{
				if (templ is CustomMonsterTemplate)
				{
					if (((CustomMonsterTemplate) templ).TemplateName.Equals(name, StringComparison.InvariantCultureIgnoreCase))
					{
						return templ;
					}
				}
			}
			return null;
		}

		public void Load()
		{
			try
			{
				LoadXml();
			}
			catch (Exception e)
			{
				Debug.LogError("chyba nacitani xml monsterdata - check monsterdata_errors.txt");
				System.IO.StreamWriter file = new System.IO.StreamWriter("MonsterData_errors.txt");
				file.WriteLine(e.Message + " \n " + e.StackTrace);
				file.Close();
			}
		}

		private void LoadXml()
		{
			XmlDocument doc = new XmlDocument();
			doc.Load("MonsterData.xml");

			int nextId = 0;

			CustomMonsterTemplate newTemplate;

			foreach (XmlNode monsterNode in doc.DocumentElement.ChildNodes)
			{
				if (!monsterNode.Name.Equals("monster")) continue;

				newTemplate = new CustomMonsterTemplate();

				foreach (XmlNode mainParam in monsterNode.ChildNodes)
				{
					switch (mainParam.Name)
					{
						case "name":
							newTemplate.TemplateName = mainParam.InnerText;
							break;
						case "visibleName":
							newTemplate.Name = mainParam.InnerText;
							break;
						case "template":

							MonsterId id = (MonsterId) Enum.Parse(typeof (MonsterId), mainParam.InnerText);
							MonsterTemplate oldTemplate = GetType(id);

							newTemplate.SetDefaultTemplate(oldTemplate);

							break;
						case "stats":

							foreach (XmlNode statNode in mainParam.ChildNodes)
							{
								switch (statNode.Name)
								{
									case "MaxHp":
										newTemplate.MaxHp = Int32.Parse(statNode.InnerText);
										break;
									case "MaxHp_scale":
										newTemplate.HpLevelScale = Int32.Parse(statNode.InnerText);
										break;
									case "MaxSpeed":
										newTemplate.MaxSpeed = Int32.Parse(statNode.InnerText);
										break;
									case "IsAggressive":
										newTemplate.IsAggressive = statNode.InnerText.ToLower() == "true";
										break;
									case "AggressionRange":
										newTemplate.AggressionRange = Int32.Parse(statNode.InnerText);
										break;
									case "RambleAround":
										newTemplate.RambleAround = statNode.InnerText.ToLower() == "true";
										break;
									case "RambleAroundMaxDist":
										newTemplate.RambleAroundMaxDist = Int32.Parse(statNode.InnerText);
										break;
									case "AlertsAllies":
										newTemplate.AlertsAllies = statNode.InnerText.ToLower() == "true";
										break;
									case "XpReward":
										newTemplate.XpReward = Int32.Parse(statNode.InnerText);
										break;
								}
							}

							break;
					}
				}

				customTypes.Add(newTemplate);
			}
		}
	}
}
