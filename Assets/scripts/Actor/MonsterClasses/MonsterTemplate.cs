using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.Actor.PlayerClasses.Base;
using Assets.scripts.Skills;

namespace Assets.scripts.Actor.MonsterClasses
{
	public abstract class MonsterTemplate
	{
		public MonsterId MonsterId { get; private set; }

		public List<Skill> TemplateSkills { get; set; }

		public int MaxHp { get; protected set; }
		public int MaxMp { get; protected set; }
		public int MaxSpeed { get; protected set; }

		protected MonsterTemplate(MonsterId id)
		{
			MonsterId = id;

			TemplateSkills = new List<Skill>();

			Init();
		}

		protected virtual void Init()
		{
			AddSkills();
		}

		protected abstract void AddSkills();
	}
}
