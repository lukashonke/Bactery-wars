﻿using System.Collections;
using Assets.scripts.Actor;
using Assets.scripts.Base;
using Assets.scripts.Skills;
using UnityEngine;

namespace Assets.scripts.Mono.ObjectData
{
	/*
		Unity Engine delegate for Player objects
	*/
	public class PlayerData : AbstractData, ICollidable
	{
		/// <summary>GameObject reprezentujici fyzicke a graficke telo hrace </summary>
		public GameObject body;
		public Rigidbody2D rb;
		private Animator anim;
		public GameObject particleSystems;

		/// <summary>GameObject reprezentujici relativni pozici tesne pred hracem</summary>
		public GameObject shootingPosition;

		/// <summary>Datova trida hrace</summary>
		public Player player;

		/// <summary>Vektor reprezentujici otoceni/natoceni (= heading) hrace</summary>
		private Vector3 heading;

		// current position in world cords to move to
		private Vector3 targetPositionWorld;

		/// setting this to false will stop the current player movement
		public bool HasTargetToMoveTo { get; set; }

		private bool allowPlayerMovePointChange;
		private bool forcedVelocity;

		public ActiveSkill ActiveConfirmationSkill { get; set; }
		public ParticleSystem castingEffects;

		// zastupne promenne GameObjektu (reflektuji datove hodnoty v tride Player)
		public int visibleHp;
		public int moveSpeed;
		public int rotateSpeed;
		public float minDistanceClickToMove;

		/// <summary>
		/// true pokud se hrac muze pohybovat i kdyz jeste neni natoceny ke svemu cili, 
		/// pokud je nastaveno na false, hrac se nebude pohybovat smerem ke vsemu cili dokud k nemu nebude natoceny
		/// </summary>
		public bool canMoveWhenNotRotated;

		/// <summary>true pokud se hrac muze hybat (nastavuje se na false napriklad pri kouzleni)</summary>
		public bool movementEnabled = true;

		/// <summary>true pokud se hrac muze otacet (nastavuje se na false napriklad pri kouzleni)</summary>
		public bool rotationEnabled = true;

		public bool IsCasting { get; set; }

		public new void Start()
		{
			base.Start();

			body = GetChildByName("Body");
			rb = body.GetComponent<Rigidbody2D>();
			anim = body.GetComponent<Animator>();
			shootingPosition = GetChildByName("Shooting Position");
            castingEffects = GetChildByName("CastingEffect").GetComponent<ParticleSystem>(); //TODO delete

			particleSystems = GetChildByName("ParticleSystems");

			player = GameSystem.Instance.RegisterNewPlayer(this, "Player");

			IsCasting = false;
			HasTargetToMoveTo = false;
			allowPlayerMovePointChange = true;
			forcedVelocity = false;

            Debug.Log("Registering new data for player " + player.Name);
		}

		public override void JumpForward(float dist, float jumpSpeed)
		{
			if (USE_VELOCITY_MOVEMENT)
			{
				ForceSetMoveDestinaton(body.transform.position + GetForwardVector()*dist, 0.5f);
				ForceSetVelocity(GetForwardVector() * jumpSpeed, 0.5f);
				HasTargetToMoveTo = true;
				UpdateHeading();
			}
			else
			{
				HasTargetToMoveTo = false;
				SetPosition(Vector3.MoveTowards(body.transform.position, body.transform.position + GetForwardVector() * dist, dist), false);
				UpdateHeading();
			}
		}

		public override void JumpForward(Vector3 direction, float dist, float jumpSpeed)
		{
			SetRotation(body.transform.position + direction.normalized*dist, true);

			if (USE_VELOCITY_MOVEMENT)
			{
				ForceSetMoveDestinaton(body.transform.position + direction.normalized * dist, 0.5f);
				ForceSetVelocity(direction.normalized * jumpSpeed, 0.5f);
				HasTargetToMoveTo = true;
				UpdateHeading();
			}
			else
			{
				HasTargetToMoveTo = false;
				SetPosition(Vector3.MoveTowards(body.transform.position, body.transform.position + direction.normalized * dist, dist), false);
				UpdateHeading();
			}
		}

