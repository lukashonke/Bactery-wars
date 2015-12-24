using System;
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

		void Start()
		{
			bool mobile = false;
#if UNITY_ANDROID
			mobile = true;
#endif

			if (mobile)
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

			GameSystem.Instance.Start(this);
		}

		void Update()
		{
			if (System.Environment.TickCount%600 == 0)
			{
				//SpawnTestMob();
			}
		}

		public void SpawnTestMob()
		{
			GameSystem.Instance.SpawnMonster(MonsterId.TestMonster, new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), 0), false);
		}
	}
}
