// Copyright (c) 2015, Lukas Honke
// ========================
using Assets.scripts.Base;
using UnityEngine;

namespace Assets.scripts.Mono
{
	/// <summary>
	/// Preposila informace o kolizi fyzickych komponentu tela do materskeho objektu
	/// Matersky objekt musi implementovat ICollidable
	/// </summary>
	public class CollisionHandler : MonoBehaviour
	{
		private ICollidable data;

		// Use this for initialization
		public void Start ()
		{
			data = GetComponent<ICollidable>();

			if (data == null)
				Debug.LogError("cant find ICollidable Mono script in parent object for " + gameObject.name);
		}
	
		public void OnCollisionEnter2D(Collision2D coll)
		{
			//data.OnCollisionEnter2D(coll);
		}

		public void OnCollisionExit2D(Collision2D coll)
		{
			//data.OnCollisionExit2D(coll);
		}

		public void OnCollisionStay2D(Collision2D coll)
		{
			//data.OnCollisionStay2D(coll);
		}

		public void OnTriggerEnter2D(Collider2D obj)
		{
			//data.OnTriggerEnter2D(obj);
		}

		public void OnTriggerExit2D(Collider2D obj)
		{
			//data.OnTriggerExit2D(obj);
		}

		public void OnTriggerStay2D(Collider2D obj)
		{
			//data.OnTriggerStay2D(obj);
		}
	}
}
