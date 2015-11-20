using System.Collections;
using Assets.scripts.Actor;
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

		/// <summary>skill ktery prave vyzaduje potvrzeni pred spustenim</summary>
		public ActiveSkill ActiveConfirmationSkill { get; set; }

		public new void Start()
		{
			base.Start();

			player = GameSystem.Instance.RegisterNewPlayer(this, "Player");

            Debug.Log("Registering new data for player " + player.Name);
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

		public override void OnCollisionEnter2D(Collision2D coll)
		{

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

			//Debug.Log("Launching skill... " + skill.Name);

			// cast this skill
			player.CastSkill(skill);
		}

		public void ConfirmSkillLaunch()
		{
			ActiveConfirmationSkill.Start();
		}

		public void SetPlayersMoveToTarget(Vector3 newTarget)
		{
			if (!allowMovePointChange)
				return;

			if (ActiveConfirmationSkill != null && ActiveConfirmationSkill.MovementBreaksConfirmation)
			{
				ActiveConfirmationSkill.AbortCast();
			}

			targetPositionWorld = newTarget;
		}

		public override Character GetOwner()
		{
			return player;
		}
	}
}
