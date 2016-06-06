﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Mono.MapGenerator;
using Assets.scripts.Skills.Base;
using Assets.scripts.Skills.SkillEffects;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.scripts.Skills.ActiveSkills
{
	public class SpecialSkill : ActiveSkill
	{
		private GameObject targettedPlayer;
		private GameObject activeProjectile;

		public SpecialSkill()
		{
			castTime = 0f;
			reuse = 1;
			coolDown = 0;
			baseDamage = 15;

			range = 12;

			requireConfirm = true;
			AvailableToPlayer = true;
		}

		public override SkillId GetSkillId()
		{
			return SkillId.SpecialSkill;
		}

		public override string GetVisibleName()
		{
			return "SpecialSkill";
		}

		public override string GetDescription()
		{
			return "Click on ground and spawn turret.";
		}

		public override Skill Instantiate()
		{
			return new SpecialSkill();
		}

		public override SkillEffect[] CreateEffects(int param)
		{
			return new SkillEffect[] { new EffectPull(100),  };
		}

		public override void InitTraits()
		{
			AddTrait(SkillTraits.Damage);
			AddTrait(SkillTraits.Missile);
		}

		public override void OnBeingConfirmed()
		{
			StartPlayerTargetting(); // TODO nakreslit kolecko ktery se hodi tam kde se klikne a odtamtud to vybere target automaticky pokud neni zadnej jinej vybranej
		}

		public override bool OnCastStart()
		{
			

			RotatePlayerTowardsMouse();

			if(castTime > 0)
				CreateCastingEffect(true);

			return true;
		}

		public override void OnLaunch()
		{
			DeleteCastingEffect();

		    Vector3 pozice = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Player player = GameSystem.Instance.CurrentPlayer;

            Monster m = GameSystem.Instance.SpawnMonster("Turret", new Vector3(pozice.x, pozice.y, 0.0f), false, 1, 1);

            WorldHolder.instance.activeMap.RegisterMonsterToMap(m);

        }

        public override void UpdateLaunched()
		{
		}

		public override void OnAbort()
		{
		}

		public override void OnFinish()
		{
		}

		public override void MonoUpdate(GameObject gameObject, bool fixedUpdate)
		{
		}

		public override void MonoTriggerEnter(GameObject gameObject, Collider2D coll)
		{
		}

		public override bool CanMove()
		{
			return !IsBeingCasted();
		}

		public override bool CanRotate()
		{
			return !IsBeingCasted();
		}
	}
}
