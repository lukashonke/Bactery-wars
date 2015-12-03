using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor.MonsterClasses;
using Assets.scripts.Actor.Status;
using Assets.scripts.AI;
using Assets.scripts.Mono.ObjectData;
using Assets.scripts.Skills;
using Assets.scripts.Skills.Base;
using UnityEngine;

namespace Assets.scripts.Actor
{
	public class Monster : Character
	{
		public MonsterTemplate Template { get; set; }
		public Monster(string name, EnemyData dataObject, MonsterTemplate template) : base(name)
		{
			Data = dataObject;

			Template = template;
		}

		public new EnemyData GetData()
		{
			return (EnemyData) Data;
		}

		protected override AbstractAI InitAI()
		{
			return new MonsterAI(this);
		}

		protected override CharStatus InitStatus()
		{
			CharStatus st = new MonsterStatus(false, Template.MaxHp, Template.MaxMp, Template); //TODO make a template for this
			GetData().SetVisibleHp(st.Hp);
			GetData().SetVisibleMaxHp(st.MaxHp);
			GetData().SetMoveSpeed(st.MoveSpeed);

			return st;
		}

		protected override SkillSet InitSkillSet()
		{
			return new SkillSet();
		}

		public void InitTemplate()
		{
			int i = 0;
			foreach (Skill templateSkill in Template.TemplateSkills)
			{
				// vytvorit novy objekt skillu
				Skill newSkill = SkillTable.Instance.CreateSkill(templateSkill.Id);
				newSkill.SetOwner(this);

				Skills.AddSkill(newSkill);

				i++;
				Debug.Log("adding to monster skill to " + i + ": " + newSkill.Name);
			}

			if (Template.MeleeSkill != null)
			{
				Skill newSkill = SkillTable.Instance.CreateSkill(Template.MeleeSkill.Id);
				newSkill.SetOwner(this);

				MeleeSkill = (ActiveSkill)newSkill;
			}
		}
	}
}
