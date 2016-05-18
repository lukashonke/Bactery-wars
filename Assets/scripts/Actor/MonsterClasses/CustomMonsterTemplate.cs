using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.AI;
using Assets.scripts.AI.Modules;
using Assets.scripts.Mono.MapGenerator.Levels;
using Assets.scripts.Mono.ObjectData;
using Assets.scripts.Skills;
using Assets.scripts.Skills.ActiveSkills;
using Assets.scripts.Skills.Base;
using Assets.scripts.Skills.SkillEffects;
using UnityEngine;

namespace Assets.scripts.Actor.MonsterClasses
{
	public struct SkillModifyInfo
	{
		public SkillId id;
		public string key;
		public string value;

		public SkillModifyInfo(SkillId id, string key, string value)
		{
			this.id = id;
			this.key = key;
			this.value = value;
		}
	}

	public struct SkillEffectInfo
	{
		public SkillId id;
		public string effectName;
		public Dictionary<string, string> parameters;

		public SkillEffectInfo(SkillId id, string effectName, Dictionary<string, string> parameters)
		{
			this.id = id;
			this.effectName = effectName;
			this.parameters = parameters;
		}
	}

	public struct AiParamInfo
	{
		public int id;
		public string module;
		public string param;
		public string value;

		public AiParamInfo(int id, string module, string param, string value)
		{
			this.id = id;
			this.module = module;
			this.param = param;
			this.value = value;
		}
	}

	public struct AddModuleInfo
	{
		public int id;
		public string module;
		public bool atHighPriority;

		public AddModuleInfo(int id, string module, bool highPriority)
		{
			this.id = id;
			this.module = module;
			this.atHighPriority = highPriority;
		}
	}

	class CustomMonsterTemplate : MonsterTemplate
	{
		public MonsterTemplate OldTemplate { get; protected set; }

		public string TemplateName { get; set; }

		public string Sprite { get; set; }
		public float SpriteSize { get; set; }
		public float Mass { get; set; }

		public string AiType { get; set; }
		public List<AiParamInfo> AiParams { get; set; }
		public List<AddModuleInfo> NewModules { get; set; } 

		public List<SkillId> NewSkills { get; private set; } 
		public List<SkillId> SkillsToRemove { get; private set; }
		public List<SkillId> DisabledEffects { get; private set; } 
		public SkillId NewAutoattack { get; set; }
		public bool DisabledMeleeEffects { get; private set; }

		public List<SkillModifyInfo> SkillModifyInfo { get; private set; }
		public List<SkillModifyInfo> AutoattackSkillModifyInfos { get; private set; }
		public List<SkillEffectInfo> SkillAddEffects { get; private set; } 
		public List<SkillEffectInfo> MeleeAddEffects { get; private set; }

		public CustomMonsterTemplate()
		{
			AiType = null;
			AiParams = new List<AiParamInfo>();
			NewModules = new List<AddModuleInfo>();
			NewSkills = new List<SkillId>();
			SkillsToRemove = new List<SkillId>();
			DisabledEffects = new List<SkillId>();
			SkillModifyInfo = new List<SkillModifyInfo>();
			AutoattackSkillModifyInfos = new List<SkillModifyInfo>();
			SkillAddEffects = new List<SkillEffectInfo>();
			MeleeAddEffects = new List<SkillEffectInfo>();
			NewAutoattack = SkillId.SkillTemplate;

			Sprite = null;
			SpriteSize = -1;
			Mass = -1;

			Name = "Custom Cell";
			MaxHp = 20;
			MaxMp = 50;
			MaxSpeed = 0;

			IsAggressive = true;
			AggressionRange = 15;
			RambleAround = false;
			AlertsAllies = false;
			XpReward = 3;
		}

		public void AddAiParam(int id, string module, string key, string value)
		{
			AiParamInfo info = new AiParamInfo(id, module, key, value);
			AiParams.Add(info);
		}

