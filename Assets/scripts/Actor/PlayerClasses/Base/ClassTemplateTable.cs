using System.Collections.Generic;

namespace Assets.scripts.Actor.PlayerClasses.Base
{
	public class ClassTemplateTable
	{
		// singleton
		private static ClassTemplateTable instance = null;
		public static ClassTemplateTable Instance
		{
			get
			{
				if (instance == null)
					instance = new ClassTemplateTable();

				return instance;
			}
		}

		private Dictionary<ClassId, ClassTemplate> types;

		public ClassTemplateTable()
		{
			types = new Dictionary<ClassId, ClassTemplate>();

			Init();
		}

		// Initialize all possible classes here
		private void Init()
		{
			AddType(new DefaultPlayerClass());
		}

		public void AddType(ClassTemplate t)
		{
			types.Add(t.ClassId, t);
		}

		public ClassTemplate GetType(ClassId type)
		{
			ClassTemplate tObject;
            types.TryGetValue(type, out tObject);
			return tObject;
		}
	}
}
