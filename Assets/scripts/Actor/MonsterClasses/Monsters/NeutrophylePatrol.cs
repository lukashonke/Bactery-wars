using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.AI;
using Assets.scripts.AI.Modules;
using Assets.scripts.Skills.Base;

namespace Assets.scripts.Actor.MonsterClasses.Monsters
{
	public class NeutrophylePatrol : MonsterTemplate
	{
		public NeutrophylePatrol()
		{
			Name = "Neutrophyle Patrol";
			MaxHp = 50;
			MaxMp = 50;
			MaxSpeed = 10;

			IsAggressive = true;
			AggressionRange = 10;
			RambleAround = true;
			AlertsAllies = true;
			XpReward = 5;
		}

		public override void AddSkillsToTemplate()
		{
			TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.NeutrophileProjectile));
			//TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.CreateCoverMob));
		}

		public override MonsterAI CreateAI(Character ch)
		{
			RangedMonsterAI a = new RangedMonsterAI(ch);
			a.GetAttackModule<EvadeModule>().interval = 2f;
			return a;
		}

		public override GroupTemplate GetGroupTemplate()
		{
			return null;
		}

		public override MonsterId GetMonsterId()
		{
			return MonsterId.Neutrophyle_Patrol;
		}
	}
}
