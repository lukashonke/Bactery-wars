using Assets.scripts.Skills.Base;
using Assets.scripts.Skills.SkillEffects;
using UnityEngine;

namespace Assets.scripts.Skills.ActiveSkills
{
	public class SkillTemplate : ActiveSkill
	{
		public SkillTemplate()
		{
			
		}

		public override SkillId GetSkillId()
		{
			return SkillId.SkillTemplate;
		}

		public override string GetVisibleName()
		{
			return "Skill Template";
		}

		public override Skill Instantiate()
		{
			return new SkillTemplate();
		}

		public override SkillEffect[] CreateEffects(int param)
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

		public override void MonoUpdate(GameObject gameObject, bool fixedUpdate)
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
