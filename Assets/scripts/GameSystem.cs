using System;
using System.Collections;
using System.Collections.Generic;
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
	/// Singleton trida
	/// Pridava hrace do hry
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

		public GameController Controller { get; private set; }

		private bool paused;
		public bool Paused
		{
			get { return paused; }
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

		public bool disableWallNodes = true;

		// starts the game, loads data, etc
		public void Start(GameController gc)
		{
			Controller = gc;
			UpgradeTable.Instance.ToString();
		}

		public void Update()
		{
			
		}

		public void BroadcastMessage(string msg, int level = 1)
		{
			if(CurrentPlayer != null)
				CurrentPlayer.Message(msg, level);
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

		public void UpdatePathfinding(Vector3 center, int tilesPerRegionX, int tilesPerRegionY, int mapWidth, int mapHeight)
		{
			Debug.Log(mapWidth + ", " + mapHeight);
			Debug.Log(center);
			AstarPath ap = Controller.GetComponent<AstarPath>();

			foreach (IUpdatableGraph g in AstarPath.active.astarData.GetUpdateableGraphs())
			{
				if (g is GridGraph)
				{
					GridGraph gridGraph = g as GridGraph;

					float tileSize = gridGraph.nodeSize;

					int nodesX = (int)(10 + mapWidth * (tileSize * tilesPerRegionX)) * 2;
					int nodesY = (int)(10 + mapHeight * (tileSize * tilesPerRegionY)) * 2;

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
		}

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

		public Monster RegisterNewMonster(EnemyData data, String name, int id, int level, Dictionary<string, string> parameters)
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
		}

		//TODO make use of parameters dictionary to pass level, etc
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

		public Npc SpawnNpc(MonsterId id, Vector3 position)
		{
			GameObject go = Resources.Load("Prefabs/entity/" + id.ToString() + "/" + id.ToString()) as GameObject;
			if (go == null)
				throw new NullReferenceException("Prefabs/entity/" + id.ToString() + "/" + id);

			GameObject result = Object.Instantiate(go, position, Quaternion.identity) as GameObject;
			EnemyData data = result.GetComponent<EnemyData>();

			return RegisterNewNpc(data, id);
		}

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
	}
}