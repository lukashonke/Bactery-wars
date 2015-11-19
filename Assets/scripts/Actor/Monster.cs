using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor.Status;
using Assets.scripts.Mono.ObjectData;
using Assets.scripts.Skills.Base;

namespace Assets.scripts.Actor
{
	public class Monster : Character
	{
		public Monster(string name, EnemyData dataObject) : base(name)
		{
			Data = dataObject;
		}

		public new EnemyData GetData()
		{
			return (EnemyData) Data;
		}

		protected override CharStatus InitStatus()
		{
			CharStatus st = new MonsterStatus(false, 20, 20, 20);
			GetData().SetVisibleHp(st.Hp);
			GetData().SetVisibleMaxHp(st.MaxHp); //TODO convert to setter

			return st;
		}

		protected override SkillSet InitSkillSet()
		{
			return new SkillSet();
		}
	}
}
