using Assets.scripts.Actor;
using Assets.scripts.Base;
using UnityEngine;

namespace Assets.scripts.Mono.ObjectData
{
	public class EnemyData : AbstractData
	{
		private Monster owner;

		public int monsterId;

		public int distanceToFollowLeader = 8;
		public bool isAggressive = false;
		public int aggressionRange = 5;

		// Use this for initialization
		public new void Start()
		{
			base.Start();

			//owner = GameSystem.Instance.RegisterNewMonster(this, "Monster", monsterId);
			//Debug.Log("Registering new data for monster ");
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

				DisableMe();

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

		// Update is called once per frame
		public override void Update()
		{
			base.Update();
		}

		public override void OnTriggerEnter2D(Collider2D obj)
		{
			/*// kolize s objectem ktery implementuje IDamagable poskodi tohoto hrace
			GameObject incoming = obj.gameObject;
			IDamagable id;
		
			foreach (IDamagable dmg in incoming.GetComponents<IDamagable>())
			{
				id = dmg;
				owner.ReceiveDamage(id.GetDamage());
			}*/
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
