using System;
using System.Collections;
using System.Collections.Generic;
using Assets.scripts.Actor;
using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.Base;
using Assets.scripts.Mono;
using Assets.scripts.Mono.ObjectData;
using Assets.scripts.Skills.Base;
using Assets.scripts.Skills.SkillEffects;
using UnityEngine;

namespace Assets.scripts.Skills.ActiveSkills
{
	public class MucusWarrior : ActiveSkill
	{
		private Monster minion;

		private float lifetime;

		public MucusWarrior()
		{
			castTime = 0f;
			reuse = 10f;
			coolDown = 0;
			requireConfirm = false;

			lifetime = 60f;
		}

		public override SkillId GetSkillId()
		{
			return SkillId.MucusWarrior;
		}

		public override string GetVisibleName()
		{
			return "Mucus Warrior";
		}

		public override Skill Instantiate()
		{
			return new MucusWarrior();
		}

		public override SkillEffect[] CreateEffects()
		{
			return null;
		}

		public override void InitTraits()
		{
			AddTrait(SkillTraits.SpawnMinion);
		}

		public override bool OnCastStart()
		{
			if(castTime > 0)
				CreateCastingEffect(true, "SkillTemplate");

			return true;
		}

		public override void OnLaunch()
		{
			DeleteCastingEffect();

			//ApplyEffects(Owner, Owner.GetData().gameObject);

			Spawn();
		}

		private Coroutine despawnTask;

		private void Spawn()
		{
			GameObject mo = CreateSkillObject("MucusWarrior", false, true);

			mo.transform.position = Utils.GenerateRandomPositionAround(GetOwnerData().GetBody().transform.position, 5);
			mo.RegisterAsMonster();

			minion = mo.GetChar() as Monster;
			Owner.AddSummon(minion);

			despawnTask = Owner.StartTask(ScheduleDespawn());
		}

		private IEnumerator ScheduleDespawn()
		{
			yield return new WaitForSeconds(lifetime);

			Despawn();
		}

		private void Despawn()
		{
			if (minion != null)
			{
				minion.GetData().SetIsDead(true);
			}
		}

		public override void OnFinish()
		{

		}

		public override void MonoUpdate(GameObject gameObject)
		{
			Character ch = gameObject.GetChar();

			if (ch == null)
				return;

			if (ch.Status.IsDead)
			{
				if (despawnTask != null)
				{
					Owner.StopTask(despawnTask);
				}
			}
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

		public override void MonoTriggerExit(GameObject gameObject, Collider2D other)
		{
			
		}

		public override void MonoTriggerStay(GameObject gameObject, Collider2D other)
		{
			
		}

		public override void MonoCollisionEnter(GameObject gameObject, Collision2D coll)
		{
			
		}

		public override void MonoCollisionExit(GameObject gameObject, Collision2D coll)
		{
		}

		public override void MonoCollisionStay(GameObject gameObject, Collision2D coll)
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
			if (IsBeingCasted())
				return false;
			return true;
		}

		public override bool CanRotate()
		{
			return CanMove();
		}
	}
}
