using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.scripts.Mono.MapGenerator
{
	public class Tile
	{
		public int tileX;
		public int tileY;
		public int tileType;
		public bool isChecked;
		public int color;

		public Tile(int x, int y, int t)
		{
			tileX = x;
			tileY = y;
			tileType = t;
			isChecked = false;
		}

		public Tile(int x, int y)
		{
			tileX = x;
			tileY = y;
			tileType = 0;
			isChecked = false;
		}

		public void Check()
		{
			isChecked = true;
		}

		public void Uncheck()
		{
			isChecked = false;
		}
	}
}
