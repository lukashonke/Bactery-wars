using UnityEngine;

namespace Assets.scripts.Mono
{
	/// <summary>
	/// Preposila informace o kolizi fyzickych komponentu tela do materskeho objektu
	/// </summary>
	public class CollisionHandler : MonoBehaviour
	{
		private PlayerData data;

		// Use this for initialization
		void Start ()
		{
			data = GetComponentInParent<PlayerData>();
		}
	
		public void OnCollisionEnter2D(Collision2D coll)
		{
			data.OnCollisionEnter2D(coll);
		}

		public void OnCollisionExit2D(Collision2D coll)
		{
			data.OnCollisionExit2D(coll);
		}

		public void OnCollisionStay2D(Collision2D coll)
		{
			data.OnCollisionStay2D(coll);
		}
	}
}
