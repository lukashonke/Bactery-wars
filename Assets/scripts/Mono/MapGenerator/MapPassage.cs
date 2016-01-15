using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.scripts.Mono.MapGenerator
{
	public class MapPassage
	{
	    public bool enabled;

		public List<Tile> tiles;
		public Tile centerTile, starTile, endTile;

		public bool isDoor;
		public GameObject gameObject;

	    public MapRoom roomA, roomB;

		public MapPassage(List<Tile> tiles, Tile center, Tile start, Tile end, MapRoom roomA, MapRoom roomB)
		{
			this.tiles = tiles;
			centerTile = center;

			starTile = start;
			endTile = end;

		    this.roomA = roomA;
		    this.roomB = roomB;

		    enabled = true;
		}

		/*private void Init()
		{
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

			int size = Mathf.Max(maxX - minX, maxY - minY) + 3;
			tilesInt = new int[size, size];
			for (int x = 0; x < tilesInt.GetLength(0); x++)
			{
				for (int y = 0; y < tilesInt.GetLength(1); y++)
				{
					tilesInt[x, y] = 0;
				}
			}

			int i = 0;
			foreach (Tile t in tiles)
			{
				int tx = t.tileX - minX;
				int ty = t.tileY - minY;

				if (tx == (size / 2 - 1) && ty == (size / 2 - 1))
					centerTile = t;

				i++;

				if (centerTile == null && i == size/2)
					centerTile = t;

				tilesInt[tx, ty] = 1;
			}

			Debug.Log(i);
		}*/

		/*public MeshFilter CreateMesh(Vector3 shiftVector)
		{
			//TODO nefunguje jeste
			//TODO sestavit 2D int matici
			// z ni sestavit mesh, ten pak vlozit do sceny a pomoci shiftvectoru
			// vypinat a zapinat = vytvoreni pruchodu
			meshGen = new MeshGenerator(GameObject.Find("Cave Generator"));
			mesh = meshGen.GenerateMesh("Passage", tilesInt, 1, shiftVector);

			mesh.transform.position = shiftVector;
			mesh.gameObject.SetActive(true);

			return mesh;
		}*/

		public void AssignGameObject(GameObject o)
		{
			gameObject = o;
		}

		public void Delete()
		{
			Object.Destroy(gameObject);
		}

	    public void SetEnabled(bool b)
	    {
	        enabled = b;
	        gameObject.SetActive(b);
	    }
	}
}
