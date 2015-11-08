using System.Collections;
using UnityEngine;

namespace Assets.scripts
{
	/// <summary>
	/// Pri startu hry spusti GameSystem
	/// </summary>
	public class GameController : MonoBehaviour
	{
		void Start()
		{
			GameSystem.Instance.Start(this);
		}
	}
}
