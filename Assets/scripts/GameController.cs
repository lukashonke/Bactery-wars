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

		void OnDrawGizmos()
		{
			/*if (map != null)
			{
				for (int x = 0; x < width; x++)
				{
					for (int y = 0; y < height; y++)
					{
						Gizmos.color = (map[x, y] == 1) ? Color.black : Color.white;
						Vector3 pos = new Vector3(-width / 2 + x + .5f, 0, -height / 2 + y + .5f);
						Gizmos.DrawCube(pos, Vector3.one);
					}
				}
			}*/
		}
	}
}
