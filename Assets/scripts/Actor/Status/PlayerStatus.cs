﻿// Copyright (c) 2015, Lukas Honke
// ========================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor.PlayerClasses;

namespace Assets.scripts.Actor.Status
{
	/*
		Status pro hrace
	*/
	public class PlayerStatus : CharStatus
	{
		private ClassTemplate template;

		public PlayerStatus(bool isDead, int hp, int mp, ClassTemplate template) : base(isDead, hp, mp, template.MaxHp, template.MaxMp, template.MaxSpeed, 25, template.Shield, template.CriticalRate, template.CriticalDamageMul, template.DamageMul, template.DamageAdd)
		{
			this.template = template;
		}
	}
}
