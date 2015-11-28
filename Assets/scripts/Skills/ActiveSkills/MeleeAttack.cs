using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Skills.SkillEffects;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.scripts.Skills.ActiveSkills
{
	public class MeleeAttack : ActiveSkill
	{
		public MeleeAttack(string name, int id) : base(name, id)
		{
			castTime = 0.5f;
			coolDown = 0f;
			reuse = 0;

			range = 5f;
		}

		public override Skill Instantiate()
		{
			return new MeleeAttack(Name, Id);
		}

		public override SkillEffect[] CreateEffects()
		{
			return new SkillEffect[] { new EffectDamage(5, 0) };
		}

		public override bool OnCastStart()
		{
			if (initTarget == null)
				return false;

			Debug.Log(Vector3.Distance(GetOwnerData().GetBody().transform.position, initTarget.transform.position));

			if (Vector3.Distance(GetOwnerData().GetBody().transform.position, initTarget.transform.position) < range)
			{
				GetOwnerData().StartMeleeAnimation();
				return true;
			}
			else
			{
				// dont melee, too far
				return false;
			}
		}

		public override void OnLaunch()
		{
			GetOwnerData().StopMeleeAnimation();
		}

		public override void OnFinish()
		{
			if (initTarget != null)
			{
				ApplyEffects(Owner, initTarget);
				GetOwnerData().MeleeAttack(initTarget); // continue with next attack
			}
		}

		public override void MonoUpdate(GameObject gameObject)
		{
		}

		public override void MonoStart(GameObject gameObject)
		{
		}

		public override void MonoDestroy(GameObject gameObject)
		{
		}

		public override void MonoTriggerEnter(GameObject gameObject, Collider2D other)
		{
		}

		public override void UpdateLaunched()
		{
		}

		public override void OnAbort()
		{
		}

		public override bool CanMove()
		{
			return !IsBeingCasted();
		}

		public override bool CanRotate()
		{
			return CanMove();
		}
	}
}
