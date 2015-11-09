using UnityEngine;
using System.Collections;

public class RandomRotation : MonoBehaviour
{
	public float rotationOffset;
	public int rotationSpeed;

	private int state;
	private int targetAngle;

	// Use this for initialization
	void Start ()
	{
		state = 0;
	}
	
	// Update is called once per frame
	void Update ()
	{
		float current = transform.rotation.eulerAngles.z;

		if (state == 0)
		{
			float offset = Random.Range(-rotationOffset, rotationOffset);
			targetAngle = (int) (current + offset);

			if (targetAngle < 0)
				targetAngle = 0;

			if (targetAngle > 360)
				targetAngle %= 360;

			state = 1;
		}

		if (Mathf.Abs(current - targetAngle) < 1)
			state = 0;
		else
			Rotate();
	}

	private void Rotate()
	{
		transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(new Vector3(0, 0, targetAngle)), Time.deltaTime * rotationSpeed);
	}
}
