using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Base;
using Assets.scripts.Mono;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.scripts.Skills.Base
{
	public abstract class AbstractServerData : IMonoReceiver
	{
		protected GameObject ownerObject;
		protected Character ownerChar;
		protected ActiveSkill template;

		protected AbstractServerData(GameObject o, ActiveSkill t)
		{
			ownerObject = o;
			template = t;
		}

		public abstract void LaunchOnServer(Vector3 startPos, Vector3 heading);
		public abstract void UpdatePlayerHeading(Vector3 heading);
		public abstract void UpdatePlayerPosition(Vector3 position);

		protected GameObject CreateSkillResource(string resourceFolderName, string fileName, bool makeChild, Vector3 spawnPosition, Quaternion rotation)
		{
			GameObject newObject = null;
			GameObject go = LoadResource("skill", resourceFolderName, fileName);

			newObject = Object.Instantiate(go, spawnPosition, rotation) as GameObject;

			return newObject;
		}

		protected GameObject LoadResource(string type, string resourceFolderName, string fileName)
		{
			GameObject go = Resources.Load("Prefabs/" + type + "/" + resourceFolderName + "/" + fileName) as GameObject;

			if (go == null)
				throw new NullReferenceException("Prefabs/" + type + "/" + resourceFolderName + "/" + fileName + " !");

			return go;
		}

		protected void AddMonoReceiver(GameObject obj)
		{
			UpdateSender us = obj.GetComponent<UpdateSender>();

			if (us == null)
			{
				Debug.LogError("a projectile doesnt have UpdateSender; adding it automatically");
				obj.AddComponent<UpdateSender>().target = this;
			}
			else
				us.target = this;
		}

		protected Character GetOwner()
		{
			if (ownerChar != null) return ownerChar;
				
			ownerChar = GetCharacter(ownerObject);

			return ownerChar;
		}

		protected Character GetCharacter(GameObject o)
		{
			AbstractData data = o.GetComponentInParent<AbstractData>();

			if (data == null)
				return null;

			Character ch = data.GetOwner();

			return ch;
		}

		public virtual void MonoStart(GameObject gameObject)
		{
		}

		public virtual void MonoUpdate(GameObject gameObject)
		{
		}

		public virtual void MonoDestroy(GameObject gameObject)
		{
		}

		public virtual void MonoCollisionEnter(GameObject gameObject, Collision2D coll)
		{
		}

		public virtual void MonoCollisionExit(GameObject gameObject, Collision2D coll)
		{
		}

		public virtual void MonoCollisionStay(GameObject gameObject, Collision2D coll)
		{
		}

		public virtual void MonoTriggerEnter(GameObject gameObject, Collider2D other)
		{
		}

		public virtual void MonoTriggerExit(GameObject gameObject, Collider2D other)
		{
		}

		public virtual void MonoTriggerStay(GameObject gameObject, Collider2D other)
		{
		}
	}
}
