// Copyright (c) 2015, Lukas Honke
// ========================
using System;
using System.IO;
using Assets.scripts.Actor;
using Assets.scripts.Fort;
using Assets.scripts.Mono.MapGenerator;
using Assets.scripts.Mono.MapGenerator.Levels;
using Assets.scripts.Mono.ObjectData;
using Assets.scripts.Skills;
using Assets.scripts.Upgrade;
using Assets.scripts.Upgrade.Classic;
using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

namespace Assets.scripts.Mono
{
	public class PlayerControls : MonoBehaviour
	{
		public GameObject body;

		public PlayerData data;
		public PlayerUI ui;
		private Rigidbody2D rb;

		// attached object to display moving to pos
		public GameObject mouseClicker;
		private GameObject currMouseClicker;

		// projectile
		public GameObject projectObject;

		// lightning
		public GameObject spotlight;

		// animators
		private Animator anim;

		private GameObject circleTarget;
		private GameObject currentCircleObject;

		public int currentTouchAction;
		public const int TOUCH_MOVEMENT = 1;
		public const int TOUCH_CONFIRMINGSKILL = 2;
		public const int TOUCH_ZOOM = 3;
		public const int TOUCH_SHIFTINGSKILLBAR = 4;

		// Use this for initialization
		public void Start()
		{
			body = gameObject;
			rb = body.GetComponent<Rigidbody2D>();
			anim = body.GetComponent<Animator>();
			data = GetComponent<PlayerData>();
			ui = GetComponent<PlayerUI>();

			circleTarget = Resources.Load<GameObject>("Sprite/misc/CircleTarget");
		}

		public void FixedUpdate()
		{
		}

		private int temp = 0;

		private void HandleSkillControls()
		{
			if (Input.GetKeyDown(KeyCode.Tab))
			{
				ui.SwitchConsole();
			}

			if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
			{
				ui.OnEnter();
			}

			if (!ui.consoleActive)
			{
				if (Input.GetKeyDown(KeyCode.Q))
				{
					data.StartMeleeTargeting(false);
				}

				if (data.ActiveConfirmationSkill == null && Input.GetMouseButtonDown(0) && !ui.MouseOverUI)
				{
					data.StartMeleeTargeting(true);
				}

				if (Input.GetKeyDown(KeyCode.W))
				{
					EquippableItem u = new HpUpgradeAdd(1);
					u.Init();
					u.SpawnGameObject(Utils.GenerateRandomPositionAround(data.GetBody().transform.position, 3));
					//data.GetOwner().AddUpgrade(u);
					//data.GetOwner().EquipUpgrade(u);
				}

				if (Input.GetKeyDown(KeyCode.I))
				{
					ui.SwitchInventory();
				}

				if (Input.GetKeyDown(KeyCode.U))
				{
					Player p = data.GetOwner() as Player;
					p.UnlockSkill(temp++, true);

					//ui.DamageMessage(data.GetBody(), 10, Color.cyan);

					//ui.ScreenMessage("Ahoasdddddddddddddddddddsssssssssddddddddddddoj" + (temp++), 1);
					//data.AddPhysicsPush(new Vector2(0, 100), ForceMode2D.Impulse);
				}

				if (Input.GetKeyDown(KeyCode.E))
				{
					InventoryItem u = new DnaItem(Random.Range(10, 20));
					u.Init();
					u.SpawnGameObject(Utils.GenerateRandomPositionAround(data.GetBody().transform.position, 3));

					InventoryItem p = new HpPotion(1);
					p.Init();
					p.SpawnGameObject(Utils.GenerateRandomPositionAround(data.GetBody().transform.position, 3));
				}

				if (Input.GetKeyDown(KeyCode.B))
				{
					AbstractLevelData levelData = WorldHolder.instance.activeMap.levelData;

					if (levelData.CanHaveBase())
					{
						levelData.CreateBase();
					}
				}

				if (Input.GetKeyDown(KeyCode.L))
				{
					ui.ShowLevelsView();
				}

				if (Input.GetKeyDown(KeyCode.M))
				{
					ui.HideLevelsView();
				}

				if (Input.GetKeyDown(KeyCode.V))
				{
					AbstractLevelData levelData = WorldHolder.instance.activeMap.levelData;

					if (levelData.HasBase())
					{
						if (SiegeManager.IsSiegeActive() == false)
							SiegeManager.StartSiege(WorldHolder.instance.activeMap);
						else
							SiegeManager.CancelSiege();
					}
				}

				if (Input.GetKeyDown(KeyCode.R))
				{
					InventoryItem u = UpgradeTable.Instance.GenerateUpgrade(ItemType.CLASSIC_UPGRADE, 1, 2, 1);
					u.Init();
					u.SpawnGameObject(Utils.GenerateRandomPositionAround(data.GetBody().transform.position, 3));
				}

				/*if (Input.GetKeyDown(KeyCode.R))
				{
					AbstractUpgrade u = data.GetOwner().Inventory.GetUpgrade(typeof (TemplateUpgrade));
					data.GetOwner().UnequipUpgrade(u);
					data.GetOwner().RemoveUpgrade(u);

					u = data.GetOwner().Inventory.GetUpgrade(typeof(TemplateUpgrade));
					data.GetOwner().UnequipUpgrade(u);
					data.GetOwner().RemoveUpgrade(u);
				}*/

				if (Input.GetKeyDown(KeyCode.Alpha1))
				{
					data.LaunchSkill(1);
				}

				if (Input.GetKeyDown(KeyCode.Alpha2))
				{
					data.LaunchSkill(2);
				}

				if (Input.GetKeyDown(KeyCode.Alpha3))
				{
					data.LaunchSkill(3);
				}

				if (Input.GetKeyDown(KeyCode.Alpha4))
				{
					data.LaunchSkill(4);
				}

				if (Input.GetKeyDown(KeyCode.Alpha5))
				{
					data.LaunchSkill(5);
				}

				if (Input.GetKeyDown(KeyCode.Alpha6))
				{
					data.LaunchSkill(6);
				}

				if (Input.GetKeyDown(KeyCode.Alpha7))
				{
					data.LaunchSkill(7);
				}

				if (Input.GetKeyDown(KeyCode.Alpha8))
				{
					data.LaunchSkill(8);
				}

				if (Input.GetKeyDown(KeyCode.Alpha9))
				{
					data.LaunchSkill(9);
				}

				if (Input.GetKeyDown(KeyCode.Alpha0))
				{
					data.LaunchSkill(10);
				}
			}
		}