		public void AddSkillModifyInfo(SkillId id, string key, string value)
		{
			SkillModifyInfo info = new SkillModifyInfo(id, key, value);
			SkillModifyInfo.Add(info);
		}

		public void AddAutoattackModifyInfo(SkillId id, string key, string value)
		{
			SkillModifyInfo info = new SkillModifyInfo(id, key, value);
			AutoattackSkillModifyInfos.Add(info);
		}

		public void AddAdditionalSkillEffects(SkillId id, string effectName, Dictionary<string, string> parameters)
		{
			// disable the original effects
			//DisableSkillEffects(id);

			SkillEffectInfo info = new SkillEffectInfo(id, effectName, parameters);
			SkillAddEffects.Add(info);
		}

		public void AddMeleeSkillEffects(SkillId id, string effectName, Dictionary<string, string> parameters)
		{
			//DisableMeleeEffects();

			SkillEffectInfo info = new SkillEffectInfo(id, effectName, parameters);
			MeleeAddEffects.Add(info);
		}

		public void AddAIModule(int id, string module, string priority, string param, string value)
		{
			bool isHighPriority = true;
			if (priority.Equals("low", StringComparison.InvariantCultureIgnoreCase))
				isHighPriority = false;

			NewModules.Add(new AddModuleInfo(id, module, isHighPriority));

			if(param != null)
				AddAiParam(id, module, param, value);
		}

		public void DisableSkillEffects(SkillId id)
		{
			DisabledEffects.Add(id);
		}

		public void DisableMeleeEffects()
		{
			DisabledMeleeEffects = true;
		}

		public override void AddSkillsToTemplate()
		{
		}

		public void InitCustomSkillsOnTemplate()
		{
			if (NewSkills != null)
			{
				foreach (SkillId id in NewSkills)
				{
					bool contains = false;
					foreach (Skill sk in TemplateSkills)
					{
						if (sk.GetSkillId() == id)
						{
							contains = true;
							break;
						}
					}

					if(!contains)
						TemplateSkills.Add(SkillTable.Instance.GetSkill(id));
				}
			}

			if (NewAutoattack != SkillId.SkillTemplate)
			{
				if (NewAutoattack == SkillId.CustomRemove)
					SetMeleeAttackSkill(null);
				else
					SetMeleeAttackSkill((ActiveSkill)SkillTable.Instance.GetSkill(NewAutoattack));
			}

			if (SkillsToRemove != null && SkillsToRemove.Count > 0)
			{
				foreach (Skill sk in TemplateSkills.ToArray())
				{
					if (SkillsToRemove.Contains(sk.GetSkillId()))
					{
						TemplateSkills.Remove(sk);
					}
				}
			}
		}

