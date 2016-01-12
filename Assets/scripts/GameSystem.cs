﻿using System;
using System.Collections;
using Assets.scripts.Actor;
using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.Actor.PlayerClasses;
using Assets.scripts.Actor.PlayerClasses.Base;
using Assets.scripts.Mono;
using Assets.scripts.Mono.MapGenerator;
using Assets.scripts.Mono.ObjectData;
using Assets.scripts.Skills;
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


		// starts the game, loads data, etc
		public void Start(GameController gc)
		{
			Controller = gc;

            Debug.Log("gamesystem initnut");
		}

		// Handles real-time updates
		// called every frame
		public void Update()
		{
			
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

		public void UpdatePathfinding()
		{
			AstarPath ap = Controller.GetComponent<AstarPath>();

			ap.Scan();
		}

		// Methods
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

			player.GetData().GetBody().transform.position = WorldHolder.instance.GetStartPosition();

			//TODO init, save, etc

			return player;
		}

		public Monster RegisterNewMonster(EnemyData data, String name, int id)
		{
			Monster monster;

			MonsterId mId = (MonsterId) Enum.Parse(typeof (MonsterId), ""+id);
			monster = new Monster(name, data, MonsterTemplateTable.Instance.GetType(mId));
			data.SetOwner(monster);
			monster.Init();

			monster.InitTemplate();

			return monster;
		}

		public Monster RegisterNewMonster(EnemyData data, MonsterId id, bool isMinion)
		{
			Monster monster;
			monster = new Monster(id.ToString(), data, MonsterTemplateTable.Instance.GetType(id));
			data.SetOwner(monster);
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

		public Monster SpawnMonster(MonsterId id, Vector3 position, bool isMinion, int team)
		{
			Monster m = SpawnMonster(id, position, isMinion);
			m.Team = team;
			return m;
		}

		public Monster SpawnMonster(MonsterId id, Vector3 position, bool isMinion)
		{
			GameObject go = Resources.Load("Prefabs/entity/" + id.ToString() + "/" + id.ToString()) as GameObject;

			if (go == null)
				throw new NullReferenceException("Prefabs/entity/" + id.ToString() + "/" + id);

			GameObject result = Object.Instantiate(go, position, Quaternion.identity) as GameObject;
			EnemyData data = result.GetComponent<EnemyData>();

			return RegisterNewMonster(data, id, isMinion);
		}
	}
}