using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.AI;
using Assets.scripts.Skills;
using Assets.scripts.Skills.Base;

namespace Assets.scripts.Actor.MonsterClasses.Monsters
{
	public class LeukocyteRanged : MonsterTemplate
	{
		public LeukocyteRanged()
		{
			MaxHp = 20;
			MaxMp = 50;
			MaxSpeed = 10;
		}

		protected override void AddSkillsToTemplate()
		{
			// no skills
			SetMeleeAttackSkill((ActiveSkill) SkillTable.Instance.GetSkill(SkillId.MeleeAttack));

			TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.SkillTestProjectile)); // the projectile test skill
		}

		public override MonsterAI CreateAI(Character ch)
		{
			return new RangedMonsterAI(ch);
		}

		public override GroupTemplate GetGroupTemplate()
		{
			return null;
		}

		public override MonsterId GetMonsterId()
		{
			return MonsterId.Leukocyte_ranged;
		}
	}
}
