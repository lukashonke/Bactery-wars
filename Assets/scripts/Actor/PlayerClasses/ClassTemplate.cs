using System.Collections.Generic;
using Assets.scripts.Actor.PlayerClasses.Base;
using Assets.scripts.Skills;

namespace Assets.scripts.Actor.PlayerClasses
{
	/*
		Hardcoded templates
	*/
	public abstract class ClassTemplate
	{
		public ClassId ClassId { get; private set; }

		public List<Skill> TemplateSkills { get; set; }

		protected ClassTemplate(ClassId classId)
		{
			ClassId = classId;

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
