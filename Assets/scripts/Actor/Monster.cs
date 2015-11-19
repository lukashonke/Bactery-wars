using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor.MonsterClasses;
using Assets.scripts.Actor.Status;
using Assets.scripts.Mono.ObjectData;
using Assets.scripts.Skills.Base;

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
	}
}
