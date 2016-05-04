using System;
using System.Collections.Generic;
using System.Linq;
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

		public CustomMonsterTemplate()
		{
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
			if (OldTemplate != null)
			{
				return OldTemplate.CreateAI(ch);
			}

			/*ImmobileMonsterAI a = new ImmobileMonsterAI(ch);
			a.loseInterestWhenOuttaRange = true;
			return a;*/

			return null;
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
