using Assets.scripts.Mono.ObjectData;
using UnityEngine;
using UnityEngine.Networking;

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
			body = gameObject;
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

			if (Input.GetKeyDown(KeyCode.Alpha6))
			{
				data.LaunchSkill(6);
			}

			if (Input.GetKeyDown(KeyCode.Alpha7))
			{
				data.LaunchSkill(7);
			}

			if (Input.GetKeyDown(KeyCode.Alpha8))
			{
				data.LaunchSkill(8);
			}

			if (Input.GetKeyDown(KeyCode.Alpha9))
			{
				data.LaunchSkill(9);
			}

			if (Input.GetKeyDown(KeyCode.Alpha0))
			{
				data.LaunchSkill(10);
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
				// if targetting active, highlight target objects
				if (data.TargettingActive)
				{
					Vector3 temp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
					RaycastHit2D[] hits=  Physics2D.RaycastAll(new Vector2(temp.x, temp.y), Vector2.zero, 0f);

					int layer;
					Rigidbody2D rb;

					bool target = false;

					foreach (RaycastHit2D hit in hits)
					{
						layer = hit.transform.gameObject.layer;
						// not target projectiles, environment and background
						if (layer == 11 || layer == 8 || layer == 9)
							continue;

						rb = hit.transform.gameObject.GetComponent<Rigidbody2D>();

						if (rb != null)
						{
							data.HoverTarget = hit.transform.gameObject;
							target = true;
                            break;
						}
					}

					if (!target)
					{
						data.HoverTarget = null;
					}
				}

				if (data.ActiveConfirmationSkill != null)
				{
					if (Input.GetMouseButtonDown(0))
					{
						data.ConfirmSkillLaunch();
					}

					if (Input.GetMouseButtonDown(1))
					{
						data.ActiveConfirmationSkill.AbortCast();
					}

					// pro tento snimek vypne dalsi Input, aby nedoslo k pohybu za targetem skillu
					Input.ResetInputAxes();
				}
				else
				{
					// change target position according to mouse when clicked
					if (Input.GetMouseButton(0))
					{
						Vector3 newTarget = Camera.main.ScreenToWorldPoint(Input.mousePosition);
						newTarget.z = body.transform.position.z;

						// momentalne nepotrebne
						//if (Vector3.Distance(body.transform.position, newTarget) > 2)
						//{
						data.SetPlayersMoveToTarget(newTarget);

						if (currMouseClicker != null)
							Destroy(currMouseClicker);

						currMouseClicker = Instantiate(mouseClicker, data.GetMovementTarget(), Quaternion.identity) as GameObject;
						data.HasTargetToMoveTo = true;
						//}
					}
				}
			}

			if (!data.HasTargetToMoveTo)
			{
				if (currMouseClicker != null)
					Destroy(currMouseClicker);
			}

			Debug.DrawRay(body.transform.position, data.GetForwardVector()*10, Color.red);
		}
    }

}