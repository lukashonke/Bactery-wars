using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.Base;
using Assets.scripts.Mono.MapGenerator;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.scripts.Fort
{
	public class Siege
	{
		public const int DIRECTION_RANDOM = 0;
		public const int DIRECTION_RIGHT = 1;
		public const int DIRECTION_LEFT = 2;
		public const int DIRECTION_UP = 3;

		public bool Active { get; set; }
		public MapHolder map;
		public Character crystalObject;
		public List<SiegeMobData> mobs;
		public List<Monster> activeMobs; 

		private float timeStart;
		private float time;
		private Coroutine task;

		public Siege(MapHolder map)
		{
			this.map = map;
		}

		public void InitSiege()
		{
			mobs = new List<SiegeMobData>();
			activeMobs = new List<Monster>();
			map.levelData.SetSiege(this);
			crystalObject = map.levelData.playerBase.baseObject;
		}

		public void StartSiege()
		{
			Active = true;
			timeStart = Time.time;
			GameSystem.Instance.StartTask(SiegeTask());

			GameSystem.Instance.BroadcastMessage("Siege started!");
		}

		private IEnumerator SiegeTask()
		{
			while (Active)
			{
				time = Time.time - timeStart;
				SiegeTick();
				yield return new WaitForSeconds(1f);
			}
		}

		private void SiegeTick()
		{
			foreach (SiegeMobData data in mobs.ToArray())
			{
				if (data.timeDelay <= time)
				{
					SpawnMob(data);
				}
			}
		}

		private void SpawnMob(SiegeMobData mob)
		{
			int direction = mob.direction;

			if (direction == 0)
				direction = Random.Range(1, 3);

			Vector3 pos = new Vector3();
			switch (direction)
			{
				case DIRECTION_LEFT:
					foreach (MapRoom room in map.GetMapRooms())
					{
						Tile mostLeft = null;
						int mostLeftX = Int32.MaxValue;

						foreach (Tile t in room.edgeTiles)
						{
							if (t.tileX < mostLeftX)
							{
								mostLeft = t;
								mostLeftX = t.tileX;
							}
						}

						pos = map.GetTileWorldPosition(mostLeft);
					}
					break;
				case DIRECTION_RIGHT:
					foreach (MapRoom room in map.GetMapRooms())
					{
						Tile mostRight = null;
						int mostRightX = Int32.MinValue;

						foreach (Tile t in room.edgeTiles)
						{
							if (t.tileX > mostRightX)
							{
								mostRight = t;
								mostRightX = t.tileX;
							}
						}

						pos = map.GetTileWorldPosition(mostRight);
					}
					break;
				default:
					Debug.LogError("not finished direction chosen");
					break;
			}

			Monster m = GameSystem.Instance.SpawnMonster(mob.id, pos, false, mob.level);
			m.isSiegeMob = true;
			m.AI.AddAggro(crystalObject, 1000);
			map.RegisterMonsterToMap(m);
			activeMobs.Add(m);

			mobs.Remove(mob);
		}

		public void OnMonsterDied(Monster m)
		{
			activeMobs.Remove(m);

			if (activeMobs.Count == 0)
			{
				EndSiege();
			}
		}

		public void EndSiege()
		{
			GameSystem.Instance.BroadcastMessage("Siege finished!");

			map.levelData.SetSiege(null);
			Active = false;
			SiegeManager.SiegeEnd();
		}

		public void CancelSiege()
		{
			foreach (Monster m in activeMobs)
			{
				if (m == null) continue;

				m.DoDie();
			}

			EndSiege();
		}
	}

	public class SiegeMobData
	{
		public MonsterId id;
		public int level;
		public float timeDelay;
		public int wave;
		public int direction;

		public SiegeMobData(MonsterId id, int level, float delay, int wave, int direction=0)
		{
			this.id = id;
			this.level = level;
			this.timeDelay = delay;
			this.wave = wave;
			this.direction = direction;
		}
	}
}
