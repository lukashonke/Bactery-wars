using Assets.scripts.Actor;
using Assets.scripts.Base;
using UnityEngine;

namespace Assets.scripts.Mono.ObjectData
{
	public class EnemyData : AbstractData
	{
		private Monster owner;

		// Use this for initialization
		public new void Start()
		{
			base.Start();

			owner = GameSystem.Instance.RegisterNewMonster(this, "Monster");

			Debug.Log("Registering new data for monster ");
		}

		public override Character GetOwner()
		{
			return owner;
		}

		public override void SetIsDead(bool isDead)
		{
			if (isDead)
			{
				ParticleSystem ps = GetComponentInChildren<ParticleSystem>();
				ps.Play();

				body.GetComponent<SpriteRenderer>().enabled = false;
				body.GetComponent<Collider2D>().enabled = false;

				if (healthBar != null)
					healthBar.gameObject.SetActive(false);

				Destroy(gameObject, 1f);

				GameObject blood = LoadResource("misc", "Blood", "Blood_red");

				ps = blood.GetComponent<ParticleSystem>();
				ps.maxParticles = Random.Range(10, 50);

				GameObject bloodObject = Instantiate(blood, body.transform.position, Quaternion.identity) as GameObject;
				Destroy(bloodObject, 10f);
			}
		}

		// Update is called once per frame
		public override void Update()
		{
			base.Update();
		}

		public override void OnTriggerEnter2D(Collider2D obj)
		{
			// kolize s objectem ktery implementuje IDamagable poskodi tohoto hrace
			GameObject incoming = obj.gameObject;
			IDamagable id;
		
			foreach (IDamagable dmg in incoming.GetComponents<IDamagable>())
			{
				id = dmg;
				owner.ReceiveDamage(id.GetDamage());
			}
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
