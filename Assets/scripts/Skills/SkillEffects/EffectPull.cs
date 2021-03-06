﻿// Copyright (c) 2015, Lukas Honke
// ========================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Mono;
using Assets.scripts.Skills.Base;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.scripts.Skills.SkillEffects
{
	public class EffectPull : SkillEffect
	{
		protected int force;
		protected int pullDamage;

		protected bool affectFriendly = false;

		public EffectPull(int force, int pullDamage =0)
		{
			this.force = force;
			this.pullDamage = pullDamage;
		}

		public override void ApplyEffect(Character source, GameObject target)
		{
			Character targetCh = Utils.GetCharacter(target);

			if (targetCh == null)
				return;

			if (affectFriendly || source.CanAttack(targetCh))
			{
				if (targetCh.GetData().IsConnected)
					return;

				Vector3 direction = source.GetData().GetBody().transform.position - target.transform.position;
				direction.Normalize();

				Rigidbody2D targetRb = target.GetComponent<Rigidbody2D>();

				if (targetRb != null)
				{
					targetCh.GetData().AddPhysicsPush(new Vector2(direction.x, direction.y)*force, ForceMode2D.Impulse, source);
					targetCh.GetData().AddPull(source, pullDamage);
				}
			}
		}

		public override void Update()
		{
		}

		public override void OnRemove()
		{
		}

		public override SkillTraits[] GetTraits()
		{
			return new SkillTraits[] { SkillTraits.Pull, };
		}
	}

	public class EffectPullAll : EffectPull
	{
		public EffectPullAll(int force)
			: base(force)
		{
			affectFriendly = true;
		}
	}
}
