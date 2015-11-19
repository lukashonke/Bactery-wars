﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor.MonsterClasses.Base;

namespace Assets.scripts.Actor.MonsterClasses
{
	public class WhiteCellTemplate : MonsterTemplate
	{
		public WhiteCellTemplate(MonsterId id) : base(id)
		{
			MaxHp = 10;
			MaxMp = 10;
			MaxSpeed = 10;
		}

		protected override void AddSkills()
		{
			// no skills
		}
	}
}
