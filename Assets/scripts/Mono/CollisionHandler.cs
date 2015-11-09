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
			// najit script v materske tride ktery implementuje ICollidable
			GameObject parent = transform.parent.gameObject;
			if (parent == null)
			{
				Debug.LogError("cant find parent object for ColisionHandler's object for " + gameObject.name + " !!! will throw errors later on");
				return;
			}

			foreach(MonoBehaviour m in GetComponentsInParent<MonoBehaviour>())
			{
				if (m is ICollidable)
				{
					data = (ICollidable) m;
					break;
				}
			}

			if (data == null)
				Debug.LogError("cant find ICollidable Mono script in parent object for " + gameObject.name);
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

		public void OnTriggerEnter2D(Collider2D obj)
		{
			data.OnTriggerEnter2D(obj);
		}

		public void OnTriggerExit2D(Collider2D obj)
		{
			data.OnTriggerExit2D(obj);
		}

		public void OnTriggerStay2D(Collider2D obj)
		{
			data.OnTriggerStay2D(obj);
		}
	}
}
