// Copyright (c) 2015, Lukas Honke
// ========================
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
using Assets.scripts.Mono.MapGenerator.Levels;
using Assets.scripts.Mono.ObjectData;
using Assets.scripts.Skills;
using Assets.scripts.Skills.Base;
using Assets.scripts.Upgrade;
using Assets.scripts.Upgrade.Classic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;
#pragma warning disable 1591

namespace Assets.scripts.Mono
{
	/// <summary>
	/// Controls player user intrface
	/// </summary>
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

		public Canvas levelsViewCanvas = null;

		private Canvas classOverviewCanvas = null;
		private int currentSkillsView = 1;
		private GameObject availableSkillsPanel = null;
		private GameObject availableSkillsPanelContent = null;
		private GameObject selectedSkillsPanel = null;
		private GameObject selectedAutoattackPanel = null;
		private GameObject skillItemTemplate = null;
		private Button switchActiveSkillsPanelButton = null;
		private Button switchAutottackPanelButton = null;
		private Text classOverviewLabel = null;
		private Text upgradeHintLabel = null;
		private GameObject skillSlotLabelTemplate = null;
		private Text upgradePointsLabel = null;
		private GameObject classViewStatsUpgrades = null;
		private List<Text> classViewStats = null;

		public GameObject inventoryTooltipObject;
		public GameObject skillTooltipObject;
		public GameObject levelTooltipObject;
		public GameObject levelLineDot;
		public bool inventoryOpened = false;
		public const int INVENTORY_SIZE = 20;
		public const int ACTIVE_UPGRADES_SIZE = 5;
		public const int BASESTAT_UPGRADES_SIZE = 5;
		public GameObject[] inventorySlots;
		public GameObject[] activeSlots;
		public GameObject[] basestatSlots;
		public GameObject trashBin;
		public GameObject disposeValObj;
		public Sprite iconEmptySprite;
		public Sprite lockedIconSprite;
		private GameObject iconTemplate;

		private GameObject basestatUpgradesContent;

		public GameObject helpCanvas;
		public GameObject helpCanvasPanel;

		public GameObject skillPanel;

		public bool draggingIcon = false;
		private GameObject draggedObject;

		private InventoryItem draggedItem;
		private int draggedUpgradeSlot;
		private Skill draggedSkill;

		public Text[] statsTexts;

		private List<SpawnData> adminSpawnedData;
		private static MonsterId[] adminSpawnableList = { MonsterId.IdleObstacleCell, MonsterId.Neutrophyle_Patrol, MonsterId.DurableMeleeCell, MonsterId.SwarmCell, MonsterId.PusherCell, MonsterId.HealerCell, MonsterId.SlowerCell, MonsterId.RogueCell, MonsterId.SniperCell, MonsterId.BigPassiveFloatingCell, MonsterId.BigPassiveCell, MonsterId.Lymfocyte_melee, MonsterId.ChargerCell, MonsterId.TurretCell, MonsterId.MorphCellBig, MonsterId.FloatingBasicCell, MonsterId.ArmoredCell, MonsterId.DementCell, MonsterId.FourDiagShooterCell, MonsterId.JumpCell, MonsterId.SuiciderCell, MonsterId.TankCell, MonsterId.SmallTankCell, MonsterId.Lymfocyte_ranged, MonsterId.SpiderCell, MonsterId.BasicCell, MonsterId.PassiveHelperCell, MonsterId.ObstacleCell, MonsterId.TankSpreadshooter, MonsterId.SwarmerBoss};
		public GameObject adminPanel;
		public Dropdown adminSpawnPanel;

		private GameObject upgradesAdminPanel;
		private Dropdown upgradesDropdownPanel;

		public GameObject highlightedObject;

		public GameObject currentTooltipObject;

		private bool firstClickDone;
		private float lastClick;
		private const float doubleClickTimeMax = 0.5f;
		private const float doubleClickTimeMin = 0.05f;
		
		// TIME_CASTED ; REUSE_DELAY
		private float[,] timers;

		private List<ScreenMsg> screenMessages = new List<ScreenMsg>(5);
		private List<ObjectMsg> objectMessages = new List<ObjectMsg>();

		public GameObject chatPosition;

		public GUIStyle msgStyle;
		public GUIStyle boxStyle;
		public GUIStyle skillHoverStyle;
		public GUIStyle skillUpgradeLabelStyle;

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

				if (skillHoverIndex - 1 < data.GetOwner().Skills.Skills.Count)
				{
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
			}

			/*if (classOverviewEnabled)
			{
				Player player = data.GetOwner() as Player;

				for (int i = 0; i < selectedSkillsPanel.transform.childCount; i++)
				{
					GameObject obj = selectedSkillsPanel.transform.GetChild(i).gameObject;

					float x = obj.transform.position.x;
					float y = obj.transform.position.y + 40;

					int slotLevel = player.SkillSlotLevels[i];

					Rect r = new Rect(x, y, 25, 14);

					Color b = Color.black;
					GUI.color = b;
					GUI.Label(r, "Level " + slotLevel, skillHoverStyle);

					Color c = Color.white;
					GUI.color = c;

					r.x -= 1;
					r.y -= 1;
					GUI.Label(r, "Level + " + slotLevel, skillHoverStyle);
				}
			}*/

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

			levelsViewCanvas = GameObject.Find("LevelsView").GetComponent<Canvas>();
			levelsViewCanvas.enabled = false;

			//msgStyle = new GUIStyle();
			//msgStyle.fontSize = 20;
			//msgStyle.alignment = TextAnchor.LowerCenter;

			//boxStyle = new GUIStyle();

			helpCanvas = GameObject.Find("HelpCanvas");
			helpCanvas.GetComponent<Canvas>().enabled = false;

			helpCanvasPanel = helpCanvas.transform.FindChild("HelpCanvasPanel").gameObject;

			skillPanel = gameMenu.transform.FindChild("SkillPanel").gameObject;

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

			UpdateSkillTimers();

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
				disposeValObj = GameObject.Find("DisposeValue");
				ShowTrashBin(false);

				iconTemplate = Resources.Load("Sprite/inventory/Slot") as GameObject;
				GameObject inventoryContentPanel = GameObject.Find("InventoryContent");
				GameObject activeStatsPanel = GameObject.Find("ActiveStatsPanel");

				//GameObject.Find("InvScrollbar").GetComponent<Scrollbar>().value = 1f;

				inventoryTooltipObject = Resources.Load("Sprite/inventory/UpgradeTooltip") as GameObject;
				skillTooltipObject = Resources.Load("Sprite/inventory/SkillTooltip") as GameObject;
				levelTooltipObject = Resources.Load("Sprite/inventory/LevelTooltip") as GameObject;
				levelLineDot = Resources.Load("Sprite/inventory/LevelLineDot") as GameObject;

				iconEmptySprite = Resources.Load<Sprite>("Sprite/inventory/icon_empty");
				lockedIconSprite = Resources.Load<Sprite>("Sprite/inventory/icon_locked");

				basestatUpgradesContent = GameObject.Find("BasestatUpgradesContent");

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
					if(o.enabled)
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

			levelIconTemplate = GameObject.Find("LevelIconTemplate");
			shopButtonTemplate = GameObject.Find("LevelsViewShopButton");
			levelsViewPanel = GameObject.Find("LevelsViewPanel");
			levelsViewTitle = GameObject.Find("LevelsTitle");

			showViewCanvas = GameObject.Find("ShopView").GetComponent<Canvas>();
			shopContent = GameObject.Find("ShopContent");
			shopStatsViewPanel = GameObject.Find("ShopStatsViewPanel");
			shopItemTemplate = Resources.Load<GameObject>("Sprite/inventory/ShopItem");

			consoleCanvas = GameObject.Find("ConsoleCanvas").GetComponent<Canvas>();
			consoleCanvas.enabled = false;

			dialogConfirmObject = GameObject.Find("ConfirmDialog");
			dialogConfirmPanel = dialogConfirmObject.transform.FindChild("ConfirmCanvasPanel").gameObject;

			classOverviewCanvas = GameObject.Find("ClassOverviewCanvas").GetComponent<Canvas>();
			availableSkillsPanel = GameObject.Find("AvailableSkillsPanel");
			selectedSkillsPanel = GameObject.Find("SelectedSkillsPanel");
			selectedAutoattackPanel = GameObject.Find("SelectedAutoattackPanel");
			availableSkillsPanelContent = availableSkillsPanel.transform.GetChild(0).gameObject;
			skillItemTemplate = Resources.Load<GameObject>("Sprite/inventory/SkillSlot");
			classOverviewLabel = GameObject.Find("ClassOverviewLabel").GetComponent<Text>();
			upgradeHintLabel = GameObject.Find("UpgradeHintLabel").GetComponent<Text>();
			skillSlotLabelTemplate = Resources.Load<GameObject>("Sprite/inventory/SkillLabel");
			upgradePointsLabel = GameObject.Find("UpgradePointsLabel").GetComponent<Text>();
			classViewStatsUpgrades = GameObject.Find("ClassViewStatsUpgrades");

			classViewStats = new List<Text>();
			foreach (Transform t in GameObject.Find("ClassViewStats").transform)
			{
				classViewStats.Add(t.gameObject.GetComponent<Text>());
			}

			switchActiveSkillsPanelButton = GameObject.Find("SwitchActiveSkillsPanelButton").GetComponent<Button>();
			switchAutottackPanelButton = GameObject.Find("SwitchAutoattackPanelButton").GetComponent<Button>();

			selectedAutoattackPanel.SetActive(false);
			switchActiveSkillsPanelButton.interactable = false;

			foreach (Scrollbar scrollBar in GameObject.FindObjectsOfType<Scrollbar>())
			{
				scrollBar.value = 1;
			}

			HideLevelsView();

