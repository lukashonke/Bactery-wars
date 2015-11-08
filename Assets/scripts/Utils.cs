using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.scripts
{
	public sealed class Utils
	{
		public static Vector3 GetDirectionVector(Vector3 from, Vector3 to)
		{
			return from - to;
		}
	}
}
