using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.Actor.PlayerClasses.Base;
using Assets.scripts.Base;
using Assets.scripts.Mono.MapGenerator;
using Assets.scripts.Mono.ObjectData;
using Assets.scripts.Skills;
using Assets.scripts.Upgrade;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Assets.scripts.Mono
{
	public sealed class PlayerUI : MonoBehaviour
	{
		public bool showObjectMessages = true;
		public bool showHelpWindows = true;

		private bool mouseOverUi = false;
		public bool MouseOverUI
		{
			get { return mouseOverUi || draggingIcon; }
		}

		public GameObject[] skillButtons;
		public Image[] skillProgresses;

		public bool adminMode;

		public Text hp;

		private PlayerData data;

		public GameObject gameMenu = null;
		public GameObject menuPanel = null;
		public GameObject settingsPanel = null;
		public GameObject inventoryPanel = null;

		public GameObject tooltipObject;
		public bool inventoryOpened = false;
		public const int INVENTORY_SIZE = 20;
		public const int ACTIVE_UPGRADES_SIZE = 5;
		public const int BASESTAT_UPGRADES_SIZE = 4;
		public GameObject[] inventorySlots;
		public GameObject[] activeSlots;
		public GameObject[] basestatSlots;
		public GameObject trashBin;
		public Sprite iconEmptySprite;
		public Sprite lockedIconSprite;

		public GameObject helpCanvas;
		public GameObject helpCanvasPanel;

		public bool draggingIcon = false;
		private GameObject draggedObject;
		private AbstractUpgrade draggedUpgrade;
		private int draggedUpgradeSlot;

		public Text[] statsTexts;

		private List<SpawnData> adminSpawnedData;
		private static MonsterId[] adminSpawnableList = { MonsterId.Neutrophyle_Patrol, MonsterId.Lymfocyte_melee, MonsterId.ChargerCell, MonsterId.TurretCell, MonsterId.MorphCellBig, MonsterId.FloatingHelperCell, MonsterId.ArmoredCell, MonsterId.DementCell, MonsterId.FourDiagShooterCell, MonsterId.JumpCell, MonsterId.SuiciderCell, MonsterId.TankCell, MonsterId.SmallTankCell, MonsterId.Lymfocyte_ranged, MonsterId.SpiderCell, MonsterId.HelperCell, MonsterId.PassiveHelperCell, MonsterId.ObstacleCell, MonsterId.TankSpreadshooter, };
		public GameObject adminPanel;
		public Dropdown adminSpawnPanel;

		private GameObject upgradesAdminPanel;
		private Dropdown upgradesDropdownPanel;

		public GameObject highlightedSlot;

		public GameObject currentTooltipObject;

		private bool firstClickDone;
		private float lastClick;
		private const float doubleClickTimeMax = 0.5f;
		private const float doubleClickTimeMin = 0.05f;

		private float[,] timers;

		private List<ScreenMsg> screenMessages = new List<ScreenMsg>(5);
		private List<ObjectMsg> objectMessages = new List<ObjectMsg>();

		public GameObject chatPosition;

		public GUIStyle msgStyle;
		public GUIStyle boxStyle;
		public GUIStyle skillHoverStyle;

		public class ScreenMsg
		{
			public string msg;
			public int level;
			public float time;
			public float opacity;
			public bool shown;

			public void Disable()
			{
				shown = false;
			}
		}

		public abstract class ObjectMsg
		{
			public Collider2D target;
			public Vector3 shift;
			public float time;
			public float opacity;
			public Color color;

			public abstract string GetMsg();
		}

		public class DamageMsg : ObjectMsg
		{
			public int dmg;

			public override string GetMsg()
			{
				return dmg.ToString();
			}
		}

		public class StringMsg : ObjectMsg
		{
			public string msg;

			public override string GetMsg()
			{
				return msg;
			}
		}

		private class HelpWindowQueue
		{
			public string title;
			public string[] text;
		}

		private Queue<HelpWindowQueue> helpWindowsQueue = new Queue<HelpWindowQueue>();
		private HelpWindowQueue currentHelpWindow;
		private bool queueAuthors = false;

		private void ShowNextHelpWindow()
		{
			HelpWindowQueue hw = helpWindowsQueue.Dequeue();

			string title = hw.title;
			string[] text = hw.text;

			currentHelpWindow = hw;
			GameSystem.Instance.Paused = true;

			if (helpCanvasPanel == null)
			{
				Debug.LogError("helpcanvaspanel is null, cant show help");
				return;
			}

			Text titleText = helpCanvasPanel.transform.FindChild("HelpTitle").gameObject.GetComponent<Text>();
			titleText.text = title;

			GameObject helpContent = helpCanvasPanel.transform.FindChild("HelpContent").gameObject;
			foreach (Transform t in helpContent.transform)
			{
				Destroy(t.gameObject);
			}

			for (int i = 0; i < text.Length; i++)
			{
				GameObject child = new GameObject("HelpContentText" + i);
				child.transform.parent = helpContent.transform;

				Text textObject = child.AddComponent<Text>();
				textObject.text = text[i];
				textObject.font = Font.CreateDynamicFontFromOSFont("Arial", 18); //TODO change to something better
				textObject.color = Color.black;
				textObject.fontSize = 18;
			}

			helpCanvas.GetComponent<Canvas>().enabled = true;
		}

		public void ShowAuthors()//TODO finish
		{
			if (currentHelpWindow != null)
			{
				queueAuthors = true;
			}
			else
			{
				GameObject authors = GameObject.Find("HelpAuthors");
				if (authors == null)
					return;

				authors.GetComponent<Image>().enabled = true;
				authors.transform.GetChild(0).GetComponent<Button>().enabled = true;
				authors.transform.GetChild(0).GetComponent<Image>().enabled = true;
			}
		}

		public void CloseAuthors()
		{
			GameObject authors = GameObject.Find("HelpAuthors");
			if (authors == null)
				return;

			Destroy(authors);

			if (helpWindowsQueue.Any())
			{
				ShowNextHelpWindow();
			}
			else
			{
				GameSystem.Instance.Paused = false;
			}
		}

		private IEnumerator ScheduleHelpWindow(string title, float time, params string[] text)
		{
			yield return new WaitForSeconds(time);
			ShowHelpWindow(title, 0, text);
		}

		public void ShowHelpWindow(HelpMessageData data, float time)
		{
			ShowHelpWindow(data.title, time, data.text);
		}

		public void ShowHelpWindow(string title, float time, params string[] text)
		{
			if (!showHelpWindows || title == null)
				return;

			if (time > 0)
			{
				data.GetOwner().StartTask(ScheduleHelpWindow(title, time, text));
				return;
			}

			HelpWindowQueue hw = new HelpWindowQueue();
			hw.title = title;
			hw.text = text;

			helpWindowsQueue.Enqueue(hw);

			if (currentHelpWindow == null)
			{
				ShowNextHelpWindow();
			}
		}

		public void ConfirmHelp()
		{
			CloseHelp();
		}

		private void CloseHelp()
		{
			currentHelpWindow = null;
			helpCanvas.GetComponent<Canvas>().enabled = false;

			if (queueAuthors)
			{
				ShowAuthors();
				return;
			}

			if (helpWindowsQueue.Any())
			{
				ShowNextHelpWindow();
			}
			else
			{
				GameSystem.Instance.Paused = false;
			}
		}

		public void ObjectMessage(GameObject target, String text, Color color)
		{
			if (!showObjectMessages)
				return;

			StringMsg msg = new StringMsg();
			msg.msg = text;
			msg.target = target.GetComponent<Collider2D>();
			msg.shift = new Vector3();
			msg.time = Time.time;
			msg.color = color;

			objectMessages.Add(msg);
		}

		public void DamageMessage(GameObject target, int damage, Color color)
		{
			if (!showObjectMessages)
				return;

			DamageMsg msg = new DamageMsg();
			msg.dmg = damage;
			msg.target = target.GetComponent<Collider2D>();
			msg.shift = new Vector3(Random.Range(-8, 8), Random.Range(-8, 8));
			msg.time = Time.time;
			msg.color = color;

			objectMessages.Add(msg);
		}

		public void ScreenMessage(string msg, int level = 1)
		{
			if (msg.Length > 45)
			{
				for (int i = 0; i <= (msg.Length / 40); i++)
				{
					ScreenMsg m = new ScreenMsg();

					int start = i * 40;
					int end = 40;
					if (start + end >= msg.Length)
						end = msg.Length - start;

					//if(level == 1)
					m.msg = msg.Substring(start, end);
					//else if (level == 2)
					//	m.msg = "<color=gray>" + msg.Substring(start, end) + "</color>";

					m.level = level;
					m.time = Time.time;
					m.shown = true;

					if (screenMessages.Count >= 5)
					{
						screenMessages.RemoveAt(screenMessages.Count - 1);
					}

					screenMessages.Insert(0, m);
				}
			}
			else
			{
				ScreenMsg m = new ScreenMsg();

				//if (level == 1)
				m.msg = msg;
				//else if (level == 2)
				//	m.msg = "<color=gray>" + msg + "</color>";

				m.level = level;
				m.time = Time.time;
				m.shown = true;

				if (screenMessages.Count >= 5)
				{
					screenMessages.RemoveAt(screenMessages.Count - 1);
				}

				screenMessages.Insert(0, m);
			}
		}

		void OnGUI()
		{
			if (skillHover)
			{
				Vector3 mousePos = (Input.mousePosition);

				float x = skillHoverObj.transform.position.x - 5;
				float y = Screen.height - skillHoverObj.transform.position.y - 50;

				Skill sk = data.GetOwner().Skills.GetSkill(skillHoverIndex - 1);

				//float x = mousePos.x;
				//float y = Screen.height - mousePos.y - 15;
				Rect r = new Rect(x, y, 25, 14);

				Color b = Color.black;
				GUI.color = b;
				GUI.Label(r, sk.GetVisibleName(), skillHoverStyle);

				Color c = Color.white;
				GUI.color = c;

				r.x -= 1;
				r.y -= 1;
				GUI.Label(r, sk.GetVisibleName(), skillHoverStyle);
			}

			if (screenMessages.Any())
			{
				const int w = 500;
				const int h = 25;

				float x = Screen.width - chatPosition.transform.position.x;
				float y = Screen.height - chatPosition.transform.position.y;

				int shift = 0;
				foreach (ScreenMsg m in screenMessages)
				{
					if (m.shown)
					{
						GUI.Box(new Rect(x - w / 2f, y - (h / 2f) - (shift * h), w, h), m.msg, boxStyle);
						shift++;
					}
				}
			}

			if (showObjectMessages && objectMessages.Any())
			{
				const int w = 50;
				const int h = 15;
				float x;
				float y;

				foreach (ObjectMsg m in objectMessages)
				{
					if (m.target == null)
						continue;

					Vector3 pos = Camera.main.WorldToScreenPoint(m.target.bounds.center);
					x = pos.x + m.shift.x;
					y = Screen.height - pos.y + -10 + m.shift.y;

					m.opacity = 1 - (Time.time - m.time) / 1.5f;

					m.shift -= new Vector3(-3 * Time.deltaTime, 10 * Time.deltaTime);

					Rect r = new Rect(x - w/2f + 1, y - (h/2f) + 1, w, h);

					Color b = Color.black;
					b.a = m.opacity;
					GUI.color = b;
					GUI.Label(r, m.GetMsg(), msgStyle);

					Color c = m.color;
					c.a = m.opacity;
					GUI.color = c;

					r.x -= 1;
					r.y -= 1;
					GUI.Label(r, m.GetMsg(), msgStyle);
				}
			}
		}

		private bool skillHover;
		private GameObject skillHoverObj;
		private int skillHoverIndex;

		public void OnSkillIconHoverEnter(int order)
		{
			skillHover = true;
			skillHoverObj = skillButtons[order - 1];
			skillHoverIndex = order;
		}

		public void OnSkillIconHoverExit(int order)
		{
			skillHover = false;
		}

		// Use this for initialization
		void Start()
		{
			data = GetComponent<PlayerData>();

			bool mobile = false;
#if UNITY_ANDROID
			mobile = true;
#endif

			if (mobile)
			{
				gameMenu = GameObject.Find("GameMenu_Mobile");
				settingsPanel = GameObject.Find("SettingsMenu_Mobile");
				showObjectMessages = false;
			}
			else
			{
				gameMenu = GameObject.Find("GameMenu");
				settingsPanel = GameObject.Find("SettingsMenu");
				showObjectMessages = true;
			}

			gameMenu.GetComponent<Canvas>().enabled = true;
			settingsPanel.GetComponent<Canvas>().enabled = true;

			chatPosition = GameObject.Find("ChatPosition");

			//msgStyle = new GUIStyle();
			//msgStyle.fontSize = 20;
			//msgStyle.alignment = TextAnchor.LowerCenter;

			//boxStyle = new GUIStyle();

			helpCanvas = GameObject.Find("HelpCanvas");
			helpCanvas.GetComponent<Canvas>().enabled = false;

			helpCanvasPanel = helpCanvas.transform.FindChild("HelpCanvasPanel").gameObject;

			GameObject skillPanel = gameMenu.transform.FindChild("SkillPanel").gameObject;

			skillButtons = new GameObject[9];
			for (int i = 1; i <= 9; i++)
			{
				foreach (Transform child in skillPanel.transform)
				{
					if (child.name.Equals("Skill" + i))
					{
						skillButtons[i - 1] = child.gameObject;
					}
				}
			}

			skillProgresses = new Image[9];
			for (int i = 1; i <= 9; i++)
			{
				GameObject sko = skillButtons[i - 1];
				skillProgresses[i - 1] = sko.transform.GetChild(0).GetComponent<Image>();
				skillProgresses[i - 1].sprite = sko.GetComponent<Image>().sprite;
			}

			timers = new float[9, 9];
			for (int i = 0; i < timers.GetLength(0); i++)
			{
				timers[i, 0] = 1;
				timers[i, 1] = -1;
			}

			if (settingsPanel != null)
				settingsPanel.SetActive(false);

			if (menuPanel != null)
				menuPanel.SetActive(false);

			inventoryPanel = GameObject.Find("Inventory");

			if (inventoryPanel != null)
			{
				inventorySlots = new GameObject[INVENTORY_SIZE];
				activeSlots = new GameObject[ACTIVE_UPGRADES_SIZE];
				basestatSlots = new GameObject[BASESTAT_UPGRADES_SIZE];

				trashBin = GameObject.Find("Trashbin");
				ShowTrashBin(false);

				GameObject iconTemplate = Resources.Load("Sprite/inventory/Slot") as GameObject;
				GameObject inventoryContentPanel = GameObject.Find("InventoryContent");
				GameObject activeStatsPanel = GameObject.Find("ActiveStatsPanel");

				GameObject.Find("InvScrollbar").GetComponent<Scrollbar>().value = 1f;

				tooltipObject = Resources.Load("Sprite/inventory/UpgradeTooltip") as GameObject;

				iconEmptySprite = Resources.Load<Sprite>("Sprite/inventory/icon_empty");
				lockedIconSprite = Resources.Load<Sprite>("Sprite/inventory/icon_locked");

				GameObject basestatUpgradesContent = GameObject.Find("BasestatUpgradesContent");

				for (int i = 0; i < BASESTAT_UPGRADES_SIZE; i++)
				{
					GameObject newIcon = Instantiate(iconTemplate);
					newIcon.name = "BaseSlot_" + (i + 1);
					newIcon.transform.parent = basestatUpgradesContent.transform;
					newIcon.transform.localScale = new Vector3(1, 1, 1);

					GameObject child = newIcon.transform.GetChild(0).gameObject;

					EventTrigger trigger = child.AddComponent<EventTrigger>();

					EventTrigger.Entry entry = new EventTrigger.Entry();
					entry.eventID = EventTriggerType.PointerDown;
					entry.callback.AddListener(delegate { OnUpgradeClick(2, newIcon); });
					trigger.triggers.Add(entry);

					entry = new EventTrigger.Entry();
					entry.eventID = EventTriggerType.PointerEnter;
					entry.callback.AddListener(delegate { OnUpgradeHover(newIcon, false, 2); });
					trigger.triggers.Add(entry);

					entry = new EventTrigger.Entry();
					entry.eventID = EventTriggerType.PointerExit;
					entry.callback.AddListener(delegate { OnUpgradeHover(newIcon, true, 2); });
					trigger.triggers.Add(entry);

					basestatSlots[i] = newIcon;
				}

				for (int i = 0; i < INVENTORY_SIZE; i++)
				{
					GameObject newIcon = Instantiate(iconTemplate);
					newIcon.name = "Slot_" + (i + 1);
					newIcon.transform.parent = inventoryContentPanel.transform;
					newIcon.transform.localScale = new Vector3(1, 1, 1);

					GameObject child = newIcon.transform.GetChild(0).gameObject;

					EventTrigger trigger = child.AddComponent<EventTrigger>();

					EventTrigger.Entry entry = new EventTrigger.Entry();
					entry.eventID = EventTriggerType.PointerDown;
					entry.callback.AddListener(delegate { OnUpgradeClick(0, newIcon); });
					trigger.triggers.Add(entry);

					entry = new EventTrigger.Entry();
					entry.eventID = EventTriggerType.PointerEnter;
					entry.callback.AddListener(delegate { OnUpgradeHover(newIcon, false, 0); });
					trigger.triggers.Add(entry);

					entry = new EventTrigger.Entry();
					entry.eventID = EventTriggerType.PointerExit;
					entry.callback.AddListener(delegate { OnUpgradeHover(newIcon, true, 0); });
					trigger.triggers.Add(entry);

					inventorySlots[i] = newIcon;
				}

				GameObject activeContentPanel = GameObject.Find("ActiveUpgradesContent");

				iconTemplate = Resources.Load("Sprite/inventory/ActiveSlot") as GameObject;

				for (int i = 0; i < ACTIVE_UPGRADES_SIZE; i++)
				{
					GameObject newIcon = Instantiate(iconTemplate);
					newIcon.name = "ActiveSlot_" + (i + 1);
					newIcon.transform.parent = activeContentPanel.transform;
					newIcon.transform.localScale = new Vector3(1, 1, 1);

					GameObject child = newIcon.transform.GetChild(0).gameObject;

					EventTrigger trigger = child.AddComponent<EventTrigger>();

					EventTrigger.Entry entry = new EventTrigger.Entry();
					entry.eventID = EventTriggerType.PointerDown;
					entry.callback.AddListener(delegate { OnUpgradeClick(1, newIcon); });
					trigger.triggers.Add(entry);

					entry = new EventTrigger.Entry();
					entry.eventID = EventTriggerType.PointerEnter;
					entry.callback.AddListener(delegate { OnUpgradeHover(newIcon, false, 1); });
					trigger.triggers.Add(entry);

					entry = new EventTrigger.Entry();
					entry.eventID = EventTriggerType.PointerExit;
					entry.callback.AddListener(delegate { OnUpgradeHover(newIcon, true, 1); });
					trigger.triggers.Add(entry);

					activeSlots[i] = newIcon;
				}

				if (activeStatsPanel != null)
				{
					int count = 0;
					foreach (Transform t in activeStatsPanel.gameObject.transform)
					{
						if (t.GetComponent<Text>() != null)
							count++;
					}

					statsTexts = new Text[count];
					int i = 0;

					foreach (Transform t in activeStatsPanel.gameObject.transform)
					{
						if (t.GetComponent<Text>() != null)
						{
							statsTexts[i++] = t.GetComponent<Text>();
						}
					}
				}

				UpdateStatsInfo();
				UpdateInventory(data.GetOwner().Inventory);

				inventoryOpened = false;
				inventoryPanel.GetComponent<Canvas>().enabled = false;
			}

			try
			{
				upgradesAdminPanel = GameObject.Find("UpgradesAdminPanel");
				upgradesDropdownPanel = upgradesAdminPanel.GetComponent<Dropdown>();

				List<String> temp = new List<string>();

				foreach (UpgradeTable.UpgradeInfo o in UpgradeTable.Instance.upgrades)
				{
					temp.Add(o.upgrade.Name);
				}

				upgradesDropdownPanel.AddOptions(temp);
			}
			catch (Exception)
			{
			}

			try
			{
				// admin setting
				adminPanel = GameObject.Find("AdminPanel");
				adminSpawnPanel = GameObject.Find("AdminMode").GetComponent<Dropdown>();

				List<String> temp = new List<string>();
				foreach (MonsterId id in adminSpawnableList)
					temp.Add(Enum.GetName(typeof(MonsterId), id));
				adminSpawnPanel.AddOptions(temp);

				UpdateAdminControls();
			}
			catch (Exception)
			{
				Debug.LogError("error initializing admin panel");
			}
		}

		public void AdminUpgradeChosen()
		{
			if (upgradesDropdownPanel != null)
			{
				string type = upgradesDropdownPanel.captionText.text;

				Type t = null;

				foreach (UpgradeTable.UpgradeInfo o in UpgradeTable.Instance.upgrades)
				{
					if (o.upgrade.Name.Equals(type))
					{
						t = o.upgrade;
						break;
					}
				}

				if (t != null)
				{
					AbstractUpgrade u = UpgradeTable.Instance.GenerateUpgrade(t, 1);
					UpgradeTable.Instance.DropItem(u, data.GetBody().transform.position);
				}
			}
		}

		private float lastMsgUpdate;
		private const float msgUpdateInterval = 0.5f;

		// Update is called once per frame
		void Update()
		{
			if (screenMessages.Count > 0 && lastMsgUpdate + msgUpdateInterval < Time.time)
			{
				foreach (ScreenMsg mess in screenMessages.ToArray())
				{
					if (mess.shown)
					{
						if (mess.time + 6f < Time.time)
						{
							mess.Disable();
						}
						else
						{
							mess.opacity = 1 - (Time.time - mess.time) / 6f;
						}
					}
				}
				lastMsgUpdate = Time.time;
			}

			if (objectMessages.Count > 0)
			{
				foreach (ObjectMsg m in objectMessages.ToArray())
				{
					if (m.time + 1.5f < Time.time)
					{
						objectMessages.Remove(m);
					}
				}
			}

			if (draggingIcon && draggedObject != null)
			{
				Vector3 mousePos = (Input.mousePosition);

				if (Input.GetMouseButton(0))
				{
					SetMouseOverUi();
					mousePos.z = 0;
					draggedObject.GetComponent<RectTransform>().position = mousePos;

					if (currentTooltipObject)
						Destroy(currentTooltipObject);
				}
				else
				{
					StoppedDragging(mousePos);
					draggingIcon = false;
					draggedUpgrade = null;
					draggedUpgradeSlot = -1;
					SetMouseNotOverUi();
					Destroy(draggedObject);
				}
			}

			if (currentTooltipObject != null)
			{
				List<RaycastResult> hits = new List<RaycastResult>();
				PointerEventData cursor = new PointerEventData(EventSystem.current);
				cursor.position = Input.mousePosition;
				EventSystem.current.RaycastAll(cursor, hits);

				bool stillAboveIcon = false;
				foreach (RaycastResult r in hits)
				{
					if (r.gameObject.Equals(highlightedSlot))
					{
						stillAboveIcon = true;
						break;
					}
				}

				if (!stillAboveIcon)
					Destroy(currentTooltipObject);
				else
				{
					currentTooltipObject.transform.position = Input.mousePosition;
				}
			}

			for (int i = 0; i < timers.GetLength(0); i++)
			{
				if (timers[i, 0] > 0)
				{
					float max = timers[i, 1];
					float passed = Time.time - timers[i, 0];
					float ratio = passed / max;

					if (ratio >= 1)
					{
						ratio = 1;
						timers[i, 0] = 0;
					}

					ratio = (ratio);

					if (max < 0)
						ratio = 0;

					//Image but = skillButtons[i-1].GetComponent<Image>();
					//but.color = new Color(ratio, ratio, ratio);

					try
					{
						skillProgresses[i - 1].fillAmount = 1 - ratio;
					}
					catch (Exception e)
					{
						//Debug.LogError(e.Message);
					}
				}
			}

			UpdateAdminInfo();
		}

		public void SetReuseTimer(Skill sk)
		{
			int id = -1;

			for (int i = 0; i < data.GetOwner().Skills.Skills.Count; i++)
			{
				if (sk.GetName().Equals(data.GetOwner().Skills.Skills[i].GetName()))
				{
					id = i + 1;
					break;
				}
			}

			if (id == -1)
				return;

			if (sk.IsLocked)
			{
				timers[id, 0] = 0;
				timers[id, 1] = -1;
			}
			else
			{
				timers[id, 0] = Time.time;
				timers[id, 1] = ((ActiveSkill)sk).GetReuse();
			}
		}

		public void ResetReuseTimer(Skill sk)
		{
			int id = -1;

			for (int i = 0; i < data.GetOwner().Skills.Skills.Count; i++)
			{
				if (sk.GetName().Equals(data.GetOwner().Skills.Skills[i].GetName()))
				{
					id = i + 1;
					break;
				}
			}

			if (id == -1)
				return;

			timers[id, 0] = Time.time - ((ActiveSkill)sk).GetReuse();
			timers[id, 1] = ((ActiveSkill)sk).GetReuse();
		}

		public void NextLevel()
		{
			GameObject.Find("Cave Generator").GetComponent<WorldHolder>().LoadNextMap();
		}

		public void SaveLevel()
		{
			GameObject.Find("Cave Generator").GetComponent<WorldHolder>().SaveCurrentMap();
		}

		public void PrevLevel()
		{
			GameObject.Find("Cave Generator").GetComponent<WorldHolder>().LoadPreviousMap();
		}

		public void SetAdminMapSeedInfo(string s)
		{

		}

		public void UpdateAdminInfo()
		{
			if (!adminMode)
				return;

			if ((int)Time.time % 1 == 0)
			{
				StringBuilder sb = new StringBuilder();

				sb.AppendLine("Map Type " + Enum.GetName(typeof(MapType), WorldHolder.instance.activeMap.MapType));
				Vector3 pos = data.GetBody().transform.position;

				int left = WorldHolder.instance.activeMap.GetMonstersLeft(WorldHolder.instance.activeMap.GetRegionFromWorldPosition(pos).GetParentOrSelf());
				sb.AppendLine("Room monsters left " + left);

				GameObject.Find("AdminMapInfo").GetComponent<Text>().text = sb.ToString();
			}
		}

		public int GetSlotNumberFromObject(GameObject slot)
		{
			int order = Int32.Parse(slot.name.Split('_')[1]) - 1;

			return order;
		}

		public AbstractUpgrade GetUpgradeFromInventory(GameObject slot, int slotType)
		{
			int order = Int32.Parse(slot.name.Split('_')[1]) - 1;

			AbstractUpgrade u = null;

			if (slotType == 1)
				u = data.GetOwner().Inventory.GetActiveUpgrade(order);
			else if (slotType == 0)
				u = data.GetOwner().Inventory.GetUpgrade(order);
			else if (slotType == 2) // base stat
				u = data.GetOwner().Inventory.GetBasestatUpgrade(order);

			return u;
		}

		public void OnUpgradeHover(GameObject slot, bool exit, int slotType)
		{
			AbstractUpgrade u = GetUpgradeFromInventory(slot, slotType);

			if (u == null)
				return;

			if (!exit)
			{
				if (draggingIcon)
					return;

				if (currentTooltipObject != null)
					Destroy(currentTooltipObject);

				highlightedSlot = slot;
				currentTooltipObject = Instantiate(tooltipObject);
				currentTooltipObject.transform.parent = inventoryPanel.transform;
				currentTooltipObject.transform.position = Input.mousePosition;

				string name = u.VisibleName;
				string description = u.Description;
				string price = u.Price;
				string addInfo = u.AdditionalInfo;

				int currentUpgradeProgress = u.CurrentProgress;
				int needForNext = u.NeedForNextLevel;

				if (u.GoesIntoBasestatSlot)
					addInfo = "Level-up progress:\n " + currentUpgradeProgress + " / " + needForNext + " upgrade modules.";

				UpgradeType type = u.Type;

				Color titleColor = new Color();

				switch (type)
				{
					case UpgradeType.CLASSIC:
						titleColor = new Color(86 / 255f, 71 / 255f, 49 / 255f);
						currentTooltipObject.GetComponent<Image>().color = new Color(253 / 255f, 253 / 255f, 224 / 225f);
						break;
					case UpgradeType.EPIC:
						titleColor = new Color(109 / 255f, 58 / 255f, 65 / 255f);
						currentTooltipObject.GetComponent<Image>().color = new Color(253 / 255f, 253 / 255f, 224 / 225f);
						break;
					case UpgradeType.RARE:
						titleColor = new Color(64 / 255f, 72 / 255f, 120 / 255f);
						currentTooltipObject.GetComponent<Image>().color = new Color(1, 1, 1);
						break;
				}

				foreach (Transform child in currentTooltipObject.transform)
				{
					if (child.name.Equals("Title"))
					{
						child.GetComponent<Text>().text = name + " [Lv" + u.Level + "]";
						child.GetComponent<Text>().color = titleColor;
						continue;
					}
					else if (child.name.Equals("Description") && description != null)
					{
						child.GetComponent<Text>().text = description;
						continue;
					}
					else if (child.name.Equals("Price") && price != null)
					{
						child.GetComponent<Text>().text = price;
						child.GetComponent<Text>().color = titleColor;
						continue;
					}
					else if (child.name.Equals("AdditionalInfo") && (addInfo != null || u.RequiredClass > 0))
					{
						string className = null;
						ClassId cId = u.RequiredClass;
						if (cId > 0)
						{
							className = Enum.GetName(typeof(ClassId), cId);
						}

						string info = "";
						if (addInfo != null)
							info = addInfo;

						if (className != null)
							info = addInfo + " Required class: " + className;

						child.GetComponent<Text>().text = info;
						continue;
					}
				}
			}
		}

		public void ShowTrashBin(bool state)
		{
			if (state)
			{
				trashBin.SetActive(true);
			}
			else
			{
				trashBin.SetActive(false);
			}
		}

		public void StoppedDragging(Vector3 pos)
		{
			List<RaycastResult> hits = new List<RaycastResult>();
			PointerEventData cursor = new PointerEventData(EventSystem.current);
			cursor.position = Input.mousePosition;
			EventSystem.current.RaycastAll(cursor, hits);

			GameObject targetSlot = null;
			int targetSlotType = 0;
			bool trashBin = false;
			foreach (RaycastResult r in hits)
			{
				if (r.gameObject.name.StartsWith("Slot"))
				{
					targetSlot = r.gameObject;
					targetSlotType = 0;
					break;
				}
				else if (r.gameObject.name.StartsWith("ActiveSlot"))
				{
					targetSlot = r.gameObject;
					targetSlotType = 1;
					break;
				}
				else if (r.gameObject.name.StartsWith("Trashbin"))
				{
					targetSlot = r.gameObject;
					trashBin = true;
					break;
				}
			}

			ShowTrashBin(false);

			if (trashBin)
			{
				if (draggedUpgradeSlot == 1) // is active, unequip first
					data.GetOwner().UnequipUpgrade(draggedUpgrade, true);

				data.GetOwner().RemoveUpgrade(draggedUpgrade);
				return;
			}

			if (targetSlotType == 1 && draggedUpgradeSlot == 1)
				return;

			if (targetSlotType == 0 && draggedUpgradeSlot == 0)
				return;

			if (targetSlot != null)
			{
				AbstractUpgrade u = GetUpgradeFromInventory(targetSlot, targetSlotType);
				int number = GetSlotNumberFromObject(targetSlot);

				// slot is not empty - swap
				if (u != null)
				{
					data.GetOwner().SwapUpgrade(draggedUpgrade, u, number, draggedUpgradeSlot, targetSlotType);
					Debug.Log("swapping for " + u.Name + " in slot " + number + " from active? " + draggedUpgradeSlot + " to active? " + (targetSlotType == 1));
				}
				else // slot is empty - simply move the upgrade there
				{
					data.GetOwner().SwapUpgrade(draggedUpgrade, u, number, draggedUpgradeSlot, targetSlotType);
					Debug.Log("puttint into slot id " + number + " from active? " + draggedUpgradeSlot + " to active? " + (targetSlotType == 1));
				}
			}
		}

		public void OnUpgradeClick(int slotType, GameObject obj)
		{
			if (slotType == 2)
				return;

			bool doubleClick = false;

			if (!firstClickDone)
			{
				lastClick = Time.time;
				firstClickDone = true;
			}
			else
			{
				float diff = Time.time - lastClick;
				if (diff < doubleClickTimeMax && diff > doubleClickTimeMin)
				{
					doubleClick = true;
					firstClickDone = false;
				}
				else
				{
					firstClickDone = true;
					lastClick = Time.time;
				}
			}

			draggedUpgrade = GetUpgradeFromInventory(obj, slotType);

			if (draggedUpgrade == null)
				return;

			draggedUpgradeSlot = slotType;

			int targetSlot = draggedUpgradeSlot == 1 ? 0 : 1;

			if (doubleClick)
			{
				data.GetOwner().SwapUpgrade(draggedUpgrade, null, -1, draggedUpgradeSlot, targetSlot);
			}
			else
			{
				draggingIcon = true;
				SetMouseOverUi();

				Vector3 mousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
				mousePos.z = 0;

				GameObject preview = new GameObject("Dragged Icon");
				Image ren = preview.AddComponent<Image>();
				ren.sprite = obj.transform.GetChild(0).GetComponent<Image>().sprite;
				preview.transform.parent = inventoryPanel.transform;
				//preview.transform.position = mousePos;
				preview.GetComponent<RectTransform>().sizeDelta = new Vector2(80, 80);

				draggedObject = preview;
				ShowTrashBin(true);
			}
		}

		public void SwitchInventory()
		{
			if (inventoryPanel != null)
			{
				if (inventoryPanel.GetComponent<Canvas>().enabled)
				{
					inventoryOpened = false;
					inventoryPanel.GetComponent<Canvas>().enabled = false;
				}
				else
				{
					inventoryOpened = true;
					inventoryPanel.GetComponent<Canvas>().enabled = true;
					UpdateStatsInfo();
				}
			}
		}

		public void UpdateStatsInfo()
		{
			if (inventoryOpened == false)
				return;

			foreach (Text t in statsTexts)
			{
				switch (t.gameObject.name)
				{
					case "Level":
						t.text = "Level " + data.level;
						break;
					case "Class":
						t.text = ((Player)data.GetOwner()).Template.GetClassId().ToString();
						break;
					case "HP":
						t.text = "HP " + data.visibleHp + " / " + data.visibleMaxHp;
						break;
					case "MoveSpeed":
						t.text = "Move Speed " + data.moveSpeed;
						break;
					case "Critical Rate":
						int rate = ((Player)data.GetOwner()).Status.CriticalRate;
						t.text = "Critical rate: " + rate / 10 + "%";
						break;
					case "Critical Damage":
						float dmg = ((Player)data.GetOwner()).Status.CriticalDamageMul;
						t.text = "Critical damage: x" + dmg;
						break;
					case "Damage Output":
						float mul = ((Player)data.GetOwner()).Status.DamageOutputMul;
						float add = ((Player)data.GetOwner()).Status.DamageOutputAdd;
						t.text = "Damage: x" + mul + " +" + add + "";
						break;
					case "Shield":
						float shield = ((Player)data.GetOwner()).Status.Shield;
						t.text = "Shield " + (int)(shield * 100 - 100) + "%";
						break;
				}
			}
		}

		public void UpdateInventory(Inventory inv)
		{
			int activeCapacity = inv.ActiveCapacity;
			int capacity = inv.Capacity;
			int basestatCapacity = inv.BasestatUpgrades.Count;

			int i = 0;
			foreach (AbstractUpgrade u in inv.Upgrades)
			{
				if (u == null || inv.IsEquipped(u))
					continue;

				GameObject o = inventorySlots[i].transform.GetChild(0).gameObject;
				Image img = o.GetComponent<Image>();
				img.sprite = u.MainSprite;

				i++;
			}

			for (int j = i; j < inventorySlots.Length; j++)
			{
				GameObject o = inventorySlots[j].transform.GetChild(0).gameObject;
				Image img = o.GetComponent<Image>();

				if (j < capacity)
					img.sprite = iconEmptySprite;
				else
					img.sprite = lockedIconSprite;
			}

			i = 0;
			foreach (AbstractUpgrade u in inv.ActiveUpgrades)
			{
				GameObject o = activeSlots[i].transform.GetChild(0).gameObject;
				Image img = o.GetComponent<Image>();
				img.sprite = u.MainSprite;

				i++;
			}

			for (int j = i; j < activeSlots.Length; j++)
			{
				GameObject o = activeSlots[j].transform.GetChild(0).gameObject;
				Image img = o.GetComponent<Image>();

				if (j < activeCapacity)
					img.sprite = iconEmptySprite;
				else
					img.sprite = lockedIconSprite;
			}

			i = 0;
			foreach (AbstractUpgrade u in inv.BasestatUpgrades)
			{
				GameObject o = basestatSlots[i].transform.GetChild(0).gameObject;
				Image img = o.GetComponent<Image>();
				img.sprite = u.MainSprite;

				i++;
			}

			for (int j = i; j < basestatSlots.Length; j++)
			{
				GameObject o = basestatSlots[j].transform.GetChild(0).gameObject;
				Image img = o.GetComponent<Image>();

				if (j < activeCapacity)
					img.sprite = iconEmptySprite;
				else
					img.sprite = lockedIconSprite;
			}

			UpdateStatsInfo();
		}

		public void MenuClick()
		{
			if (menuPanel.activeSelf)
				menuPanel.SetActive(false);
			else
				menuPanel.SetActive(true);
		}

		public void OpenSettings()
		{
			if (settingsPanel.activeSelf)
			{
				SetMouseNotOverUi();
				GameSystem.Instance.Paused = false;
				settingsPanel.SetActive(false);
			}
			else
			{
				SetMouseOverUi();
				GameSystem.Instance.Paused = true;
				settingsPanel.SetActive(true);
			}
		}

		public void ToggleChanged()
		{
			bool val = GameObject.Find("ToggleCameraMovement").GetComponent<Toggle>().isOn;
			CameraMovement.cameraFollowsPlayer = val;

			val = GameObject.Find("PlayerUsePathfinding").GetComponent<Toggle>().isOn;
			data.usesPathfinding = val;

			val = GameObject.Find("FogOfWar").GetComponent<Toggle>().isOn;
			GameSystem.Instance.Controller.fogOfWar = val;

			val = GameObject.Find("AutoAttackTargetting").GetComponent<Toggle>().isOn;
			data.autoAttackTargetting = val;

			val = GameObject.Find("CastingBreaksMovement").GetComponent<Toggle>().isOn;
			data.castingBreaksMovement = val;

			val = GameObject.Find("MoveOnlyWithMousePressed").GetComponent<Toggle>().isOn;
			data.moveOnlyWhenMousePressed = val;

			val = GameObject.Find("ShowDamageMessages").GetComponent<Toggle>().isOn;
			showObjectMessages = val;

			val = GameObject.Find("DetailedPathfinding").GetComponent<Toggle>().isOn;
			if (GameSystem.Instance.detailedPathfinding != val)
			{
				GameSystem.Instance.detailedPathfinding = val;
				WorldHolder.instance.activeMap.UpdatePathfinding();
			}
		}

		public void OnConsoleCommand(String s)
		{
			Debug.Log("ahoj");
			if (s.ToLower().Equals("restart"))
			{
				RestartGame();
			}
		}

		public void RestartGame()
		{
			OpenSettings();
			Application.LoadLevel(Application.loadedLevel);
		}

		public void TestSpawnMonsters()
		{
			MonsterId mId = MonsterId.TestMonster;

			switch (Random.Range(1, 2))
			{
				case 1:
					mId = MonsterId.Leukocyte_melee;
					break;
				case 2:
					mId = MonsterId.Leukocyte_ranged;
					break;
			}

			GameSystem.Instance.SpawnMonster(mId, data.GetBody().transform.position + new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), 0), false, 1);
		}

		public void TestSpawnMonsters2()
		{
			MonsterId mId = MonsterId.TestMonster;
			GameSystem.Instance.SpawnMonster(mId, data.GetBody().transform.position + new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), 0), false, 1);
		}

		public void Skill(int order)
		{
			Debug.Log("calling skill at .. " + Time.frameCount);
			data.LaunchSkill(order);
		}

		public void Skill1()
		{
			data.LaunchSkill(1);
		}

		public void Skill2()
		{
			data.LaunchSkill(2);
		}

		public void Skill3()
		{
			data.LaunchSkill(3);
		}

		public void Skill4()
		{
			data.LaunchSkill(4);
		}

		public void Skill5()
		{
			data.LaunchSkill(5);
		}

		public void SetMouseOverUi()
		{
			//Debug.Log("mouse over UI");
			mouseOverUi = true;
		}

		public void SetMouseNotOverUi()
		{
			//Debug.Log("mouse not UI");
			mouseOverUi = false;
		}

		public void SwitchAdminMode()
		{
			adminMode = !adminMode;
			UpdateAdminControls();
		}

		public void AdminClick(Vector3 position, int mouseButton)
		{
			position.z = 0;

			if (mouseButton == 0)
			{
				Monster m = GameSystem.Instance.SpawnMonster((MonsterId)Enum.Parse(typeof(MonsterId), adminSpawnPanel.captionText.text), position, false, 1);
				WorldHolder.instance.activeMap.RegisterMonsterToMap(m);
			}
			else if (mouseButton == 1)
			{
				Collider2D cd = Physics2D.OverlapPoint(position);

				if (cd != null && cd.gameObject != null)
				{
					if (cd.gameObject.GetData() != null)
					{
						cd.gameObject.GetChar().DoDie();
					}
				}
			}
		}

		struct SpawnData
		{
			public MonsterId id;
			public Vector3 pos;

			public SpawnData(MonsterId id, Vector3 pos)
			{
				this.id = id;
				this.pos = pos;
			}
		}

		public void SavePositions()
		{
			adminSpawnedData = new List<SpawnData>();

			foreach (GameObject o in GameObject.FindObjectsOfType<GameObject>())
			{
				if (o != null && o.activeSelf && o.activeInHierarchy)
				{
					if (o.GetData() != null && o.GetData() is EnemyData && !(o.GetChar() is Npc))
					{
						SpawnData data = new SpawnData((o.GetChar() as Monster).Template.GetMonsterId(), o.GetData().GetBody().transform.position);
						adminSpawnedData.Add(data);
					}
				}
			}
		}

		public void RespawnSaved()
		{
			foreach (SpawnData data in adminSpawnedData)
			{
				Monster m = GameSystem.Instance.SpawnMonster(data.id, data.pos, false, 1);
				WorldHolder.instance.activeMap.RegisterMonsterToMap(m);
			}
		}

		public void RemoveAllMobs()
		{
			foreach (GameObject o in GameObject.FindObjectsOfType<GameObject>())
			{
				if (o != null && o.activeSelf && o.activeInHierarchy)
				{
					if (o.GetData() != null && o.GetData() is EnemyData && !(o.GetChar() is Npc))
					{
						o.GetChar().DoDie();
					}
				}
			}
		}

		public void RegenerateLevel()
		{
			WorldHolder.instance.RegenMap();
		}

		private void UpdateAdminControls()
		{
			if (adminMode)
			{
				adminPanel.SetActive(true);
				//adminSpawnPanel.gameObject.SetActive(true);
			}
			else
			{
				adminPanel.SetActive(false);
				//adminSpawnPanel.gameObject.SetActive(false);
			}
		}
	}
}
