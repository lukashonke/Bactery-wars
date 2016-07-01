// Copyright (c) 2015, Lukas Honke
// ========================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor.MonsterClasses;
using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.Actor.Status;
using Assets.scripts.AI;
using Assets.scripts.Base;
using Assets.scripts.Mono.MapGenerator;
using Assets.scripts.Mono.ObjectData;
using Assets.scripts.Skills;
using Assets.scripts.Skills.Base;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.scripts.Actor
{
	public class Monster : Character
	{
		public MonsterTemplate Template { get; set; }

        public MonsterSpawnInfo SpawnInfo { get; private set; }

		private Character master;

		private bool hasMaster;
		public bool isMinion;

		public bool isSiegeMob;

		public Monster(string name, EnemyData dataObject, MonsterTemplate template) : base(name)
		{
			Data = dataObject;

			Template = template;
		}

		public Monster(string name, EnemyData dataObject, MonsterTemplate template, AbstractAI ai) : base(name, ai)
		{
			Data = dataObject;

			Template = template;
		}

	    public void SetSpawnInfo(MonsterSpawnInfo info)
	    {
	        SpawnInfo = info;
	    }

		public void SetMaster(Character master)
		{
			if (master != null)
			{
				this.master = master;
				GetData().tag = master.GetData().tag;
				Team = master.Team;
				hasMaster = true;
			}
			else
			{
				hasMaster = false;
			}
		}

		public void MasterAttacked(Character target)
		{
			if(((MonsterAI)AI).GetAggro(target) == 0)
				((MonsterAI)AI).AddAggro(target, 1);
		}

		public void MasterIsAttacked(Character attacker)
		{
			if (((MonsterAI)AI).GetAggro(attacker) == 0)
				((MonsterAI)AI).AddAggro(attacker, 1);
		}

		public void MasterForcedStopAttack()
		{
			MonsterAI ai = (MonsterAI) AI;

			ai.ClearAggro();
		}

		public Character GetMaster()
		{
			return master;
		}

		public bool HasMaster()
		{
			return hasMaster;
		}

		public override void DoDie(Character killer=null, SkillId skillId=0)
		{
			if (killer != null && killer is Player)
			{
				((Player)killer).AddXp(Template.GetXp(this));
			}

			base.DoDie(killer, skillId);

			if (SpawnInfo != null)
			{
				SpawnInfo.Drop.DoDrop(this, killer);
			}
		}

		public new EnemyData GetData()
		{
			return (EnemyData) Data;
		}

		protected override AbstractAI InitAI()
		{
			MonsterAI ai = Template.CreateAI(this);
		    return ai;
		}

		protected override CharStatus InitStatus()
		{
			CharStatus st = new MonsterStatus(false, Template.MaxHp, Template.MaxMp, Template); //TODO make a template for this
			GetData().SetVisibleHp(st.Hp);
			GetData().SetVisibleMaxHp(st.MaxHp);
			GetData().SetMoveSpeed(st.MoveSpeed);
			GetData().SetRotateSpeed((int) st.RotationSpeed);

			return st;
		}

		protected override SkillSet InitSkillSet()
		{
			return new SkillSet();
		}

		public void InitTemplate()
		{
			int i = 0;
			foreach (Skill templateSkill in Template.TemplateSkills)
			{
				// vytvorit novy objekt skillu
				Skill newSkill = SkillTable.Instance.CreateSkill(templateSkill.GetSkillId());
				newSkill.SetOwner(this);

				Skills.AddSkill(newSkill);

				i++;
				//Debug.Log("adding to monster skill to " + i + ": " + newSkill.GetVisibleName());
			}

			if (Template.MeleeSkill != null)
			{
				Skill newSkill = SkillTable.Instance.CreateSkill(Template.MeleeSkill.GetSkillId());
				newSkill.SetOwner(this);

				MeleeSkill = (ActiveSkill)newSkill;
			}

			Template.InitSkillsOnMonster(Skills, MeleeSkill, Level);

			Template.InitMonsterStats(this, Level);

			Template.InitAppearanceData(this, GetData());

			HealMe();

			GroupTemplate gt = Template.GetGroupTemplate();
			if (!isMinion && gt != null)
			{
				AI.CreateGroup();
				foreach (KeyValuePair<MonsterId, int> e in gt.MembersToSpawn)
				{
					for (i = 0; i < e.Value; i++)
					{
						Vector3 rndPos = Utils.GenerateRandomPositionAround(GetData().GetBody().transform.position, GetData().distanceToFollowLeader/2f);

						//TODO cannot change level of minions - add a parameter
						Monster mon = GameSystem.Instance.SpawnMonster(e.Key, rndPos, true, 1);
						mon.AI.JoinGroup(this);
					}
				}
			}

			AI.InitModules();
			AI.AnalyzeSkills();

			if (Template.ShowNameInGame)
			{
				GetData().showObjectName = true;
			}
		}

		public void OnAfterSpawned()
		{
			Template.OnAfterSpawned(this);
		}

		public void SpawnAssociatedMonster(string monsterTypeName, int level)
		{
			MapHolder map = SpawnInfo.Map;

			MonsterSpawnInfo info = new MonsterSpawnInfo(map, monsterTypeName, SpawnInfo.SpawnPos, null, Team);
			info.level = level;
			info.SetRegion(this.SpawnInfo.Region);

			map.AddMonsterToMap(info);
		}

		public Monster SpawnAssociatedMonster(string monsterTypeName, int level, Vector3 pos)
		{
			MapHolder map = SpawnInfo.Map;

			MonsterSpawnInfo info = new MonsterSpawnInfo(map, monsterTypeName, pos, null, this.Team);
			info.level = level;
			info.SetRegion(this.SpawnInfo.Region);

			return map.AddMonsterToMap(info);
		}

		public Monster SpawnAndConnectMonster(string monsterTypeName, int level, Vector3 pos)
		{
			MapHolder map = SpawnInfo.Map;

			MonsterSpawnInfo info = new MonsterSpawnInfo(map, monsterTypeName, pos, null, this.Team);
			info.level = level;
			info.SetRegion(this.SpawnInfo.Region);

			Monster m = map.AddMonsterToMap(info);
			GetData().ConnectChildCharacter(m.GetData());
			return m;
		}

		public override bool IsInteractable()
		{
			return false;
		}
	}
}
