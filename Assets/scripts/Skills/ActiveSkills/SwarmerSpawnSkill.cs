﻿// Copyright (c) 2015, Lukas Honke
// ========================
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Assets.scripts.Actor;
using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.Base;
using Assets.scripts.Mono;
using Assets.scripts.Mono.MapGenerator;
using Assets.scripts.Mono.ObjectData;
using Assets.scripts.Skills.Base;
using Assets.scripts.Skills.SkillEffects;
using UnityEngine;

namespace Assets.scripts.Skills.ActiveSkills
{
	public class SwarmerSpawnSkill : ActiveSkill
	{
		private Monster[] minions;

		public int countMinions = 1;

		public string mobToSpawn;

		public Monster lastSpawned;

		public SwarmerSpawnSkill()
		{
			castTime = 1f;
			reuse = 1f;
			coolDown = 0;
			requireConfirm = false;
			canBeCastSimultaneously = true;

			mobToSpawn = MonsterId.Lymfocyte_melee.ToString();
		}

		public override SkillId GetSkillId()
		{
			return SkillId.SwarmerSpawnSkill;
		}

		public override string GetVisibleName()
		{
			return "Swarmer Spawn";
		}

		public override Skill Instantiate()
		{
			return new SwarmerSpawnSkill();
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
			for (int i = 0; i < countMinions; i++)
			{
				Monster m = GameSystem.Instance.SpawnMonster(mobToSpawn, Utils.GenerateRandomPositionAround(Owner.GetData().GetBody().transform.position, 5, 3), false, 1);
				WorldHolder.instance.activeMap.RegisterMonsterToMap(m);
				lastSpawned = m;
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
