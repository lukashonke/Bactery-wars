using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.AI;

namespace Assets.scripts.Actor.MonsterClasses.Monsters
{
	public class CrystalObject : MonsterTemplate
	{
		public CrystalObject()
		{
			MaxHp = 200;
			MaxMp = 50;
			MaxSpeed = 4;

			IsAggressive = false;
			AggressionRange = 30;
			RambleAround = true;
			XpReward = 3;
		}

		public override void AddSkillsToTemplate()
		{
		}

		public override MonsterAI CreateAI(Character ch)
		{
			MonsterAI ai = new IdleMonsterAI(ch);
			return ai;
		}

		public override GroupTemplate GetGroupTemplate()
		{
			return null;
		}

		public override MonsterId GetMonsterId()
		{
			return MonsterId.SimpleBase;
		}
	}
}
