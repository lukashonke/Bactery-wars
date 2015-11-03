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

		// clicked position to rotate the sprite to
		private Vector3 rotationPosition;

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

		// Update is called once per frame
		public void FixedUpdate()
		{
			// fire TODO delete
			if (Input.GetKeyDown("space"))
			{
				Instantiate(projectObject, body.transform.position, body.transform.rotation);
			}

			if (Input.GetKeyDown(KeyCode.Escape))
			{
				data.AbortSkills();
			}

			HandleSkillControls();

			if (!ui.MouseOverUI)
			{
				// change target position according to mouse when clicked
				if (Input.GetMouseButton(0) && Vector3.Distance(body.transform.position, Input.mousePosition) > 10)
				{
					targetPositionWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
					targetPositionWorld.z = body.transform.position.z;
					rotationPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z);

					if (currMouseClicker != null)
					{
						Destroy(currMouseClicker);
					}

					currMouseClicker = Instantiate(mouseClicker, targetPositionWorld, Quaternion.identity) as GameObject;
				}
			}

			// move to mouse
			if (Vector3.Distance(body.transform.position, targetPositionWorld) > 1)
			{
				Vector3 mousePos = rotationPosition;
				mousePos.z = 10; //The distance between the camera and object

				Vector3 objectPos = Camera.main.WorldToScreenPoint(body.transform.position);
				mousePos.x = mousePos.x - objectPos.x;
				mousePos.y = mousePos.y - objectPos.y;
				float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
				float currentAngle = body.transform.rotation.eulerAngles.z;

				bool move = true;

				// TODO wtf ! predelat vypocet stupnu na otoceni a prepsat bez pouziti podminky
				float diff = 360 - (Mathf.Abs(angle - 90)) - currentAngle;
				if(angle >= 90 && angle <= 180)
					diff = (angle - 90) - currentAngle;

				if (Mathf.Abs(diff) > 5 && !data.canMoveWhenNotRotated)
				{
					move = false;
				}

				if (move)
				{
					anim.SetFloat("MOVE_SPEED", 1);
					body.transform.position = Vector3.MoveTowards(body.transform.position, targetPositionWorld, Time.deltaTime * data.moveSpeed);
				}

				if (data.rotateSpeed > 20) // instantni rotace
				{
					body.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
				}
				else
				{
					//TODO unsmooth this
					body.transform.rotation = Quaternion.Slerp(body.transform.rotation, Quaternion.Euler(new Vector3(0, 0, angle - 90)), Time.deltaTime * data.rotateSpeed);
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
		}
    }

}