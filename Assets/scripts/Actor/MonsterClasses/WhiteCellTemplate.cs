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
			MaxHp = 50;
			MaxMp = 50;
			MaxSpeed = 10;
		}

		protected override void AddSkills()
		{
			// no skills
		}
	}
}