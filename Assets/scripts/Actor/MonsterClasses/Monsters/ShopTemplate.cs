using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.AI;
using Assets.scripts.Mono.MapGenerator;
using Assets.scripts.Mono.ObjectData;
using UnityEngine;

namespace Assets.scripts.Actor.MonsterClasses.Monsters
{
	public class ShopTemplate : MonsterTemplate
	{
		public ShopTemplate()
		{
			MaxHp = 20;
			MaxMp = 50;
			MaxSpeed = 10;
		}

		protected override void AddSkillsToTemplate()
		{
			// no skills
		}

		public override MonsterAI CreateAI(Character ch)
		{
			return new DefaultMonsterAI(ch);
		}

		public override GroupTemplate GetGroupTemplate()
		{
			return null;
		}

		public override void OnTalkTo(Character source)
		{
			if (source is Player)
			{
				WorldHolder.instance.OnShopOpen(((Player)source));
			}
		}

		public override MonsterId GetMonsterId()
		{
			return MonsterId.Shop;
		}
	}
}
