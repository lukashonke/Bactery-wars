using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Assets.scripts.Base
{
	public static class Messages
	{
		public static HelpMessageData ShowHelpWindow(string type, params Object[] o)
		{
			int lang = GameSystem.Instance.Language;

			if (lang == 0)
			{
				switch (type)
				{
					case "game_start":
						{
							return CreateHelpMessageData("Welcome to Bactery Wars", new string[] { "Alfa version {0}" }, o);
						}
					case "game_start_controls":
						{
							return CreateHelpMessageData("Game controls", new string[] { "- Use left mouse button to move.", "- Right mouse button to shoot", "- Keys 1-5 to use skills" }, o);
						}
					case "first_skill_unlocked":
						{
							return CreateHelpMessageData("Unlocked a skill!", new string[] { "Skill {0} is now available!", "This skill {1}", "Press button 2 to use it." }, o);
						}
					case "first_skill_unlocked_commoncold":
						{
							return CreateHelpMessageData("Unlocked a skill!", new string[] { "Skill {0} is now available!", "Use it to avoid enemies or to charge into them to push them away.", "Press button 2 to use it." }, o);
						}
				}
			}
			else if (lang == 1)
			{
				switch (type)
				{
					case "game_start":
						{
							return CreateHelpMessageData("Vitej v Bactery Wars!", new string[] { "Alfa verze {0}" }, o);
						}
					case "game_start_controls":
						{
							return CreateHelpMessageData("Ovladani hry", new string[] { "- Levé tlačítko myši k pohybu", "- Pravé tlačítko myši ke střílení", "- Klávesy 1-5 k použití skillů." }, o);
						}
					case "first_skill_unlocked":
						{
							return CreateHelpMessageData("Unlocked a skill!", new string[] { "Skill {0} je nyní dostupný!", "Tento skill {1}", "Stiskni tlačítko 2 k aktivování." }, o);
						}
				}
			}

			return new HelpMessageData();
		}

		private static HelpMessageData CreateHelpMessageData(string title, string[] text, params Object[] o)
		{
			HelpMessageData d = new HelpMessageData();
			d.title = String.Format(title, o);

			d.text = new string[text.Length];
			for (int i = 0; i < text.Length; i++)
			{
				d.text[i] = String.Format(text[i], o);
			}
			
			return d;
		}
	}

	public struct HelpMessageData
	{
		public string title;
		public string[] text;
	}
}
