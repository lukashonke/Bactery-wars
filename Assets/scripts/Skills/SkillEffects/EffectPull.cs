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
	public class EffectPull : SkillEffect
	{
		protected int force;
		protected int pullDamage;

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

			if (source.CanAttack(targetCh))
			{
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
	}
}
