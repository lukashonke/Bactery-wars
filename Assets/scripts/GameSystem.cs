using System;
using System.Collections;
using Assets.scripts.Actor;
using Assets.scripts.Actor.PlayerClasses;
using Assets.scripts.Actor.PlayerClasses.Base;
using Assets.scripts.Mono;
using Assets.scripts.Skills;
using UnityEngine;

namespace Assets.scripts
{
	public class GameSystem
	{
		// singleton 
		private static readonly GameSystem instance = new GameSystem();
		public static GameSystem Instance
		{
			get
			{
				return instance;
			}
		}

		public GameController Controller { get; private set; }

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

		// Methods
		public Player RegisterNewPlayer(PlayerData data, String name)
		{
			Player player = new Player(name, data, ClassTemplateTable.Instance.GetType(ClassId.Default));

			Debug.Log("Player " + player.Name + " has template " + player.Template.ClassId);

			player.InitTemplate();

			//TODO init, save, etc

			return player;
		}
	}
}