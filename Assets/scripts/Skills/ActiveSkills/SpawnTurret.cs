// Copyright (c) 2015, Lukas Honke
// ========================
using System;
using System.Collections;
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
	public class SpawnTurret : ActiveSkill
	{
		private GameObject activeProjectile;

		private Monster minion;

		private float lifetime;

		private Coroutine despawnTask;

		public SpawnTurret()
		{
			castTime = 0f;
			reuse = 1;
			coolDown = 0;
			baseDamage = 15;

			range = 12;

			lifetime = 60;

			requireConfirm = true;
			AvailableToPlayer = true;
		}

		public override SkillId GetSkillId()
		{
			return SkillId.SpawnTurret;
		}

		public override string GetVisibleName()
		{
			return "Spawn Turret";
		}

		public override string GetDescription()
		{
			return "Click on ground and spawn turret.";
		}

		public override Skill Instantiate()
		{
			return new SpawnTurret();
		}

		public override SkillEffect[] CreateEffects(int param)
		{
			return new SkillEffect[] {  };
		}

		public override void InitTraits()
		{
			AddTrait(SkillTraits.SpawnMinion);
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

			if (minion != null)
			{
				minion.DoDie();
				minion = null;
			}

			Vector3 pozice = Camera.main.ScreenToWorldPoint(Input.mousePosition);

			Player player = GameSystem.Instance.CurrentPlayer;

			Monster m = GameSystem.Instance.SpawnMonster("RangedTurret", new Vector3(pozice.x, pozice.y, 0.0f), false, 1, 1);
			WorldHolder.instance.activeMap.RegisterMonsterToMap(m);

			minion = m;

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
