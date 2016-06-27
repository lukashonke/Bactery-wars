// Copyright (c) 2015, Lukas Honke
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
	public class EffectPushaway : SkillEffect
	{
		protected int force;
		protected bool affectFriendly = false;

		public EffectPushaway(int force)
		{
			this.force = force;
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

				Vector3 direction = target.transform.position - source.GetData().GetBody().transform.position;
				direction.Normalize();

				Rigidbody2D targetRb = target.GetComponent<Rigidbody2D>();

				if (targetRb != null)
				{
					targetCh.GetData().AddPhysicsPush(new Vector2(direction.x, direction.y)*force, ForceMode2D.Impulse, source);
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
			return new SkillTraits[] { SkillTraits.Push, };
		}
	}

	public class EffectPushawayAll : EffectPushaway
	{
		public EffectPushawayAll(int force) : base(force)
		{
			affectFriendly = true;
		}
	}
}
