using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.scripts.Mono
{
	/// <summary>
	/// The class for all graphical/physical objects in Unity
	/// </summary>
	public abstract class AbstractData : MonoBehaviour
	{
		public bool USE_VELOCITY_MOVEMENT;

		protected Dictionary<string, GameObject> childs;

		public AbstractData()
		{
			childs = new Dictionary<string, GameObject>();
		}

		public void Start()
		{
			AddChildObjects(transform);
		}

		private void AddChildObjects(Transform t)
		{
			foreach (Transform child in t)
			{
				childs.Add(child.gameObject.name, child.gameObject);
				AddChildObjects(child);
			}
        }

		protected GameObject GetChildByName(string n)
		{
			GameObject val;

			childs.TryGetValue(n, out val);

			return val;
		}

		public GameObject CreateProjectile(string folderName, string name)
		{
			GameObject go = Resources.Load("Prefabs/projectiles/" + folderName + "/" + name) as GameObject;
			if (go == null)
				throw new NullReferenceException("cannot find " + folderName + "/" + name + " !");

			GameObject newProjectile = Instantiate(go, GetShootingPosition().transform.position, GetBody().transform.rotation) as GameObject;
			if (newProjectile != null)
			{
				newProjectile.tag = gameObject.tag;
			}

			return newProjectile;
		}


		public abstract void JumpForward(float dist, float jumpSpeed);

		public abstract GameObject GetBody();
		public abstract GameObject GetShootingPosition();
		public abstract Vector3 GetForwardVector();
		public abstract Vector3 GetForwardVector(int angle); //TODO unabstract this here
	}
}
