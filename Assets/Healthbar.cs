using UnityEngine;
using System.Collections;

public class Healthbar : MonoBehaviour
{
	public GameObject dmg;
	public int hp;
	public int maxHp;

	public int percent;
	public int currentPercent;

	public float distance;

	private GameObject center;

	void Start()
	{
		foreach (Transform child in transform.parent.transform)
		{
			if (child.gameObject.name.Equals("Healthbar Center"))
			{
				center = child.gameObject;
				break;
			}
		}
	}

	// Update is called once per frame
	void Update ()
	{
		if (center != null)
		{
			Vector3 pos = center.transform.position + new Vector3(0, distance, 0);
			transform.position = pos;
			transform.rotation = Quaternion.identity;
		}

		if (hp == 0 && maxHp == 0)
			return;

		percent = (int) (hp/(float) maxHp*100);
		percent = 100 - percent;

		if (currentPercent != percent)
		{
			dmg.transform.localScale = new Vector3(0.0341f*percent, 0.68f, 0);
			currentPercent = percent;
		}
	}
}
