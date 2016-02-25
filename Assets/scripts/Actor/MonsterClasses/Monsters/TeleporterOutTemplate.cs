using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.AI;
using Assets.scripts.Mono.MapGenerator;
using Assets.scripts.Mono.ObjectData;

namespace Assets.scripts.Actor.MonsterClasses.Monsters
{
	public class TeleporterOutTemplate : MonsterTemplate
	{
		public TeleporterOutTemplate()
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
				PlayerData data = ((Player)source).GetData();

			    if (WorldHolder.instance.activeMap.CanTeleportToNext())
			    {
					WorldHolder.instance.activeMap.OnTeleportOut((Player) source);

                    WorldHolder.instance.LoadNextMap();

					// teleport player to new start
					data.transform.position = WorldHolder.instance.GetStartPosition();

					WorldHolder.instance.activeMap.OnTeleportIn((Player)source);
			    }
			    else
			    {
				    data.GetOwner().Message("Not all enemies are dead yet.");
			    }
			}
		}

		public override MonsterId GetMonsterId()
		{
			return MonsterId.TeleporterOut;
		}
	}
}
