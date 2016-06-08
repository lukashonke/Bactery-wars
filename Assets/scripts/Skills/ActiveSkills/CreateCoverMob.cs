// Copyright (c) 2015, Lukas Honke
// ========================
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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
	public class CreateCovermOB : ActiveSkill
	{
		private Monster[] minions;

		private float lifetime;
		public int countMinions = 5;

		public CreateCovermOB()
		{
			castTime = 1f;
			reuse = 60f;
			coolDown = 0;
			requireConfirm = false;
			canBeCastSimultaneously = true;

			lifetime = 60f;
		}

		public override SkillId GetSkillId()
		{
			return SkillId.CreateCoverMob;
		}

		public override string GetVisibleName()
		{
			return "Cover Mob";
		}

		public override Skill Instantiate()
		{
			return new CreateCovermOB();
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

			Spawn();
		}

		private Coroutine despawnTask;

		private void Spawn()
		{
			minions = new Monster[countMinions];
			for (int i = 0; i < countMinions; i++)
			{
				GameObject mo = CreateSkillObject("CoverMob", false, true);

				mo.transform.position = Utils.GenerateRandomPositionOnCircle(GetOwnerData().GetBody().transform.position, 5);
				mo.RegisterAsMonster();

				minions[i] = mo.GetChar() as Monster;

				Owner.AddSummon(minions[i]);
			}

			despawnTask = Owner.StartTask(ScheduleDespawn());
		}

		private IEnumerator ScheduleDespawn()
		{
			yield return new WaitForSeconds(lifetime);

			Despawn();
		}

		private void Despawn()
		{
			for (int i = 0; i < minions.Length; i++)
			{
				if (minions[i] != null)
				{
					minions[i].DoDie();
				}
			}
		}

		public override void OnFinish()
		{

		}

		public override void MonoUpdate(GameObject gameObject, bool fixedUpdate)
		{
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
