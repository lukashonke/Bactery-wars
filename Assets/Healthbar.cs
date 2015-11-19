using UnityEngine;
using System.Collections;

public class Healthbar : MonoBehaviour
{
	public GameObject dmg;
	public int hp;
	public int maxHp;

	public int percent;
	public int currentPercent;

	// Update is called once per frame
	void Update () 
	{
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
