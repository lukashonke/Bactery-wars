using System.Collections;
using System.Runtime.InteropServices;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.AI;
using Assets.scripts.Base;
using Assets.scripts.Mono.MapGenerator;
using Assets.scripts.Skills;
using Assets.scripts.Upgrade;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.scripts.Mono.ObjectData
{
	/*
		Unity Engine delegate for Player objects
	*/
	public class PlayerData : AbstractData
	{
		/// <summary>Datova trida hrace</summary>
		public Player player;

		[HideInInspector]
		public PlayerUI ui;

		/// <summary>skill ktery prave vyzaduje potvrzeni pred spustenim</summary>
		public ActiveSkill ActiveConfirmationSkill { get; set; }

		public bool TargettingActive { get; set; }

		public bool MoveButtonDown { get; set; }


		// test parameters
		public bool autoAttackTargetting = false;
		public bool castingBreaksMovement = true;
		public bool moveOnlyWhenMousePressed = false;

		public new void Start()
		{
			base.Start();

			TargettingActive = true;

			player = GameSystem.Instance.RegisterNewPlayer(this, "Player");

			if (aiType != null && !aiType.Equals("default"))
			{
				switch (aiType)
				{
					case "monster":
						player.ChangeAI(new DefaultMonsterAI(player));
						break;
					case "melee":
						player.ChangeAI(new MeleeMonsterAI(player));
						break;
					case "ranged":
						player.ChangeAI(new RangedMonsterAI(player));
						break;
				}
			}

			ui = GetComponent<PlayerUI>();

			IsVisibleToPlayer = true;

			Invoke("PostInit", 0.2f);

            Debug.Log("Registering new data for player " + player.Name);
		}

		public void PostInit()
		{
			player.UnlockSkill(0, false);

			bool noTutorial = WorldHolder.instance.skipTutorial;

			if(noTutorial)
			{
				for (int i = 1; i < 5; i++)
				{
					player.UnlockSkill(i, false);
				}

				player.SetLevel(2);
			}

			ui.ShowHelpWindow(Messages.ShowHelpWindow("game_start", 0.1), 0);

			if (GameSystem.Instance.Controller.isAndroid)
			{
				
			}
			else
			{
				ui.ShowHelpWindow(Messages.ShowHelpWindow("game_start_controls"), 0);
			}
		}

		public new void Awake()
		{
			base.Awake();
		}

		public override void Update()
		{
			// if the player is waiting to confirm skill casting, call the skills method to render the confirmation elements (eg. arrow to select where the skill should be casted, etc)
			if (ActiveConfirmationSkill != null)
			{
				ActiveConfirmationSkill.OnBeingConfirmed();
			}

			// updatnout pohyb
			base.Update();
		}

		public override void SetOwner(Character ch)
		{
			player = (Player) ch;
		}

		public override void OnCollisionEnter2D(Collision2D coll)
		{
			base.OnCollisionEnter2D(coll);
		}

		public override void OnCollisionExit2D(Collision2D coll)
		{

		}

		public override void OnCollisionStay2D(Collision2D coll)
		{

		}

		public override void OnTriggerEnter2D(Collider2D obj)
		{
			base.OnTriggerEnter2D(obj);
		}

		public override void OnTriggerExit2D(Collider2D obj)
		{

		}

		public override void OnTriggerStay2D(Collider2D obj)
		{

		}

		public override void SetSkillReuseTimer(ActiveSkill activeSkill, bool reset=false)
		{
			if(reset)
				ui.ResetReuseTimer(activeSkill);
			else
				ui.SetReuseTimer(activeSkill);
		}

		/// <summary>
		/// Spusti i-tý skill hrace (vola se po stisknuti klavesy 1-5)
		/// </summary>
		/// <param name="key">1-5</param>
		public void LaunchSkill(int key)
		{
			// select the skill mapped to the key
			Skill skill = player.Skills.GetSkill(key-1);
			if (skill == null)
			{
				Debug.Log("NPE nemuzu najit skill " + key);
				return;
			}

#if UNITY_ANDROID
			if (ActiveConfirmationSkill != null && ActiveConfirmationSkill.Equals(skill))
			{
				ActiveConfirmationSkill.AbortCast();
				return;
			}
#endif

			//Debug.Log("Launching skill... " + skill.Name);

			//MovementChanged();

			// cast this skill
			player.CastSkill(skill);
		}

		public void ConfirmSkillLaunch()
		{
			if(ActiveConfirmationSkill.CanUse() && GetOwner().CanCastSkill(ActiveConfirmationSkill))
				ActiveConfirmationSkill.Start(Target);
		}

		public void ConfirmSkillLaunch(Vector3 mousePosition)
		{
			ActiveConfirmationSkill.Start(mousePosition);
		}

		public void SetPlayersMoveToTarget(GameObject newTarget)
		{
			AbortMeleeAttacking();

			if (!allowMovePointChange)
				return;

			if (ActiveConfirmationSkill != null && ActiveConfirmationSkill.MovementBreaksConfirmation)
			{
				ActiveConfirmationSkill.AbortCast();
			}

			SetMovementTarget(newTarget);
		}

		private void CheckSkillsToAbort()
		{
			foreach (Skill sk in GetOwner().Skills.Skills)
			{
				if (sk is ActiveSkill)
				{
					ActiveSkill s = (ActiveSkill) sk;

					if (s.IsActive() && s.movementAbortsSkill)
					{
						s.AbortCast();
					}
				}
			}
		}

		public bool SetPlayersMoveToTarget(Vector3 newTarget)
		{
			AbortMeleeAttacking();
			CheckSkillsToAbort();

			if (!allowMovePointChange)
				return false;

			if (Utils.IsNotAccessible(GetBody().transform.position, newTarget))
				return false;

			if (ActiveConfirmationSkill != null && ActiveConfirmationSkill.MovementBreaksConfirmation)
			{
				ActiveConfirmationSkill.AbortCast();
			}

			return SetMovementTarget(newTarget);
		}

		public void HighlightTarget(GameObject target, bool enable)
		{
			if (!(GetOwner().AI is PlayerAI))
				return;

			if (target == null)
				return;

			SpriteRenderer sr = target.GetComponent<SpriteRenderer>();

			if (sr == null)
				return;

			Material mat = sr.material;

			if (enable)
			{
				//sr.material.SetColor("_Emission", new Color(0.2f, 0.2f, 0.14f));
				sr.material.color = Color.red;
			}
			else
			{
				//sr.material.SetColor("_Emission", Color.black);
				sr.material.color = Color.white;
			}
		}

		public void StartMeleeTargeting(bool rightClick)
		{
			if (ActiveConfirmationSkill != null && !ActiveConfirmationSkill.Equals(GetOwner().MeleeSkill))
			{
				BreakCasting();

				if (rightClick)
					return;
			}

			if (GetOwner().MeleeSkill != null && GetOwner().CanCastSkill(GetOwner().MeleeSkill) && GetOwner().MeleeSkill.CanUse())
			{
				GetOwner().MeleeSkill.DoAutoattack();
			}
		}

		public void OpenShopUI(ShopData data)
		{
			ui.ShowShopView(data);
		}

		public override void UpdateInventory(Inventory inv)
		{
			if(ui != null)
				ui.UpdateInventory(inv);
		}

		public override void UpdateStats() //TODO call this on level change, hp change etc
		{
			if(ui != null)
				ui.UpdateStatsInfo();
		}

		public override Character GetOwner()
		{
			return player;
		}
	}
}
