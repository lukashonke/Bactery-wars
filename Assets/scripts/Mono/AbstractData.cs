using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Base;
using Assets.scripts.Skills;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace Assets.scripts.Mono
{
	/// <summary>
	/// The class for all graphical/physical objects in Unity
	/// 
	/// kazdy nosic tohoto objektu musi mit pod sebou objekty:
	/// "body" pro reprezentaci fyzickeho tela
	/// body musi mit Animator
	/// "Shooting position" pro reprezentaci pozice ze ktere vychazeji projektily a efekty
	/// "ParticleSystems" pro efekty
	/// </summary>
	public abstract class AbstractData : MonoBehaviour, ICollidable
	{
		public bool USE_VELOCITY_MOVEMENT;

		// ovlivnuje presnost ovladani zejmena hrace (pokud je objekt blize ke svemu cili nez je tato vzdalenost, pohyb se zastavi)
		public float minDistanceClickToMove;
		
		// child objects mapped by name
		protected Dictionary<string, GameObject> childs;

		/// <summary>GameObjecty reprezentujici fyzicke a graficke telo objektu </summary>
		public GameObject body;
		public Rigidbody2D rb;
		protected Animator anim;
		public GameObject particleSystems;
		public Healthbar healthBar;

		/// <summary>GameObject reprezentujici relativni pozici ze ktere vychazeji strely a nektere efekty</summary>
		public GameObject shootingPosition;

		// zastupne promenne objektu
		public int visibleMaxHp;
		public int visibleHp;
		public int moveSpeed;
		public int rotateSpeed;

		public bool isDead;

		/// <summary>Vektor reprezentujici otoceni/natoceni (= heading) objektu</summary>
		protected Vector3 heading;

		// current position in world cords to move to
		protected Vector3 targetPositionWorld;

		/// setting this to false will stop the current player movement
		public bool HasTargetToMoveTo { get; set; }

		protected bool allowMovePointChange;
		protected bool forcedVelocity;

		/// <summary>
		/// true pokud se objekt muze pohybovat i kdyz jeste neni natoceny ke svemu cili, 
		/// pokud je nastaveno na false, objekt se nebude pohybovat smerem ke svemu cili dokud k nemu nebude natoceny
		/// </summary>
		public bool canMoveWhenNotRotated;

		/// <summary>true pokud se objekt muze hybat (nastavuje se na false napriklad pri kouzleni)</summary>
		public bool movementEnabled = true;

		/// <summary>true pokud se objekt muze otacet (nastavuje se na false napriklad pri kouzleni)</summary>
		public bool rotationEnabled = true;

		public bool IsCasting { get; set; }

		protected AbstractData()
		{
			childs = new Dictionary<string, GameObject>();
		}

		public void Start()
		{
			// loads all the child objects
			AddChildObjects(transform);

			body = gameObject;

			rb = body.GetComponent<Rigidbody2D>();
			anim = body.GetComponent<Animator>();
			shootingPosition = GetChildByName("Shooting Position");
			particleSystems = GetChildByName("ParticleSystems");

			if (GetChildByName("Healthbar") != null)
			{
				healthBar = GetChildByName("Healthbar").GetComponent<Healthbar>();
				if (healthBar != null)
				{
					healthBar.hp = visibleHp;
					healthBar.maxHp = visibleMaxHp;
				}
			}

			IsCasting = false;
			HasTargetToMoveTo = false;
			allowMovePointChange = true;
			forcedVelocity = false;
		}

		public virtual void Update()
		{
			// update movement
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

						SetVelocity(newVelocity * moveSpeed);
					}
					else
					{
						SetPosition(Vector3.MoveTowards(body.transform.position, targetPositionWorld, Time.deltaTime * moveSpeed), false);
					}
				}
				else
				{
					ResetVelocity();
				}

				if (rotate)
				{
					SetRotation(Quaternion.Slerp(body.transform.rotation, newRotation, Time.deltaTime*rotateSpeed), false);
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
				if (!allowMovePointChange && forcedVelocity)
				{
					allowMovePointChange = true;
					forcedVelocity = false;
				}

				ResetVelocity();
			}

			GetOwner().OnUpdate();
		}

		private void AddChildObjects(Transform t)
		{
			foreach (Transform child in t)
			{
				childs.Add(child.gameObject.name, child.gameObject);
				AddChildObjects(child);
			}
        }

		protected GameObject GetChildByName(string n)
		{
			GameObject val;

			childs.TryGetValue(n, out val);

			return val;
		}

		/// <summary>
		/// Clones gameobject prefab from Resources/prefabs/[type]/[resourceFolderName]/[fileName].prefab
		/// </summary>
		public GameObject LoadResource(string type, string resourceFolderName, string fileName)
		{
			GameObject go = Resources.Load("Prefabs/" + type + "/" + resourceFolderName + "/" + fileName) as GameObject;

			if (go == null)
				throw new NullReferenceException("Prefabs/" + type + "/" + resourceFolderName + "/" + fileName + " !");

			return go;
		}

		public GameObject InstantiateObject(GameObject template)
		{
			return Instantiate(template, GetBody().transform.position, GetBody().transform.rotation) as GameObject;
		}

		public GameObject InstantiateObject(GameObject template, Vector3 position)
		{
			return Instantiate(template, position, GetBody().transform.rotation) as GameObject;
		}

		public GameObject InstantiateObject(GameObject template, Vector3 position, Quaternion rotation)
		{
			return Instantiate(template, position, rotation) as GameObject;
;		}

		public void SetChild(GameObject o)
		{
			o.transform.parent = GetBody().transform;
		}

		/// <summary>
		/// Instantiates an object with from: Resources/Prefabs/skill/[resourceFolderName]/[fileName].prefab 
		/// Places it into [spawnPosition]
		/// [makeChild] will fix the position to the characters body
		/// </summary>
		public GameObject CreateSkillResource(string resourceFolderName, string fileName, bool makeChild, Vector3 spawnPosition)
		{
			GameObject go = LoadResource("skill", resourceFolderName, fileName);

			GameObject newObject = Instantiate(go, spawnPosition, GetBody().transform.rotation) as GameObject;

			if (newObject != null)
			{
				if (makeChild)
					SetChild(newObject);

				newObject.tag = gameObject.tag;
			}

			return newObject;
		}

		public void JumpForward(float dist, float jumpSpeed)
		{
			if (USE_VELOCITY_MOVEMENT)
			{
				ForceSetMoveDestinaton(body.transform.position + GetForwardVector() * dist, 0.5f);
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

		public void JumpForward(Vector3 direction, float dist, float jumpSpeed)
		{
			SetRotation(body.transform.position + direction.normalized * dist, true);

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

			allowMovePointChange = false;
			targetPositionWorld = newDest;

			IEnumerator task = ScheduleResetAllowPlayerMovement(duration);
			StartCoroutine(task);
		}

		private IEnumerator ScheduleResetAllowPlayerMovement(float duration)
		{
			yield return new WaitForSeconds(duration);

			allowMovePointChange = true;

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
			if (updateHeading)
				UpdateHeading();
		}

		public void SetRotation(Quaternion newRot, bool updateHeading)
		{
			if (USE_VELOCITY_MOVEMENT)
				rb.transform.rotation = newRot;
			else
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
		/// Vrati true pokud se objekt muze pohybovat (nemuze se pohybovat napriklad kdyz kouzli skill, ktery vyzaduje aby objekt stal na miste)
		/// </summary>
		public bool CanMove()
		{
			if (!movementEnabled)
				return false;

			foreach (Skill skill in GetOwner().Skills.Skills)
			{
				if (skill is ActiveSkill)
				{
					// at least one active skill blocks movement
					if (((ActiveSkill)skill).IsActive() && !((ActiveSkill)skill).CanMove())
					{
						return false;
					}
				}
			}

			return true;
		}

		/// <summary>
		/// Vrati true pokud se objekt muze otacet
		/// </summary>
		public bool CanRotate()
		{
			if (!rotationEnabled)
				return false;

			foreach (Skill skill in GetOwner().Skills.Skills)
			{
				if (skill is ActiveSkill)
				{
					// at least one active skill blocks movement
					if (((ActiveSkill)skill).IsActive() && !((ActiveSkill)skill).CanRotate())
					{
						return false;
					}
				}
			}

			return true;
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

			if (healthBar != null)
			{
				healthBar.hp = visibleHp;
			}
		}

		public void SetVisibleMaxHp(int newHp)
		{
			visibleMaxHp = newHp;

			if (healthBar != null)
			{
				healthBar.maxHp = visibleMaxHp;
			}
		}
		

		public virtual void SetIsDead(bool isDead)
		{
			if (isDead)
			{
				Debug.Log(name + " died");
				Destroy(gameObject, 5f);

				if (healthBar != null)
				{
					healthBar.enabled = false;
				}
			}
		}

		public void BreakCasting()
		{
			GetOwner().BreakCasting();
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

		public void SetMovementTarget(Vector3 newTarget)
		{
			targetPositionWorld = newTarget;
		}

		public Vector3 GetMovementTarget()
		{
			return targetPositionWorld;
		}

		public GameObject GetBody()
		{
			return body;
		}

		public GameObject GetParticleSystemObject()
		{
			return particleSystems;
		}

		/// <summary>
		/// Vrati vektor smeru objektu
		/// </summary>
		/// <returns></returns>
		public Vector3 GetForwardVector()
		{
			return heading;
		}

		/// <summary>
		/// Vrati vektor smeru objektu ke kteremu se pricte uhel
		/// </summary>
		public Vector3 GetForwardVector(int angle)
		{
			// 1. moznost
			//Vector3 nv = Quaternion.AngleAxis(angle, Vector3.forward) * heading.normalized;

			// 2. moznost - asi je lepsi
			Vector3 nv = Quaternion.Euler(new Vector3(0, 0, angle)) * heading;
			return nv;
		}

		public GameObject GetShootingPosition()
		{
			return shootingPosition;
		}

		public abstract Character GetOwner();
		public abstract void OnCollisionEnter2D(Collision2D coll);
		public abstract void OnCollisionExit2D(Collision2D coll);
		public abstract void OnCollisionStay2D(Collision2D coll);
		public abstract void OnTriggerEnter2D(Collider2D obj);
		public abstract void OnTriggerExit2D(Collider2D obj);
		public abstract void OnTriggerStay2D(Collider2D obj);
	}
}
