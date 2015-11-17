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

		/// <summary>
		/// Clones gameobject prefab from Resources/prefabs/[type]/[resourceFolderName]/[fileName].prefab
		/// </summary>
		public GameObject LoadResource(string type, string resourceFolderName, string fileName)
		{
			GameObject go = Resources.Load("Prefabs/" + type + "/" + resourceFolderName + "/" + fileName) as GameObject;
			if (go == null)
				throw new NullReferenceException("Prefabs/" + type + "/" + resourceFolderName + "/" + fileName + " !");

			return go;
		}

		public GameObject CreateSkillResource(string resourceFolderName, string fileName, bool makeChild, Vector3 position)
		{
			GameObject go = LoadResource("skill", resourceFolderName, fileName);

			GameObject newObject = Instantiate(go, position, GetBody().transform.rotation) as GameObject;

			if (newObject != null)
			{
				if (makeChild)
					newObject.transform.parent = GetBody().transform;

				newObject.tag = gameObject.tag;
			}

			return newObject;
		}

		/// <summary>
		/// Instantiates an object from Resources/Prefabs/skill folder
		/// </summary>
		public GameObject CreateSkillParticleEffect(string folderName, string name, bool makeChild)
		{
			GameObject go = LoadResource("skill", folderName, name);

			GameObject newProjectile = Instantiate(go, GetParticleSystemObject().transform.position, GetBody().transform.rotation) as GameObject;
			if (newProjectile != null)
			{
				if (makeChild)
					newProjectile.transform.parent = GetParticleSystemObject().transform;

				newProjectile.tag = gameObject.tag;
			}

			return newProjectile;
		}

		/// <summary>
		/// Clones gameobject from: Resources/prefabs/skill/[foldername]/[name].prefab and places it on GetShootingPosition() position
		/// </summary>
		/// <param name="folderName"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public GameObject CreateProjectile(string folderName, string name)
		{
			GameObject go = LoadResource("skill", folderName, name);

			GameObject newProjectile = Instantiate(go, GetShootingPosition().transform.position, GetBody().transform.rotation) as GameObject;
			if (newProjectile != null)
			{
				newProjectile.tag = gameObject.tag;
			}

			return newProjectile;
		}


		public abstract void JumpForward(float dist, float jumpSpeed);
		public abstract void JumpForward(Vector3 direction, float dist, float jumpSpeed);

		public abstract GameObject GetParticleSystemObject();
		public abstract GameObject GetBody();
		public abstract GameObject GetShootingPosition();
		public abstract Vector3 GetForwardVector();
		public abstract Vector3 GetForwardVector(int angle); //TODO unabstract this here
	}
}
