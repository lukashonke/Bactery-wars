﻿using System;
using Assets.scripts.Actor.MonsterClasses.Base;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.scripts
{
	/// <summary>
	/// Pri startu hry spusti GameSystem
	/// </summary>
	public class GameController : MonoBehaviour
	{
		public GameObject prefabToSpawn;

        public bool fogOfWar;
		public const bool DEV_BUILD = true;
		public bool isAndroid;

		void Start()
		{
#if UNITY_ANDROID
			isAndroid = true;
#endif

			if (isAndroid)
			{
				try
				{
					GameObject.Find("GameMenu").SetActive(false);
					GameObject.Find("GameMenu_Mobile").SetActive(true);
				}
				catch (Exception)
				{
				}
			}

			try
			{
				if(isAndroid == false)
					GameObject.Find("SettingsMenu").GetComponent<Canvas>().enabled = true;
				if(isAndroid)
					GameObject.Find("SettingsMenu_Mobile").GetComponent<Canvas>().enabled = true;
			}
			catch (Exception)
			{
				
			}

			Input.simulateMouseWithTouches = false;

			GameSystem.Instance.Start(this);
		}

		void Update()
		{
			//if (System.Environment.TickCount%600 == 0)
			//{
				//SpawnTestMob();
			//}
		}

		public void SpawnTestMob()
		{
			//GameSystem.Instance.SpawnMonster(MonsterId.TestMonster, new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), 0), false);
		}
	}
}
