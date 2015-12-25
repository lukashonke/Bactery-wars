using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.scripts.Mono.MapGenerator
{
	public class MapPassage
	{
		public List<Tile> tiles; 
		public MapPassage(List<Tile> tiles)
		{
			this.tiles = tiles;
		}
	}
}
