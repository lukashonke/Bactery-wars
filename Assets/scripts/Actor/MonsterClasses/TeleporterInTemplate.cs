using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.AI;
using Assets.scripts.Mono.MapGenerator;
using Assets.scripts.Mono.ObjectData;
using Assets.scripts.Skills;
using Assets.scripts.Skills.Base;

namespace Assets.scripts.Actor.MonsterClasses
{
	public class TeleporterInTemplate : MonsterTemplate
	{
		public TeleporterInTemplate()
		{
			MaxHp = 20;
			MaxMp = 50;
			MaxSpeed = 10;
		}

		protected override void AddSkills()
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
				PlayerData data = ((Player) source).GetData();

				WorldHolder.instance.LoadPreviousMap();

				//TODO bug - it teleports anyway
				// teleport player to new start
				data.transform.position = WorldHolder.instance.GetStartPosition();
			}
		}

		public override MonsterId GetMonsterId()
		{
			return MonsterId.TeleporterIn;
		}
	}
}
