using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.Mono.MapGenerator;
using UnityEngine;

namespace Assets.scripts.Base
{
    public class MonsterSpawnInfo
    {
	    public MapHolder Map { get; set; }
        public MonsterId MonsterId { get; private set; }
        public Vector3 SpawnPos { get; set; }
        public MapRegion Region { get; private set; }
	    public int level;

        public bool mustDieToProceed = true;

	    public MonsterSpawnInfo master;

        public MonsterSpawnInfo(MapHolder map, MonsterId id, Vector3 spawnPos, MapRegion region=null)
        {
	        Map = map;
            MonsterId = id;
            SpawnPos = spawnPos;
            Region = region;
	        level = 1;
        }

        public void SetRegion(MapRegion region)
        {
            Region = region;
        }

        /*public override bool Equals(object obj)
        {
            if (obj == null || !(obj is MonsterSpawnInfo))
            {
                return false;
            }

            return ((MonsterSpawnInfo)obj).
        }*/
    }
}
