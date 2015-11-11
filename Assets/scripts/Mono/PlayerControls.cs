using UnityEngine;

namespace Assets.scripts.Mono
{
	public class PlayerControls : MonoBehaviour
	{
		public GameObject body;

		public PlayerData data;
		public PlayerUI ui;

		// position in world cords to move to
		private Vector3 targetPositionWorld;
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

		public void FixedUpdate()
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
					targetPositionWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
					targetPositionWorld.z = body.transform.position.z; // do not update the z-axis

					if (currMouseClicker != null)
						Destroy(currMouseClicker);

					currMouseClicker = Instantiate(mouseClicker, targetPositionWorld, Quaternion.identity) as GameObject;
				}
			}

			// move to mouse
			if (Vector3.Distance(body.transform.position, targetPositionWorld) > 1)
			{
				Quaternion newRotation = Quaternion.LookRotation(body.transform.position - targetPositionWorld, Vector3.forward);
				newRotation.x = 0;
				newRotation.y = 0;

				float angle = Quaternion.Angle(body.transform.rotation, newRotation);

				bool move = true;
				bool rotate = true;

				if (angle-90 > 1 && !data.canMoveWhenNotRotated)
					move = false;

				if (!data.CanMove())
					move = false;

				if (!data.CanRotate())
					rotate = false;

				if (move)
				{
					anim.SetFloat("MOVE_SPEED", 1);
					body.transform.position = Vector3.MoveTowards(body.transform.position, targetPositionWorld, Time.deltaTime * data.moveSpeed);
				}

				if (rotate)
				{
					body.transform.rotation = Quaternion.Slerp(body.transform.rotation, newRotation, Time.deltaTime * data.rotateSpeed);

					float angleRad = (body.transform.rotation.eulerAngles.z + 90) * Mathf.Deg2Rad;
					data.UpdateHeading(new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad), 0));
				}
			}
			else
			{
				anim.SetFloat("MOVE_SPEED", 0);

				if (currMouseClicker != null)
				{
					Destroy(currMouseClicker);
				}
			}

			Debug.DrawRay(body.transform.position, data.GetForwardVector()*100, Color.red);
		}
    }

}