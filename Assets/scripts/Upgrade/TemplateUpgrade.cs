using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Skills;
using Assets.scripts.Skills.ActiveSkills;
using Assets.scripts.Skills.Base;
using Assets.scripts.Skills.SkillEffects;

namespace Assets.scripts.Upgrade
{
	public class TemplateUpgrade : AbstractUpgrade
	{
		private int originalRange;

		public TemplateUpgrade(int level) : base(level)
		{
			Name = "template_upgrade";
			VisibleName = "Template upgrade";
		}

		public override void ApplySkillChanges(SkillSet set, ActiveSkill melee)
		{
			if (Level == 1)
			{
				originalRange = melee.range;
				melee.range = originalRange * 10;
			}
			else if(Level == 2)
			{
				originalRange = melee.range;
				melee.range = (int) (originalRange * 1.5f);
			}
		}

		public override void RestoreSkillChanges(SkillSet set, ActiveSkill melee)
		{
			melee.range = originalRange;
		}

		public override void ModifySkillEffects(Skill sk, SkillEffect[] effects)
		{
			/*foreach (SkillEffect ef in effects)
			{
				if (ef is EffectDamage)
				{
					((EffectDamage) ef).Dmg = 1;
				}
			}*/
		}

		public override void ModifyMaxHp(ref int maxHp)
		{
			//maxHp *= 2;
		}

		public override SkillEffect[] CreateAdditionalSkillEffects(Skill sk, SkillEffect[] effects)
		{
			/*if (sk is CommonColdAutoattack)
			{
				CommonColdAutoattack aa = (CommonColdAutoattack) sk;

				return new SkillEffect[]
				{
					new EffectPushaway(100)
				};
			}*/

			return null;
		}

		public override void ModifyRunSpeed(ref int runSpeed)
		{
			runSpeed *= 2;
		}
	}
}
