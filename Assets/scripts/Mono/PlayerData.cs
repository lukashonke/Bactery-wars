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

		public bool movementEnabled = true;
		public bool rotationEnabled = true;

		public void Start()
		{
			player = GameSystem.Instance.RegisterNewPlayer(this, "Player");
			Debug.Log("Registering new data for player " + player.Name);
		}

		public void Update()
		{
			player.OnUpdate();
		}

		public void OnCollisionEnter2D(Collision2D coll)
		{
		}

		public void OnCollisionExit2D(Collision2D coll)
		{
		}

		public void OnCollisionStay2D(Collision2D coll)
		{
		}

		public bool CanMove()
		{
			if (!movementEnabled)
				return false;

			foreach (Skill skill in player.Skills.Skills)
			{
				if (skill is ActiveSkill)
				{
					// at least one active skill blocks movement
					if (((ActiveSkill) skill).IsBeingCasted() && !((ActiveSkill) skill).CanMove())
					{
						return false;
					}
				}
			}

			return true;
		}

		public bool CanRotate()
		{
			if (!rotationEnabled)
				return false;

			foreach (Skill skill in player.Skills.Skills)
			{
				if (skill is ActiveSkill)
				{
					// at least one active skill blocks movement
					if (((ActiveSkill)skill).IsBeingCasted() && !((ActiveSkill)skill).CanRotate())
					{
						return false;
					}
				}
			}

			return true;
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

		public void SetMovementEnabled(bool val)
		{
			movementEnabled = val;
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

		public void SetRotationEnabled(bool var)
		{
			rotationEnabled = var;
		}
	}
}
