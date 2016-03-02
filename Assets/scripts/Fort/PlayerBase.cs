using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.Mono;
using Assets.scripts.Mono.MapGenerator;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.scripts.Fort
{
	public class PlayerBase
	{
		public GameObject baseObjectTemplate;
		public Monster baseObject;

		public MapHolder map;

		public Vector3 crystalPosition;

		public PlayerBase(MapHolder map, Vector3 position, GameObject baseTemplate)
		{
			this.map = map;
			this.crystalPosition = position;

			if (baseTemplate != null)
			{
				baseObjectTemplate = baseTemplate;
			}
			else
			{
				baseObjectTemplate = Resources.Load<GameObject>("prefabs/base/SimpleBase");
			}
		}

		public void LoadBase()
		{
			baseObject = GameSystem.Instance.SpawnMonster(MonsterId.SimpleBase, crystalPosition, false, 1, GameSystem.Instance.CurrentPlayer.Team);
		}

		public void DeloadBase()
		{
			if (baseObject != null)
			{
				baseObject.DoDie();
			}
		}
	}
}
