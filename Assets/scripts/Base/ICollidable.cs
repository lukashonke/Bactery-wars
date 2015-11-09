using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.scripts.Base
{
	interface ICollidable
	{
		void OnCollisionEnter2D(Collision2D coll);

		void OnCollisionExit2D(Collision2D coll);

		void OnCollisionStay2D(Collision2D coll);
	}
}
