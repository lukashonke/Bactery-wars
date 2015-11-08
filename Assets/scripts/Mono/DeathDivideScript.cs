using UnityEngine;

namespace Assets.scripts.Mono
{
    public class DeathDivideScript : MonoBehaviour
    {
        public Texture2D source;
        public int speed;

        // Use this for initialization
        void Start()
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

            //Debug.Log (temp + " " + pieceSizeX) ;

            /*Vector3 shiftX = Camera.main.ScreenToWorldPoint(new Vector3 (pieceSizeX, 0, 0));
            Vector3 shiftY = Camera.main.ScreenToWorldPoint(new Vector3 (0, pieceSizeY, 0));

            Debug.Log (shiftX);
            Debug.Log (shiftY);*/

            //Debug.Log ((transform.position.x - temp * pieceSizeX) + " " + (transform.position.y - temp2 * pieceSizeY));

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

	                Debug.Log("test");

                    if (i < temp)
                    {
                        if (j < temp2)
                        {
                            rb.velocity = new Vector2(-1, -1) * Time.deltaTime * speed;
                        }
                        else if (j == temp2)
                        {
                            rb.velocity = new Vector2(-1, 0) * Time.deltaTime * speed;
                        }
                        else
                        {
                            rb.velocity = new Vector2(-1, 1) * Time.deltaTime * speed;
                        }
                    }
                    else if (i == temp)
                    {
                        if (j < temp2)
                        {
                            rb.velocity = new Vector2(0, -1) * Time.deltaTime * speed;
                        }
                        else if (j == temp2)
                        {
                            rb.velocity = new Vector2(0, -1) * Time.deltaTime * speed;
                        }
                        else
                        {
                            rb.velocity = new Vector2(0, 1) * Time.deltaTime * speed;
                        }
                    }
                    else
                    {
                        if (j < temp2)
                        {
                            rb.velocity = new Vector2(1, -1) * Time.deltaTime * speed;
                        }
                        else if (j == temp2)
                        {
                            rb.velocity = new Vector2(1, 0) * Time.deltaTime * speed;
                        }
                        else
                        {
                            rb.velocity = new Vector2(1, 1) * Time.deltaTime * speed;
                        }
                    }

                    SpriteRenderer sr = nObject.AddComponent<SpriteRenderer>();
                    sr.sprite = newSprite;

                    pieceXShift = sr.bounds.extents.x * 2;
                    pieceYShift = sr.bounds.extents.y * 2;

                    nObject.transform.position = new Vector3((+i) * pieceXShift, (+j) * pieceYShift, 0);
                    nObject.transform.parent = gameObject.transform;
                }
            }

            GetComponent<SpriteRenderer>().enabled = false;
            Destroy(gameObject, 1.5f);


            /*for (int i = 0; i < 8; i++) 
            {
                for(int j = 0; j < 8; j++)
                {


                    Sprite newSprite = Sprite.Create(source, new Rect(i*128, j*128, 128, 128), new Vector2(0.5f, 0.5f));

                    GameObject n = new GameObject();
                    n.name = "Game Object Piece";

                    SpriteRenderer sr = n.AddComponent<SpriteRenderer>();
                    sr.sprite = newSprite;

                    n.transform.position = new Vector3(i*2, j*2 , 0);
                    n.transform.parent = gameObject.transform;

                    Destroy(n, 1000);
                }
            }*/
        }

        // Update is called once per frame
        void Update()
        {
        }

        public void Divide()
        {

        }

    }

}
