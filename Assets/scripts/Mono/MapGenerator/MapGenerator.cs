using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.scripts.Mono.MapGenerator
{
	public abstract class MapGenerator
	{
		protected int width;
		protected int height;
		protected string seed;
		protected bool DoDebug;
		protected int shiftX;
		protected int shiftY;
		protected Vector3 shiftVector;

		public MapGenerator(int width, int height, string seed, int shiftX, int shiftY, Vector3 shiftVector, bool DoDebug)
		{
			this.width = width;
			this.height = height;
			this.seed = seed;
			this.DoDebug = DoDebug;
			this.shiftVector = shiftVector;
			this.shiftX = shiftX;
			this.shiftY = shiftY;
		}

		public virtual void OnDrawGizmos()
		{
			
		}

		public abstract int[,] GenerateMap();
		public abstract MeshGenerator GenerateMesh(GameObject parent, int[,] map, float squareSize, int x, int y, bool doDebug);
		public abstract MeshGenerator GetMeshGenerator();
	}
}