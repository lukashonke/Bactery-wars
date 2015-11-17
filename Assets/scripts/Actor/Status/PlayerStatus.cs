using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.scripts.Actor.Status
{
	/*
		Status pro hrace
	*/
	public class PlayerStatus : CharStatus
	{
		public PlayerStatus(bool isDead, int hp, int mp, int maxHp) : base(isDead, hp, mp, maxHp)
		{

		}
	}
}
