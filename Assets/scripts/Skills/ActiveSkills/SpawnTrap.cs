﻿using System;
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
	public class SpawnTrap : ActiveSkill
	{
		private Monster minion;

		private float lifetime;

		public SpawnTrap()
		{
			castTime = 1f;
			reuse = 10f;
			coolDown = 0;
			requireConfirm = false;
			canBeCastSimultaneously = true;

			lifetime = 60f;
			AvailableToPlayer = true;
		}

		public override SkillId GetSkillId()
		{
			return SkillId.SpawnTrap;
		}

		public override string GetDescription()
		{
			return "Spawns a trap.";
		}

		public override string GetVisibleName()
		{
			return "Summon Trap";
		}

		public override Skill Instantiate()
		{
			return new SpawnTrap();
		}

		public override SkillEffect[] CreateEffects(int param)
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
			Monster m = GameSystem.Instance.SpawnMonster("Trap", GetOwnerData().GetBody().transform.position, false, 1, 1);
			minion = m;

			//GameObject mo = CreateSkillObject("Warrior", false, true);

			//mo.transform.position = Utils.GenerateRandomPositionOnCircle(GetOwnerData().GetBody().transform.position, 5);
			//mo.RegisterAsMonster();

			//minion = mo.GetChar() as Monster;

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
				minion.DoDie();
			}
		}

		public override void OnFinish()
		{

		}

		public override void MonoUpdate(GameObject gameObject, bool fixedUpdate)
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

		public override void MonoTriggerEnter(GameObject gameObject, Collider2D coll)
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