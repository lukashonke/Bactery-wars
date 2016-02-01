using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor.MonsterClasses;
using Assets.scripts.AI;
using Assets.scripts.Mono.ObjectData;

namespace Assets.scripts.Actor
{
	public class Boss : Monster
	{
		public Boss(string name, EnemyData dataObject, BossTemplate template) : base(name, dataObject, template)
		{

		}

		public Boss(string name, EnemyData dataObject, BossTemplate template, AbstractAI ai) : base(name, dataObject, template, ai)
		{

		}
	}
}
