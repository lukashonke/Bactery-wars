using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor.MonsterClasses;

namespace Assets.scripts.Actor.Status
{
	public class MonsterStatus : CharStatus
	{
		public MonsterStatus(bool isDead, int hp, int mp, MonsterTemplate template) : base(isDead, hp, mp, template.MaxHp, template.MaxMp, template.MaxSpeed, template.Shield, template.CriticalRate, template.CriticalDamageMul)
		{

		}
	}
}
