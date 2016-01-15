using System.Collections;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.AI;
using Assets.scripts.Base;
using Assets.scripts.Skills;
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

		public PlayerUI ui;

		/// <summary>skill ktery prave vyzaduje potvrzeni pred spustenim</summary>
		public ActiveSkill ActiveConfirmationSkill { get; set; }

		public bool TargettingActive { get; set; }

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
					case "meleeMonster":
						player.ChangeAI(new MeleeMonsterAI(player));
						break;
					case "rangedMonster":
						player.ChangeAI(new RangedMonsterAI(player));
						break;
				}
			}

			ui = GetComponent<PlayerUI>();

		    IsVisibleToPlayer = true;

            Debug.Log("Registering new data for player " + player.Name);
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

		}

		public override void OnTriggerExit2D(Collider2D obj)
		{

		}

		public override void OnTriggerStay2D(Collider2D obj)
		{

		}

		public override void SetSkillReuseTimer(ActiveSkill activeSkill)
		{
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

		public bool SetPlayersMoveToTarget(Vector3 newTarget)
		{
			AbortMeleeAttacking();

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

		public override Character GetOwner()
		{
			return player;
		}
	}
}