			InitStash();
		}

		private bool classOverviewEnabled;
		private bool skillUpgradeMode = false;

		public void SwitchUpgradeMode()
		{
			skillUpgradeMode = !skillUpgradeMode;
			UpdateClassOverview(true, true);
		}

		public void ShowClassOverview()
		{
			if (classOverviewCanvas.enabled)
			{
				HideClassOverview();
				return;
			}

			classOverviewEnabled = true;
			classOverviewCanvas.enabled = true;

			UpdateClassOverview(true, true);
		}

		public void UpdateClassOverview(bool updateSkills=true, bool updateStats=false)
		{
			upgradePointsLabel.text = "Upgrade Points: " + ((Player) data.GetOwner()).UpgradePoints;

			if (skillUpgradeMode)
			{
				classOverviewLabel.text = "";
				upgradeHintLabel.text = "Click any icon to upgrade it.";
			}
			else if (currentSkillsView == 1)
				classOverviewLabel.text = "Selected Skills:";

			if (!skillUpgradeMode)
			{
				upgradeHintLabel.text = "";
			}

			if (updateStats)
			{
				foreach (Text t in classViewStats)
				{
					switch (t.gameObject.name)
					{
						case "Level":
							t.text = "Level " + data.level;
							break;
						case "XP":
							t.text = data.GetOwner().Status.XP + " / " + GameProgressTable.GetXpForLevel(data.level + 1) + " XP";
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
						case "DNA": //TODO add to inventory
							t.text = ((Player)data.GetOwner()).DnaPoints + "p";
							break;
					}
				}

				UpdateStatsInfo();

				foreach (Transform g in classViewStatsUpgrades.transform)
				{
					Destroy(g.gameObject);
				}

				Inventory inv = data.GetOwner().Inventory;

				for (int i = 0; i < BASESTAT_UPGRADES_SIZE; i++)
				{
					GameObject newIcon = Instantiate(iconTemplate);
					newIcon.name = "BaseSlot_" + (i + 1);
					newIcon.transform.parent = classViewStatsUpgrades.transform;
					newIcon.transform.localScale = new Vector3(1, 1, 1);

					GameObject child = newIcon.transform.GetChild(0).gameObject;

					EventTrigger trigger = child.AddComponent<EventTrigger>();

					int tempI = i;

					EventTrigger.Entry entry;

					if (skillUpgradeMode)
					{
						entry = new EventTrigger.Entry();
						entry.eventID = EventTriggerType.PointerDown;
						entry.callback.AddListener(delegate { OnClassViewUpgradeClick(tempI, newIcon); });
						trigger.triggers.Add(entry);
					}

					entry = new EventTrigger.Entry();
					entry.eventID = EventTriggerType.PointerEnter;
					entry.callback.AddListener(delegate { OnUpgradeHover(newIcon, false, 2, classOverviewCanvas.gameObject); });
					trigger.triggers.Add(entry);

					entry = new EventTrigger.Entry();
					entry.eventID = EventTriggerType.PointerExit;
					entry.callback.AddListener(delegate { OnUpgradeHover(newIcon, true, 2, classOverviewCanvas.gameObject); });
					trigger.triggers.Add(entry);

					Image img = newIcon.transform.GetChild(0).GetComponent<Image>();

					EquippableItem item = null;

					try
					{
						item = inv.BasestatUpgrades[i];
						img.sprite = item.MainSprite;
					}
					catch (Exception)
					{
						img.sprite = iconEmptySprite;
						return;
					}

					if (skillUpgradeMode)
					{
						GameObject label = Instantiate(skillSlotLabelTemplate);
						label.transform.parent = newIcon.transform;
						label.transform.localPosition = new Vector3(0, -50, 0);
						label.transform.localScale = new Vector3(1, 1, 1);

						int price = ((Player) data.GetOwner()).GetStatUpgradePrice(item.Level);
						if (((Player) data.GetOwner()).UpgradePoints >= price)
						{
							label.GetComponent<Text>().color = Color.green;
						}
						else
						{
							label.GetComponent<Text>().color = Color.red;
						}

						label.GetComponent<Text>().text = "Price " + price;

						label = Instantiate(skillSlotLabelTemplate);
						label.transform.parent = newIcon.transform;
						label.transform.localPosition = new Vector3(0, 47, 0);
						label.transform.localScale = new Vector3(1, 1, 1);

						label.GetComponent<Text>().color = Color.yellow;
						label.GetComponent<Text>().text = "Level " + item.Level;
					}
				}
			}

			if (updateSkills)
			{
				foreach (Transform g in selectedSkillsPanel.transform)
				{
					Destroy(g.gameObject);
				}

				foreach (Transform g in availableSkillsPanelContent.transform)
				{
					Destroy(g.gameObject);
				}

				foreach (Transform g in selectedSkillsPanel.transform.parent.transform)
				{
					if(g.name.StartsWith("SkillLabel"))
						Destroy(g.gameObject);
				}

				List<GameObject> temp = new List<GameObject>();

				if (currentSkillsView == 1)
				{
					SkillSet playerSkills = data.GetOwner().Skills;

					int count = 0;

					if (skillUpgradeMode)
					{
						for (int i = count; i < 5; i++)
						{
							GameObject itemObject = Instantiate(skillItemTemplate, new Vector3(0, 0), Quaternion.identity) as GameObject;
							itemObject.name = "EmptySkillSlot_" + i;
							itemObject.transform.parent = selectedSkillsPanel.transform;
							itemObject.transform.localPosition = new Vector3(0, 0);
							itemObject.transform.localScale = new Vector3(1, 1);

							EventTrigger trigger = itemObject.AddComponent<EventTrigger>();

							int tt = i;

							EventTrigger.Entry entry = new EventTrigger.Entry();
							entry.eventID = EventTriggerType.PointerDown;
							entry.callback.AddListener(delegate { TrySlotUpgrade(tt); });
							trigger.triggers.Add(entry);

							temp.Add(itemObject);
						}
					}
					else
					{
						foreach (Skill sk in playerSkills.Skills)
						{
							if (sk == null)
								continue;

							Skill skTemp = sk;

							count++;

							GameObject itemObject = Instantiate(skillItemTemplate, new Vector3(0, 0), Quaternion.identity) as GameObject;
							itemObject.name = "ActiveSkillSlot_" + sk.GetName();
							itemObject.transform.parent = selectedSkillsPanel.transform;
							itemObject.transform.localPosition = new Vector3(0, 0);
							itemObject.transform.localScale = new Vector3(1, 1);

							Image img = itemObject.GetComponent<Image>();
							img.sprite = sk.Icon;

							EventTrigger trigger = itemObject.AddComponent<EventTrigger>();

							EventTrigger.Entry entry = new EventTrigger.Entry();
							entry.eventID = EventTriggerType.PointerDown;
							entry.callback.AddListener(delegate { OnSkillClick(itemObject, skTemp); });
							trigger.triggers.Add(entry);

							entry = new EventTrigger.Entry();
							entry.eventID = EventTriggerType.PointerEnter;
							entry.callback.AddListener(delegate { OnSkillHover(itemObject, false, skTemp); });
							trigger.triggers.Add(entry);

							entry = new EventTrigger.Entry();
							entry.eventID = EventTriggerType.PointerExit;
							entry.callback.AddListener(delegate { OnSkillHover(itemObject, true, skTemp); });
							trigger.triggers.Add(entry);

							temp.Add(itemObject);
						}

						for (int i = count; i < 5; i++)
						{
							GameObject itemObject = Instantiate(skillItemTemplate, new Vector3(0, 0), Quaternion.identity) as GameObject;
							itemObject.name = "EmptySkillSlot_" + i;
							itemObject.transform.parent = selectedSkillsPanel.transform;
							itemObject.transform.localPosition = new Vector3(0, 0);
							itemObject.transform.localScale = new Vector3(1, 1);

							temp.Add(itemObject);
						}
					}
				}
				else if (currentSkillsView == 2)
				{
					ActiveSkill currentAutoattack = ((Player)data.GetOwner()).MeleeSkill;
					GameObject mainPanel = selectedAutoattackPanel.transform.GetChild(0).gameObject;
					foreach (Transform child in mainPanel.transform)
					{
						if (child.name.Equals("Icon"))
						{
							child.GetComponent<Image>().sprite = currentAutoattack.Icon;
							continue;
						}
						else if (child.name.Equals("Name"))
						{
							child.GetComponent<Text>().text = currentAutoattack.GetVisibleName();
							continue;
						}
						else if (child.name.Equals("Description"))
						{
							child.GetComponent<Text>().text = currentAutoattack.GetDescription();
							continue;
						}
						else
						{
							Destroy(child.gameObject);
						}
					}
				}

				List<Skill> skillData = null;

				if (currentSkillsView == 1)
				{
					skillData = ((Player)data.GetOwner()).AvailableSkills;
				}
				else if (currentSkillsView == 2)
				{
					skillData = ((Player)data.GetOwner()).AvailableAutoattacks;
				}
				else return;

				foreach (Skill sk in skillData)
				{
					Skill skTemp = sk;

					if (sk == null || sk.Icon == null || (!sk.AvailableToPlayer && !sk.AvailableToDeveloper && currentSkillsView == 1) || (!sk.AvailableToPlayerAsAutoattack && currentSkillsView == 2))
						continue;

					if (((Player)data.GetOwner()).Skills.HasSkill(sk.GetSkillId()) || (((Player)data.GetOwner()).MeleeSkill != null && ((Player)data.GetOwner()).MeleeSkill.GetSkillId() == sk.GetSkillId()))
						continue;

					GameObject itemObject = Instantiate(skillItemTemplate, new Vector3(0, 0), Quaternion.identity) as GameObject;
					itemObject.name = "AvailableSlot_" + sk.GetName();
					itemObject.transform.parent = availableSkillsPanelContent.transform;
					itemObject.transform.localPosition = new Vector3(0, 0);
					itemObject.transform.localScale = new Vector3(1, 1);

					Image img = itemObject.GetComponent<Image>();

					img.sprite = sk.Icon;

					EventTrigger trigger = itemObject.AddComponent<EventTrigger>();

					EventTrigger.Entry entry = new EventTrigger.Entry();
					entry.eventID = EventTriggerType.PointerDown;
					entry.callback.AddListener(delegate { OnSkillClick(itemObject, skTemp); });
					trigger.triggers.Add(entry);

					entry = new EventTrigger.Entry();
					entry.eventID = EventTriggerType.PointerEnter;
					entry.callback.AddListener(delegate { OnSkillHover(itemObject, false, skTemp); });
					trigger.triggers.Add(entry);

					entry = new EventTrigger.Entry();
					entry.eventID = EventTriggerType.PointerExit;
					entry.callback.AddListener(delegate { OnSkillHover(itemObject, true, skTemp); });
					trigger.triggers.Add(entry);
				}

				if (currentSkillsView == 1)
				{
					Player player = data.GetOwner() as Player;

					int i = 0;
					foreach(GameObject obj in temp)
					{
						GameObject label = Instantiate(skillSlotLabelTemplate);
						label.transform.parent = obj.transform;
						label.transform.localPosition = new Vector3(0, -50, 0);
						label.transform.localScale = new Vector3(1, 1, 1);

						if (skillUpgradeMode)
						{
							int price = player.GetSlotUpgradePrice(i, player.SkillSlotLevels[i] + 1);

							if (((Player)data.GetOwner()).UpgradePoints >= price)
							{
								label.GetComponent<Text>().color = Color.green;
							}
							else
							{
								label.GetComponent<Text>().color = Color.red;
							}

							label.GetComponent<Text>().text = "Price " + price;

							GameObject label2 = Instantiate(skillSlotLabelTemplate);
							label2.transform.parent = obj.transform;
							label2.transform.localPosition = new Vector3(0, 47, 0);
							label2.transform.localScale = new Vector3(1, 1, 1);

							label2.GetComponent<Text>().color = Color.yellow;
							label2.GetComponent<Text>().text = "Level " + player.SkillSlotLevels[i];
						}
						else
						{
							label.GetComponent<Text>().text = "Slot Lv" + player.SkillSlotLevels[i];
						}

						i++;
					}
				}
			}
		}

		public void OnClassViewUpgradeClick(int slotId, GameObject slotObject)
		{
			Player player = ((Player) data.GetOwner());

			Inventory inv = player.Inventory;

			EquippableItem item = inv.BasestatUpgrades[slotId];

			int price = player.GetStatUpgradePrice(item.Level);

			if (player.UpgradePoints >= price)
			{
				player.UpgradePoints -= price;
				item.AddUpgradeProgress(null);
				player.UpdateStats();

				UpdateClassOverview(true, true);
			}
		}

		public void HideClassOverview()
		{
			if (stashOpened)
			{
				SwitchStash();
			}

			classOverviewEnabled = false;

			foreach (Transform g in classViewStatsUpgrades.transform)
			{
				Destroy(g.gameObject);
			}

			foreach (Transform g in selectedSkillsPanel.transform)
			{
				Destroy(g.gameObject);
			}

			foreach (Transform g in availableSkillsPanelContent.transform)
			{
				Destroy(g.gameObject);
			}

			foreach (Transform g in selectedSkillsPanel.transform.parent.transform)
			{
				if (g.name.StartsWith("SkillLabel"))
					Destroy(g.gameObject);
			}

			classOverviewCanvas.enabled = false;
		}

		public void SwitchClassSkillsView(int val)
		{
			if (val == 1)
			{
				currentSkillsView = 1;
				selectedSkillsPanel.SetActive(true);
				selectedAutoattackPanel.SetActive(false);
				classOverviewLabel.text = "Selected Skills";
				switchActiveSkillsPanelButton.interactable = false;
				switchAutottackPanelButton.interactable = true;
				UpdateClassOverview(true);
			}
			else if (val == 2)
			{
				currentSkillsView = 2;
				selectedSkillsPanel.SetActive(false);
				selectedAutoattackPanel.SetActive(true);
				classOverviewLabel.text = "";
				switchActiveSkillsPanelButton.interactable = true;
				switchAutottackPanelButton.interactable = false;
				UpdateClassOverview(true);
			}
		}

		public void TrySlotUpgrade(int slot)
		{
			if (skillUpgradeMode)
			{
				((Player) data.GetOwner()).LevelUpSlot(slot);
				UpdateClassOverview(true, true);
				return;
			}
		}

		public void OnSkillClick(GameObject skillObject, Skill skill)
		{
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

			draggedSkill = skill;

			if (draggedSkill == null)
				return;

			//draggedUpgradeSlot = slotType;

			if (doubleClick)
			{
				if (currentSkillsView == 2)
				{
					((Player)data.GetOwner()).SelectAutoattack(draggedSkill);
					UpdateClassOverview(true);
					return;
				}

				//TODO move to first slot available
				/*if (draggedItem is ActivableItem)
				{
					((ActivableItem)draggedItem).OnActivate();
					if (((ActivableItem)draggedItem).ConsumeOnUse)
					{
						data.GetOwner().RemoveItem(draggedItem, false);
					}
				}
				else
					data.GetOwner().SwapItem(draggedItem, null, -1, draggedUpgradeSlot, targetSlot);*/
			}
			else
			{
				draggingIcon = true;
				SetMouseOverUi();

				Vector3 mousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
				mousePos.z = 0;

				GameObject preview = new GameObject("Dragged Icon");
				Image ren = preview.AddComponent<Image>();
				ren.sprite = skill.Icon;
				preview.transform.parent = classOverviewCanvas.gameObject.transform;
				//preview.transform.position = mousePos;
				preview.GetComponent<RectTransform>().sizeDelta = new Vector2(80, 80);

				draggedObject = preview;
				//ShowTrashBin(true, draggedItem.DisposePrice);
			}
		}

		public void OnSkillHover(GameObject skillObject, bool exit, Skill skill)
		{
			if (exit)
			{

			}
			else
			{
				string name = skill.GetVisibleName();
				string description = skill.GetDescription();

				if (description == null)
					description = "No description";

				if (draggingIcon)
					return;

				if (currentTooltipObject != null)
					Destroy(currentTooltipObject);

				highlightedObject = skillObject;
				currentTooltipObject = Instantiate(skillTooltipObject);
				currentTooltipObject.transform.parent = classOverviewCanvas.gameObject.transform;
				currentTooltipObject.transform.position = Input.mousePosition;
				currentTooltipObject.transform.localScale = new Vector3(1, 1, 1);
				UpdatePivotForTooltip(currentTooltipObject);

				Color titleColor = GetSkillSlotBgColor(skill.RequiredSlotLevel);
				currentTooltipObject.GetComponent<Image>().color = titleColor;

				foreach (Transform child in currentTooltipObject.transform)
				{
					if (child.name.Equals("Title"))
					{
						Color color = GetSkillSlotTitleColor(skill.RequiredSlotLevel);

						child.GetComponent<Text>().text = name;
						child.GetComponent<Text>().color = color;
						//child.GetComponent<Text>().color = titleColor;
						continue;
					}
					else if (child.name.Equals("Description"))
					{
						child.GetComponent<Text>().text = Utils.StringWrap(description, 40);
						continue;
					}
					else if (child.name.Equals("BaseInfo"))
					{
						string baseInfo = skill.GetBaseInfo();
						if(baseInfo != null)
							child.GetComponent<Text>().text = skill.GetBaseInfo();
						else
							Destroy(child.gameObject);

						continue;
					}
					else if (child.name.Equals("AdditionalInfo"))
					{
						Color color = GetSkillSlotLevelColor(skill.RequiredSlotLevel);
						
						child.GetComponent<Text>().text = "Requires slot of level " + skill.RequiredSlotLevel;
						child.GetComponent<Text>().color = color;
						continue;
					}
					else
					{
						Destroy(child.gameObject);
					}
				}

				return;
			}

			/*if (u == null)
			{
				int order = Int32.Parse(slot.name.Split('_')[1]);
				string description = GameProgressTable.GetDescriptionOnLockedSlot(order, slotType);

				if (description == null || draggingIcon)
					return;

				if (currentTooltipObject != null)
					Destroy(currentTooltipObject);

				highlightedObject = slot;
				currentTooltipObject = Instantiate(inventoryTooltipObject);
				currentTooltipObject.transform.parent = inventoryPanel.transform;
				currentTooltipObject.transform.position = Input.mousePosition;

				Color titleColor = new Color();
				titleColor = new Color(86 / 255f, 71 / 255f, 49 / 255f);
				currentTooltipObject.GetComponent<Image>().color = new Color(253 / 255f, 253 / 255f, 224 / 225f);

				foreach (Transform child in currentTooltipObject.transform)
				{
					if (child.name.Equals("Title"))
					{
						child.GetComponent<Text>().text = "Locked slot";
						child.GetComponent<Text>().color = titleColor;
						continue;
					}
					else if (child.name.Equals("Description"))
					{
						child.GetComponent<Text>().text = description;
						continue;
					}
					else
					{
						Destroy(child.gameObject);
					}
				}

				return;
			}*/

			/*if (!exit)
			{
				if (draggingIcon)
					return;

				if (currentTooltipObject != null)
					Destroy(currentTooltipObject);

				highlightedObject = slot;
				currentTooltipObject = Instantiate(inventoryTooltipObject);
				currentTooltipObject.transform.parent = inventoryPanel.transform;
				currentTooltipObject.transform.position = Input.mousePosition;

				string name = u.VisibleName;
				string typeName = u.TypeName;
				string description = Utils.StringWrap(u.Description, 40);
				string price = u.Price;
				string addInfo = u.AdditionalInfo;

				if (u.Level == 0)
				{
					description = "No bonus effect yet.\nKill enemies to evolve this module.";
				}

				if (u.IsUpgrade())
				{
					EquippableItem upg = u as EquippableItem;

					price = Enum.GetName(typeof(ItemType), ((EquippableItem)u).Type);

					int currentUpgradeProgress = upg.CurrentProgress;
					int needForNext = upg.NeedForNextLevel;

					if (upg.GoesIntoBasestatSlot && needForNext > 1)
						addInfo = "Level-up progress:\n " + currentUpgradeProgress + " / " + needForNext + " upgrade modules.";
				}

				if (addInfo == null)
				{
					if (u is EquippableItem && ((EquippableItem)u).GoesIntoBasestatSlot)
					{
						addInfo = "Cannot be disposed.";
					}
					else
					{
						addInfo = "Dispose value: " + u.DisposePrice + " DNA";
					}
				}

				ItemType type = u.Type;

				Color titleColor = new Color();

				switch (type)
				{
					case ItemType.CLASSIC_UPGRADE:
						titleColor = new Color(86 / 255f, 71 / 255f, 49 / 255f);
						currentTooltipObject.GetComponent<Image>().color = new Color(253 / 255f, 253 / 255f, 224 / 225f);
						break;
					case ItemType.EPIC_UPGRADE:
						titleColor = new Color(109 / 255f, 58 / 255f, 65 / 255f);
						currentTooltipObject.GetComponent<Image>().color = new Color(253 / 255f, 253 / 255f, 224 / 225f);
						break;
					case ItemType.RARE_UPGRADE:
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
					else if (child.name.Equals("Type") && description != null)
					{
						child.GetComponent<Text>().text = "[" + typeName + " Upgrade] [" + price + "]";
						continue;
					}
					else if (child.name.Equals("Description") && description != null)
					{
						child.GetComponent<Text>().text = description;
						continue;
					}*/
					/*else if (child.name.Equals("Price") && price != null)
					{
						child.GetComponent<Text>().text = price;
						child.GetComponent<Text>().color = titleColor;
						continue;
					}*/
					/*else if (child.name.Equals("AdditionalInfo") && (addInfo != null))
					{
						string info = addInfo;

						child.GetComponent<Text>().text = info;
						continue;
					}
				}
			}*/
		}

		private Color GetSkillSlotLevelColor(int level)
		{
			Color color;
			switch (level)
			{
				case 1:
					color = new Color(113 / 255f, 10 / 255f, 10 / 255f);
					break;
				/*case 2:
					color = new Color(86 / 255f, 71 / 255f, 49 / 255f);
					break;
				case 3:
					color = new Color(86 / 255f, 71 / 255f, 49 / 255f);
					break;
				case 4:
					color = new Color(86 / 255f, 71 / 255f, 49 / 255f);
					break;
				case 5:
					color = new Color(86 / 255f, 71 / 255f, 49 / 255f);
					break;*/
				default:
					color = new Color(113 / 255f, 10 / 255f, 10 / 255f);
					break;
			}
			return color;
		}

		private Color GetSkillSlotTitleColor(int level)
		{
			Color color;
			switch (level)
			{
				case 1:
					color = new Color(64 / 255f, 83 / 255f, 120 / 255f);
					break;
				case 2:
					color = new Color(42 / 255f, 99 / 255f, 47 / 255f);
					break;
				case 3:
					color = new Color(114 / 255f, 71 / 255f, 24 / 255f);
					break;
				case 4:
					color = new Color(106 / 255f, 24 / 255f, 114 / 255f);
					break;
				case 5:
					color = new Color(114 / 255f, 24 / 255f, 24 / 255f);
					break;
				default:
					color = new Color(114 / 255f, 24 / 255f, 24 / 255f);
					break;
			}
			return color;
		}

		private Color GetSkillSlotBgColor(int level)
		{
			Color color;
			switch (level)
			{
				case 1:
					color = new Color(255f / 255f, 255f / 255f, 255f / 255f, 224f / 255f);
					break;
				case 2:
					color = new Color(253 / 255f, 255 / 255f, 227 / 255f, 224f / 255f);
					break;
				default:
					color = new Color(253 / 255f, 255 / 255f, 227 / 255f, 224f / 255f);
					break;
			}
			return color;
		}

		private GameObject levelIconTemplate;
		private GameObject shopButtonTemplate;
		private GameObject levelsViewPanel;

		private GameObject levelsViewTitle;

		private Dictionary<GameObject, LevelTree> levelsViewIcons = new Dictionary<GameObject, LevelTree>();

		private DrawData[] nodeDrawData; // index = id of the node
		private bool isDrawn = false;

		public class DrawData
		{
			public LevelTree node;

			// 0 = right, 1 = left
			public int mainDirection;
			public int children;

			public DrawData(LevelTree n)
			{
				this.node = n;
				mainDirection = 0;
				children = 0;
			}
		}

		public void DrawLevelsView()
		{
			// delete if not empty
			if (levelsViewIcons.Any())
			{
				foreach (GameObject o in levelsViewIcons.Keys)
				{
					Destroy(o);
				}

				foreach (GameObject o in dots)
				{
					Destroy(o);
				}

				dots.Clear();

				levelsViewIcons.Clear();
			}

			levelsViewTitle.GetComponent<Text>().text = "World " + WorldHolder.instance.worldLevel;

			LevelTree main = WorldHolder.instance.mapTree;

			List<LevelTree> mainNodes = main.GetAllMainNodes();
			List<LevelTree> nodes = main.GetAllNodes();

			nodeDrawData = new DrawData[nodes.Count];

			foreach (LevelTree n in nodes)
			{
				nodeDrawData[n.Id] = new DrawData(n);
			}

			const int maxNodesDown = 5;

			int spacingY = Screen.height / maxNodesDown;
			int spacingX = Math.Min(spacingY, Screen.width / 7);
			int rndHeight = spacingY / 5;
			int rndWidth = spacingX / 2;

			float x = 0;
			float y = 0;
			int depth;
			bool done;

			// draw main nodes
			for (int i = 0; i < 100; i++)
			{
				done = false;
				foreach (LevelTree t in mainNodes)
				{
					depth = t.Depth;

					y = (Screen.height / 2f) - (spacingY) * (depth + 1) + Random.Range(-rndHeight, rndHeight) + (spacingY / 2f);
					x = Random.Range(-rndWidth, rndWidth);

					Debug.Log(y);

					GameObject newImg = Instantiate(levelIconTemplate);
					newImg.GetComponent<Image>().enabled = true;
					newImg.name = "Main" + t.Name + "IconD" + t.Depth;
					newImg.transform.parent = levelsViewPanel.transform;
					RectTransform trans = newImg.GetComponent<RectTransform>();
					trans.localPosition = new Vector3(x, y);

					if (t.levelData != null && t.levelData.shopData != null)
					{
						GameObject textObj = Instantiate(shopButtonTemplate);
						textObj.GetComponent<Image>().enabled = true;
						textObj.GetComponent<Button>().enabled = true;
						textObj.transform.parent = newImg.transform;
						textObj.transform.localPosition = new Vector3(newImg.GetComponent<RectTransform>().sizeDelta.x, 0);

						var t1 = t;
						textObj.GetComponent<Button>().onClick.AddListener(delegate { OnShopButtonClick(t1); });
					}

					if (!t.Unlocked)
					{
						newImg.GetComponent<Image>().color = Color.gray;
					}

					if (t.CurrentlyActive)
					{
						newImg.GetComponent<Image>().color = Color.yellow;
					}

					AddLevelHoverAction(newImg);
					levelsViewIcons.Add(newImg, t);

					if (t.IsLastNode)
						done = true;
				}

				if (done) // all main nodes done
					break;
			}

			// draw secondary nodes
			done = false;

			// draw the secondary nodes
			for (int i = 0; i < 100; i++)
			{
				foreach (LevelTree node in nodes)
				{
					if (node.Depth == i && node.LevelNodeType == LevelTree.LEVEL_EXTRA)
					{
						GameObject parentNodeObj = null;
						LevelTree parentNode = node.Parent;

						// get the main node for the current node
						foreach (KeyValuePair<GameObject, LevelTree> e in levelsViewIcons)
						{
							if (e.Value.Depth == node.Depth && node.Parent.Equals(e.Value))
							{
								parentNodeObj = e.Key;
							}
						}

						if (parentNodeObj == null || parentNode == null)
						{
							Debug.LogError("main or parent node are null");
							continue;
						}

						depth = node.Depth;

						//TODO optimize - draw only once then on update
						//TODO if main node has more than 2 childs, place them well
						//TODO if special node has one child, put it on the same side

						DrawData parentDrawData = nodeDrawData[parentNode.Id];
						DrawData childDrawData = nodeDrawData[node.Id];

						float mainX = parentNodeObj.transform.localPosition.x;

						x = 0;
						y = parentNodeObj.transform.localPosition.y - Random.Range(rndHeight / 2, rndHeight);

						switch (parentDrawData.children)
						{
							case 0: // no childs, place to the right

								if (parentDrawData.mainDirection == 0)
								{
									x = mainX + spacingX;
									childDrawData.mainDirection = 0;
								}
								else
								{
									x = mainX - spacingX;
									childDrawData.mainDirection = 0;
								}

								break;
							case 1: // place to the left
								x = mainX - spacingX;
								childDrawData.mainDirection = 1;
								break;
							case 2:
								x = mainX + spacingX;
								y -= (spacingY / 2f);
								childDrawData.mainDirection = 0;
								break;
							case 3:
								x = mainX - spacingX;
								y -= (spacingY / 2f);
								childDrawData.mainDirection = 1;
								break;
							case 4:
								x = mainX + 1.5f * spacingX;
								y -= (spacingY / 2f);
								childDrawData.mainDirection = 0;
								break;
							case 5:
								x = mainX + 1.5f * spacingX;
								y -= (spacingY / 2f);
								childDrawData.mainDirection = 1;
								break;
						}

						parentDrawData.children ++;

						GameObject newImg = Instantiate(levelIconTemplate);
						newImg.GetComponent<Image>().enabled = true;
						//newImg.GetComponent<Image>().color = Color.red;

						if (node.CurrentlyActive)
						{
							newImg.GetComponent<Image>().color = Color.yellow;
						}

						newImg.name = "Extra" + node.Name + "IconD" + node.Depth;
						newImg.transform.parent = levelsViewPanel.transform;
						RectTransform trans = newImg.GetComponent<RectTransform>();
						trans.localPosition = new Vector3(x, y);

						if (!node.Unlocked)
						{
							newImg.GetComponent<Image>().color = Color.gray;
						}

						AddLevelHoverAction(newImg);
						levelsViewIcons.Add(newImg, node);

						if (node.IsLastNode)
							done = true;
					}
				}

				if (done)
					break;
			}

			

			// draw lines

			foreach (KeyValuePair<GameObject, LevelTree> e in levelsViewIcons)
			{
				GameObject first = e.Key;
				GameObject second;

				foreach (LevelTree n in e.Value.Childs)
				{
					if (n.IsLastNode)
					{
						Debug.Log(e.Value.Name + " has last node");
					}

					foreach (KeyValuePair<GameObject, LevelTree> e1 in levelsViewIcons)
					{
						if (e1.Value.Id == n.Id)
						{
							if (e1.Value.IsLastNode)
							{
								Debug.Log("connecting to last node node " + e.Value.Name);
							}

							second = e1.Key;
							ConnectIcons(first, second, n.Unlocked);
							second = null;
							break;
						}
					}
				}

				first = null;
			}
		}

		public void ShowLevelsView()
		{
			if (!isDrawn)
			{
				shopButtonTemplate.SetActive(true);
				DrawLevelsView();
				shopButtonTemplate.SetActive(false);
			}

			levelsViewCanvas.enabled = true;
			SetMouseOverUi();
			//END here
		}

		public void OnShopButtonClick(LevelTree node)
		{
			if (node.levelData != null && node.levelData.shopData != null)
			{
				ShowShopView(node.levelData.shopData);
			}
		}

		private void ConnectIcons(GameObject first, GameObject second, bool secondUnlocked)
		{
			int x1 = (int) first.transform.localPosition.x;
			int y1 = (int) first.transform.localPosition.y;

			int x2 = (int) second.transform.localPosition.x;
			int y2 = (int) second.transform.localPosition.y;

			int d = 0;

			int dx = Math.Abs(x2 - x1);
			int dy = Math.Abs(y2 - y1);

			int dy2 = (dy << 1);
			int dx2 = (dx << 1);

			int ix = x1 < x2 ? 1 : -1;
			int iy = y1 < y2 ? 1 : -1;

			if (dy <= dx)
			{
				for (;;)
				{
					PlantDot(x1, y1, secondUnlocked ? Color.white : Color.gray);

					if (Math.Abs(x1 - x2) < Math.Abs(ix))
						break;

					x1 += ix;
					d += dy2;

					if (d > dx)
					{
						y1 += iy;
						d -= dx2;
					}
				}
			}
			else
			{
				for (;;)
				{
					PlantDot(x1, y1, secondUnlocked ? Color.white : Color.gray);

					if (Math.Abs(y1 - y2) < Math.Abs(iy))
						break;

					y1 += iy;
					d += dx2;
					if (d > dy)
					{
						x1 += ix;
						d -= dy2;
					}
				}
			}
		}

		private int counter = 0;
		private List<GameObject> dots = new List<GameObject>(); 

		private void PlantDot(float x, float y, Color color)
		{
			counter ++;
			if (counter%20 == 0)
			{
				GameObject dot = Instantiate(levelLineDot);
				dot.GetComponent<Image>().color = color;
				dot.transform.parent = levelsViewPanel.transform;
				dot.transform.localPosition = new Vector3(x, y);
				dot.transform.SetAsFirstSibling();
				dots.Add(dot);
			}
		}

		private void AddLevelHoverAction(GameObject target)
		{
			EventTrigger trigger = target.AddComponent<EventTrigger>();

			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerDown;
			entry.callback.AddListener(delegate { OnLevelIconClick(target); });
			trigger.triggers.Add(entry);

			entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerEnter;
			entry.callback.AddListener(delegate { OnLevelIconHover(target, false); });
			trigger.triggers.Add(entry);

			entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerExit;
			entry.callback.AddListener(delegate { OnLevelIconHover(target, true); });
			trigger.triggers.Add(entry);
		}

		public void HideLevelsView(bool clear=false)
		{
			if (clear)
			{
				foreach (GameObject o in levelsViewIcons.Keys)
				{
					Destroy(o);
				}

				foreach (GameObject o in dots)
				{
					Destroy(o);
				}

				dots.Clear();

				levelsViewIcons.Clear();
			}

			levelsViewCanvas.enabled = false;
			SetMouseNotOverUi();
		}

		public void OnLevelIconClick(GameObject target)
		{
			LevelTree node;

			if (!levelsViewIcons.TryGetValue(target, out node))
				return;

			if(WorldHolder.instance.OnLevelSelect(data, node))
				HideLevelsView();
		}

		public void OnLevelIconHover(GameObject target, bool exit)
		{
			LevelTree node;

			if (!levelsViewIcons.TryGetValue(target, out node))
				return;

			if (!exit)
			{
				if (currentTooltipObject != null)
					Destroy(currentTooltipObject);

				highlightedObject = target;
				currentTooltipObject = Instantiate(levelTooltipObject);
				currentTooltipObject.transform.parent = levelsViewPanel.transform;
				currentTooltipObject.transform.position = Input.mousePosition;
				currentTooltipObject.transform.localScale = new Vector3(1, 1, 1);
				UpdatePivotForTooltip(currentTooltipObject);

				Color titleColor = new Color();
				titleColor = new Color(86 / 255f, 71 / 255f, 49 / 255f);
				currentTooltipObject.GetComponent<Image>().color = new Color(253 / 255f, 253 / 255f, 224 / 225f);

				string additInfo = (node.Unlocked ? null : " [Locked]");

				foreach (Transform child in currentTooltipObject.transform)
				{
					if (child.name.Equals("Title"))
					{
						child.GetComponent<Text>().text = "Level " + Enum.GetName(typeof (MapType), node.LevelParams.levelType);
						child.GetComponent<Text>().color = titleColor;
						continue;
					}
					else if (child.name.Equals("Description"))
					{
						if(node.Description != null)
							child.GetComponent<Text>().text = Utils.StringWrap(node.Description, 40);
						continue;
					}
					else if (child.name.Equals("Difficulty"))
					{
						string difficulty = "Unknown";
						switch (node.Difficulty)
						{
							case 1:
								difficulty = "Easy";break;
							case 2:
								difficulty = "Medium"; break;
							case 3:
								difficulty = "Hard"; break;
							case 4:
								difficulty = "Very Hard"; break;
						}
						child.GetComponent<Text>().text = difficulty + " difficulty";
						continue;
					}
					/*else if (child.name.Equals("Price") && price != null)
					{
						child.GetComponent<Text>().text = price;
						child.GetComponent<Text>().color = titleColor;
						continue;
					}*/
					else if (child.name.Equals("Reward"))
					{
						child.GetComponent<Text>().text = Utils.StringWrap(node.RewardDescription, 40);
						continue;
					}
					else if (child.name.Equals("AdditionalInfo") && additInfo != null)
					{
						child.GetComponent<Text>().text = Utils.StringWrap(additInfo, 40);
						continue;
					}
					else
					{
						Destroy(child.gameObject);
					}
				}
			}
			else
			{
				//if (currentTooltipObject != null)
				//	Destroy(currentTooltipObject);
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
					InventoryItem u = UpgradeTable.Instance.GenerateUpgrade(t, 1);
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
					draggedItem = null;
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
					if (r.gameObject.Equals(highlightedObject))
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

					if (max > -1)
					{
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
							skillProgresses[i].fillAmount = 1 - ratio;
						}
						catch (Exception e)
						{
							//Debug.LogError(e.Message);
						}
					}
				}
			}

			UpdateAdminInfo();
		}

		public void UpdateSkillTimers()
		{
			Player player = data.GetOwner() as Player;

			int count = player.Skills.Skills.Count;

			if (count == 0)
				return;

			foreach (Image img in skillProgresses)
			{
				img.fillAmount = 0;
			}

			skillButtons = new GameObject[5];
			timers = new float[count, 2];
			skillProgresses = new Image[count];

			for (int i = 0; i < count; i++)
			{
				Skill sk = player.Skills.GetSkill(i);

				timers[i, 0] = 1;
				timers[i, 1] = -1;

				foreach (Transform child in skillPanel.transform)
				{
					if (child.name.Equals("Skill" + (i+1)))
					{
						skillButtons[i] = child.gameObject;
						child.GetComponent<Image>().sprite = sk.Icon;
					}
				}

				GameObject sko = skillButtons[i];
				skillProgresses[i] = sko.transform.GetChild(0).GetComponent<Image>();
				skillProgresses[i].sprite = sko.GetComponent<Image>().sprite;
				skillProgresses[i].fillAmount = 0;
			}

			for (int i = count; i < 5; i++)
			{
				foreach (Transform child in skillPanel.transform)
				{
					if (child.name.Equals("Skill" + (i+1)))
					{
						skillButtons[i] = child.gameObject;
						child.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprite/ui/icons/empty_icon");
					}
				}
			}
		}

		public void SetReuseTimer(Skill sk)
		{
			int id = -1;

			for (int i = 0; i < data.GetOwner().Skills.Skills.Count; i++)
			{
				if (sk.GetName().Equals(data.GetOwner().Skills.Skills[i].GetName()))
				{
					id = i;
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
				timers[id, 1] = ((ActiveSkill)sk).GetReuse(false);
			}
		}

		public void ResetReuseTimer(Skill sk)
		{
			int id = -1;

			for (int i = 0; i < data.GetOwner().Skills.Skills.Count; i++)
			{
				if (sk.GetName().Equals(data.GetOwner().Skills.Skills[i].GetName()))
				{
					id = i;
					break;
				}
			}

			if (id == -1)
				return;

			timers[id, 0] = Time.time - ((ActiveSkill)sk).GetReuse(false);
			timers[id, 1] = ((ActiveSkill)sk).GetReuse(false);
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

		public InventoryItem GetUpgradeFromInventory(GameObject slot, int slotType)
		{
			int order = Int32.Parse(slot.name.Split('_')[1]) - 1;

			InventoryItem u = null;

			if (slotType == 1)
				u = data.GetOwner().Inventory.GetActiveUpgrade(order);
			else if (slotType == 0)
				u = data.GetOwner().Inventory.GetItem(order);
			else if (slotType == 2) // base stat
				u = data.GetOwner().Inventory.GetBasestatUpgrade(order);

			return u;
		}

		public InventoryItem GetItemFromStash(GameObject slot)
		{
			int order = Int32.Parse(slot.name.Split('_')[1]) - 1;

			InventoryItem u = ((Player) data.GetOwner()).ItemStash.GetItem(order);
			return u;
		}

		public InventoryItem GetUpgradeFromInventory(int slotIndex, int slotType)
		{
			InventoryItem u = null;

			if (slotType == 1)
				u = data.GetOwner().Inventory.GetActiveUpgrade(slotIndex);
			else if (slotType == 0)
				u = data.GetOwner().Inventory.GetItem(slotIndex);
			else if (slotType == 2) // base stat
				u = data.GetOwner().Inventory.GetBasestatUpgrade(slotIndex);

			return u;
		}

		private void UpdatePivotForTooltip(GameObject obj)
		{
			RectTransform rect = obj.GetComponent<RectTransform>();
			
			float xPos = rect.position.x;
			float yPos = rect.position.y;

			if(xPos > Screen.width/2f)
				rect.pivot = new Vector2(1.05f, rect.pivot.y);

			if(yPos > Screen.height/2f)
				rect.pivot = new Vector2(rect.pivot.x, 1.0f);
			else
				rect.pivot = new Vector2(rect.pivot.x, 0);
		}

		public void OnUpgradeHover(GameObject slot, bool exit, int slotType, GameObject targetPanel=null, bool stashMode=false, bool itemInStash=false)
		{
			InventoryItem u = GetUpgradeFromInventory(slot, slotType);

			if (itemInStash)
			{
				u = GetItemFromStash(slot);
			}

			if (u == null)
			{
				int order = Int32.Parse(slot.name.Split('_')[1]);
				string description = GameProgressTable.GetDescriptionOnLockedSlot(order, slotType);

				if (stashMode)
					description = "This slot is empty.";

				if (description == null || draggingIcon)
					return;

				if (currentTooltipObject != null)
					Destroy(currentTooltipObject);

				if (targetPanel == null)
					targetPanel = inventoryPanel;

				highlightedObject = slot;
				currentTooltipObject = Instantiate(inventoryTooltipObject);
				currentTooltipObject.transform.parent = targetPanel.transform;
				currentTooltipObject.transform.position = Input.mousePosition;
				currentTooltipObject.transform.localScale = new Vector3(1, 1, 1);
				UpdatePivotForTooltip(currentTooltipObject);

				Color titleColor = new Color();
				titleColor = new Color(86 / 255f, 71 / 255f, 49 / 255f);
				currentTooltipObject.GetComponent<Image>().color = new Color(253 / 255f, 253 / 255f, 224 / 225f);

				foreach (Transform child in currentTooltipObject.transform)
				{
					if (child.name.Equals("Title"))
					{
						child.GetComponent<Text>().text = "Locked slot";
						child.GetComponent<Text>().color = titleColor;
						continue;
					}
					else if (child.name.Equals("Description"))
					{
						child.GetComponent<Text>().text = description;
						continue;
					}
					else
					{
						Destroy(child.gameObject);
					}
				}

				return;
			}

			if (!exit)
			{
				if (draggingIcon)
					return;

				if (currentTooltipObject != null)
					Destroy(currentTooltipObject);

				if (targetPanel == null)
					targetPanel = inventoryPanel;

				highlightedObject = slot;
				currentTooltipObject = Instantiate(inventoryTooltipObject);
				currentTooltipObject.transform.parent = targetPanel.transform;
				currentTooltipObject.transform.position = Input.mousePosition;
				currentTooltipObject.transform.localScale = new Vector3(1, 1, 1);
				UpdatePivotForTooltip(currentTooltipObject);

				string name = u.VisibleName;
				string typeName = u.TypeName;
				string description = Utils.StringWrap(u.Description, 40);
				string price = u.Price;
				string addInfo = u.AdditionalInfo;

				if (u.Level == 0)
				{
					description = "No bonus effect yet.\nKill enemies to evolve this module.";
				}

				if (u.IsUpgrade())
				{
					EquippableItem upg = u as EquippableItem;

					price = Enum.GetName(typeof(ItemType), ((EquippableItem)u).Type);

					int currentUpgradeProgress = upg.CurrentProgress;
					int needForNext = upg.NeedForNextLevel;

					if (upg.GoesIntoBasestatSlot && needForNext > 1)
						addInfo = "Level-up progress:\n " + currentUpgradeProgress + " / " + needForNext + " upgrade modules.";
				}

				if (addInfo == null)
				{
					if (u is EquippableItem && ((EquippableItem) u).GoesIntoBasestatSlot)
					{
						addInfo = "Cannot be disposed.";
					}
					else
					{
						addInfo = "Dispose value: " + u.DisposePrice + " DNA";
					}
				}
				
				ItemType type = u.Type;

				Color titleColor = new Color();

				switch (type)
				{
					case ItemType.CLASSIC_UPGRADE:
						titleColor = new Color(86 / 255f, 71 / 255f, 49 / 255f);
						currentTooltipObject.GetComponent<Image>().color = new Color(253 / 255f, 253 / 255f, 224 / 225f);
						break;
					case ItemType.EPIC_UPGRADE:
						titleColor = new Color(109 / 255f, 58 / 255f, 65 / 255f);
						currentTooltipObject.GetComponent<Image>().color = new Color(253 / 255f, 253 / 255f, 224 / 225f);
						break;
					case ItemType.RARE_UPGRADE:
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
					else if (child.name.Equals("Type") && description != null)
					{
						child.GetComponent<Text>().text = "[" + typeName + " Upgrade] [" + price + "]";
						continue;
					}
					else if (child.name.Equals("Description") && description != null)
					{
						child.GetComponent<Text>().text = description;
						continue;
					}
					/*else if (child.name.Equals("Price") && price != null)
					{
						child.GetComponent<Text>().text = price;
						child.GetComponent<Text>().color = titleColor;
						continue;
					}*/
					else if (child.name.Equals("AdditionalInfo") && (addInfo != null))
					{
						string info = addInfo;

						child.GetComponent<Text>().text = info;
						continue;
					}
				}
			}
		}

		public void ShowTrashBin(bool state, int price=0)
		{
			if (state)
			{
				trashBin.SetActive(true);
				disposeValObj.GetComponent<Text>().text = "You will gain: " + price + " DNA";
				disposeValObj.SetActive(true);
			}
			else
			{
				trashBin.SetActive(false);
				disposeValObj.SetActive(false);
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
			
			bool draggingSkill = false;
			bool toActiveSkillSlot = false;
			string targetSkillName = null;
			int targetSlotId = -1;
			bool swappingAutoattack = false;

			bool stashMode = false;
			// 1 = inventory, 2 = stash
			int stashTarget = -1; 

			bool selectedAny = false;
			
			bool trashBin = false;
			foreach (RaycastResult r in hits)
			{
				if (r.gameObject.name.StartsWith("Slot"))
				{
					targetSlot = r.gameObject;
					targetSlotType = 0;
					selectedAny = true;
					break;
				}
				else if (r.gameObject.name.StartsWith("ActiveSlot"))
				{
					targetSlot = r.gameObject;
					targetSlotType = 1;
					selectedAny = true;
					break;
				}
				else if (r.gameObject.name.StartsWith("Trashbin"))
				{
					targetSlot = r.gameObject;
					trashBin = true;
					selectedAny = true;
					break;
				}
				else if (r.gameObject.name.StartsWith("EmptySkillSlot_"))
				{
					targetSkillName = null;
					targetSlotId = Int32.Parse(r.gameObject.name.Split('_')[1]);
					draggingSkill = true;
					toActiveSkillSlot = true;
					selectedAny = true;
					break;
				}
				else if (r.gameObject.name.StartsWith("AvailableSlot_"))
				{
					targetSkillName = r.gameObject.name.Split('_')[1];
					draggingSkill = true;
					toActiveSkillSlot = false;
					selectedAny = true;
					break;
				}
				else if (r.gameObject.name.StartsWith("ActiveSkillSlot_"))
				{
					targetSkillName = r.gameObject.name.Split('_')[1];
					toActiveSkillSlot = true;
					draggingSkill = true;
					selectedAny = true;
					break;
				}
				else if (r.gameObject.name.StartsWith("SelectedAutoattackPanel"))
				{
					swappingAutoattack = true;
					draggingSkill = true;
					selectedAny = true;
					break;
				}
				else if (r.gameObject.name.StartsWith("StashSlot_"))
				{
					stashMode = true;
					targetSlotId = Int32.Parse(r.gameObject.name.Split('_')[1]);
					stashTarget = STASHSLOT_STASH;

					selectedAny = true;
					break;
				}
				else if (r.gameObject.name.StartsWith("StashItemsPanel"))
				{
					stashMode = true;
					stashTarget = STASHSLOT_STASH;
					selectedAny = true;
					break;
				}
				else if (r.gameObject.name.StartsWith("StashTrashbin"))
				{
					targetSlot = r.gameObject;
					trashBin = true;
					stashMode = true;
					selectedAny = true;
					break;
				}
				else if (r.gameObject.name.StartsWith("ShashInventoryPanel"))
				{
					stashMode = true;
					stashTarget = STASHSLOT_INVENTORY;
					selectedAny = true;
					break;
				}
				else if (r.gameObject.name.StartsWith("InvSlot_"))
				{
					stashMode = true;
					targetSlotId = Int32.Parse(r.gameObject.name.Split('_')[1]);
					stashTarget = STASHSLOT_INVENTORY;

					selectedAny = true;
					break;
				}
				else if (r.gameObject.name.StartsWith("StashActiveSlot_"))
				{
					stashMode = true;
					targetSlotId = Int32.Parse(r.gameObject.name.Split('_')[1]);
					stashTarget = STASHSLOT_ACTIVE;

					selectedAny = true;
					break;
				}
				else if (r.gameObject.name.StartsWith("StashActiveItemsPanel"))
				{
					stashMode = true;
					stashTarget = STASHSLOT_ACTIVE;
					selectedAny = true;
					break;
				}
			}

			if (!selectedAny && draggedSkill != null)
			{
				draggingSkill = true;
				toActiveSkillSlot = false;
			}

			if (stashMode)
			{
				if (trashBin)
				{
					if (data.GetOwner().Inventory.HasInInventory(draggedItem))
					{
						data.player.AddDnaPoints(draggedItem.DisposePrice);
						data.player.Message("You have received " + draggedItem.DisposePrice + " DNA.");

						data.GetOwner().RemoveItem(draggedItem);
						UpdateStash();
						return;
					}
					else
					{
						data.player.AddDnaPoints(draggedItem.DisposePrice);
						data.player.Message("You have received " + draggedItem.DisposePrice + " DNA.");

						draggedItem.SetOwner(null);
						((Player)data.GetOwner()).ItemStash.RemoveItem(draggedItem);
						UpdateStash();
						return;
					}
					
				}

				if (stashTarget == STASHSLOT_STASH)
				{
					if (draggedStashSlotType == STASHSLOT_INVENTORY || draggedStashSlotType == STASHSLOT_ACTIVE)
					{
						PutToStash(draggedItem);
					}
				}
				else if (stashTarget == STASHSLOT_INVENTORY)
				{
					if (draggedStashSlotType == STASHSLOT_STASH || draggedStashSlotType == STASHSLOT_ACTIVE)
					{
						PutToInventory(draggedItem);
					}
				}
				else if (stashTarget == STASHSLOT_ACTIVE)
				{
					if (draggedStashSlotType == STASHSLOT_INVENTORY || draggedStashSlotType == STASHSLOT_STASH)
					{
						PutToActiveSlot(draggedItem, targetSlotId);
					}
				}

				ShowStashTrashBin(false);
			}

			if (!draggingSkill)
			{
				ShowTrashBin(false);

				if (trashBin)
				{
					if (draggedUpgradeSlot == 1) // is active, unequip first
						data.GetOwner().UnequipItem(draggedItem, true);

					data.player.AddDnaPoints(draggedItem.DisposePrice);
					data.player.Message("You have received " + draggedItem.DisposePrice + " DNA.");

					data.GetOwner().RemoveItem(draggedItem);
					return;
				}

				if (targetSlotType == 1 && draggedUpgradeSlot == 1)
					return;

				if (targetSlotType == 0 && draggedUpgradeSlot == 0)
					return;

				if (targetSlot != null)
				{
					InventoryItem u = GetUpgradeFromInventory(targetSlot, targetSlotType);
					int number = GetSlotNumberFromObject(targetSlot);

					// slot is not empty - swap
					if (u != null)
					{
						data.GetOwner().SwapItem(draggedItem, u, number, draggedUpgradeSlot, targetSlotType);
						Debug.Log("swapping for " + u.TypeName + " in slot " + number + " from active? " + draggedUpgradeSlot +
						          " to active? " + (targetSlotType == 1));
					}
					else // slot is empty - simply move the upgrade there
					{
						data.GetOwner().SwapItem(draggedItem, u, number, draggedUpgradeSlot, targetSlotType);
						Debug.Log("puttint into slot id " + number + " from active? " + draggedUpgradeSlot + " to active? " +
						          (targetSlotType == 1));
					}
				}
			}
			else
			{
				Player player = data.GetOwner() as Player;
				Skill firstSkill = draggedSkill;

				if (swappingAutoattack)
				{
					player.SelectAutoattack(firstSkill);
				}
				else if (toActiveSkillSlot)
				{
					// swapping slots?
					if (player.Skills.HasSkill(firstSkill.GetSkillId()))
					{
						player.SwapSkills(firstSkill, targetSkillName);
						UpdateSkillTimers();
					}
					else // activating/deactivating skills
					{
						if (targetSlotId > 0)
						{
							player.ActivateSkill(firstSkill, targetSlotId);
							UpdateSkillTimers();
						}
						else
						{
							int currentSlot = player.Skills.GetSkillSlot(targetSkillName);

							int slotLevel = player.SkillSlotLevels[currentSlot];

							if (firstSkill.RequiredSlotLevel > slotLevel)
							{
								player.Message("This skill requires slot of level " + firstSkill.RequiredSlotLevel + ".");
								return;
							}

							player.DeactivateSkill(targetSkillName);
							player.ActivateSkill(firstSkill, currentSlot);
							UpdateSkillTimers();
						}
					}
				}
				else
				{
					player.DeactivateSkill(firstSkill.GetName());
					UpdateSkillTimers();
				}

				UpdateClassOverview();
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

			draggedItem = GetUpgradeFromInventory(obj, slotType);

			if (draggedItem == null)
				return;

			draggedUpgradeSlot = slotType;

			int targetSlot = draggedUpgradeSlot == 1 ? 0 : 1;

			if (doubleClick)
			{
				if (draggedItem is ActivableItem)
				{
					((ActivableItem) draggedItem).OnActivate();
					if (((ActivableItem) draggedItem).ConsumeOnUse)
					{
						data.GetOwner().RemoveItem(draggedItem, false);
					}
				}
				else
					data.GetOwner().SwapItem(draggedItem, null, -1, draggedUpgradeSlot, targetSlot);
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
				ShowTrashBin(true, draggedItem.DisposePrice);
			}
		}

		public bool consoleActive = false;
		private Canvas consoleCanvas;

		public void SwitchConsole()
		{
			InputField inputField = consoleCanvas.transform.FindChild("InputField").GetComponent<InputField>();

			if (consoleCanvas.enabled)
			{
				consoleCanvas.enabled = false;
				consoleActive = false;

				inputField.text = "";
			}
			else
			{
				consoleCanvas.enabled = true;
				consoleActive = true;

				EventSystem.current.SetSelectedGameObject(inputField.gameObject, null);
				inputField.OnPointerClick(new PointerEventData(EventSystem.current));

				inputField.text = "";
			}
		}

		public void OnEnter()
		{
			if (consoleCanvas.enabled)
			{
				InputField field = consoleCanvas.transform.FindChild("InputField").GetComponent<InputField>();
				string msg = field.text;

				GameSystem.Instance.AdminCommand(msg);
				SwitchConsole();
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

		private bool stashOpened = false;
		private GameObject stashMainPanel = null;
		private GameObject stashItemsPanel = null;
		private GameObject stashInventoryPanel = null;
		private GameObject stashActiveItemsPanel = null;

		private Text stashStashLabel = null;
		private Text stashInventoryLabel = null;
		private const int STASH_CAPACITY = 50;
		private const int STASHSLOT_INVENTORY = 1;
		private const int STASHSLOT_STASH = 2;
		private const int STASHSLOT_ACTIVE = 3;

		public GameObject stashTrashbin;
		public GameObject stashDisposeValObj;

		private int draggedStashSlotType;

		private GameObject[] stashSlots;
		private GameObject[] stashInventorySlots;
		private GameObject[] stashActiveSlots;

		private void InitStash()
		{
			//GameObject.Find("StashScrollbar").GetComponent<Scrollbar>().value = 1f;
			//GameObject.Find("StashScrollbar2").GetComponent<Scrollbar>().value = 1f;

			stashSlots = new GameObject[STASH_CAPACITY];
			stashInventorySlots = new GameObject[INVENTORY_SIZE];
			stashActiveSlots = new GameObject[ACTIVE_UPGRADES_SIZE];

			stashMainPanel = GameObject.Find("StashMainPanel");
			stashItemsPanel = GameObject.Find("StashItemsPanel");
			stashInventoryPanel = GameObject.Find("StashInventoryPanel");
			stashActiveItemsPanel = GameObject.Find("StashActiveItemsPanel");

			stashStashLabel = GameObject.Find("StashStashLabel").GetComponent<Text>();
			stashInventoryLabel = GameObject.Find("StashInventoryLabel").GetComponent<Text>();

			stashTrashbin = GameObject.Find("StashTrashbin");
			stashDisposeValObj = GameObject.Find("DisposeValue_Stash");

			ShowStashTrashBin(false);

			stashMainPanel.SetActive(false);
			stashOpened = false;

			for (int i = 0; i < STASH_CAPACITY; i++)
			{
				GameObject newIcon = Instantiate(this.iconTemplate);
				newIcon.name = "StashSlot_" + (i + 1);
				newIcon.transform.parent = stashItemsPanel.transform.GetChild(0);
				newIcon.transform.localScale = new Vector3(1, 1, 1);

				GameObject child = newIcon.transform.GetChild(0).gameObject;

				EventTrigger trigger = child.AddComponent<EventTrigger>();

				int tempI = i;

				EventTrigger.Entry entry = new EventTrigger.Entry();
				entry.eventID = EventTriggerType.PointerDown;
				entry.callback.AddListener(delegate { OnStashItemClick(2, tempI, newIcon); });
				trigger.triggers.Add(entry);

				entry = new EventTrigger.Entry();
				entry.eventID = EventTriggerType.PointerEnter;
				entry.callback.AddListener(delegate { OnUpgradeHover(newIcon, false, 0, stashMainPanel, true, true); });
				trigger.triggers.Add(entry);

				entry = new EventTrigger.Entry();
				entry.eventID = EventTriggerType.PointerExit;
				entry.callback.AddListener(delegate { OnUpgradeHover(newIcon, true, 0, stashMainPanel, true, true); });
				trigger.triggers.Add(entry);

				stashSlots[i] = newIcon;
			}

			for (int i = 0; i < INVENTORY_SIZE; i++)
			{
				GameObject newIcon = Instantiate(this.iconTemplate);
				newIcon.name = "InvSlot_" + (i + 1);
				newIcon.transform.parent = stashInventoryPanel.transform.GetChild(0);
				newIcon.transform.localScale = new Vector3(1, 1, 1);

				GameObject child = newIcon.transform.GetChild(0).gameObject;

				EventTrigger trigger = child.AddComponent<EventTrigger>();

				int tempI = i;

				EventTrigger.Entry entry = new EventTrigger.Entry();
				entry.eventID = EventTriggerType.PointerDown;
				entry.callback.AddListener(delegate { OnStashItemClick(1, tempI, newIcon); });
				trigger.triggers.Add(entry);

				entry = new EventTrigger.Entry();
				entry.eventID = EventTriggerType.PointerEnter;
				entry.callback.AddListener(delegate { OnUpgradeHover(newIcon, false, 0, stashMainPanel, true); });
				trigger.triggers.Add(entry);

				entry = new EventTrigger.Entry();
				entry.eventID = EventTriggerType.PointerExit;
				entry.callback.AddListener(delegate { OnUpgradeHover(newIcon, true, 0, stashMainPanel, true); });
				trigger.triggers.Add(entry);

				stashInventorySlots[i] = newIcon;
			}

			GameObject iconTemplate = Resources.Load("Sprite/inventory/ActiveSlot") as GameObject;

			for (int i = 0; i < ACTIVE_UPGRADES_SIZE; i++)
			{
				GameObject newIcon = Instantiate(iconTemplate);
				newIcon.name = "StashActiveSlot_" + (i + 1);
				newIcon.transform.parent = stashActiveItemsPanel.transform;
				newIcon.transform.localScale = new Vector3(1, 1, 1);

				GameObject child = newIcon.transform.GetChild(0).gameObject;

				EventTrigger trigger = child.AddComponent<EventTrigger>();

				int tempI = i;

				EventTrigger.Entry entry = new EventTrigger.Entry();
				entry.eventID = EventTriggerType.PointerDown;
				entry.callback.AddListener(delegate { OnStashItemClick(3, tempI, newIcon); });
				trigger.triggers.Add(entry);

				entry = new EventTrigger.Entry();
				entry.eventID = EventTriggerType.PointerEnter;
				entry.callback.AddListener(delegate { OnUpgradeHover(newIcon, false, 1, stashMainPanel, true); });
				trigger.triggers.Add(entry);

				entry = new EventTrigger.Entry();
				entry.eventID = EventTriggerType.PointerExit;
				entry.callback.AddListener(delegate { OnUpgradeHover(newIcon, true, 1, stashMainPanel, true); });
				trigger.triggers.Add(entry);

				stashActiveSlots[i] = newIcon;
			}

			UpdateStash();
		}

		public void PutToStash(InventoryItem item)
		{
			Player player = data.GetOwner() as Player;

			if (player.ItemStash.CanAdd(item))
			{
				player.Inventory.RemoveItem(item);
				player.ItemStash.AddItem(item);

				UpdateStash();
			}
		}

		public void PutToInventory(InventoryItem item)
		{
			Player player = data.GetOwner() as Player;

			bool equipped = false;

			if (item is EquippableItem && player.Inventory.IsEquipped((EquippableItem) item))
			{
				equipped = true;
				player.UnequipItem(item, true);
			}

			if (!equipped && player.Inventory.CanAdd(item))
			{
				player.ItemStash.RemoveItem(item);
				player.Inventory.AddItem(item);

			}

			UpdateStash();
		}

		public void PutToActiveSlot(InventoryItem item, int targetSlotId)
		{
			Player player = data.GetOwner() as Player;

			if (!(item is EquippableItem))
			{
				player.Message("This item cannot be equipped.");
				return;
			}

/*			InventoryItem itemInSlot = GetUpgradeFromInventory(targetSlotId, 1);
			if (targetSlotId >= 0 && itemInSlot != null)
			{
				player.Inventory.RemoveItem(itemInSlot);

				// moomentalni item je ve stashi - pridat tam prohazovany
				if (player.ItemStash.Items.Contains(item))
				{
					player.ItemStash.AddItem(itemInSlot, true);
					player.ItemStash.RemoveItem(item);
				}
				// moomentalni item je v inventari - pridat tam prohazovany
				else
				{
					player.Inventory.AddItem(itemInSlot, true);
					player.Inventory.RemoveItem(item);
				}

				player.Inventory.ForceEquipUpgrade((EquippableItem) item);
			}
			else
			{*/
				if (player.ItemStash.Items.Contains(item))
				{
					player.ItemStash.RemoveItem(item);
				}

			if (!player.Inventory.ForceEquipUpgrade((EquippableItem) item))
			{
				player.Message("All active slots are occupied.");
			}
			//}

			UpdateStash();
		}

		// 1=inventory, 2=stash, 3=activeitems
		public void OnStashItemClick(int slotType, int slotId, GameObject slotObject)
		{
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

			Player player = data.GetOwner() as Player;

			if (slotType == STASHSLOT_INVENTORY)
			{
				draggedItem = GetUpgradeFromInventory(slotId, 0);
				draggedStashSlotType = STASHSLOT_INVENTORY;
			}
			else if(slotType == STASHSLOT_STASH)
			{
				draggedItem = player.ItemStash.GetItem(slotId);
				draggedStashSlotType = STASHSLOT_STASH;
			}
			else if (slotType == STASHSLOT_ACTIVE)
			{
				draggedItem = GetUpgradeFromInventory(slotId, 1);
				draggedStashSlotType = STASHSLOT_ACTIVE;
			}

			if (draggedItem == null)
				return;

			//draggedUpgradeSlot = slotType;

			if (doubleClick)
			{
				if (slotType == STASHSLOT_INVENTORY)
				{
					PutToStash(draggedItem);
				}
				else if (slotType == STASHSLOT_STASH)
				{
					PutToInventory(draggedItem);
				}
				else if (slotType == STASHSLOT_ACTIVE)
				{
					PutToStash(draggedItem);
				}
			}
			else
			{
				draggingIcon = true;
				SetMouseOverUi();

				Vector3 mousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
				mousePos.z = 0;

				GameObject preview = new GameObject("Dragged Icon");
				Image ren = preview.AddComponent<Image>();
				ren.sprite = slotObject.transform.GetChild(0).GetComponent<Image>().sprite;
				preview.transform.parent = stashMainPanel.transform;
				//preview.transform.position = mousePos;
				preview.GetComponent<RectTransform>().sizeDelta = new Vector2(80, 80);

				draggedObject = preview;
				ShowStashTrashBin(true, draggedItem.DisposePrice);
			}
		}

		public void ShowStashTrashBin(bool state, int price = 0)
		{
			if (state)
			{
				stashTrashbin.SetActive(true);
				stashDisposeValObj.GetComponent<Text>().text = "You will gain: " + price + " DNA";
				stashDisposeValObj.SetActive(true);
			}
			else
			{
				stashTrashbin.SetActive(false);
				stashDisposeValObj.SetActive(false);
			}
		}

		public void SwitchStash()
		{
			if (!stashOpened)
			{
				stashOpened = true;
				stashMainPanel.SetActive(true);

				UpdateStash();
			}
			else
			{
				stashOpened = false;
				stashMainPanel.SetActive(false);

				UpdateInventory(data.GetOwner().Inventory);
			}
		}

		public void UpdateStash()
		{
			Player player = data.GetOwner() as Player;
			Inventory inv = player.Inventory;

			int activeCapacity = inv.ActiveCapacity;
			int capacity = inv.Capacity;
			int stashCapacity = STASH_CAPACITY;

			// inventory
			int i = 0;
			foreach (InventoryItem u in inv.Items)
			{
				if (u == null || (u.IsUpgrade() && inv.IsEquipped((EquippableItem)u)))
					continue;

				GameObject o = stashInventorySlots[i].transform.GetChild(0).gameObject;
				Image img = o.GetComponent<Image>();
				img.sprite = u.MainSprite;

				i++;
			}

			for (int j = i; j < stashInventorySlots.Length; j++)
			{
				GameObject o = stashInventorySlots[j].transform.GetChild(0).gameObject;
				Image img = o.GetComponent<Image>();

				if (j < capacity)
					img.sprite = iconEmptySprite;
				else
					img.sprite = lockedIconSprite;
			}

			// stash
			i = 0;
			foreach (InventoryItem u in player.ItemStash.Items)
			{
				if (u == null)
					continue;

				GameObject o = stashSlots[i].transform.GetChild(0).gameObject;
				Image img = o.GetComponent<Image>();
				img.sprite = u.MainSprite;

				i++;
			}

			for (int j = i; j < stashSlots.Length; j++)
			{
				GameObject o = stashSlots[j].transform.GetChild(0).gameObject;
				Image img = o.GetComponent<Image>();

				if (j < stashCapacity)
					img.sprite = iconEmptySprite;
				else
					img.sprite = lockedIconSprite;
			}
			// active items
			i = 0;
			foreach (EquippableItem u in inv.ActiveUpgrades)
			{
				GameObject o = stashActiveSlots[i].transform.GetChild(0).gameObject;
				Image img = o.GetComponent<Image>();
				img.sprite = u.MainSprite;

				i++;
			}

			for (int j = i; j < stashActiveSlots.Length; j++)
			{
				GameObject o = stashActiveSlots[j].transform.GetChild(0).gameObject;
				Image img = o.GetComponent<Image>();

				if (j < activeCapacity)
					img.sprite = iconEmptySprite;
				else
					img.sprite = lockedIconSprite;
			}

			stashStashLabel.text = "Stash (" + player.ItemStash.Items.Count + " / " + stashCapacity + ")";
			stashInventoryLabel.text = "Player Inventory (" + inv.Items.Count + " / " + capacity + ")";

			UpdateStatsInfo();
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
					case "XP":
						t.text = data.GetOwner().Status.XP + " / " + GameProgressTable.GetXpForLevel(data.level + 1) + " XP";
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
					case "DNA": //TODO add to inventory
						t.text = ((Player) data.GetOwner()).DnaPoints + "p";
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
			foreach (InventoryItem u in inv.Items)
			{
				if (u == null || (u.IsUpgrade() && inv.IsEquipped((EquippableItem) u)))
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
			foreach (EquippableItem u in inv.ActiveUpgrades)
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
			foreach (EquippableItem u in inv.BasestatUpgrades)
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
			/*MonsterId mId = MonsterId.TestMonster;

			switch (Random.Range(1, 2))
			{
				case 1:
					mId = MonsterId.Leukocyte_melee;
					break;
				case 2:
					mId = MonsterId.Leukocyte_ranged;
					break;
			}

			GameSystem.Instance.SpawnMonster(mId, data.GetBody().transform.position + new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), 0), false, 1);*/
		}

		public void TestSpawnMonsters2()
		{
			/*MonsterId mId = MonsterId.TestMonster;
			GameSystem.Instance.SpawnMonster(mId, data.GetBody().transform.position + new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), 0), false, 1);*/
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
				Monster m = GameSystem.Instance.SpawnMonster(adminSpawnPanel.captionText.text, position, false, 1);
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
			//public MonsterId id;
			public string monsterTypeName;
			public Vector3 pos;

			public SpawnData(string monsterTypeName, Vector3 pos)
			{
				this.monsterTypeName = monsterTypeName;
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
						SpawnData data = new SpawnData((o.GetChar() as Monster).Template.GetMonsterTypeName(), o.GetData().GetBody().transform.position);
						adminSpawnedData.Add(data);
					}
				}
			}
		}

		public void RespawnSaved()
		{
			foreach (SpawnData data in adminSpawnedData)
			{
				Monster m = GameSystem.Instance.SpawnMonster(data.monsterTypeName, data.pos, false, 1);
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

		private ShopData activeShopData;
		private Canvas showViewCanvas;
		private GameObject shopContent;
		private GameObject shopItemTemplate;
		private GameObject shopStatsViewPanel;

		public void ShowShopView(ShopData shopData)
		{
			if (showViewCanvas.enabled)
			{
				HideShopView();
				activeShopData = null;
				return;
			}

			showViewCanvas.enabled = true;

			activeShopData = shopData;

			Text txt;

			foreach (Transform child in shopStatsViewPanel.transform)
			{
				switch (child.name)
				{
					case "Money":
						txt = child.GetComponent<Text>();
						txt.text = ((Player) data.GetOwner()).DnaPoints + " DNA";
						break;
					case "Slots":
						txt = child.GetComponent<Text>();
						int taken = ((Player) data.GetOwner()).Inventory.Items.Count;
						txt.text = "Inventory slots: " + taken + "/" + ((Player) data.GetOwner()).Inventory.Capacity + " slots";
						break;
				}
			}

			foreach (ShopItem item in shopData.Items)
			{
				GameObject itemObject = Instantiate(shopItemTemplate, new Vector3(0, 0), Quaternion.identity) as GameObject;
				itemObject.transform.parent = shopContent.transform;
				itemObject.transform.localPosition = new Vector3(0, 0);
				itemObject.transform.localScale = new Vector3(1, 1);

				foreach (Transform child in itemObject.transform)
				{
					switch (child.name)
					{
						case "ShopItemFrame":
							Image img = child.GetChild(0).GetComponent<Image>();
							img.sprite = item.item.MainSprite;
							break;
						case "ShopItemName":
							txt = child.GetComponent<Text>();
							txt.text = item.item.VisibleName;
							break;
						case "ShopItemDesc":
							txt = child.GetComponent<Text>();
							txt.text = item.item.Description;
							break;
						case "ShopItemPrice":
							txt = child.GetComponent<Text>();

							txt.text = item.price + " DNA";
							child.GetChild(0).GetComponent<Text>().text = txt.text;
							break;
					}
				}

				EventTrigger trigger = itemObject.AddComponent<EventTrigger>();

				EventTrigger.Entry entry = new EventTrigger.Entry();
				entry.eventID = EventTriggerType.PointerClick;

				var item1 = item;
				entry.callback.AddListener(delegate { OnClickShopItem(itemObject, item1); });
				trigger.triggers.Add(entry);
			}
		}

		private GameObject dialogConfirmObject;
		private GameObject dialogConfirmPanel;

		private GameObject activePurchasingItemObject;
		private ShopItem activePurchasingItem;

		public void OnClickShopItem(GameObject obj, ShopItem item)
		{
			string text = "Are you sure you want to purchase " + item.item.VisibleName + "?";

			if (dialogConfirmObject == null)
			{
				Debug.LogError("helpcanvaspanel is null, cant show help");
				return;
			}

			activePurchasingItem = item;
			activePurchasingItemObject = obj;

			GameObject helpContent = dialogConfirmPanel.transform.FindChild("ConfirmDialogContent").gameObject;
			foreach (Transform t in helpContent.transform)
			{
				if (t.name.Equals("ConfirmDialogText"))
				{
					t.gameObject.GetComponent<Text>().text = text;
				}
			}

			dialogConfirmObject.GetComponent<Canvas>().enabled = true;
		}

		public void DoConfirm(bool value)
		{
			dialogConfirmObject.GetComponent<Canvas>().enabled = false;

			if (value && activePurchasingItem != null)
			{
				DoPurchase();
			}
		}

		private void DoPurchase()
		{
			Player p = data.player;

			if (data.player.DnaPoints >= activePurchasingItem.price)
			{
				data.player.ReduceDnaPoints(activePurchasingItem.price);

				activeShopData.DoPurchase(activePurchasingItem);

				HideShopView();
				ShowShopView(activeShopData);
			}
			else
			{
				data.player.Message("You dont have enought DNA.");
			}

			activePurchasingItem = null;
			activePurchasingItemObject = null;
		}

		public void HideShopView()
		{
			foreach (Transform t in shopContent.transform)
			{
				Destroy(t.gameObject);
			}

			showViewCanvas.enabled = false;
		}
	}
}
