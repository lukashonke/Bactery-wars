using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.scripts.Actor.Status
{
	public class MonsterStatus : CharStatus
	{
		public MonsterStatus(bool isDead, int hp, int mp, int maxHp, int maxMp, int moveSpeed) : base(isDead, hp, mp, maxHp, maxMp, moveSpeed)
		{

		}
	}
}
