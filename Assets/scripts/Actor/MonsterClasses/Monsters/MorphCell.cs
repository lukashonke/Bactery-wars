using System.Collections.Generic;
using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.AI;
using Assets.scripts.Base;
using Assets.scripts.Skills.Base;

namespace Assets.scripts.Actor.MonsterClasses.Monsters
{
	public class MorphCellBig : MonsterTemplate
	{
		public MorphCellBig()
		{
			MaxHp = 50;
			MaxMp = 50;
			MaxSpeed = 10;

			IsAggressive = false;
			AggressionRange = 10;
			RambleAround = true;
			AlertsAllies = true;
		}

		protected override void AddSkillsToTemplate()
		{
			TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.NeutrophileProjectile));
		}

		public override MonsterAI CreateAI(Character ch)
		{
			RangedMonsterAI a = new RangedMonsterAI(ch);
		    a.evadeInterval = 2f;
		    return a;
		}

		public override void OnDie(Monster m)
		{
			Monster child = m.SpawnAssociatedMonster(MonsterId.MorphCellMedium, m.Level, m.GetData().GetBody().transform.position);
			child.AI.CopyAggroFrom(m.AI);
		}

		public override GroupTemplate GetGroupTemplate()
		{
			return null;
		}

		public override MonsterId GetMonsterId()
		{
			return MonsterId.MorphCellBig;
		}
	}

	public class MorphCellMedium : MonsterTemplate
	{
		public MorphCellMedium()
		{
			MaxHp = 25;
			MaxMp = 50;
			MaxSpeed = 10;

			IsAggressive = false;
			AggressionRange = 10;
			RambleAround = true;
			AlertsAllies = true;
		}

		protected override void AddSkillsToTemplate()
		{
			TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.NeutrophileProjectile));
		}

		public override MonsterAI CreateAI(Character ch)
		{
			RangedMonsterAI a = new RangedMonsterAI(ch);
			a.evadeInterval = 2f;
			return a;
		}

		public override void OnDie(Monster m)
		{
			Monster child = m.SpawnAssociatedMonster(MonsterId.MorphCellSmall, m.Level, m.GetData().GetBody().transform.position);
			child.AI.CopyAggroFrom(m.AI);
		}

		public override GroupTemplate GetGroupTemplate()
		{
			return null;
		}

		public override MonsterId GetMonsterId()
		{
			return MonsterId.MorphCellMedium;
		}
	}

	public class MorphCellSmall : MonsterTemplate
	{
		public MorphCellSmall()
		{
			MaxHp = 10;
			MaxMp = 50;
			MaxSpeed = 10;

			IsAggressive = false;
			AggressionRange = 10;
			RambleAround = true;
			AlertsAllies = true;
		}

		protected override void AddSkillsToTemplate()
		{
			TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.NeutrophileProjectile));
		}

		public override MonsterAI CreateAI(Character ch)
		{
			RangedMonsterAI a = new RangedMonsterAI(ch);
			a.evadeInterval = 2f;
			return a;
		}

		public override void OnDie(Monster m)
		{
		}

		public override GroupTemplate GetGroupTemplate()
		{
			return null;
		}

		public override MonsterId GetMonsterId()
		{
			return MonsterId.MorphCellSmall;
		}
	}
}
