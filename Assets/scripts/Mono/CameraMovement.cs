using UnityEngine;

namespace Assets.scripts.Mono
{
	/// <summary>
	/// Stara se o posouvani kamery pokud hrac drzi prave tlacitko
	/// </summary>
	public class CameraMovement : MonoBehaviour
	{
		public float scrollSpeed = 0.3f;

		public float edgeScrollSpeed = 10f;
		public int scrollArea = 25;
		public int followSpeed = 2;

		public GameObject objectToFollow;

		// Use this for initialization
		public void Start()
		{

		}

		// Update is called once per frame
		public void Update()
        {
            // pravy tlacitko stisknuty
	        if (Input.GetMouseButton(1))
	        {
		        Vector3 cameraPos;

		        float mouseX;
		        float mouseY;

		        mouseX = Input.GetAxis("Mouse X");
		        mouseY = Input.GetAxis("Mouse Y");

		        cameraPos = new Vector3(-mouseX, -mouseY, 0);

		        transform.position += cameraPos*scrollSpeed;
	        }
	        else
	        {
				if (objectToFollow != null)
				{
					Vector3 pos = objectToFollow.transform.position;
					pos.z = -10;

					transform.position = Vector3.Lerp(transform.position, pos, followSpeed * Time.deltaTime);
				}

				var mPosX = Input.mousePosition.x;
				var mPosY = Input.mousePosition.y;
				float screenX = Screen.width;
				float screenY = Screen.height;

				if (mPosX < 0 || mPosX > screenX || mPosY < 0 || mPosY > screenY)
					return;

				// Do camera movement by mouse position
		        if (mPosX < scrollArea)
		        {
			        transform.Translate(Vector3.right*-edgeScrollSpeed*Time.deltaTime);
		        }
		        if (mPosX >= Screen.width - scrollArea)
		        {
			        transform.Translate(Vector3.right * edgeScrollSpeed * Time.deltaTime);
		        }
		        if (mPosY < scrollArea)
		        {
			        transform.Translate(Vector3.up * -edgeScrollSpeed * Time.deltaTime);
		        }
		        if (mPosY >= Screen.height - scrollArea)
		        {
					transform.Translate(Vector3.up * edgeScrollSpeed * Time.deltaTime);
		        }
	        }
        }
	}

}
