using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Mono;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.scripts.Skills.SkillEffects
{
	public class EffectPushaway : SkillEffect
	{
		protected int force;

		public EffectPushaway(int force)
		{
			this.force = force;
		}

		public override void ApplyEffect(Character source, GameObject target)
		{
			Character targetCh = Utils.GetCharacter(target);

			if (targetCh == null)
				return;

			if (source.CanAttack(targetCh))
			{
				Vector3 direction = target.transform.position - source.GetData().GetBody().transform.position;
				direction.Normalize();

				Rigidbody2D targetRb = target.GetComponent<Rigidbody2D>();

				if (targetRb != null)
				{
					targetCh.GetData().AddPhysicsPush(new Vector2(direction.x, direction.y)*force, ForceMode2D.Impulse);
				}
			}
		}

		public override void Update()
		{
		}

		public override void OnRemove()
		{
		}
	}
}
