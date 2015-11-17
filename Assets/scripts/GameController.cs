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
			Instantiate(prefabToSpawn, new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), 0), Quaternion.identity);
		}
	}
}
