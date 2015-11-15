using Assets.scripts.Base;
using Assets.scripts.Mono;
using UnityEngine;

namespace Assets.scripts
{
	/// <summary>
	/// TODO redo this into a particle effect
	/// </summary>
	public class ProjectileBlackTestData : MonoBehaviour, IDamagable
	{
		public Texture2D source;
		public int collapseSpeed;

		// Use this for initialization
		void Start()
		{
			SpriteRenderer sr = GetComponent<SpriteRenderer>();
			source = sr.sprite.texture;

			if (source == null)
				Debug.LogError("chyba - texture2d pro " + gameObject.name + " neexistuje!");
		}

		// Update is called once per frame
		void Update()
		{

		}

		public void OnTriggerEnter2D(Collider2D obj)
		{
			// TODO: nepouzivat tagy, ale pridat promennou pres extension metody do gameobject pro porovnavani tymu
			if (obj.gameObject.tag == gameObject.tag)
				return;

			collapse();
		}

		private void collapse()
		{
			// pouze sude hodnoty pro spravnou explozi! 2x2, 4x4, 6x6, 8x8
			const int piecesX = 4;
			const int piecesY = 4;
			int sizeX = source.width;
			int sizeY = source.height;

			int pieceSizeX = (int)(sizeX / (float)piecesX);
			int pieceSizeY = (int)(sizeY / (float)piecesY);

			float temp = piecesX / 2f;
			float temp2 = piecesY / 2f;

			float pieceXShift;
			float pieceYShift;

			for (int i = 0; i < piecesX; i++)
			{
				for (int j = 0; j < piecesY; j++)
				{
					Sprite newSprite = Sprite.Create(source, new Rect(i * pieceSizeX, j * pieceSizeY, pieceSizeX, pieceSizeY), new Vector2(0, 0));

					GameObject nObject = new GameObject();
					nObject.name = "Object Piece " + i + ", " + j;
					Rigidbody2D rb = nObject.AddComponent<Rigidbody2D>();
					rb.gravityScale = 0;

					nObject.AddComponent<FadeOut>();

					if (i < temp)
					{
						if (j < temp2)
						{
							rb.velocity = new Vector2(-1, -1) * Time.deltaTime * collapseSpeed;
						}
						else if (j == temp2)
						{
							rb.velocity = new Vector2(-1, 0) * Time.deltaTime * collapseSpeed;
						}
						else
						{
							rb.velocity = new Vector2(-1, 1) * Time.deltaTime * collapseSpeed;
						}
					}
					else if (i == temp)
					{
						if (j < temp2)
						{
							rb.velocity = new Vector2(0, -1) * Time.deltaTime * collapseSpeed;
						}
						else if (j == temp2)
						{
							rb.velocity = new Vector2(0, -1) * Time.deltaTime * collapseSpeed;
						}
						else
						{
							rb.velocity = new Vector2(0, 1) * Time.deltaTime * collapseSpeed;
						}
					}
					else
					{
						if (j < temp2)
						{
							rb.velocity = new Vector2(1, -1) * Time.deltaTime * collapseSpeed;
						}
						else if (j == temp2)
						{
							rb.velocity = new Vector2(1, 0) * Time.deltaTime * collapseSpeed;
						}
						else
						{
							rb.velocity = new Vector2(1, 1) * Time.deltaTime * collapseSpeed;
						}
					}

					SpriteRenderer sr = nObject.AddComponent<SpriteRenderer>();
					sr.sprite = newSprite;

					pieceXShift = sr.bounds.extents.x * 2;
					pieceYShift = sr.bounds.extents.y * 2;

					nObject.transform.parent = gameObject.transform;
					nObject.layer = 1;
					nObject.transform.localPosition = new Vector3((+i) * pieceXShift, (+j) * pieceYShift, 0);
				}
			}

			GetComponent<SpriteRenderer>().enabled = false;
			GetComponent<Collider2D>().enabled = false;

			// stop the movement so that it doesnt penetrate and hit other objects
			GetComponent<Rigidbody2D>().velocity = new Vector3(0, 0, 0);

			// destroy the object after 1.5 seconds
			Destroy(gameObject, 1.5f);
		}

		public int GetDamage()
		{
			return 1;
		}
	}
}
