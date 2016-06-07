using System;
using Assets.scripts.Actor;
using Assets.scripts.Actor.MonsterClasses.Base;
using Assets.scripts.Base;
using Assets.scripts.Mono.MapGenerator;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.scripts.Mono.ObjectData
{
	/// <summary>
	/// rozsiruje funkcionalitu AbstractData pro monstra a jine nepratele hrace
	/// </summary>
	public class EnemyData : AbstractData
	{
		private Monster owner;

		public int monsterId;
		public string monsterTypeName = null;

		public int distanceToFollowLeader = 8;

		// Use this for initialization
		public new void Start()
		{
			base.Start();

			if (monsterTypeName == null)
			{
				try
				{
					monsterTypeName = Enum.GetName(typeof(MonsterId), "" + monsterId);
				}
				catch (Exception)
				{
					Debug.LogError("cant get name for monsterId " + monsterId);
				}
			}
            Debug.Log("Neco:" + monsterTypeName);

			//owner = GameSystem.Instance.RegisterNewMonster(this, "Monster", monsterId);
			//Debug.Log("Registering new data for monster ");
		}

		public new void Awake()
		{
			base.Awake();
		}

		public override Character GetOwner()
		{
			return owner;
		}

		public override void SetOwner(Character ch)
		{
			owner = (Monster) ch;
		}

		public override void SetIsDead(bool isDead)
		{
			if (isDead)
			{
				GameObject ch = GetChildByName("Die Effect");

				if (ch != null)
				{
					ParticleSystem ps = ch.GetComponent<ParticleSystem>();
					if (ps != null)
						ps.Play();
				}

				DisableObjectData();

				DisableChildObjects();

				Destroy(gameObject, 1f);

				DropBlood(50);
			}
		}

		public void DropBlood(int ammount)
		{
			ParticleSystem ps;
            GameObject blood = LoadResource("misc", "Blood", "Blood_red");

			GameObject bloodObject = Instantiate(blood, body.transform.position, Quaternion.identity) as GameObject;

			if (bloodObject != null)
			{
				ps = bloodObject.GetComponent<ParticleSystem>();
				ps.maxParticles = Random.Range(10, ammount);

				Destroy(bloodObject, 20f);
			}
		}

		public void UpdateCollider(string nameTurret)
		{
			Collider2D coll = null;

			coll = GetComponent<CircleCollider2D>();
			if (coll != null)
			{
				bool trigger = coll.isTrigger;

				Destroy(coll);
				CircleCollider2D newColl = GetBody().AddComponent<CircleCollider2D>();

				newColl.isTrigger = trigger;

                if (nameTurret == "Attack turret")
                {
                    newColl.isTrigger = true;
                    Debug.Log("nastaveno");

                }

			}
			else
			{
				coll = GetComponent<BoxCollider2D>();

				if (coll != null)
				{
					bool trigger = coll.isTrigger;

					Destroy(coll);
					BoxCollider2D newColl = GetBody().AddComponent<BoxCollider2D>();

					newColl.isTrigger = trigger;
				}
			}

		 
        
        }

		public void SetSprite(Sprite sprite, float scale)
		{
			SpriteRenderer sr = GetComponent<SpriteRenderer>();
			sr.sprite = sprite;

			Vector2 sprite_size = GetComponent<SpriteRenderer>().sprite.rect.size;
			Vector2 local_sprite_size = sprite_size / GetComponent<SpriteRenderer>().sprite.pixelsPerUnit; //TODO use this?

			float newScale = Mathf.Max(sprite_size.x, sprite_size.y) / 200f;

			if (scale > 0)
			{
				this.transform.localScale = new Vector3(1 / newScale * scale, 1 / newScale * scale, 1);
			}
			else
			{
				this.transform.localScale = new Vector3(1 / newScale, 1 / newScale, 1);
			}
		}

		public void SetSprite(string name, float scale)
		{
			Sprite sprite = Resources.Load<Sprite>(name);
			if (sprite == null)
			{
				Debug.LogError("neexistuje soubor  " + name);
				return;
			}

			SpriteRenderer sr = GetComponent<SpriteRenderer>();
			sr.sprite = sprite;

			Vector2 sprite_size = GetComponent<SpriteRenderer>().sprite.rect.size;
			Vector2 local_sprite_size = sprite_size / GetComponent<SpriteRenderer>().sprite.pixelsPerUnit; //TODO use this?

			float newScale = Mathf.Max(sprite_size.x, sprite_size.y)/200f;

			if (scale > 0)
			{
				this.transform.localScale = new Vector3(1/newScale*scale, 1/newScale*scale, 1);
			}
			else
			{
				this.transform.localScale = new Vector3(1/newScale, 1/newScale, 1);
			}
		}

		public void SetMass(float mass)
		{
			if (mass > 0)
			{
				Rigidbody2D rb = GetComponent<Rigidbody2D>();
				rb.mass = mass;
			}
		}

		// Update is called once per frame
		public override void Update()
		{
			base.Update();
		}

		public override void OnTriggerEnter2D(Collider2D obj)
		{
			base.OnTriggerEnter2D(obj);
		}

		public override void OnTriggerExit2D(Collider2D obj)
		{
		}

		public override void OnTriggerStay2D(Collider2D obj)
		{
		}

		public override void OnCollisionEnter2D(Collision2D coll)
		{
			base.OnCollisionEnter2D(coll);
		}

		public override void OnCollisionExit2D(Collision2D coll)
		{
		}

		public override void OnCollisionStay2D(Collision2D coll)
		{
		}
	}
}
