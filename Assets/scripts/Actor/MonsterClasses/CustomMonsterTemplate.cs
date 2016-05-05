using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.AI;
using Assets.scripts.Mono.ObjectData;
using Assets.scripts.Skills;
using Assets.scripts.Skills.ActiveSkills;
using Assets.scripts.Skills.Base;
using UnityEngine;

namespace Assets.scripts.Actor.MonsterClasses
{
	class CustomMonsterTemplate : MonsterTemplate
	{
		public MonsterTemplate OldTemplate { get; protected set; }

		public string TemplateName { get; set; }

		public string AiType { get; set; }
		public Dictionary<string, string> AiParams { get; set; } 

		public CustomMonsterTemplate()
		{
			AiType = null;
			AiParams = new Dictionary<string, string>();

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

		public void AddAiParam(string key, string value)
		{
			AiParams.Add(key, value);
		}

		protected override void AddSkillsToTemplate()
		{
			//TODO inject custom code
		}

		public override void InitSkillsOnMonster(SkillSet set, ActiveSkill meleeSkill, int level)
		{
			if (OldTemplate != null)
			{
				OldTemplate.InitSkillsOnMonster(set, meleeSkill, level);
			}

			/*SkillTestProjectile sk = set.GetSkill(SkillId.SkillTestProjectile) as SkillTestProjectile;

			sk.castTime = 0.5f;
			sk.range = AggressionRange;*/
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

				FieldInfo field;
				foreach (KeyValuePair<string, string> e in AiParams)
				{
					string key = e.Key;
					string value = e.Value;

					field = ai.GetType().GetField(key, BindingFlags.Public | BindingFlags.Instance);
					if (field != null)
					{
						if (field.FieldType == typeof (int))
						{
							field.SetValue(ai, Int32.Parse(value));
						}
						else if (field.FieldType == typeof(float))
						{
							field.SetValue(ai, float.Parse(value));
						}
						else if (field.FieldType == typeof (double))
						{
							field.SetValue(ai, Double.Parse(value));
						}
						else if (field.FieldType == typeof (string))
						{
							field.SetValue(ai, value);
						}
						else
						{
							throw new ArgumentException("invalid custom AI field " + key + " - has unknown type (not int, float, double or string)");
						}
					}
					else
					{
						Debug.LogError("Cant write to property " + key + " in " + ai.GetType().Name + " (property is null)");
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