		public override void InitSkillsOnMonster(SkillSet set, ActiveSkill meleeSkill, int level)
		{
			if (OldTemplate != null)
			{
				OldTemplate.InitSkillsOnMonster(set, meleeSkill, level);
			}

			FieldInfo field;

			foreach (SkillModifyInfo info in SkillModifyInfo)
			{
				string key = info.key;
				string value = info.value;
				Skill sk = set.GetSkill(info.id);

				field = sk.GetType().GetField(key, BindingFlags.Public | BindingFlags.Instance);

				if (field != null)
				{
					if (field.FieldType == typeof(int))
					{
						field.SetValue(sk, Int32.Parse(value));
					}
					else if (field.FieldType == typeof(float))
					{
						field.SetValue(sk, float.Parse(value));
					}
					else if (field.FieldType == typeof(double))
					{
						field.SetValue(sk, Double.Parse(value));
					}
					else if (field.FieldType == typeof(bool))
					{
						field.SetValue(sk, Boolean.Parse(value));
					}
					else if (field.FieldType == typeof(string))
					{
						field.SetValue(sk, value);
					}
					else
					{
						throw new ArgumentException("invalid custom skill field " + key + " - has unknown type (not int, float, double or string)");
					}
				}
				else
				{
					Debug.LogError("Cant write to property " + key + " in " + sk.GetType().Name + " (property is null)");
				}
			}

			if(MeleeSkill != null)
			foreach (SkillModifyInfo info in AutoattackSkillModifyInfos)
			{
				string key = info.key;
				string value = info.value;

				field = meleeSkill.GetType().GetField(key, BindingFlags.Public | BindingFlags.Instance);

				if (field != null)
				{
					if (field.FieldType == typeof(int))
					{
						field.SetValue(meleeSkill, Int32.Parse(value));
					}
					else if (field.FieldType == typeof(float))
					{
						field.SetValue(meleeSkill, float.Parse(value));
					}
					else if (field.FieldType == typeof(double))
					{
						field.SetValue(meleeSkill, Double.Parse(value));
					}
					else if (field.FieldType == typeof(bool))
					{
						field.SetValue(meleeSkill, Boolean.Parse(value));
					}
					else if (field.FieldType == typeof(string))
					{
						field.SetValue(meleeSkill, value);
					}
					else
					{
						throw new ArgumentException("invalid custom autoattack skill field " + key + " - has unknown type (not int, float, double or string)");
					}
				}
				else
				{
					Debug.LogError("Cant write to property " + key + " in " + meleeSkill.GetType().Name + " (property is null)");
				}
			}

			if (DisabledEffects.Count > 0)
			{
				foreach (SkillId id in DisabledEffects)
				{
					Skill sk = set.GetSkill(id);
					sk.DisableOriginalEffects();
				}
			}

			if (DisabledMeleeEffects)
			{
				if(meleeSkill != null)
					meleeSkill.DisableOriginalEffects();
			}

			if (SkillAddEffects.Count > 0)
			{
				foreach (SkillEffectInfo info in SkillAddEffects)
				{
					Skill sk = set.GetSkill(info.id);
					string name = info.effectName;

					object[] parameters;

					SkillEffect effect = null;
					foreach (Type t in Utils.GetTypesInNamespace("Assets.scripts.Skills.SkillEffects", true, typeof (SkillEffect)))
					{
						if (t.Name.Equals("Effect" + name, StringComparison.InvariantCultureIgnoreCase))
						{
							foreach (ConstructorInfo ci in t.GetConstructors())
							{
								parameters = new object[ci.GetParameters().Length];

								bool matches = true;

								// pro dany konstruktor zjistit, jestli jsou jeho parametry definovane
								for (int i = 0; i < ci.GetParameters().Length; i++)
								{
									ParameterInfo pi = ci.GetParameters()[i];
									string val;
									if (!info.parameters.TryGetValue(pi.Name, out val))
									{
										matches = false;
										break;
									}
									else
									{
										if (pi.ParameterType == typeof(int))
										{
											parameters[i] = Int32.Parse(val);
										}
										else if (pi.ParameterType == typeof(float))
										{
											parameters[i] = float.Parse(val);
										}
										else if (pi.ParameterType == typeof(double))
										{
											parameters[i] = Double.Parse(val);
										}
										else if (pi.ParameterType == typeof(bool))
										{
											parameters[i] = Boolean.Parse(val);
										}
										else if (pi.ParameterType == typeof(string))
										{
											parameters[i] = val;
										}
										else
										{
											throw new ArgumentException("invalid param when constructing skill effect " + pi.ParameterType + " - cant parse");
										}
									}
								}

								// this constructor will be used
								if (matches)
								{
									effect = (SkillEffect) ci.Invoke(parameters);
									break;
								}
							}

							break;
						}
					}

					if (effect != null)
					{
						sk.AddAdditionalEffect(effect);
					}
					else
					{
						Debug.LogError("couldnt create instance of " + name + " with parameters " + info.parameters);
					}
				}
			}



			if (MeleeAddEffects.Count > 0 && meleeSkill != null)
			{
				foreach (SkillEffectInfo info in MeleeAddEffects)
				{
					string name = info.effectName;

					object[] parameters;

					SkillEffect effect = null;
					foreach (Type t in Utils.GetTypesInNamespace("Assets.scripts.Skills.SkillEffects", true, typeof(SkillEffect)))
					{
						if (t.Name.Equals("Effect" + name, StringComparison.InvariantCultureIgnoreCase))
						{
							foreach (ConstructorInfo ci in t.GetConstructors())
							{
								parameters = new object[ci.GetParameters().Length];

								bool matches = true;

								// pro dany konstruktor zjistit, jestli jsou jeho parametry definovane
								for (int i = 0; i < ci.GetParameters().Length; i++)
								{
									ParameterInfo pi = ci.GetParameters()[i];
									string val;
									if (!info.parameters.TryGetValue(pi.Name, out val))
									{
										matches = false;
										break;
									}
									else
									{
										if (pi.ParameterType == typeof(int))
										{
											parameters[i] = Int32.Parse(val);
										}
										else if (pi.ParameterType == typeof(float))
										{
											parameters[i] = float.Parse(val);
										}
										else if (pi.ParameterType == typeof(double))
										{
											parameters[i] = Double.Parse(val);
										}
										else if (pi.ParameterType == typeof(bool))
										{
											parameters[i] = Boolean.Parse(val);
										}
										else if (pi.ParameterType == typeof(string))
										{
											parameters[i] = val;
										}
										else
										{
											throw new ArgumentException("invalid param when constructing skill effect " + pi.ParameterType + " - cant parse");
										}
									}
								}

								// this constructor will be used
								if (matches)
								{
									effect = (SkillEffect)ci.Invoke(parameters);
									break;
								}
							}

							break;
						}
					}

					if (effect != null)
					{
						meleeSkill.AddAdditionalEffect(effect);
					}
					else
					{
						Debug.LogError("couldnt create instance of " + name + " with parameters " + info.parameters);
					}
				}
			}
		}

