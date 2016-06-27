// Copyright (c) 2015, Lukas Honke
// ========================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using Assets.scripts.Actor.PlayerClasses;
using Assets.scripts.Actor.PlayerClasses.Base;
using Assets.scripts.Skills;
using Assets.scripts.Skills.Base;
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
				try
				{
					t = Activator.CreateInstance(type) as MonsterTemplate;
					AddType(t);
				}
				catch (Exception)
				{
					Debug.LogError("error when initing template " + type.Name);
				}
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
				customTypes.Clear();
				LoadXml("MonsterData.xml");
			}
			catch (Exception e)
			{
				Debug.LogError("chyba nacitani xml monsterdata - check monsterdata_errors.txt");
				Debug.LogError(e.Message + ", " + e.StackTrace);
				System.IO.StreamWriter file = new System.IO.StreamWriter("MonsterData_errors.txt");
				file.WriteLine(e.Message + " \n " + e.StackTrace);
				file.Close();
			}

			try
			{
				LoadXml("MonsterData_Templates.xml");
			}
			catch (Exception e)
			{
				Debug.LogError("chyba nacitani xml monsterdata - check monsterdata_templates_errors.txt");
				Debug.LogError(e.Message + ", " + e.StackTrace);
				System.IO.StreamWriter file = new System.IO.StreamWriter("MonsterData_Templates_errors.txt");
				file.WriteLine(e.Message + " \n " + e.StackTrace);
				file.Close();
			}
		}

		private void LoadXml(string fileName)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(fileName);

			int nextId = 0;

			CustomMonsterTemplate newTemplate;

			foreach (XmlNode monsterNode in doc.DocumentElement.ChildNodes)
			{
				if (!monsterNode.Name.Equals("monster")) continue;

				newTemplate = new CustomMonsterTemplate();

				if (monsterNode.Attributes != null)
				{
					foreach (XmlAttribute attr in monsterNode.Attributes)
					{
						if (attr.Name == "name")
						{
							newTemplate.TemplateName = attr.Value;
						}
					}
				}

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
						case "sprite":
							newTemplate.Sprite = mainParam.InnerText;
							break;
						case "size":
							newTemplate.SpriteSize = float.Parse(mainParam.InnerText);
							break;
						case "mass":
							newTemplate.Mass = float.Parse(mainParam.InnerText);
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
						case "spawn_on_die":

							foreach (XmlNode mNode in mainParam.ChildNodes)
							{
								string name = mNode.Name;
								newTemplate.AddOnSpawnOnDie(name);
							}

							break;
						case "attached_cells":

							foreach (XmlNode mNode in mainParam.ChildNodes)
							{
								string name = mNode.Name;
								newTemplate.AddAttachedCell(name);
							}

							break;
						case "ai":

							string aiType = "Blank";

							if (mainParam.Attributes != null)
							{
								foreach (XmlAttribute attr in mainParam.Attributes)
								{
									if (attr.Name == "type")
									{
										aiType = attr.Value;
									}
								}
							}

							if (aiType == null)
								continue;

							newTemplate.AiType = aiType + "MonsterAI";

							foreach (XmlNode statNode in mainParam.ChildNodes)
							{
								if (statNode.Name == "set")
								{
									string idString = null;
									string module = null;
									string param = null;
									string value = null;

									if (statNode.Attributes != null)
									{
										foreach (XmlAttribute attrib in statNode.Attributes)
										{
											switch (attrib.Name)
											{
												case "id_module":
													idString = attrib.Value;
													break;
												case "module":
													module = attrib.Value;
													break;
												case "param":
													param = attrib.Value;
													break;
												case "value":
													value = attrib.Value;
													break;
											}
										}
									}

									if ((module != null || idString != null) && param != null && value != null)
									{
										int idVal = -1;
										if(idString != null)
										{
											idVal = Int32.Parse(idString);
										}

										newTemplate.AddAiParam(idVal, module, param, value);
									}
								}
								else if (statNode.Name == "add")
								{
									string idString = null;
									string module = null;
									string param = null;
									string value = null;
									string priority = "low";

									if (statNode.Attributes != null)
									{
										foreach (XmlAttribute attrib in statNode.Attributes)
										{
											switch (attrib.Name)
											{
												case "id_module":
													idString = attrib.Value;
													break;
												case "priority": // "low", "high" 
													priority = attrib.Value;
													break;
												case "module":
													module = attrib.Value;
													break;
												case "param":
													param = attrib.Value;
													break;
												case "value":
													value = attrib.Value;
													break;
											}
										}
									}

									if (module != null)
									{
										int idNumber = -1;
										if (idString != null)
										{
											idNumber = Int32.Parse(idString);
										}

										newTemplate.AddAIModule(idNumber, module, priority, param, value);
									}
								}
							}

							break;
						case "add_skills":

							foreach (XmlNode skillNode in mainParam.ChildNodes)
							{
								string skillName = skillNode.Name;
								SkillId skillId = (SkillId) Enum.Parse(typeof (SkillId), skillName);

								newTemplate.NewSkills.Add(skillId);

								foreach (XmlNode skillParamNode in skillNode.ChildNodes)
								{
									if (skillParamNode.Name == "add_effect")
									{
										string effectName = null;
										Dictionary<string, string> parameters = new Dictionary<string, string>();
										foreach (XmlAttribute attr in skillParamNode.Attributes)
										{
											if (attr.Name == "name")
											{
												effectName = attr.Value;
											}
											else
											{
												parameters.Add(attr.Name, attr.Value);
											}
										}

										newTemplate.AddAdditionalSkillEffects(skillId, effectName, parameters);
									}
									else if (skillParamNode.Name == "remove_effects")
									{
										newTemplate.DisableSkillEffects(skillId);
									}
									else
									{
										string paramName = skillParamNode.Name;
										string val = skillParamNode.InnerText;

										newTemplate.AddSkillModifyInfo(skillId, paramName, val);
									}
								}
							}

							break;
						case "modify_skills":

							foreach (XmlNode skillNode in mainParam.ChildNodes)
							{
								string skillName = skillNode.Name;
								SkillId skillId = (SkillId)Enum.Parse(typeof(SkillId), skillName);

								foreach (XmlNode skillParamNode in skillNode.ChildNodes)
								{
									if (skillParamNode.Name == "add_effect")
									{
										string effectName = null;
										Dictionary<string, string> parameters = new Dictionary<string, string>();
										foreach (XmlAttribute attr in skillParamNode.Attributes)
										{
											if (attr.Name == "name")
											{
												effectName = attr.Value;
											}
											else
											{
												parameters.Add(attr.Name, attr.Value);
											}
										}

										newTemplate.AddAdditionalSkillEffects(skillId, effectName, parameters);
									}
									else if (skillParamNode.Name == "remove_effects")
									{
										newTemplate.DisableSkillEffects(skillId);
									}
									else
									{
										string paramName = skillParamNode.Name;
										string val = skillParamNode.InnerText;

										newTemplate.AddSkillModifyInfo(skillId, paramName, val);
									}
								}
							}

							break;
						case "remove_skills":

							foreach (XmlNode skillNode in mainParam.ChildNodes)
							{
								string skillName = skillNode.Name;
								SkillId skillId = (SkillId)Enum.Parse(typeof(SkillId), skillName);

								newTemplate.SkillsToRemove.Add(skillId);
							}

							break;
						case "add_autoattack":

							foreach (XmlNode skillNode in mainParam.ChildNodes)
							{
								string skillName = skillNode.Name;
								SkillId skillId = (SkillId)Enum.Parse(typeof(SkillId), skillName);

								newTemplate.NewAutoattack = skillId;

								foreach (XmlNode skillParamNode in skillNode.ChildNodes)
								{
									if (skillParamNode.Name == "add_effect")
									{
										string effectName = null;
										Dictionary<string, string> parameters = new Dictionary<string, string>();
										foreach (XmlAttribute attr in skillParamNode.Attributes)
										{
											if (attr.Name == "name")
											{
												effectName = attr.Value;
											}
											else
											{
												parameters.Add(attr.Name, attr.Value);
											}
										}

										newTemplate.AddMeleeSkillEffects(skillId, effectName, parameters);
									}
									else if (skillParamNode.Name == "remove_effects")
									{
										newTemplate.DisableMeleeEffects();
									}
									else
									{
										string paramName = skillParamNode.Name;
										string val = skillParamNode.InnerText;

										newTemplate.AddAutoattackModifyInfo(skillId, paramName, val);
									}
								}

								break;
							}

							break;
						case "remove_autoattack":

							newTemplate.NewAutoattack = SkillId.CustomRemove;

							break;
					}
				}

				newTemplate.InitCustomSkillsOnTemplate();

				customTypes.Add(newTemplate);
			}
		}
	}
}
