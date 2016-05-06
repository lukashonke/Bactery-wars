using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.AI;
using Assets.scripts.Mono.MapGenerator;
using Assets.scripts.Mono.ObjectData;

namespace Assets.scripts.Actor.MonsterClasses.Monsters
{
	public class TeleporterInTemplate : MonsterTemplate
	{
		public TeleporterInTemplate()
		{
			MaxHp = 20;
			MaxMp = 50;
			MaxSpeed = 10;
		}

		public override void AddSkillsToTemplate()
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
				if (WorldHolder.instance.activeMap.levelData.IsUnderSiege())
				{
					GameSystem.Instance.BroadcastMessage("Cant teleport when your base is under siege!");
					return;
				}

				PlayerData data = ((Player) source).GetData();

				data.ui.ShowLevelsView();

				/*WorldHolder.instance.LoadPreviousMap();

				//TODO bug - it teleports anyway
				// teleport player to new start
				data.transform.position = WorldHolder.instance.GetStartPosition();*/
			}
		}

		public override MonsterId GetMonsterId()
		{
			return MonsterId.TeleporterIn;
		}
	}
}
