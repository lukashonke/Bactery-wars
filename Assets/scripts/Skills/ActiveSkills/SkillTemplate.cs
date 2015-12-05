using Assets.scripts.Skills.SkillEffects;
using UnityEngine;

namespace Assets.scripts.Skills.ActiveSkills
{
	public class SkillTemplate : ActiveSkill
	{
		public SkillTemplate(string name, int id) : base(name, id)
		{
			
		}

		public override Skill Instantiate()
		{
			return new SkillTemplate(Name, Id);
		}

		public override SkillEffect[] CreateEffects()
		{
			return null;
		}

		public override void InitTraits()
		{
		}

		public override bool OnCastStart()
		{
			return true;
		}

		public override void OnLaunch()
		{
		}

		public override void UpdateLaunched()
		{
		}

		public override void OnAbort()
		{
			
		}

		public override void OnFinish()
		{
		}

		public override void MonoUpdate(GameObject gameObject)
		{
		}

		public override bool CanMove()
		{
			return true;
		}

		public override bool CanRotate()
		{
			return true;
		}
	}
}
