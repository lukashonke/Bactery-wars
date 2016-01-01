using UnityEngine;

namespace Assets.scripts.Mono
{
	/// <summary>
	/// Stara se o posouvani kamery pokud hrac drzi prave tlacitko
	/// </summary>
	public class CameraMovement : MonoBehaviour
	{
		public PlayerControls controller;

		public float scrollSpeed = 0.3f;

		public float edgeScrollSpeed = 10f;
		public int scrollArea = 25;
		public int followSpeed = 2;

		public float minZoom = 8;
		public float maxZoom = 20;

		public GameObject objectToFollow;

		public bool follow = false;
		public bool mobile = false;

		// Use this for initialization
		public void Start()
		{
			controller = GameObject.Find("Player").GetComponent<PlayerControls>();

#if UNITY_ANDROID
			follow = true;
			mobile = true;
#endif
		}

		// Update is called once per frame
		public void Update()
		{
            // pravy tlacitko stisknuty
			if (!mobile && Input.GetMouseButton(1))
	        {
		        Vector3 cameraPos;

		        float mouseX;
		        float mouseY;

		        mouseX = Input.GetAxis("Mouse X");
		        mouseY = Input.GetAxis("Mouse Y");

		        cameraPos = new Vector3(-mouseX, -mouseY, 0);

		        float zoomBoost = Mathf.Max(1f, Camera.main.orthographicSize/10);

				transform.position += cameraPos * scrollSpeed * zoomBoost;
	        }
	        else
	        {
		        if (mobile)
		        {
			        if (Input.touchCount == 1 && Input.GetTouch(0).tapCount == 3 && Input.GetTouch(0).phase.Equals(TouchPhase.Began))
			        {
				        float currentSize = Camera.main.orthographicSize;

				        if (currentSize < maxZoom)
					        Camera.main.orthographicSize = maxZoom;
				        else
					        Camera.main.orthographicSize = minZoom;
			        }

			        if (Input.touchCount == 2)
			        {
						//if(Input.GetTouch(0).phase.Equals(TouchPhase.Began))
						controller.currentTouchAction = PlayerControls.TOUCH_ZOOM;

				        // Store both touches.
				        Touch touchZero = Input.GetTouch(0);
				        Touch touchOne = Input.GetTouch(1);

				        // Find the position in the previous frame of each touch.
				        Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
				        Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

				        // Find the magnitude of the vector (the distance) between the touches in each frame.
				        float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
				        float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

				        // Find the difference in the distances between each frame.
				        float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

				        Camera.main.orthographicSize += deltaMagnitudeDiff*0.05f;

				        // Make sure the orthographic size never drops below zero.
						Camera.main.orthographicSize = Mathf.Max(Camera.main.orthographicSize, minZoom);
						Camera.main.orthographicSize = Mathf.Min(Camera.main.orthographicSize, maxZoom);
			        }
			        else
			        {
				        if (controller.currentTouchAction == PlayerControls.TOUCH_ZOOM)
					        controller.currentTouchAction = 0;
			        }
		        }

		        if (Input.GetAxis("Mouse ScrollWheel") > 0)
		        {
					if(Camera.main.orthographicSize <= maxZoom)
						Camera.main.orthographicSize += 1;
		        }
				if (Input.GetAxis("Mouse ScrollWheel") < 0)
				{
					if (Camera.main.orthographicSize >= minZoom)
						Camera.main.orthographicSize -= 1;
				}

				if (follow)
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