		public void Update()
		{
			/*if (!castingEffects.isPlaying)
				castingEffects.Play(true);

			// update effects
			if (IsCasting)
			{
				if (!castingEffects.enableEmission)
				{
					castingEffects.enableEmission = true;
                }
			}
			else
			{
				if (castingEffects.enableEmission)
				{
					castingEffects.enableEmission = false;
				}
			}*/

			// if the player is waiting to confirm skill casting, call the skills method to render the confirmation elements (eg. arrow to select where the skill should be casted, etc)
			if (ActiveConfirmationSkill != null)
			{
				ActiveConfirmationSkill.OnBeingConfirmed();
			}

			// update movement
			// move to mouse
			if (HasTargetToMoveTo && Vector3.Distance(body.transform.position, targetPositionWorld) > minDistanceClickToMove)
			{
				Quaternion newRotation = Quaternion.LookRotation(body.transform.position - targetPositionWorld, Vector3.forward);
				newRotation.x = 0;
				newRotation.y = 0;

				float angle = Quaternion.Angle(body.transform.rotation, newRotation);

				bool move = true;
				bool rotate = true;

				if (angle - 90 > 1 && !canMoveWhenNotRotated)
					move = false;

				if (!CanMove())
					move = false;

				if (!CanRotate())
					rotate = false;

				if (move)
				{
					anim.SetFloat("MOVE_SPEED", 1);

					if (USE_VELOCITY_MOVEMENT)
					{
						Vector3 newVelocity = targetPositionWorld - body.transform.position;
						newVelocity.Normalize();

						SetVelocity(newVelocity*moveSpeed);
					}
					else
					{
						SetPosition(Vector3.MoveTowards(body.transform.position, targetPositionWorld, Time.deltaTime*moveSpeed), false);
					}
				}
				else
				{
					ResetVelocity();
				}

				if (rotate)
				{
					SetRotation(Quaternion.Slerp(body.transform.rotation, newRotation, Time.deltaTime * rotateSpeed), false);
				}

				if (move || rotate)
				{
					UpdateHeading();
				}
			}
			else
			{
				anim.SetFloat("MOVE_SPEED", 0);
				HasTargetToMoveTo = false;

				// the player had fixed position and velocity to move to, if he is there already, unfix this
				if (!allowPlayerMovePointChange && forcedVelocity)
				{
					allowPlayerMovePointChange = true;
					forcedVelocity = false;
				}

				ResetVelocity();
			}

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

		public void OnTriggerEnter2D(Collider2D obj)
		{
		}

		public void OnTriggerExit2D(Collider2D obj)
		{
		}

		public void OnTriggerStay2D(Collider2D obj)
		{
		}

		public void BreakMovement()
		{
			HasTargetToMoveTo = false;
		}

		/// <summary>
		/// Forces player to move towards this direction (clicking on screen wont change it)
		/// After 'duration' passes, unforces it
		/// 
		/// works only if USE_VELOCITY_MOVEMENT = true
		/// </summary>
		public void ForceSetMoveDestinaton(Vector3 newDest, float duration)
		{
			if (USE_VELOCITY_MOVEMENT == false)
				return;

			allowPlayerMovePointChange = false;
			targetPositionWorld = newDest;

			IEnumerator task = ScheduleResetAllowPlayerMovement(duration);
			StartCoroutine(task);
		}

		private IEnumerator ScheduleResetAllowPlayerMovement(float duration)
		{
			yield return new WaitForSeconds(duration);

			allowPlayerMovePointChange = true;

			yield return null;
		}

		/// <summary>
		/// Forces the object to have this velocity for 'duration' seconds
		/// 
		/// works only if USE_VELOCITY_MOVEMENT = true
		/// </summary>
		public void ForceSetVelocity(Vector3 newVel, float duration)
		{
			if (USE_VELOCITY_MOVEMENT == false)
				return;

			forcedVelocity = true;
			rb.velocity = newVel;

			IEnumerator task = ScheduleUnforceVelocity(duration);
			StartCoroutine(task);
		}

		private IEnumerator ScheduleUnforceVelocity(float duration)
		{
			yield return new WaitForSeconds(duration);

			ForceVelocityUnset();

			yield return null;
		}

		public void ForceVelocityUnset()
		{
			forcedVelocity = false;
		}

		public void SetVelocity(Vector3 newVel)
		{
			if (forcedVelocity)
				return;

			rb.velocity = newVel;
		}

		public void SetPosition(Vector3 newPos, bool updateHeading)
		{
			body.transform.position = newPos;
			if(updateHeading)
				UpdateHeading();
		}

		public void SetRotation(Quaternion newRot, bool updateHeading)
		{
			body.transform.rotation = newRot;

			if (updateHeading)
				UpdateHeading();
		}

		public void SetRotation(Vector3 target, bool updateHeading)
		{
			Quaternion newRotation = Quaternion.LookRotation(body.transform.position - target, Vector3.forward);
			newRotation.x = 0;
			newRotation.y = 0;

			SetRotation(newRotation, updateHeading);
		}

		/// <summary>
		/// Vrati true pokud se hrac muze pohybovat (nemuze se pohybovat napriklad kdyz kouzli skill, ktery vyzaduje aby hrac stal na miste)
		/// </summary>
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

		/// <summary>
		/// Vrati true pokud se hrac muze otacet
		/// </summary>
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

			Debug.Log("Launching skill... " + skill.Name);

			// cast this skill
			player.CastSkill(skill);
		}

