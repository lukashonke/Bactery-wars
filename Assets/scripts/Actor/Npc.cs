// Copyright (c) 2015, Lukas Honke
// ========================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor.MonsterClasses;
using Assets.scripts.AI;
using Assets.scripts.Mono.ObjectData;

namespace Assets.scripts.Actor
{
	public class Npc : Monster
	{
		public Npc(string name, EnemyData dataObject, MonsterTemplate template) : base(name, dataObject, template)
		{
		}

		public Npc(string name, EnemyData dataObject, MonsterTemplate template, AbstractAI ai) : base(name, dataObject, template, ai)
		{
		}

		protected override AbstractAI InitAI()
		{
			return new IdleMonsterAI(this);
		}

		public override bool IsInteractable()
		{
			return true;
		}
	}
}
