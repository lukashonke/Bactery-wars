using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.Mono.MapGenerator;
using Assets.scripts.Mono.ObjectData;
using Assets.scripts.Skills;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Assets.scripts.Mono
{
	public sealed class PlayerUI : MonoBehaviour
	{
		private bool mouseOverUi = false;
		public bool MouseOverUI
		{
			get { return mouseOverUi; }
		}

		public GameObject[] skillButtons;

        public bool adminMode;

		public Text hp;

		private PlayerData data;

		public GameObject gameMenu = null;
		public GameObject menuPanel = null;
		public GameObject settingsPanel = null;

	    private List<SpawnData> adminSpawnedData;
        private static MonsterId[] adminSpawnableList = { MonsterId.Neutrophyle_Patrol, MonsterId.Lymfocyte_melee, MonsterId.TurretCell, MonsterId.FloatingHelperCell, MonsterId.ArmoredCell, MonsterId.DementCell, MonsterId.FourDiagShooterCell, MonsterId.JumpCell, MonsterId.SuiciderCell, MonsterId.TurretCell, MonsterId.TankCell, MonsterId.Lymfocyte_ranged, MonsterId.SpiderCell, MonsterId.TestBoss, MonsterId.HelperCell, MonsterId.PassiveHelperCell, MonsterId.ObstacleCell,  };
	    public GameObject adminPanel;
	    public Dropdown adminSpawnPanel;

		private float[,] timers;

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
			}
			else
			{
				gameMenu = GameObject.Find("GameMenu");
				settingsPanel = GameObject.Find("SettingsMenu");
			}

			gameMenu.GetComponent<Canvas>().enabled = true;
			settingsPanel.GetComponent<Canvas>().enabled = true;

			skillButtons = new GameObject[9];
			for (int i = 1; i <= 9; i++)
			{
				foreach (Transform child in gameMenu.transform)
				{
					if (child.name.Equals("Skill" + i))
					{
						skillButtons[i - 1] = child.gameObject;
					}
				}
			}

			timers = new float[9,9];

			if (settingsPanel != null)
				settingsPanel.SetActive(false);

			if(menuPanel != null)
				menuPanel.SetActive(false);

            // admin setting
		    adminPanel = GameObject.Find("AdminPanel");
		    adminSpawnPanel = GameObject.Find("AdminMode").GetComponent<Dropdown>();

            List<String> temp = new List<string>();
		    foreach (MonsterId id in adminSpawnableList)
		        temp.Add(Enum.GetName(typeof (MonsterId), id));
	        adminSpawnPanel.AddOptions(temp);

            UpdateAdminControls();
		}

		// Update is called once per frame
		void Update()
		{
			for (int i = 0; i < timers.GetLength(0); i++)
			{
				if (timers[i,0] > 0)
				{
					float max = timers[i, 1];
					float passed = Time.time - timers[i,0];
					float ratio = passed / max;

					if (ratio >= 1)
					{
						ratio = 1;
						timers[i, 0] = 0;
					}

					ratio = (ratio);

					Image but = skillButtons[i-1].GetComponent<Image>();
					but.color = new Color(ratio, ratio, ratio);
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
					id = i+1;
					break;
				}
			}

			if (id == -1)
				return;

			timers[id,0] = Time.time;
			timers[id,1] = ((ActiveSkill)sk).GetReuse();
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

			if ((int) Time.time%1 == 0)
			{
				StringBuilder sb = new StringBuilder();

				sb.AppendLine("Map Type " + Enum.GetName(typeof (MapType), WorldHolder.instance.activeMap.MapType));
				Vector3 pos = data.GetBody().transform.position;

				int left = WorldHolder.instance.activeMap.GetMonstersLeft(WorldHolder.instance.activeMap.GetRegionFromWorldPosition(pos).GetParentOrSelf());
				sb.AppendLine("Room monsters left " + left);

				GameObject.Find("AdminMapInfo").GetComponent<Text>().text = sb.ToString();
			}
		}

		public void MenuClick()
		{
			if(menuPanel.activeSelf)
				menuPanel.SetActive(false);
			else
				menuPanel.SetActive(true);
		}

		public void OpenSettings()
		{
			if (settingsPanel.activeSelf)
			{
				mouseOverUi = false;
				GameSystem.Instance.Paused = false;
				settingsPanel.SetActive(false);
			}
			else
			{
				mouseOverUi = true;
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
	        foreach(GameObject o in GameObject.FindObjectsOfType<GameObject>())
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
