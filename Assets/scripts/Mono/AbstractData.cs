using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.Base;
using Assets.scripts.Mono.MapGenerator;
using Assets.scripts.Mono.ObjectData;
using Assets.scripts.Skills;
using Pathfinding;
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
		public bool USE_VELOCITY_MOVEMENT = true;
		public bool keyboardMovementAllowed;
		public bool rotateTowardsMouse;
		public string aiType;
		public bool usesPathfinding;

		// ovlivnuje presnost ovladani zejmena hrace (pokud je objekt blize ke svemu cili nez je tato vzdalenost, pohyb se zastavi)
		public float minDistanceClickToMove = 0.2f;
		public int nextWaypointDistance = 3;

		// child objects mapped by name
		protected Dictionary<string, GameObject> childs;

		/// <summary>GameObjecty reprezentujici fyzicke a graficke telo objektu </summary>
		public GameObject body;

		public Rigidbody2D rb;
		protected Animator anim;
		public GameObject particleSystems;
		public Healthbar healthBar;

		private Seeker seeker;

		public bool IsVisibleToPlayer { get; set; }

		private GameObject target;

		public GameObject Target
		{
			get { return target; }
			set
			{
				if (target != null)
				{
					if (target.Equals(value))
						return;

					if (this is PlayerData)
						((PlayerData) this).HighlightTarget(target, false);
				}

				target = value;

				if (this is PlayerData)
					((PlayerData) this).HighlightTarget(target, true);
			}
		}

		/// <summary>GameObject reprezentujici relativni pozici ze ktere vychazeji strely a nektere efekty</summary>
		[HideInInspector] public GameObject shootingPosition;

		// zastupne promenne objektu
		public int visibleMaxHp;
		public int visibleHp;
		public int moveSpeed;
		public int rotateSpeed;
		public int level;

		[HideInInspector] public bool isDead;

		/// <summary>Vektor reprezentujici otoceni/natoceni (= heading) objektu</summary>
		protected Vector3 heading;

		// velocity saved on fixedupate - used when calculating collision damage
		protected Vector3 lastVelocity;

		// current position in world cords to move to
		protected Vector3 targetPositionWorld;
		protected GameObject targetMoveObject;

		protected bool keyboardMoving;
		protected float keyboardHorizontalMovement;
		protected float keyboardVerticalMovement;

		/// setting this to false will stop the current player movement
		public bool HasTargetToMoveTo { get; set; }

		[HideInInspector] public Vector3 lastClickPositionWorld;

		public bool RepeatingMeleeAttack { get; set; }
		public bool QueueMelee { get; set; }
		public bool QueueMeleeRepeat { get; set; }
		public GameObject QueueMeleeTarget { get; set; }
		private bool fixedRotation;

		public bool allowMovePointChange;
		public bool forcedVelocity;

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

			if (particleSystems == null)
				particleSystems = body;

			if (shootingPosition == null)
				shootingPosition = body;

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
			QueueMelee = false;
			allowMovePointChange = true;
			forcedVelocity = false;

			IsVisibleToPlayer = false;
			currentlyVisible = false;
			SetVisibility(false);
		}

		//TODO improve this - nemelo by to byt v jedne oddelene metode
		public void Awake()
		{
			seeker = GetComponent<Seeker>();
			if (usesPathfinding && seeker == null)
				Debug.LogError("object " + gameObject.name + " does not have a Seeker component yet it uses pathfinding");
		}

		protected Path currentPath;
		private int currentPathNode;

		private float lastRepath;
		public float repathRate = 0.25f;
		private Coroutine nextSearch;
		private bool searchingPath;

		private void SearchPath()
		{
			//Debug.Log("searching new path");

			if (HasTargetToMoveTo == false)
			{
				return;
			}

			seeker.StartPath(body.transform.position, targetPositionWorld, OnPathFindComplete);
			searchingPath = true;
		}

		private void CalculatePathfindingNodes()
		{
			if (!usesPathfinding)
				return;

			if (lastRepath + repathRate < Time.time)
			{
				SearchPath();
				lastRepath = Time.time;
			}
			else
			{
				if (nextSearch != null) // already scheduling next search as soon as possible
					return;

				if (Time.time - lastRepath < 0.1f)
					return;

				float repathIn = repathRate - (Time.time - lastRepath);
				nextSearch = StartCoroutine(ScheduleRepath(repathIn));
			}
		}

		private IEnumerator ScheduleRepath(float time)
		{
			yield return new WaitForSeconds(time);
			SearchPath();
			nextSearch = null;
		}

		public void OnPathFindComplete(Path p)
		{
			searchingPath = false;
			//Debug.Log("* calculated!");
			p.Claim(this);

			if (!p.error)
			{
				if (currentPath != null)
					currentPath.Release(this);

				currentPath = p;
				currentPathNode = 0;

				Vector3 p1 = Time.time - lastFoundWaypointTime < 0.3f ? lastFoundWaypointPosition : ((ABPath) p).originalStartPoint;
				Vector3 p2 = body.transform.position;
				Vector3 dir = p2 - p1;
				float magn = dir.magnitude;
				dir /= magn;
				int steps = (int) (magn/nextWaypointDistance);

				for (int i = 0; i <= steps; i++)
				{
					GetNextPathfindingNode(p1);
					p1 += dir;
				}
			}
			else
			{
				p.Release(this);
				//Debug.LogError(p.error);
			}
		}

		private void GetNextPathfindingNode(Vector3 positionFrom)
		{
			currentPathNode++;
		}

		public void ResetPath()
		{
			if (currentPath != null)
				currentPath.Release(this);

			currentPath = null;
			currentPathNode = 0;
		}

		private float lastCheckVelocityTime;
		private bool wasCloseTozero;
		private const float VELOCITY_CHECK_INTERVAL = 1f;

		private void CheckVelocityToStop()
		{
			if (HasTargetToMoveTo && lastCheckVelocityTime + VELOCITY_CHECK_INTERVAL < Time.time)
			{
				lastCheckVelocityTime = Time.time;

				if (Mathf.Abs(rb.velocity.x) < 0.1f && Mathf.Abs(rb.velocity.y) < 0.1f)
				{
					if (!wasCloseTozero)
					{
						wasCloseTozero = true;
						//Debug.Log("close to zero set to true");
					}
					else
					{
						//Debug.Log("breaking movementEnabled!");
						BreakMovement(true);
						wasCloseTozero = false;
					}
				}
				else
				{
					//Debug.Log("close to zero set to false...");
					wasCloseTozero = false;
				}
			}
		}

		private Vector3 lastFoundWaypointPosition;
		private float lastFoundWaypointTime;

		private bool currentlyVisible;

		public void SetVisibility(bool b)
		{
			if (!GameSystem.Instance.Controller.fogOfWar)
				return;

			if (healthBar != null)
			{
				healthBar.gameObject.SetActive(b);
			}

			foreach (GameObject o in childs.Values)
			{
				if (o.name.Equals("Die Effect") || o.transform.parent != null && o.transform.parent.name.Equals("Die Effect"))
					continue;

				o.SetActive(b);
			}

			body.GetComponent<SpriteRenderer>().enabled = b;
			body.GetComponent<Collider2D>().isTrigger = !b;
			currentlyVisible = b;
		}

		public struct PhysicsPush
		{
			public Vector2 force;
			public ForceMode2D mode;

			public PhysicsPush(Vector2 force, ForceMode2D mode)
			{
				this.force = force;
				this.mode = mode;
			}
		}

		private List<PhysicsPush> physicsPushes = new List<PhysicsPush>();
		public void AddPhysicsPush(Vector2 force, ForceMode2D mode)
		{
			physicsPushes.Add(new PhysicsPush(force, mode));
		}

		public virtual void FixedUpdate()
		{
			if (physicsPushes.Count > 0)
			{
				foreach (PhysicsPush p in physicsPushes)
				{
					rb.AddForce(p.force, p.mode);
				}

				physicsPushes.Clear();
			}

			lastVelocity = rb.velocity;
		}

		public virtual void Update()
		{
			if (currentlyVisible && !IsVisibleToPlayer)
			{
				SetVisibility(false);
			}
			else if (!currentlyVisible && IsVisibleToPlayer)
			{
				SetVisibility(true);
			}

			if (!HasTargetToMoveTo && keyboardMovementAllowed)
			{
				if (allowMovePointChange && CanMove())
				{
					Vector3 dir = new Vector3(keyboardHorizontalMovement*moveSpeed, keyboardVerticalMovement*moveSpeed, 0);
					SetVelocity(dir);

					if (CanRotate())
					{
						if (rotateTowardsMouse)
						{
							Vector3 temp = Camera.main.ScreenToWorldPoint(Input.mousePosition);

							Quaternion newRotation = Quaternion.LookRotation(body.transform.position - temp, Vector3.forward);
							newRotation.x = 0;
							newRotation.y = 0;

							SetRotation(Quaternion.Slerp(body.transform.rotation, newRotation, Time.deltaTime*rotateSpeed), false);

							UpdateHeading();
						}
						else if (keyboardMoving)
						{
							Quaternion newRotation = Quaternion.LookRotation(body.transform.position - (body.transform.position + dir),
								Vector3.forward);
							newRotation.x = 0;
							newRotation.y = 0;

							SetRotation(Quaternion.Slerp(body.transform.rotation, newRotation, Time.deltaTime*rotateSpeed), false);

							UpdateHeading();
						}
					}
				}
			}
			else
			{
				CheckVelocityToStop();

				// pokud mame kontrolovat vzdalenost od targetu, je treba obnovovat polohu targetu
				if (targetMoveObject != null && minRangeToTarget > 0)
				{
					targetPositionWorld = targetMoveObject.transform.position;

					CalculatePathfindingNodes();
				}

				float dist = Vector3.Distance(body.transform.position, targetPositionWorld);

				// update movement
				if (HasTargetToMoveTo && (dist > minRangeToTarget) && dist > minDistanceClickToMove)
				{
					Vector3 currentDestination = targetPositionWorld;
					if (usesPathfinding && currentPath != null && allowMovePointChange)
					{
						if (currentPathNode < 1)
							currentPathNode = 1;

						if (currentPathNode < currentPath.vectorPath.Count)
						{
							//Debug.Log("current node index " + currentPathNode);
							currentDestination = currentPath.vectorPath[currentPathNode];

							if (Utils.DistancePwr(body.transform.position, currentDestination) < nextWaypointDistance*nextWaypointDistance)
							{
								lastFoundWaypointPosition = body.transform.position;
								lastFoundWaypointTime = Time.time;
								currentPathNode++;
							}
						}
						else
							ResetPath();
					}

					Quaternion newRotation = Quaternion.LookRotation(body.transform.position - currentDestination, Vector3.forward);
					newRotation.x = 0;
					newRotation.y = 0;

					float angle = Quaternion.Angle(body.transform.rotation, newRotation);

					bool move = true;
					bool rotate = true;

					// no path available yet, dont move and wait
					if (usesPathfinding && currentPath == null && searchingPath)
					{
						rotate = false;
						move = false;
					}

					if (angle - 90 > 1 && !canMoveWhenNotRotated && !fixedRotation)
						move = false;

					if (!CanMove())
						move = false;

					if (!CanRotate())
						rotate = false;

					if (move)
					{
						if (this is PlayerData) anim.SetFloat("MOVE_SPEED", 1);

						if (USE_VELOCITY_MOVEMENT)
						{
							Vector3 newVelocity = currentDestination - body.transform.position;
							newVelocity.Normalize();

							SetVelocity(newVelocity*moveSpeed);
						}
						else
						{
							SetPosition(Vector3.MoveTowards(body.transform.position, currentDestination, Time.deltaTime*moveSpeed), false);
						}
					}
					else
					{
						ResetVelocity();
					}

					if (rotate)
					{
						SetRotation(Quaternion.Lerp(body.transform.rotation, newRotation, Time.deltaTime*rotateSpeed), false);
					}

					if (move || rotate)
					{
						UpdateHeading();
					}
				}
				else
				{
					if (this is PlayerData) anim.SetFloat("MOVE_SPEED", 0);

					HasTargetToMoveTo = false;

					ArrivedAtDestination();

					ResetVelocity();
				}
			}

			//TODO optimize
			if (rb.velocity.x > 0 || rb.velocity.y > 0)
			{
				WorldHolder.instance.PositionEnter(rb.transform.position);
			}

			try
			{
				GetOwner().OnUpdate();
			}
			catch (NullReferenceException)
			{
				if (this is EnemyData)
				{
					SetOwner(GameSystem.Instance.RegisterNewMonster((EnemyData) this, "Monster", ((EnemyData) this).monsterId, 1, null));
					Debug.LogWarning(name + " was not registered! Registering it implitely as Monster to template " +
					                 ((Monster) GetOwner()).Template.GetType().Name);
				}
			}
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
			;
		}

		public void SetChild(GameObject o)
		{
			o.transform.parent = GetBody().transform;
		}

		public GameObject CreateDirectionArrow(string resourceFolderName, string fileName, bool makeChild,
			Vector3 spawnPosition, int range)
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

		/// <summary>
		/// Instantiates an object with from: Resources/Prefabs/skill/[resourceFolderName]/[fileName].prefab 
		/// Places it into [spawnPosition]
		/// [makeChild] will fix the position to the characters body
		/// </summary>
		public GameObject CreateSkillResource(string resourceFolderName, string fileName, bool makeChild,
			Vector3 spawnPosition)
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
			MovementChanged();

			if (USE_VELOCITY_MOVEMENT)
			{
				HasTargetToMoveTo = true;
				ForceSetMoveDestinaton(body.transform.position + GetForwardVector()*dist, 0.5f);
				ForceSetVelocity(GetForwardVector()*jumpSpeed, 0.5f);
				UpdateHeading();
			}
			else
			{
				HasTargetToMoveTo = false;
				SetPosition(Vector3.MoveTowards(body.transform.position, body.transform.position + GetForwardVector()*dist, dist),
					false);
				UpdateHeading();
			}
		}

		public void MovementChanged()
		{
			if (QueueMelee)
				QueueMelee = false;

			foreach (Skill skill in GetOwner().Skills.Skills)
			{
				if (skill is ActiveSkill)
				{
					if (((ActiveSkill) skill).IsActive())
					{
						((ActiveSkill) skill).OnMove();
					}
				}
			}

			if (GetOwner().MeleeSkill != null)
			{
				if (GetOwner().MeleeSkill.IsActive())
					GetOwner().MeleeSkill.OnMove();
			}
		}

		public void JumpForward(Vector3 direction, float dist, float jumpSpeed)
		{
			MovementChanged();
			SetRotation(body.transform.position + direction.normalized*dist, true);

			if (USE_VELOCITY_MOVEMENT)
			{
				HasTargetToMoveTo = true;
				ForceSetMoveDestinaton(body.transform.position + direction.normalized*dist, 0.75f);
				ForceSetVelocity(direction.normalized*jumpSpeed, 0.75f);

				Debug.DrawLine(body.transform.position, body.transform.position + direction.normalized * dist, Color.red, 1f);

				UpdateHeading();
			}
			else
			{
				HasTargetToMoveTo = false;
				SetPosition(
				Vector3.MoveTowards(body.transform.position, body.transform.position + direction.normalized*dist, dist), false);
				UpdateHeading();
			}
		}

		public void BreakMovement(bool arrivedAtDestination)
		{
			// unfix the rotation
			fixedRotation = false;

			if (arrivedAtDestination)
			{
				targetMoveObject = null;
				ArrivedAtDestination();
			}

			if (usesPathfinding)
			{
				ResetPath();
			}

			minRangeToTarget = -1;
			MovementChanged();
			HasTargetToMoveTo = false;
		}

		private void ArrivedAtDestination()
		{
			// unfix the rotation
			fixedRotation = false;

			// the player had fixed position and velocity to move to, if he is there already, unfix this
			if (!allowMovePointChange && forcedVelocity)
			{
				allowMovePointChange = true;
				forcedVelocity = false;
			}

			if (usesPathfinding)
			{
				ResetPath();
			}

			minRangeToTarget = -1;
			targetMoveObject = null;

			if (QueueMelee)
			{
				MeleeInterract(QueueMeleeTarget, QueueMeleeRepeat);
				QueueMelee = false;
			}
		}

		/// <summary>
		/// Forces player to move towards this direction (clicking on screen wont change it)
		/// After 'duration' passes, unforces it
		/// 
		/// works only if USE_VELOCITY_MOVEMENT = true
		/// </summary>
		public void ForceSetMoveDestinaton(Vector3 newDest, float duration)
		{
			if (!USE_VELOCITY_MOVEMENT)
				return;

			MovementChanged();

			if (usesPathfinding)
				ResetPath();

			allowMovePointChange = false;
			targetPositionWorld = newDest;

			IEnumerator task = ScheduleResetAllowPlayerMovement(duration);
			StartCoroutine(task);
		}

		private IEnumerator ScheduleResetAllowPlayerMovement(float duration)
		{
			yield return new WaitForSeconds(duration);

			targetPositionWorld = lastClickPositionWorld;
			allowMovePointChange = true;
			CalculatePathfindingNodes();

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
			if (USE_VELOCITY_MOVEMENT) // TODO looks like this workso only for players
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
			if (GetOwner().Status.IsDead)
				return false;

			if (!movementEnabled)
				return false;

			if (GetOwner().Status.IsImmobilized())
				return false;

			foreach (Skill skill in GetOwner().Skills.Skills)
			{
				if (skill is ActiveSkill)
				{
					// at least one active skill blocks movement
					if (((ActiveSkill) skill).IsActive() && !((ActiveSkill) skill).CanMove())
					{
						return false;
					}
				}
			}

			if (GetOwner().MeleeSkill != null)
			{
				if (GetOwner().MeleeSkill.IsActive() && !GetOwner().MeleeSkill.CanMove())
					return false;
			}

			return true;
		}

		/// <summary>
		/// Vrati true pokud se objekt muze otacet
		/// </summary>
		public bool CanRotate()
		{
			if (GetOwner().Status.IsDead)
				return false;

			if (!rotationEnabled)
				return false;

			if (fixedRotation)
				return false;

			foreach (Skill skill in GetOwner().Skills.Skills)
			{
				if (skill is ActiveSkill)
				{
					// at least one active skill blocks movement
					if (((ActiveSkill) skill).IsActive() && !((ActiveSkill) skill).CanRotate())
					{
						return false;
					}
				}
			}

			if (GetOwner().MeleeSkill != null)
			{
				if (GetOwner().MeleeSkill.IsActive() && !GetOwner().MeleeSkill.CanMove())
					return false;
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

		public int GetMoveSpeed()
		{
			return moveSpeed;
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

		public void SetVisibleLevel(int val)
		{
			level = val;
		}

		public virtual void SetIsDead(bool isDead)
		{
			if (isDead)
			{
				DisableObjectData();

				Destroy(gameObject, 5f);

				DisableChildObjects();
			}
		}

		public virtual void DeleteMe()
		{
			GetOwner().DeleteMe();
			DisableObjectData();
			Destroy(gameObject);

			DisableChildObjects();
		}

		protected virtual void DisableChildObjects()
		{
			if (healthBar != null)
			{
				healthBar.gameObject.SetActive(false);
			}
		}

		public void DisableObjectData()
		{
			foreach (GameObject o in childs.Values)
			{
				if (o == null)
					continue;

				if (o.name.Equals("Die Effect") || o.transform.parent != null && o.transform.parent.name.Equals("Die Effect"))
					continue;

				o.SetActive(false);
			}

			body.GetComponent<SpriteRenderer>().enabled = false;
			body.GetComponent<Collider2D>().enabled = false;
		}

		public void BreakCasting()
		{
			GetOwner().BreakCasting();
		}

		public void UpdateHeading()
		{
			float angleRad = (body.transform.rotation.eulerAngles.z + 90)*Mathf.Deg2Rad;
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

		public void SetMovementTarget(GameObject newTarget)
		{
			MovementChanged();
			targetPositionWorld = newTarget.transform.position;
			targetMoveObject = newTarget;

			CalculatePathfindingNodes();
		}

		public bool SetMovementTarget(Vector3 newTarget)
		{
			MovementChanged();
			targetPositionWorld = newTarget;
			targetMoveObject = null;

			CalculatePathfindingNodes();
			return true;
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
			Vector3 nv = Quaternion.Euler(new Vector3(0, 0, angle))*heading;
			return nv;
		}

		public GameObject GetShootingPosition()
		{
			return shootingPosition;
		}

		private bool meleeAnimationActive;

		public void StartMeleeAnimation(float duration)
		{
			meleeAnimationActive = true;
		}

		public void StopMeleeAnimation()
		{
			meleeAnimationActive = false;
		}

		public bool IsMeleeActive()
		{
			return meleeAnimationActive;
		}

		public bool IsMeleeAttacking()
		{
			ActiveSkill sk = GetOwner().GetMeleeAttackSkill();

			return sk != null && sk.IsActive();
		}

		public void MeleeInterract(GameObject target, bool repeat)
		{
			if (target == null || gameObject == null)
				return;

			AbstractData data = target.GetComponent<AbstractData>();

			if (data == null || data.Equals(this))
				return;

			bool doAttack = true;
			if (data.GetOwner().IsInteractable())
			{
				doAttack = false;
			}

			ActiveSkill sk = GetOwner().GetMeleeAttackSkill();

			//TODO pridat zpetnou vazbu pokud melee utok jeste neni mozny - idealne ho zretezit co nejdrive nebo to vyresi animace?

			// no melee attack
			if (doAttack && (sk == null || sk.IsActive() || sk.IsBeingCasted() || !sk.CanUse()))
				return;

			int range = sk.range;
			if (!doAttack)
				range = 4;

			float d = Vector3.Distance(GetBody().transform.position, target.transform.position);

			if (Vector3.Distance(GetBody().transform.position, target.transform.position) < range)
			{
				if (!doAttack)
				{
					if (TalkTo(data))
						return;
				}
				else if (GetOwner().CanAttack(data.GetOwner()))
				{
					if (repeat)
						RepeatingMeleeAttack = true;

					BreakMovement(false);
					sk.Start(target);
				}
			}
			else
			{
				RepeatingMeleeAttack = false;
				MoveTo(target);

				if (range > 1)
					SetMinRangeToMoveToTarget(range);

				QueueMelee = true;
				QueueMeleeTarget = target;
				QueueMeleeRepeat = repeat;
			}
		}

		private bool TalkTo(AbstractData data)
		{
			if (data.isDead || isDead || GetOwner().Status.IsImmobilized() || GetOwner().Status.IsStunned())
				return false;

			if (data.GetOwner() is Npc)
			{
				Npc npc = (Npc) data.GetOwner();

				npc.Template.OnTalkTo(GetOwner());
			}

			return false;
		}

		public void AbortMeleeAttacking()
		{
			RepeatingMeleeAttack = false;
		}

		private float minRangeToTarget;

		public void SetMinRangeToMoveToTarget(float range)
		{
			minRangeToTarget = range;
		}

		public void MoveTo(GameObject target)
		{
			if (this is PlayerData)
			{
				HasTargetToMoveTo = true;
				((PlayerData) this).SetPlayersMoveToTarget(target);
			}
			else if (this is EnemyData)
			{
				HasTargetToMoveTo = true;
				((EnemyData) this).SetMovementTarget(target); //TODO might cause problems
			}
		}

		public bool MoveTo(Vector3 target, bool fixedRotation = false)
		{
			if (this is PlayerData)
			{
				HasTargetToMoveTo = true;
				this.fixedRotation = fixedRotation;
				return ((PlayerData) this).SetPlayersMoveToTarget(target);
			}
			else if (this is EnemyData)
			{
				HasTargetToMoveTo = true;
				this.fixedRotation = fixedRotation;
				return ((EnemyData) this).SetMovementTarget(target);
			}

			return false;
		}

		public void SetKeyboardMovement(float horizontal, float vertical)
		{
			if (keyboardMovementAllowed)
			{
				keyboardHorizontalMovement = horizontal;
				keyboardVerticalMovement = vertical;

				if (horizontal > 0 || vertical > 0 || horizontal < 0 || vertical < 0)
				{
					BreakMovement(false);
					keyboardMoving = true;
				}
				else
					keyboardMoving = false;
			}
		}

		public abstract Character GetOwner();
		public abstract void SetOwner(Character ch);

		public virtual void OnCollisionEnter2D(Collision2D coll)
		{
			// hit the wall
			if (coll.gameObject != null && coll.gameObject.name.Equals("Cave Generator"))
			{
				float velocity = lastVelocity.sqrMagnitude;

				if (velocity > 100*100)
				{
					velocity = lastVelocity.magnitude;

					int damage = (int) (velocity/100f*3);

					Debug.Log(gameObject.name + "received " + damage + " wallhit damage");

					GetOwner().ReceiveDamage(null, damage);
				}
			}

			foreach (Skill sk in GetOwner().Skills.Skills)
			{
				if (sk is ActiveSkill)
				{
					((ActiveSkill)sk).OnCollision(false, coll, coll.collider);
				}
			}

			if (targetMoveObject != null && targetMoveObject.Equals(coll.gameObject))
			{
				if (minRangeToTarget > -1)
				{
					return; // ignore, this is calculated 
				}

				BreakMovement(true);
			}
		}

		public abstract void OnCollisionExit2D(Collision2D coll);
		public abstract void OnCollisionStay2D(Collision2D coll);

		public virtual void OnTriggerEnter2D(Collider2D obj)
		{
			foreach (Skill sk in GetOwner().Skills.Skills)
			{
				if (sk is ActiveSkill)
				{
					((ActiveSkill)sk).OnCollision(true, null, obj);
				}
			}
		}

		public abstract void OnTriggerExit2D(Collider2D obj);
		public abstract void OnTriggerStay2D(Collider2D obj);

		public virtual void SetSkillReuseTimer(ActiveSkill activeSkill)
		{
		}
	}
}