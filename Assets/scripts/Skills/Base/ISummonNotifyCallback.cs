using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.scripts.Actor;
using Assets.scripts.AI.Modules;
using JetBrains.Annotations;

namespace Assets.scripts.Skills.Base
{
	interface ISummonNotifyCallback
	{
		void SetCallback(SpawnMinionModule.SummonNotify del);
		SpawnMinionModule.SummonNotify GetCallback();
	}
}
