using UnityEngine;

namespace Assets.scripts.Mono
{
    public class CameraMovement : MonoBehaviour
    {
        public float scrollSpeed = 0.3f;

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

                transform.position += cameraPos * scrollSpeed;
            }
        }
    }

}
