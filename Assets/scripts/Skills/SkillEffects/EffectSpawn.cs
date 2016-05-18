using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.Mono;
using Assets.scripts.Mono.MapGenerator;
using Assets.scripts.Skills.Base;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.scripts.Skills.SkillEffects
{
	public class EffectSpawn : SkillEffect
	{
		public string nameToSpawn;
		public float maxDistance;
		public bool forceAttack;

		public EffectSpawn(string monsterName, float distance, bool forceAttack=true)
		{
			this.nameToSpawn = monsterName;
			this.maxDistance = distance;
			this.forceAttack = forceAttack;
		}

		public override void ApplyEffect(Character source, GameObject target)
		{
			bool canSpawn = false;
			Character targetCh = Utils.GetCharacter(target);

			if (targetCh == null) // target may be a Destroyable object
			{
				Destroyable d = target.GetComponent<Destroyable>();

				if (d != null && source.CanAttack(d))
				{
					canSpawn = true;
				}
			}
			else // target may be a character
			{
				if (source.CanAttack(targetCh))
				{
					canSpawn = true;
				}
			}

			if(canSpawn)
			{
				Vector3 targetPos = target.transform.position;
				Vector3 finalTarget = new Vector3();
				bool found = false;

				int limit = 15;
				while (limit > 0)
				{
					finalTarget = Utils.GenerateRandomPositionAround(targetPos, maxDistance, 1f);
					if (Utils.IsNotAccessible(source.GetData().GetBody().transform.position, finalTarget))
					{
						limit--;
						continue;
					}
					else
					{
						found = true;
						break;
					}
				}

				if (found)
				{
					Monster m = GameSystem.Instance.SpawnMonster(nameToSpawn, finalTarget, false, 1, source.Team);
					WorldHolder.instance.activeMap.RegisterMonsterToMap(m);

					if (forceAttack)
					{
						m.AI.CopyAggroFrom(source.AI);
					}
				}
			}
		}

		public override void Update()
		{
		}

		public override void OnRemove()
		{
		}

		public override SkillTraits[] GetTraits()
		{
			return new SkillTraits[] { SkillTraits.SpawnMinion, };
		}
	}
}