		private void KeyboardMovement()
		{
			data.SetKeyboardMovement(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
		}

		public void Update()
		{
			bool usingTouches = false;

#if UNITY_STANDALONE || UNITY_EDITOR

			// fire TODO delete
			if (Input.GetKeyDown("space"))
			{
				//Instantiate(projectObject, body.transform.position, body.transform.rotation);
			}

			if (Input.GetKeyDown(KeyCode.Escape))
			{
				data.BreakCasting();
			}

			KeyboardMovement();

			HandleSkillControls();
#endif

#if UNITY_ANDROID
			usingTouches = true;
#endif

		    if (ui.adminMode)
		    {
		        if (!ui.MouseOverUI && Input.GetMouseButtonDown(0))
		        {
                    ui.AdminClick(Camera.main.ScreenToWorldPoint(Input.mousePosition), 0);
		        }

                if (!ui.MouseOverUI && Input.GetMouseButtonDown(1))
		        {
		            ui.AdminClick(Camera.main.ScreenToWorldPoint(Input.mousePosition), 1);
		        }

		        return;
		    }


			Vector3 inputPosition;
			bool touched = false;
			Touch firstTouch = new Touch();

			if (!usingTouches)
			{
				inputPosition = Input.mousePosition;
			}
			else
			{
				if (Input.touchCount > 0)
				{
					touched = true;
					inputPosition = Input.GetTouch(0).position;
					firstTouch = Input.GetTouch(0);

					if (firstTouch.phase.Equals(TouchPhase.Began))
					{
						currentTouchAction = 0;
					}
					else if (firstTouch.phase.Equals(TouchPhase.Ended))
					{
						currentTouchAction = 0;
					}
				}
				else
				{
					inputPosition = new Vector3(0, 0, 0);
					currentTouchAction = 0;
				}
			}

			if (usingTouches)
			{
				/*if (ui.MouseOverUI && touched)
				{
					Debug.Log(firstTouch.position);
					if (firstTouch.phase.Equals(TouchPhase.Began))
					{
						//currentTouchAction = TOUCH_SHIFTINGSKILLBAR;
					}

					if (firstTouch.phase.Equals(TouchPhase.Moved))
					{
						Vector2 move = firstTouch.deltaPosition;

						foreach (GameObject butObject in ui.skillButtons)
						{
							butObject.transform.position = butObject.transform.position + new Vector3(move.x, 0, 0);
						}
					}
				}*/

				if (!ui.MouseOverUI)
				{
					// if targetting active, highlight target objects
					if (data.TargettingActive)
					{
						Vector3 temp = Camera.main.ScreenToWorldPoint(inputPosition);
						RaycastHit2D[] hits = Physics2D.RaycastAll(new Vector2(temp.x, temp.y), Vector2.zero, 0f);

						int layer;
						Rigidbody2D rb;

						bool target = false;

						foreach (RaycastHit2D hit in hits)
						{
							layer = hit.transform.gameObject.layer;
							// not target projectiles, environment and background
							if (layer == 11 || layer == 8 || layer == 9)
								continue;

							if (hit.transform.gameObject.Equals(data.GetBody()))
								continue;

							rb = hit.transform.gameObject.GetComponent<Rigidbody2D>();

							if (rb != null)
							{
								data.Target = hit.transform.gameObject;
								target = true;
								break;
							}
						}

						if (!target)
						{
							data.Target = null;
						}
					}

					if (data.ActiveConfirmationSkill != null)
					{
						bool breakMouseMovement = data.ActiveConfirmationSkill.breaksMouseMovement;

						if (touched && firstTouch.phase.Equals(TouchPhase.Began))
						{
							currentTouchAction = TOUCH_CONFIRMINGSKILL;
							Vector3 temp = Camera.main.ScreenToWorldPoint(inputPosition);
							temp.z = body.transform.position.z;
							data.lastClickPositionWorld = temp;

							data.ConfirmSkillLaunch(temp);
							Debug.Log("confirmed");
						}
						else
						{
							Debug.Log("not confirming");
						}

						if (Input.GetMouseButtonDown(1))
						{
							data.ActiveConfirmationSkill.AbortCast();
						}

						// pro tento snimek vypne dalsi Input, aby nedoslo k pohybu za targetem skillu
						if (breakMouseMovement)
						{
							Input.ResetInputAxes();
						}
					}
					else
					{
						if (data.Target != null)
						{
							if (touched && firstTouch.phase.Equals(TouchPhase.Began))
							{
								Vector3 temp = Camera.main.ScreenToWorldPoint(inputPosition);
								temp.z = body.transform.position.z;
								data.lastClickPositionWorld = temp;

								data.MeleeInterract(data.Target, true);
								Input.ResetInputAxes();
							}
						}
						else
						{
							// change target position according to mouse when clicked
							//TODO this touchphase Began means jump skill will not continue movement 
							if (touched && ((currentTouchAction == 0 && firstTouch.phase.Equals(TouchPhase.Began)) || currentTouchAction == TOUCH_MOVEMENT))
							{
								if (currentTouchAction == 0)
									currentTouchAction = TOUCH_MOVEMENT;

								Vector3 newTarget = Camera.main.ScreenToWorldPoint(inputPosition);
								newTarget.z = body.transform.position.z;

								data.lastClickPositionWorld = newTarget;

								// momentalne nepotrebne
								//if (Vector3.Distance(body.transform.position, newTarget) > 2)
								//{
								data.HasTargetToMoveTo = true;
								data.SetPlayersMoveToTarget(newTarget);

								if (currMouseClicker != null)
									Destroy(currMouseClicker);

								currMouseClicker = Instantiate(mouseClicker, data.GetMovementTarget(), Quaternion.identity) as GameObject;
								//}
							}
						}
					}
				}
			}
			else
			{
				if (!ui.MouseOverUI)
				{
					// if targetting active, highlight target objects
					if (data.TargettingActive)
					{
						Vector3 temp = Camera.main.ScreenToWorldPoint(inputPosition);

						int layer;
						Rigidbody2D rb;

						bool target = false;

						if (!data.SkillTargetting)
						{
							if (currentCircleObject != null)
							{
								Destroy(currentCircleObject);
								currentCircleObject = null;
							}
						}

						if (data.SkillTargetting)
						{
							Collider2D[] hits = Physics2D.OverlapCircleAll(new Vector2(temp.x, temp.y), 2f);

							if (currentCircleObject == null)
							{
								currentCircleObject = Instantiate(circleTarget);
								currentCircleObject.transform.parent = data.transform;
								currentCircleObject.transform.position = data.GetBody().transform.position;
								currentCircleObject.transform.localScale = new Vector3(0.2f * data.SkillTargettingRange, 0.2f * data.SkillTargettingRange);
							}

							if (Utils.DistanceSqr(data.GetBody().transform.position, new Vector3(temp.x, temp.y)) <
							    (data.SkillTargettingRange*data.SkillTargettingRange))
							{
								GameObject circleObj = Instantiate(circleTarget);
								circleObj.transform.position = new Vector3(temp.x, temp.y, 0);
								circleObj.transform.localScale = new Vector3(0.2f, 0.2f);
								Destroy(circleObj, 0.05f);
							}

							float dist = 99999999;

							foreach (Collider2D hit in hits)
							{
								layer = hit.gameObject.layer;
								// not target projectiles, environment and background
								if (layer == 11 || layer == 8 || layer == 9)
									continue;

								if (hit.gameObject.Equals(data.GetBody()))
									continue;

								rb = hit.gameObject.GetComponent<Rigidbody2D>();

								if (rb != null)
								{
									Character ch = hit.gameObject.GetChar();

									if (ch == null)
										continue;

									float distBetween = Utils.DistanceSqr(hit.gameObject.transform.position, new Vector3(temp.x, temp.y));

									if (data.SkillTargettingRange > 0 &&
										Utils.DistanceSqr(hit.gameObject.transform.position, data.GetBody().transform.position) > (data.SkillTargettingRange*data.SkillTargettingRange))
										continue;

									if (data.GetOwner().CanAttack(ch) && data.SkillTargettingEnemiesOnly && distBetween < dist)
									{
										data.Target = hit.gameObject;
										target = true;
										dist = distBetween;
										//break;
									}
									else if (!data.GetOwner().CanAttack(ch) && !data.SkillTargettingEnemiesOnly && distBetween < dist)
									{
										data.Target = hit.gameObject;
										target = true;
										dist = distBetween;
										//break;
									}
								}
							}
						}
						else
						{
							RaycastHit2D[] hits = Physics2D.RaycastAll(new Vector2(temp.x, temp.y), Vector2.zero, 0f);

							foreach (RaycastHit2D hit in hits)
							{
								layer = hit.transform.gameObject.layer;
								// not target projectiles, environment and background
								if (layer == 11 || layer == 8 || layer == 9)
									continue;

								if (hit.transform.gameObject.Equals(data.GetBody()))
									continue;

								rb = hit.transform.gameObject.GetComponent<Rigidbody2D>();

								if (rb != null)
								{
									data.Target = hit.transform.gameObject;
									target = true;
									break;
								}
							}
						}

						if (!target)
						{
							data.Target = null;
						}
					}

					if (data.ActiveConfirmationSkill != null)
					{
						bool breakMouseMovement = data.ActiveConfirmationSkill.breaksMouseMovement;

						if (Input.GetMouseButtonDown(0))
						{
							Vector3 temp = Camera.main.ScreenToWorldPoint(inputPosition);
							temp.z = body.transform.position.z;
							data.lastClickPositionWorld = temp;

							data.ConfirmSkillLaunch();
						}

						if (Input.GetMouseButtonDown(1) && !data.ActiveConfirmationSkill.Equals(data.GetOwner().MeleeSkill)) //TODO temp solution for right click melee not cancelling
						{
							data.ActiveConfirmationSkill.AbortCast();
						}

						// pro tento snimek vypne dalsi Input, aby nedoslo k pohybu za targetem skillu
						if (breakMouseMovement)
						{
							Input.ResetInputAxes();
						}
					}
					else
					{
						if (data.Target != null)
						{
							if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
							{
								Vector3 temp = Camera.main.ScreenToWorldPoint(inputPosition);
								temp.z = body.transform.position.z;
								data.lastClickPositionWorld = temp;

								data.MeleeInterract(data.Target, true);
								Input.ResetInputAxes();

								data.MouseClicked();
							}
						}
						else
						{
							// change target position according to mouse when clicked
							if (Input.GetMouseButton(1))
							{
								Vector3 newTarget = Camera.main.ScreenToWorldPoint(inputPosition);
								newTarget.z = body.transform.position.z;

								data.lastClickPositionWorld = newTarget;

								// momentalne nepotrebne
								//if (Vector3.Distance(body.transform.position, newTarget) > 2)
								//{
								data.HasTargetToMoveTo = true;
								data.SetPlayersMoveToTarget(newTarget);

								if (currMouseClicker != null)
									Destroy(currMouseClicker);

								data.MoveButtonDown = true;

								currMouseClicker = Instantiate(mouseClicker, data.GetMovementTarget(), Quaternion.identity) as GameObject;
								//}
							}
							else
							{
								data.MoveButtonDown = false;
								if (data.moveOnlyWhenMousePressed && data.HasTargetToMoveTo && !data.forcedVelocity && data.allowMovePointChange)
									data.HasTargetToMoveTo = false;
							}
						}
					}
				}
			}

			if (!data.HasTargetToMoveTo)
			{
				if (currMouseClicker != null)
					Destroy(currMouseClicker);
			}

			Debug.DrawRay(body.transform.position, data.GetForwardVector()*10, Color.red);
		}
    }

}