using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.Actor.Status;
using Assets.scripts.AI;
using Assets.scripts.Skills;
using Assets.scripts.Skills.ActiveSkills;
using Assets.scripts.Skills.Base;
using UnityEngine;

namespace Assets.scripts.Actor.MonsterClasses.Boss
{
	public class SwarmerBoss : BossTemplate
	{
		public SwarmerBoss()
		{
			MaxHp = 150;
			MaxMp = 50;
			MaxSpeed = 10;

			IsAggressive = false;
			AggressionRange = 10;
			RambleAround = false;
			AlertsAllies = true;
		}

		protected override void AddSkillsToTemplate()
		{
			TemplateSkills.Add(SkillTable.Instance.GetSkill(SkillId.SwarmerSpawnSkill));
		}

		public override void InitSkillsOnMonster(SkillSet set, ActiveSkill meleeSkill, int level)
		{
			SwarmerSpawnSkill sk = set.GetSkill(SkillId.SwarmerSpawnSkill) as SwarmerSpawnSkill;

			sk.castTime = 0;
			sk.reuse = 0f;
			sk.mobToSpawn = MonsterId.SwarmerMeleeCell;
		}

		public override MonsterAI CreateAI(Character ch)
		{
			SwarmerBossAI a = new SwarmerBossAI(ch);
			a.alwaysActive = true;
			a.maxMonstersSpawned = 20;
			a.spawnMinInterval = 2f;
			a.spawnMaxInterval = 6f;

			return a;
		}

		public override void InitMonsterStats(Monster m, int level)
		{
			base.InitMonsterStats(m, level);
		}

		public override GroupTemplate GetGroupTemplate()
		{
			return null;
		}

		public override MonsterId GetMonsterId()
		{
			return MonsterId.SwarmerBoss;
		}
	}
}
