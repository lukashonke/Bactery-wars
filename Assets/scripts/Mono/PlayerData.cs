using System.Collections;
using Assets.scripts.Actor;
using Assets.scripts.Skills;
using UnityEngine;

namespace Assets.scripts.Mono
{
	/*
		Unity Engine delegate for Player objects
	*/
	public class PlayerData : MonoBehaviour
	{
		public Player player;

		public int hp;
		public int moveSpeed;
		public int rotateSpeed;
		public bool canMoveWhenNotRotated;

		public void Start()
		{
			player = GameSystem.Instance.RegisterNewPlayer(this, "Player");
			Debug.Log("Registering new data for player " + player.Name);
		}

		public void Update()
		{
			player.OnUpdate();
		}

		public void LaunchSkill(int key)
		{
			// select the skill mapped to the key
			Skill skill = player.Skills.GetSkill(key-1);
			if (skill == null)
			{
				Debug.Log("NPE nemuzu najit skill " + key);
				return;
			}

			Debug.Log("Launching skill... " + skill.Name);

			// cast this skill
			player.CastSkill(skill);
		}

		public void SetMoveSpeed(int speed)
		{
			moveSpeed = speed;
		}

		public void SetRotateSpeed(int speed)
		{
			rotateSpeed = speed;
		}

		public void SetHp(int newHp)
		{
			hp = newHp;
        }

		public void AbortSkills()
		{
			player.BreakCasting();
		}
	}
}
