using System;
using System.Collections;
using Assets.scripts.Actor;
using Assets.scripts.Base;
using Assets.scripts.Skills;
using UnityEngine;

namespace Assets.scripts.Mono
{
	/*
		Unity Engine delegate for Player objects
	*/
	public class PlayerData : AbstractData, ICollidable
	{
		/// <summary>GameObject reprezentujici fyzicke a graficke telo hrace </summary>
		public GameObject body;

		/// <summary>GameObject reprezentujici relativni pozici tesne pred hracem</summary>
		public GameObject shootingPosition;

		/// <summary>Datova trida hrace</summary>
		public Player player;

		/// <summary>Vektor reprezentujici otoceni/natoceni (= heading) hrace</summary>
		private Vector3 heading;

		/// setting this to false will stop the current player movement
		public bool HasTargetToMoveTo { get; set; }

		public ParticleSystem castingEffects;

		// zastupne promenne GameObjektu (reflektuji datove hodnoty v tride Player)
		public int visibleHp;
		public int moveSpeed;
		public int rotateSpeed;

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
			shootingPosition = GetChildByName("Shooting Position");
            castingEffects = GetChildByName("Casting Effect").GetComponent<ParticleSystem>();

			player = GameSystem.Instance.RegisterNewPlayer(this, "Player");

			IsCasting = false;
			HasTargetToMoveTo = false;

            Debug.Log("Registering new data for player " + player.Name);
		}

		public override void JumpForward(float dist, float jumpSpeed)
		{
			HasTargetToMoveTo = false;
			MoveToPosition(Vector3.MoveTowards(body.transform.position, body.transform.position + GetForwardVector()*dist, dist), false);
			UpdateHeading();
		}

		public void Update()
		{
			if (!castingEffects.isPlaying)
				castingEffects.Play(true);

			if (IsCasting)
			{
				if (!castingEffects.enableEmission)
				{
					castingEffects.enableEmission = true;
                }
				//castingEffects.Play(true);
			}
			else
			{
				if (castingEffects.enableEmission)
				{
					castingEffects.enableEmission = false;
					//castingEffects.Pause(true);
				}
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

		public void ShootProjectileForward(string folderName, string name, int plusAngle)
		{
			GameObject go = Resources.Load(folderName + "/" + name) as GameObject;
			if (go == null)
				throw new NullReferenceException("cannot find " + folderName + "/" + name + " !");

			GameObject newProjectile = Instantiate(go, shootingPosition.transform.position, body.transform.rotation) as GameObject;

			if (newProjectile != null)
			{
				newProjectile.tag = gameObject.tag;

				Rigidbody2D rb = newProjectile.GetComponent<Rigidbody2D>();
				rb.velocity = (GetForwardVector(plusAngle) *15);

				Debug.DrawRay(shootingPosition.transform.position, rb.velocity, Color.green, 5f);

				Destroy(newProjectile, 5f);
			}
		}

		public void BreakMovement()
		{
			HasTargetToMoveTo = false;
		}

		public void MoveToPosition(Vector3 newPos, bool updateHeading)
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

		private void SetHeading(Vector3 v)
		{
			heading = v;
		}

		/// <summary>
		/// Vrati vektor smeru hrace
		/// </summary>
		/// <returns></returns>
		public Vector3 GetForwardVector()
		{
			return heading;
		}

		/// <summary>
		/// Vrati vektor smeru hrace ke kteremu se pricte uhel
		/// </summary>
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
