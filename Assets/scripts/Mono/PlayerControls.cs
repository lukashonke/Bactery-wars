using UnityEngine;

namespace Assets.scripts.Mono
{
	public class PlayerControls : MonoBehaviour
	{
		public GameObject body;

		public PlayerData data;
		public PlayerUI ui;
		private Rigidbody2D rb;

		// attached object to display moving to pos
		public GameObject mouseClicker;
		private GameObject currMouseClicker;

		// projectile
		public GameObject projectObject;

		// lightning
		public GameObject spotlight;

		// animators
		private Animator anim;

		// Use this for initialization
		public void Start()
		{
			body = GameObject.Find("Body");
			rb = body.GetComponent<Rigidbody2D>();
			anim = body.GetComponent<Animator>();
			data = GetComponent<PlayerData>();
			ui = GetComponent<PlayerUI>();
        }

		private void HandleSkillControls()
		{
			if (Input.GetKeyDown(KeyCode.Alpha1))
			{
				data.LaunchSkill(1);
			}

			if (Input.GetKeyDown(KeyCode.Alpha2))
			{
				data.LaunchSkill(2);
			}

			if (Input.GetKeyDown(KeyCode.Alpha3))
			{
				data.LaunchSkill(3);
			}

			if (Input.GetKeyDown(KeyCode.Alpha4))
			{
				data.LaunchSkill(4);
			}

			if (Input.GetKeyDown(KeyCode.Alpha5))
			{
				data.LaunchSkill(5);
			}
        }

		public void Update()
		{
			// fire TODO delete
			if (Input.GetKeyDown("space"))
			{
				Instantiate(projectObject, body.transform.position, body.transform.rotation);
			}

			if (Input.GetKeyDown(KeyCode.Escape))
			{
				data.BreakCasting();
			}

			HandleSkillControls();

			if (!ui.MouseOverUI)
			{
				// change target position according to mouse when clicked
				if (Input.GetMouseButton(0) && Vector3.Distance(body.transform.position, Input.mousePosition) > 1)
				{
					Vector3 newTarget = Camera.main.ScreenToWorldPoint(Input.mousePosition);
					newTarget.z = body.transform.position.z;
					data.SetMovementTarget(newTarget);

					if (currMouseClicker != null)
						Destroy(currMouseClicker);

					currMouseClicker = Instantiate(mouseClicker, data.GetMovementTarget(), Quaternion.identity) as GameObject;
					data.HasTargetToMoveTo = true;
                }
			}

			if (data.HasTargetToMoveTo == false)
			{
				if (currMouseClicker != null)
					Destroy(currMouseClicker);
			}

			Debug.DrawRay(body.transform.position, data.GetForwardVector()*10, Color.red);
		}
    }

}