// Copyright (c) 2015, Lukas Honke
// ========================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.scripts.Mono.MapGenerator
{
	/// <summary>
	/// reprezentuje jednu "dlazdici" na mape
	/// </summary>
	public class Tile
	{
		public int tileX;
		public int tileY;
		public int tileType;
		public bool isChecked;

		// pro debugovani
		private int color;
		public static int GREEN = 1;
		public static int MAGENTA = 2;
		public static int YELLOW = 3;
		public static int PINK = 4;
		public static int BLUE = 5;
		public static int RED = 6;
		public static int PURPLE = 7;
		public static int ORANGE = 8;
		public static int BROWN = 9;

		// pridruzeny region mapy
		public MapRegion region;

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

		public void AssignRegion(MapRegion r)
		{
			region = r;
		}

		public void Check()
		{
			isChecked = true;
		}

		public void Uncheck()
		{
			isChecked = false;
		}

		public void SetColor(int i)
		{
			if (color == 0)
			{
				
			}
			color = i;
		}

		public int GetColor()
		{
			return color;
		}
	}
}
