using UnityEngine;
using System.Collections;
using Assets.scripts;
using Assets.scripts.Actor;
using Assets.scripts.Actor.MonsterClasses;
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

	//public bool showObjectName;

	public float distance = 1.5f;

	private GameObject center;

	private GameObject cooldownBar;
	private GameObject cooldownMarker;

	void Start()
	{
		ownerData = transform.parent.gameObject.GetData();

		labelStyle = null;

		//if (ownerData != null)
		//	showObjectName = ownerData.showObjectName;

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
						mat = cooldownMarker.GetComponent<SpriteRenderer>().material;
						break;
					}
				}
				break;
			}
		}
	}

	private Material mat;
	public GUIStyle labelStyle;

	void OnGUI()
	{
		if (ownerData == null || ownerData.showObjectName == false)
			return;

		if (center != null)
		{
			if (labelStyle == null)
			{
				labelStyle = new GUIStyle(GUI.skin.label);
				labelStyle.alignment = TextAnchor.UpperCenter;
				labelStyle.normal.textColor = Color.white;
				labelStyle.fontSize = 15;
			}

			Vector3 position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 1, 0));

			float x = position.x - 45;
			float y = Screen.height - position.y;

			string name = ownerData.GetOwner().Name;

			if (ownerData.GetOwner() is Monster)
			{
				MonsterTemplate template = ((Monster) ownerData.GetOwner()).Template;
				name = template.Name;
			}

			GUI.Label(new Rect(x, y, 90, 20), name, labelStyle);
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
			float reuse = sk.GetReuse(false);

			float passed = Time.time - lastUse;
			float ratio = passed / reuse;

			//Debug.Log(ratio);

			percent = (int)(ratio * 100);

			if (percent > 100)
				percent = 100;

			if (currentPercentCooldown != percent)
			{
				if (percent == 100)
				{
					mat.SetColor("_Color", new Color(227 / 255f, 176 / 255f, 57 / 255f, 187 / 255f));
				}
				else if(currentPercentCooldown == 100)
				{
					mat.SetColor("_Color", new Color(176 / 255f, 145 / 255f, 73 / 255f, 187 / 255f));
				}

				cooldownMarker.transform.localScale = new Vector3(0.0341f * percent, 0.68f, 0);
				currentPercentCooldown = percent;
			}
		}
	}
}
