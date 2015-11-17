using Assets.scripts.Actor;
using Assets.scripts.Base;
using UnityEngine;

namespace Assets.scripts.Mono.ObjectData
{
	public class EnemyTestData : AbstractData, ICollidable
	{
		public int hp;

		// Use this for initialization
		public new void Start()
		{
			base.Start();

			hp = 1;
		}

		public override Character GetOwner()
		{
			throw new System.NotImplementedException();
		}

		// Update is called once per frame
		public override void Update()
		{

		}

		public void OnTriggerEnter2D(Collider2D obj)
		{
			GameObject incoming = obj.gameObject;
			IDamagable id;
		
			foreach (IDamagable dmg in incoming.GetComponents<IDamagable>())
			{
				id = dmg;
				ReceiveDamage(id.GetDamage());
			}
		}

		public void OnTriggerExit2D(Collider2D obj)
		{
		}

		public void OnTriggerStay2D(Collider2D obj)
		{
		}

		public void OnCollisionEnter2D(Collision2D coll)
		{
		}

		private void ReceiveDamage(int damage)
		{
			hp -= damage;
			if (hp < 0)
				hp = 0;

			if (hp == 0)
				DoDie();
		}

		private void DoDie()
		{
			ParticleSystem ps = GetComponentInChildren<ParticleSystem>();
			ps.Play();

			gameObject.GetComponentInChildren<SpriteRenderer>().enabled = false;
			Destroy(gameObject, 1f);
		}

		public void OnCollisionExit2D(Collision2D coll)
		{
		}

		public void OnCollisionStay2D(Collision2D coll)
		{
		}
	}
}
