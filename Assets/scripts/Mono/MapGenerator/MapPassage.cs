using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.scripts.Mono.MapGenerator
{
	public class MapPassage
	{
		public List<Tile> tiles; 
		public MapPassage(List<Tile> tiles)
		{
			this.tiles = tiles;
		}

		public void CreateMesh()
		{
			//TODO nefunguje jeste
			//TODO sestavit 2D int matici
			// z ni sestavit mesh, ten pak vlozit do sceny a pomoci shiftvectoru
			// vypinat a zapinat = vytvoreni pruchodu
			MeshGenerator meshGen;

			int maxX = -1;
			int minX = -1;
			int maxY = -1;
			int minY = -1;

			foreach (Tile t in tiles)
			{
				if (t.tileX > maxX || maxX == -1)
				{
					maxX = t.tileX;
				}

				if (t.tileX < minX || minX == -1)
				{
					minX = t.tileX;
				}

				if (t.tileY > maxY || maxY == -1)
				{
					maxY = t.tileY;
				}

				if (t.tileY < minY || minY == -1)
				{
					minY = t.tileY;
				}
			}

			int size = Mathf.Max(maxX, maxY);
			int i = 0;
			int[,] tilesInt = new int[size, size];
			for (int x = 0; x < tilesInt.GetLength(0); x++)
			{
				for (int y = 0; y < tilesInt.GetLength(1); y++)
				{
					tilesInt[x, y] = 0;

					foreach (Tile t in tiles)
					{
						if (t.tileX == x || t.tileY == y)
						{
							i++;
							tilesInt[x, y] = 1;
						}
					}
				}
			}

			Debug.Log(i);

			meshGen = new MeshGenerator(GameObject.Find("Cave Generator"));
			MeshFilter mesh = meshGen.GenerateMesh("Passage", tilesInt, 1, new Vector3());

			mesh.transform.position = new Vector3();
			mesh.gameObject.SetActive(true);
		}
	}
}
