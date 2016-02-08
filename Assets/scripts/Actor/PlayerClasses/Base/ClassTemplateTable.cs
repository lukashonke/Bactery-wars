using System;
using System.Collections.Generic;
using UnityEngine;

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
			ClassTemplate t;
			List<Type> types = Utils.GetTypesInNamespace("Assets.scripts.Actor.PlayerClasses", true, typeof(ClassTemplate));

			foreach (Type type in types)
			{
				t = Activator.CreateInstance(type) as ClassTemplate;
				AddType(t);
			}

			Debug.Log("Loaded " + this.types.Count + " player classes.");
		}

		public void AddType(ClassTemplate t)
		{
			types.Add(t.GetClassId(), t);
		}

		public ClassTemplate GetType(ClassId type)
		{
			ClassTemplate tObject;
            types.TryGetValue(type, out tObject);
			return tObject;
		}
	}
}
