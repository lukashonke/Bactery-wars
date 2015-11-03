using UnityEngine;

namespace Assets.scripts.Mono
{
	public class MouseClicker : MonoBehaviour
	{
		bool smaller = true;
		int counter = 0;

		// Use this for initialization
		private void Start()
		{
			Destroy(gameObject, 5);
		}

		// Update is called once per frame
		public void Update()
		{
			if (smaller)
			{
				Vector3 sc = transform.localScale;

				sc.x = sc.x * 0.95f;
				sc.y = sc.y * 0.95f;

				transform.localScale = sc;

				counter--;

				if (counter <= 0)
				{
					smaller = false;
					counter = 10;
				}
			}
			else
			{
				Vector3 sc = transform.localScale;

				sc.x = sc.x * 1.05f;
				sc.y = sc.y * 1.05f;

				transform.localScale = sc;

				counter--;

				if (counter <= 0)
				{
					smaller = true;
					counter = 10;
				}
			}
		}
	}

}
