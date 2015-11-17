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

		public static Quaternion GetRotationToDirectionVector(Vector3 vector)
		{
			return Quaternion.LookRotation(vector, Vector3.forward);
		}

		public static Vector3 GetDirectionVectorToMousePos(Transform from)
		{
			Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			mousePos.z = 0;

			Vector3 target = mousePos - from.position;
			return target;
		}

		public static Quaternion GetRotationToMouse(Transform t)
		{
			Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			mousePos.z = 0;

			Vector3 target = mousePos - t.position;
			Quaternion r = Quaternion.LookRotation(target);
			return r;
		}

		public static Vector3 RotateDirectionVector(Vector3 vector, int angle)
		{
			Vector3 nv = Quaternion.Euler(new Vector3(0, 0, angle)) * vector;
			return nv;
		}
	}
}
