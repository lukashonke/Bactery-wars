using System.Collections;
using UnityEngine;

namespace Assets.scripts
{
	public class GameController : MonoBehaviour
	{
		// Use this for initialization
		void Start()
		{
			GameSystem.Instance.Start(this);
		}

		// Update is called once per frame
		void Update()
		{
			//GameSystem.Instance.Update();
		}
	}
}