		public void SetMovementEnabled(bool val)
		{
			movementEnabled = val;
        }

		public void SetRotationEnabled(bool val)
		{
			rotationEnabled = val;
		}

		public void SetMoveSpeed(int speed)
		{
			moveSpeed = speed;
		}

		public void SetRotateSpeed(int speed)
		{
			rotateSpeed = speed;
		}

		public void SetVisibleHp(int newHp)
		{
			visibleHp = newHp;
        }

		public void BreakCasting()
		{
			player.BreakCasting();
		}

		public void UpdateHeading()
		{
			float angleRad = (body.transform.rotation.eulerAngles.z + 90) * Mathf.Deg2Rad;
			SetHeading(new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad), 0));
		}

		public void ResetVelocity()
		{
			rb.velocity = Vector3.zero;
		}

		private void SetHeading(Vector3 v)
		{
			heading = v;
		}

		public void SetPlayersMoveToTarget(Vector3 newTarget)
		{
			if (!allowPlayerMovePointChange)
				return;

			if (ActiveConfirmationSkill != null && ActiveConfirmationSkill.MovementBreaksConfirmation)
			{
				ActiveConfirmationSkill.AbortCast();
			}

			targetPositionWorld = newTarget;
		}

		public Vector3 GetMovementTarget()
		{
			return targetPositionWorld;
		}

		public override GameObject GetBody()
		{
			return body;
		}

		public override GameObject GetParticleSystemObject()
		{
			return particleSystems;
		}

		/// <summary>
		/// Vrati vektor smeru hrace
		/// </summary>
		/// <returns></returns>
		public override Vector3 GetForwardVector()
		{
			return heading;
		}

		/// <summary>
		/// Vrati vektor smeru hrace ke kteremu se pricte uhel
		/// </summary>
		public override Vector3 GetForwardVector(int angle)
		{
			// 1. moznost
			//Vector3 nv = Quaternion.AngleAxis(angle, Vector3.forward) * heading.normalized;

			// 2. moznost - asi je lepsi
			Vector3 nv = Quaternion.Euler(new Vector3(0, 0, angle)) * heading;
			return nv;
		}

		public override GameObject GetShootingPosition()
		{
			return shootingPosition;
		}

		public void ConfirmSkillLaunch()
		{
			ActiveConfirmationSkill.Start();
		}
	}
}
