using UnityEngine;
using System.Collections;
using Assets.scripts;
using Assets.scripts.Mono;
using Assets.scripts.Skills;

public class Healthbar : MonoBehaviour
{
	private AbstractData ownerData;

	public GameObject dmg;
	public int hp;
	public int maxHp;

	public int percent;
	public int currentPercent;
	public int currentPercentCooldown;

	public float distance = 1.5f;

	private GameObject center;

	private GameObject cooldownBar;
	private GameObject cooldownMarker;

	void Start()
	{
		ownerData = transform.parent.gameObject.GetData();

		foreach (Transform child in transform.parent.transform)
		{
			if (child.gameObject.name.Equals("Healthbar Center"))
			{
				center = child.gameObject;
				break;
			}
		}

		foreach (Transform child in transform)
		{
			if (child.gameObject.name.Equals("AutoattackCooldown"))
			{
				cooldownBar = child.gameObject;

				foreach (Transform t in cooldownBar.transform)
				{
					if (t.gameObject.name.Equals("cooldown"))
					{
						cooldownMarker = t.gameObject;
						break;
					}
				}
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

		if (cooldownBar != null)
		{
			ActiveSkill sk = ownerData.GetOwner().MeleeSkill;

			float lastUse = sk.LastUsed;
			float reuse = sk.GetReuse();

			float passed = Time.time - lastUse;
			float ratio = passed / reuse;

			//Debug.Log(ratio);

			percent = (int)(ratio * 100);

			if (percent > 100)
				percent = 100;

			if (currentPercentCooldown != percent)
			{
				cooldownMarker.transform.localScale = new Vector3(0.0341f * percent, 0.68f, 0);
				currentPercentCooldown = percent;
			}
		}
	}
}
