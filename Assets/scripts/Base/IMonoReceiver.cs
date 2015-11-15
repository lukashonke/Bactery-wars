using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.scripts.Base
{
	public interface IMonoReceiver
	{
		void MonoStart(GameObject gameObject);
		void MonoUpdate(GameObject gameObject);
		void MonoDestroy(GameObject gameObject);

		void MonoCollisionEnter(GameObject gameObject, Collision2D coll);
		void MonoCollisionExit(GameObject gameObject, Collision2D coll);
		void MonoCollisionStay(GameObject gameObject, Collision2D coll);
	}
}