		public override void InitMonsterStats(Monster m, int level)
		{
			if (OldTemplate != null)
			{
				OldTemplate.InitMonsterStats(m, level);
			}

			base.InitMonsterStats(m, level);
		}

		public override MonsterAI CreateAI(Character ch)
		{
			if (AiType != null)
			{
				MonsterAI ai = null;
				MonsterTemplate t;
				List<Type> types = Utils.GetTypesInNamespace("Assets.scripts.AI", true, typeof(MonsterAI));

				foreach (Type type in types)
				{
					if (type.Name.Equals(AiType, StringComparison.InvariantCultureIgnoreCase))
					{
						ai = Activator.CreateInstance(type, ch) as MonsterAI;
						break;
					}
				}

				if (ai == null)
					throw new NullReferenceException("CustomAIType " + AiType + " doesnt exist!");

				foreach (AddModuleInfo info in NewModules)
				{
					int id = info.id;
					string module = info.module + "Module";
					bool highPrior = info.atHighPriority;

					List<Type> moduleTypes = Utils.GetTypesInNamespace("Assets.scripts.AI.Modules", true, typeof(AIAttackModule));
					AIAttackModule newModule = null;

					foreach (Type type in moduleTypes)
					{
						if (type.Name.Equals(module, StringComparison.InvariantCultureIgnoreCase))
						{
							newModule = Activator.CreateInstance(type, ai) as AIAttackModule;
							break;
						}
					}

					if (newModule != null)
					{
						newModule.id = id;

						Debug.Log("added new module " + newModule.id + " name " + module);

						ai.AddPriorityAttackModule(newModule, highPrior);
					}
					else
					{
						Debug.LogError("unknown module: " + module);
					}
				}

				FieldInfo field;
				foreach(AiParamInfo info in AiParams)
				{
					int id = info.id;
					string module = info.module + "Module";
					string key = info.param;
					string value = info.value;

					AIAttackModule moduleClass;

					// find by ID
					if(id > 0)
						moduleClass = ai.GetAttackModule(id);
					else // find by module name
						moduleClass = ai.GetAttackModule(module);

					field = moduleClass.GetType().GetField(key, BindingFlags.Public | BindingFlags.Instance);
					if (field != null)
					{
						if (field.FieldType == typeof (int))
						{
							field.SetValue(moduleClass, Int32.Parse(value));
						}
						else if (field.FieldType == typeof(float))
						{
							field.SetValue(moduleClass, float.Parse(value));
						}
						else if (field.FieldType == typeof (double))
						{
							field.SetValue(moduleClass, Double.Parse(value));
						}
						else if (field.FieldType == typeof(bool))
						{
							field.SetValue(moduleClass, Boolean.Parse(value));
						}
						else if (field.FieldType == typeof (string))
						{
							field.SetValue(moduleClass, value);
						}
						else
						{
							throw new ArgumentException("invalid custom AI field " + key + " - has unknown type (not int, float, double or string)");
						}
					}
					else
					{
						Debug.LogError("Cant write to property " + key + " in " + moduleClass.GetType().Name + " (property is null)");
					}
				}

				Debug.Log("Created custom " + ai.GetType().Name + " AI for " + ch.Name);
				return ai;
			}
			else if (OldTemplate != null)
			{
				return OldTemplate.CreateAI(ch);
			}

			throw new NullReferenceException("null AI (not set for some reason");
		}

