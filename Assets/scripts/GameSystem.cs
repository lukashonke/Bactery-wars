// Copyright (c) 2015, Lukas Honke
// ========================
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Assets.scripts.Actor;
using Assets.scripts.Actor.MonsterClasses;
using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.Actor.PlayerClasses;
using Assets.scripts.Actor.PlayerClasses.Base;
using Assets.scripts.Mono;
using Assets.scripts.Mono.MapGenerator;
using Assets.scripts.Mono.ObjectData;
using Assets.scripts.Skills;
using Assets.scripts.Upgrade;
using Pathfinding;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.scripts
{
	/// <summary>
	/// Poskytuje operace nad celkovou hrou - umistovani monster, ziskani objektu hrace, vypsani zpravy, pauznuti hry, atd.
	/// </summary>
	public class GameSystem
	{
		// singleton 
		private static GameSystem instance;
		public static GameSystem Instance
		{
			get
			{
				if(instance == null)
					instance = new GameSystem();
				return instance;
			}
		}

		public Player CurrentPlayer { get; set; }
		public int Language { get; set; }

		public GameController Controller { get; private set; }

		private bool paused;
		public bool Paused
		{
			get
			{
				return paused;
			}
			set
			{
				paused = value;
				if (paused)
				{
					Time.timeScale = 0;
				}
				else
				{
					Time.timeScale = 1;
				}
			}
		}

		private int lastPlayerId = 0;

		public bool disableWallNodes = false;

		// starts the game, loads data, etc
		public void Start(GameController gc)
		{
			Controller = gc;
			UpgradeTable.Instance.ToString();
		}

		public void Update()
		{
			
		}

		/// <summary>
		/// Vypise zpravu kterou hrac uvidi
		/// </summary>
		/// <param name="msg"></param>
		/// <param name="level"></param>
		public void BroadcastMessage(string msg, int level = 1)
		{
			if(CurrentPlayer != null)
				CurrentPlayer.Message(msg, level);
		}

		/// <summary>
		/// Zpracuje prikaz z konzole
		/// </summary>
		/// <param name="msg"></param>
		public void AdminCommand(string msg)
		{
			if (msg.ToLower().StartsWith("help"))
			{
				BroadcastMessage("group [group-ID]   - spawne grupu mobu z SpawnData.xml (automaticky zvoli groupy z kategorie main_room)");
				//BroadcastMessage("spawn [type] [group-ID]   - spawne grupu mobu z SpawnData.xml (za type se pise \"main\" \"side\" \"bonus\" \"boss\" \"end\" \"start\" a to da kategorii ze ktery se groupa bere)");
			}

			if (msg.ToLower().StartsWith("heal"))
			{
				CurrentPlayer.HealMe();
			}

			if (msg.ToLower().StartsWith("speed "))
			{
				string[] args = msg.Split(' ');
				int val = -1;

				try
				{
					val = Int32.Parse(args[1]);
				}
				catch (Exception)
				{
					BroadcastMessage("spatne parametry - zadejte cislo");
					return;
				}

				CurrentPlayer.SetMoveSpeed(val);
			}

			if (msg.ToLower().StartsWith("maxhp "))
			{
				string[] args = msg.Split(' ');
				int val = -1;

				try
				{
					val = Int32.Parse(args[1]);
				}
				catch (Exception)
				{
					BroadcastMessage("spatne parametry - zadejte cislo");
					return;
				}

				CurrentPlayer.UpdateMaxHp(val);
				CurrentPlayer.HealMe();
			}

			if (msg.ToLower().Equals("invis"))
			{
				CurrentPlayer.UpdateMaxHp(99999);
				CurrentPlayer.HealMe();
			}

			if (msg.ToLower().StartsWith("spawn "))
			{
				MonsterTemplateTable.Instance.Load();

				string[] args = msg.Split(' ');
				string type = null;
				int team = 0;

				try
				{
					type = args[1];
				}
				catch (Exception)
				{
					BroadcastMessage("spatne parametry - treba zadat jmeno monstera, ve jmenu nesmi byt mezera");
					return;
				}

				try
				{
					team = Int32.Parse(args[2]);
				}
				catch (Exception)
				{
				}

				Tile t = WorldHolder.instance.activeMap.GetTileFromWorldPosition(CurrentPlayer.GetData().GetBody().transform.position);

				if (t == null)
					return;

				Monster m = GameSystem.Instance.SpawnMonster(type, Utils.GenerateRandomPositionAround(CurrentPlayer.GetData().GetBody().transform.position, 7, 4), false, 1, team);
				WorldHolder.instance.activeMap.RegisterMonsterToMap(m);
			}

			// spawn GROUP_ID || spawn TYPE GROUP_ID
			if (msg.ToLower().StartsWith("spawngroup ") || msg.ToLower().StartsWith("group "))
			{
				MonsterGenerator.Instance.LoadXmlFile();

				string[] args = msg.ToLower().Split(' ');
				int id;
				string type;

				try
				{
					id = Int32.Parse(args[1]);
					type = "main";
				}
				catch (Exception)
				{
					type = args[1];
					id = Int32.Parse(args[2]);
				}
				

				MonsterGenerator.RoomType currentRoomType = MonsterGenerator.RoomType.MAIN_ROOM;

				switch (type)
				{
					case "main":
						currentRoomType = MonsterGenerator.RoomType.MAIN_ROOM;
						break;
					case "side":
						currentRoomType = MonsterGenerator.RoomType.SIDE_ROOM;
						break;
					case "bonus":
						currentRoomType = MonsterGenerator.RoomType.BONUS_ROOM;
						break;
					case "boss":
						currentRoomType = MonsterGenerator.RoomType.BOSS_ROOM;
						break;
					case "end":
						currentRoomType = MonsterGenerator.RoomType.END_ROOM;
						break;
					case "start":
						currentRoomType = MonsterGenerator.RoomType.START_ROOM;
						break;
				}

				Tile t = WorldHolder.instance.activeMap.GetTileFromWorldPosition(CurrentPlayer.GetData().GetBody().transform.position);

				if (t == null)
					return;

				MapRoom room = t.region.GetParentOrSelf().GetMapRoom();

				MonsterGenerator.Instance.GenerateGenericEnemyGroup(room, WorldHolder.instance.activeMap.levelData, currentRoomType, 2, id);

				room.Unexclude();
				BroadcastMessage("Spawned group ID " + id + " (roomtype " + currentRoomType + ")");
			}
		}

		public Coroutine StartTask(IEnumerator task)
		{
			return Controller.StartCoroutine(task);
		}

		public void StopTask(Coroutine c)
		{
			Controller.StopCoroutine(c);
		}

		public void StopTask(IEnumerator t)
		{
			Controller.StopCoroutine(t);
		}

		private static Vector3 RoundVector3(Vector3 v)
		{
			if (Mathf.Abs(v.x - Mathf.Round(v.x)) < 0.001f) v.x = Mathf.Round(v.x);
			if (Mathf.Abs(v.y - Mathf.Round(v.y)) < 0.001f) v.y = Mathf.Round(v.y);
			if (Mathf.Abs(v.z - Mathf.Round(v.z)) < 0.001f) v.z = Mathf.Round(v.z);
			return v;
		}

		public bool detailedPathfinding = false;
		public bool pathfindingError = false;

		public void PathfindingError()
		{
			pathfindingError = true;
		}

		/// <summary>
		/// Nastavi graf pro pathfinding tak aby pokryval celou vygenerovanou mapu
		/// </summary>
		public void UpdatePathfinding(Vector3 center, int tilesPerRegionX, int tilesPerRegionY, int mapWidth, int mapHeight)
		{
			AstarPath ap = Controller.GetComponent<AstarPath>();

			foreach (IUpdatableGraph g in AstarPath.active.astarData.GetUpdateableGraphs())
			{
				if (g is GridGraph)
				{
					GridGraph gridGraph = g as GridGraph;

					if (detailedPathfinding)
					{
						gridGraph.erosionUseTags = true;
						gridGraph.erosionFirstTag = 1;
						gridGraph.erodeIterations = 2;
						gridGraph.nodeSize = 0.5f;
					}
					else
					{
						gridGraph.erosionUseTags = true;
						gridGraph.erosionFirstTag = 1;
						gridGraph.erodeIterations = 1;
						gridGraph.nodeSize = 0.75f;
					}

					float tileSize = gridGraph.nodeSize;
					int nodesX;
					int nodesY;

					if (detailedPathfinding)
					{
						nodesX = (int)(10 + mapWidth * (1 - tileSize + 1 * tilesPerRegionX)) * 2;
						nodesY = (int)(10 + mapHeight * (1 - tileSize + 1 * tilesPerRegionY)) * 2;
					}
					else
					{
						nodesX = (int)(10 + mapWidth * (tileSize * tilesPerRegionX)) * 2;
						nodesY = (int)(10 + mapHeight * (tileSize * tilesPerRegionY)) * 2;
					}
					

					gridGraph.Width = nodesX;
					gridGraph.Depth = nodesY;

					gridGraph.center = center + new Vector3(nodesX/2f*tileSize - 3, nodesY / 2f * tileSize - 3, 0);

					//gridGraph.center = new Vector3(51, 51, 0);
					gridGraph.UpdateSizeFromWidthDepth();
				}
			}

			ap.Scan();

			if (disableWallNodes)
			{
				foreach (IUpdatableGraph g in AstarPath.active.astarData.GetUpdateableGraphs())
				{
					if (g is GridGraph)
					{
						GridGraph gridGraph = g as GridGraph;

						foreach (GraphNode n in gridGraph.nodes)
						{
							if (n.Area == 1)
							{
								n.Tag = 2;
								n.Walkable = false;
							}
						}
					}
				}
			}

			Controller.WaitForPathfindingError();
		}

		private void CheckError()
		{
			
		}

		/// <summary>
		/// Registruje hrace do hry
		/// </summary>
		public Player RegisterNewPlayer(PlayerData data, String name)
		{
			ClassId cId = (ClassId) Enum.Parse(typeof (ClassId), GameSession.className);

			Player player = new Player(name + (++lastPlayerId), data, ClassTemplateTable.Instance.GetType(cId));

			player.Init();

			//TODO improve this
			if (data.tag == "Team1")
			{
				player.Team = 1;
			}
			else if (data.tag == "Team2")
			{
				player.Team = 2;
			}
			else if (data.tag == "Team3")
			{
				player.Team = 3;
			}
			else if (data.tag == "Team4")
			{
				player.Team = 4;
			}

			Debug.Log("Player " + player.Name + " of team " + player.Team + " has template " + player.Template.GetClassId());

			player.InitTemplate();
			player.SetLevel(1);

			player.GetData().GetBody().transform.position = WorldHolder.instance.GetStartPosition();

			CurrentPlayer = player;

			//TODO init, save, etc

			return player;
		}

		/*public Monster RegisterNewMonster(EnemyData data, String name, int id, int level, Dictionary<string, string> parameters)
		{
			Monster monster;

			MonsterId mId = (MonsterId) Enum.Parse(typeof (MonsterId), ""+id);

			MonsterTemplate mt = MonsterTemplateTable.Instance.GetType(mId);

			if (mt is BossTemplate)
			{
				monster = new Boss(name, data, (BossTemplate) mt);
			}
			else
			{
				monster = new Monster(name, data, mt);
			}
			
			data.SetOwner(monster);
			monster.SetLevel(level);

			monster.Init();
			monster.InitTemplate();

			return monster;
		}*/

		//TODO make use of parameters dictionary to pass level, etc
		/// <summary>
		/// Registruje monstrum do hry podle MonsterID
		/// </summary>
		private Monster RegisterNewMonster(EnemyData data, MonsterId id, bool isMinion, int level, Dictionary<string, string> parameters=null)
		{
			Monster monster;
			MonsterTemplate mt = MonsterTemplateTable.Instance.GetType(id);

			if (mt is BossTemplate)
			{
				monster = new Boss(id.ToString(), data, (BossTemplate) mt);
			}
			else
			{
				monster = new Monster(id.ToString(), data, mt);	
			}
			
			data.SetOwner(monster);
			monster.SetLevel(level);
			monster.Init();

			monster.isMinion = isMinion;
			monster.InitTemplate();

			return monster;
		}

		/// <summary>
		/// Registruje monstrum do hry, pokud je jiz znama MonsterTemplate
		/// </summary>
		public Monster RegisterNewCustomMonster(EnemyData data, MonsterTemplate mt, bool isMinion, int level, Dictionary<string, string> parameters = null)
		{
			Monster monster;

			if (mt is BossTemplate)
			{
				monster = new Boss(mt.Name, data, (BossTemplate)mt);
			}
			else
			{
				monster = new Monster(mt.Name, data, mt);
			}

			data.SetOwner(monster);
			monster.SetLevel(level);
			monster.Init();

			monster.isMinion = isMinion;
			monster.InitTemplate();

			return monster;
		}

		/// <summary>
		/// Registruje monstrum do hry pokud je zname jmeno typu monstra
		/// </summary>
		public Monster RegisterNewCustomMonster(EnemyData data, string monsterTypeName, bool isMinion, int level, Dictionary<string, string> parameters = null)
		{
			Monster monster;
			MonsterTemplate mt = MonsterTemplateTable.Instance.GetType(monsterTypeName);

			if (mt is BossTemplate)
			{
				monster = new Boss(mt.Name, data, (BossTemplate)mt);
			}
			else
			{
				monster = new Monster(mt.Name, data, mt);
			}

			data.SetOwner(monster);
			monster.SetLevel(level);
			monster.Init();

			monster.isMinion = isMinion;
			monster.InitTemplate();

			return monster;
		}

		/// <summary>
		/// Registruje NPC do hry
		/// </summary>
		public Npc RegisterNewNpc(EnemyData data, MonsterId id)
		{
			Npc npc;
			npc = new Npc(id.ToString(), data, MonsterTemplateTable.Instance.GetType(id));
			data.SetOwner(npc);
			npc.Init();

			npc.isMinion = false;
			npc.InitTemplate();

			return npc;
		}

		public Npc SpawnNpc(string npcTypeName, Vector3 position)
		{
			MonsterId id = (MonsterId) Enum.Parse(typeof (MonsterId), npcTypeName);

			return SpawnNpc(id, position);
		}

		private Npc SpawnNpc(MonsterId id, Vector3 position)
		{
			GameObject go = Resources.Load("Prefabs/entity/" + id.ToString() + "/" + id.ToString()) as GameObject;
			if (go == null)
				throw new NullReferenceException("Prefabs/entity/" + id.ToString() + "/" + id);

			GameObject result = Object.Instantiate(go, position, Quaternion.identity) as GameObject;
			EnemyData data = result.GetComponent<EnemyData>();

			return RegisterNewNpc(data, id);
		}

		[Obsolete]
		public Monster SpawnMonster(MonsterId id, Vector3 position, bool isMinion, int level, int team=0)
		{
			GameObject go = Resources.Load("Prefabs/entity/" + id.ToString() + "/" + id.ToString()) as GameObject;
			if (go == null)
				throw new NullReferenceException("Prefabs/entity/" + id.ToString() + "/" + id);

			GameObject result = Object.Instantiate(go, position, Quaternion.identity) as GameObject;
			EnemyData data = result.GetComponent<EnemyData>();

			Monster m = RegisterNewMonster(data, id, isMinion, level);
			if (team > 0)
				m.Team = team;

			return m;
		}

		/// <summary>
		/// Spawn monster do hry dle jmena jeho typu
		/// </summary>
		public Monster SpawnMonster(string monsterTypeName, Vector3 position, bool isMinion, int level, int team = 0)
		{
			MonsterTemplate template = MonsterTemplateTable.Instance.GetType(monsterTypeName);

			if (template == null)
				throw new NullReferenceException("cant find monstertemplate for " + monsterTypeName);

			string name = template.GetFolderName();

			GameObject go = Resources.Load("Prefabs/entity/" + name + "/" + name) as GameObject;

			if (go == null)
				throw new NullReferenceException("Prefabs/entity/" + name + "/" + name);

			GameObject result = Object.Instantiate(go, position, Quaternion.identity) as GameObject;
			EnemyData data = result.GetComponent<EnemyData>();

			Monster m = RegisterNewCustomMonster(data, template, isMinion, level);
			if (team > 0)
				m.Team = team;

			return m;
		}

		/// <summary>
		/// Spawne bosse do hry dle jmena jeho typu
		/// </summary>
		public Boss SpawnBoss(string monsterTypeName, Vector3 position, int level)
		{
			MonsterTemplate template = MonsterTemplateTable.Instance.GetType(monsterTypeName);

			if (template == null)
				throw new NullReferenceException("cant find monstertemplate for " + monsterTypeName);

			string name;
			if (template.GetMonsterId() == MonsterId.CustomMonster)
				name = ((CustomMonsterTemplate)template).GetOldTemplateFolderId();
			else
				name = template.GetMonsterId().ToString();

			GameObject go = Resources.Load("Prefabs/entity/" + name + "/" + name) as GameObject;
			if (go == null)
				throw new NullReferenceException("Prefabs/entity/" + name + "/" + name);

			GameObject result = Object.Instantiate(go, position, Quaternion.identity) as GameObject;
			EnemyData data = result.GetComponent<EnemyData>();

			//Monster m = RegisterNewMonster(data, id, false, level);
			Monster m = RegisterNewCustomMonster(data, template, false, level);
			if (m is Boss)
			{
				return (Boss)m;
			}
			else
			{
				Debug.LogError("tried to spawn boss - but it was a monster ! fix it in data");
				throw new NullReferenceException();
			}
		}

		public Boss SpawnBoss(MonsterId id, Vector3 position, int level)
		{
			GameObject go = Resources.Load("Prefabs/entity/" + id.ToString() + "/" + id.ToString()) as GameObject;
			if (go == null)
				throw new NullReferenceException("Prefabs/entity/" + id.ToString() + "/" + id);

			GameObject result = Object.Instantiate(go, position, Quaternion.identity) as GameObject;
			EnemyData data = result.GetComponent<EnemyData>();

			Monster m = RegisterNewMonster(data, id, false, level);
			if (m is Boss)
			{
				return (Boss) m;
			}
			else
			{
				Debug.LogError("tried to spawn boss - but it was a monster ! fix it in data");
				throw new NullReferenceException();
			}
		}

		public GameObject Instantiate(GameObject template, Vector3 position)
		{
			return Controller.Instantiate(template, position);
		}
	}
}