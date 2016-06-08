// Copyright (c) 2015, Lukas Honke
// ========================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.scripts.Actor.MonsterClasses.Base
{
	public class GroupTemplate
	{
		public Dictionary<MonsterId, int> MembersToSpawn { get; private set; }

		public GroupTemplate()
		{
			MembersToSpawn = new Dictionary<MonsterId, int>();
		}

		public GroupTemplate Add(MonsterId id, int count)
		{
			MembersToSpawn.Add(id, count);
			return this;
		}
	}
}
