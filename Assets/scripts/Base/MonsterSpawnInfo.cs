﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.Mono.MapGenerator;
using Assets.scripts.Upgrade;
using Assets.scripts.Upgrade.Classic;
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

		public DropInfo Drop { get; private set; }

        public bool mustDieToProceed = true;

	    public MonsterSpawnInfo master;

        public MonsterSpawnInfo(MapHolder map, MonsterId id, Vector3 spawnPos, MapRegion region=null)
        {
	        Map = map;
            MonsterId = id;
            SpawnPos = spawnPos;
            Region = region;
	        level = 1;
			Drop = new DropInfo();
        }

		public MonsterSpawnInfo AddHealDrop(int chance, int level=1)
		{
			Drop.drops.Add(new DropInfo.Drop(typeof(Heal), chance, level, -1));
			return this;
		}

		public MonsterSpawnInfo AddDrop(int chance, Type upgradeType, int category, int level=1)
	    {
			Drop.drops.Add(new DropInfo.Drop(upgradeType, chance, level, category));
			return this;
	    }

		public MonsterSpawnInfo AddDrop(int chance, ItemType randomType, int rarity, int category, int level = 1)
		{
			Drop.randomDrops.Add(new DropInfo.RandomDrop(randomType, chance, level, rarity, rarity, category));
			return this;
		}

	    public MonsterSpawnInfo AddDrop(int chance, ItemType randomType, int minRarity, int maxRarity, int category, int level=1)
	    {
		    Drop.randomDrops.Add(new DropInfo.RandomDrop(randomType, chance, level, minRarity, maxRarity, category));
		    return this;
	    }

        public void SetRegion(MapRegion region)
        {
            Region = region;
        }

		public MonsterSpawnInfo SetMustDieToProceed(bool val)
		{
			mustDieToProceed = val;
			return this;
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
