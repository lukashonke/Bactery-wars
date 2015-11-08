using System;
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
		public GameObject body;
		public GameObject shootingPosition;
		public Player player;

		private Vector3 heading;

		public int hp;
		public int moveSpeed;
		public int rotateSpeed;
		public bool canMoveWhenNotRotated;

		public bool movementEnabled = true;
		public bool rotationEnabled = true;

		public void Start()
		{
			body = GameObject.Find("Body");
			shootingPosition = GameObject.Find("Shooting Position");
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

		public void ShootProjectileForward(string folderName, string name)
		{
			GameObject go = Resources.Load(folderName + "/" + name) as GameObject;
			if (go == null)
				throw new NullReferenceException("cannot find " + folderName + "/" + name + " !");

			GameObject newProjectile = Instantiate(go, shootingPosition.transform.position, body.transform.rotation) as GameObject;

			Rigidbody2D rb = null;

			if (newProjectile != null)
			{
				rb = newProjectile.GetComponent<Rigidbody2D>();
				rb.velocity = newProjectile.transform.position + heading*15;

				Debug.DrawRay(shootingPosition.transform.position, rb.velocity, Color.red, 5f);

				Destroy(newProjectile, 5f);
			}

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

		public void UpdateHeading(Vector3 v)
		{
			heading = v;
		}

		public Vector3 GetForwardVector()
		{
			return heading;
		}

		// forward vector + fixed angle
		public Vector3 GetForwardVector(int angle)
		{
			// 1. moznost
			//Vector3 nv = Quaternion.AngleAxis(angle, Vector3.forward) * heading.normalized;

			// 2. moznost - asi je lepsi
			Vector3 nv = Quaternion.Euler(new Vector3(0, 0, angle)) * heading;
			return nv;
		}
	}
}
