using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.AI;
using Assets.scripts.Skills;
using Assets.scripts.Skills.ActiveSkills;
using Assets.scripts.Skills.Base;

namespace Assets.scripts.Actor.MonsterClasses.Monsters
{
    public class SlowerCell : MonsterTemplate
    {
        public SlowerCell()
        {
			Name = "Cell";
			MaxHp = 20;
            MaxMp = 50;
            MaxSpeed = 10;

            IsAggressive = true;
            AggressionRange = 10;
            RambleAround = true;
			XpReward = 2;
		}

	    public override void AddSkillsToTemplate()
        {
            TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.SlowBeam));
        }

		public override void InitSkillsOnMonster(SkillSet set, ActiveSkill meleeSkill, int level)
		{
			SlowBeam skill = set.GetSkill(SkillId.SlowBeam) as SlowBeam;

			if (skill != null)
			{

			}
			//skill.AddAdditionalEffect(new EffectKillSelf());
		}

		public override MonsterAI CreateAI(Character ch)
        {
            MonsterAI ai = new RangedMonsterAI(ch);
            return ai;
        }

        public override GroupTemplate GetGroupTemplate()
        {
            return null;
        }

        public override MonsterId GetMonsterId()
        {
            return MonsterId.SlowerCell;
        }
    }

	public class PusherCell : MonsterTemplate
	{
		public PusherCell()
		{
			Name = "Cell";
			MaxHp = 20;
			MaxMp = 50;
			MaxSpeed = 10;

			IsAggressive = true;
			AggressionRange = 10;
			RambleAround = true;
			XpReward = 2;
		}

		public override void AddSkillsToTemplate()
		{
			TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.PushBeam));
		}

		public override void InitSkillsOnMonster(SkillSet set, ActiveSkill meleeSkill, int level)
		{
			PushBeam skill = set.GetSkill(SkillId.PushBeam) as PushBeam;

			if (skill != null)
			{
				skill.baseDamage = 40;
				skill.rotateSpeed = 40;
			}
			//skill.AddAdditionalEffect(new EffectKillSelf());
		}

		public override MonsterAI CreateAI(Character ch)
		{
			MonsterAI ai = new RangedMonsterAI(ch);
			return ai;
		}

		public override GroupTemplate GetGroupTemplate()
		{
			return null;
		}

		public override MonsterId GetMonsterId()
		{
			return MonsterId.PusherCell;
		}
	}

	public class SwarmCell : MonsterTemplate
	{
		public SwarmCell()
		{
			Name = "Cell";
			MaxHp = 20;
			MaxMp = 50;
			MaxSpeed = 10;

			IsAggressive = true;
			AggressionRange = 15;
			RambleAround = true;
			XpReward = 2;
		}

		public override void AddSkillsToTemplate()
		{
			TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.SwarmSkill));
		}

		public override void InitSkillsOnMonster(SkillSet set, ActiveSkill meleeSkill, int level)
		{
			SwarmSkill skill = set.GetSkill(SkillId.SwarmSkill) as SwarmSkill;

			if (skill != null)
			{
				skill.countMinions = 3;
			}

			//skill.AddAdditionalEffect(new EffectKillSelf());
		}

		public override MonsterAI CreateAI(Character ch)
		{
			SummonerMonsterAI ai = new SummonerMonsterAI(ch);
			return ai;
		}

		public override GroupTemplate GetGroupTemplate()
		{
			return null;
		}

		public override MonsterId GetMonsterId()
		{
			return MonsterId.SwarmCell;
		}
	}
}