		public override void InitAppearanceData(Monster m, EnemyData data)
		{
			if (OldTemplate != null)
			{
				OldTemplate.InitAppearanceData(m, data);
			}

			if (Sprite != null)
			{
				data.SetSprite("prefabs/entity/CustomCell/" + Sprite, SpriteSize);
			}

			if (Mass > 0)
			{
				data.SetMass(Mass);
			}
		}

		public virtual void OnTalkTo(Character source)
		{
			if (OldTemplate != null)
			{
				OldTemplate.OnTalkTo(source);
			}
		}

		public override GroupTemplate GetGroupTemplate()
		{
			if (OldTemplate != null)
			{
				return OldTemplate.GetGroupTemplate();
			}
			return null;
		}

		public override MonsterId GetMonsterId()
		{
			/*if (OldTemplate != null)
			{
				return OldTemplate.GetMonsterId();
			}*/

			return MonsterId.CustomMonster;
		}

		public override string GetMonsterTypeName()
		{
			return TemplateName;
		}

		public override string GetFolderName()
		{
			if (OldTemplate != null)
			{
				return OldTemplate.GetMonsterId().ToString();
			}

			return MonsterId.CustomMonster.ToString();
		}

		private MonsterId GetOldTemplateId()
		{
			if(OldTemplate != null)
			{
				return OldTemplate.GetMonsterId();
			}

			return MonsterId.CustomMonster;
		}

		public string GetOldTemplateFolderId()
		{
			if (OldTemplate != null)
			{
				return OldTemplate.GetMonsterId().ToString();
			}

			return MonsterId.CustomMonster.ToString();
		}

		public void SetDefaultTemplate(MonsterTemplate template)
		{
			OldTemplate = template;

			MaxHp = OldTemplate.MaxHp;
			HpLevelScale = OldTemplate.HpLevelScale;
			CriticalRate = OldTemplate.CriticalRate;
			CriticalDamageMul = OldTemplate.CriticalDamageMul;
			Shield = OldTemplate.Shield;

			MaxMp = OldTemplate.MaxMp;
			MaxSpeed = OldTemplate.MaxSpeed;

			IsAggressive = OldTemplate.IsAggressive;
			AggressionRange = OldTemplate.AggressionRange;
			RambleAround = OldTemplate.RambleAround;
			RambleAroundMaxDist = OldTemplate.RambleAroundMaxDist;
			AlertsAllies = OldTemplate.AlertsAllies;

			XpReward = OldTemplate.XpReward;
			XpLevelMul = OldTemplate.XpLevelMul;

			MeleeSkill = OldTemplate.MeleeSkill;

			foreach (Skill sk in OldTemplate.TemplateSkills)
			{
				TemplateSkills.Add(sk);
			}
		}
	}
}
