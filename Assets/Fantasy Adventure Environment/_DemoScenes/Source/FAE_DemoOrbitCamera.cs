using UnityEngine;
using System.Collections;

namespace FAE
{
    public class FAE_DemoOrbitCamera : MonoBehaviour
    {

        public Transform target;
        public Transform cam;
        public Vector3 offset = Vector3.zero;
        public float rotationSpeed = 7f;

        private float cameraRotSide;
        private float cameraRotUp;
        private float cameraRotSideCur;
        private float cameraRotUpCur;
        private float distance;

        float horizontalAxis;
        float verticalAxis;

        bool m_inputCaptured;

        void Start()
        {
            cameraRotSide = transform.eulerAngles.y;
            cameraRotSideCur = transform.eulerAngles.y;
            cameraRotUp = transform.eulerAngles.x;
            cameraRotUpCur = transform.eulerAngles.x;
            distance = -cam.localPosition.z;
        }

        void CaptureInput()
        {
            Cursor.lockState = CursorLockMode.Locked;

            Cursor.visible = false;
            m_inputCaptured = true;


        }

        void ReleaseInput()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            m_inputCaptured = false;
        }

        void OnApplicationFocus(bool focus)
        {
            if (m_inputCaptured && !focus)
                ReleaseInput();
        }

        void Update()
        {
            if (!m_inputCaptured)
            {
                if (Input.GetMouseButton(1))
                    CaptureInput();
            }

            //if (!m_inputCaptured)
               // return;

            if (m_inputCaptured)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                    ReleaseInput();
                else if (Input.GetMouseButtonUp(1))
                    ReleaseInput();
            }

            cameraRotSide += Input.GetAxis("Mouse X") * rotationSpeed;
            cameraRotUp -= Input.GetAxis("Mouse Y") * rotationSpeed;

            cameraRotSideCur = Mathf.LerpAngle(cameraRotSideCur, cameraRotSide, Time.deltaTime * 5);
            cameraRotUpCur = Mathf.Lerp(cameraRotUpCur, cameraRotUp, Time.deltaTime * 5);

            distance *= (1 - 1 * Input.GetAxis("Mouse ScrollWheel"));

            transform.position = new Vector3(target.position.x, target.position.y + 1.2f, target.position.z);
            transform.rotation = Quaternion.Euler(cameraRotUpCur, cameraRotSideCur, 0);

            float dist = Mathf.Lerp(-cam.transform.localPosition.z, distance, Time.deltaTime * 20);
            cam.localPosition = -Vector3.forward * dist;
        }
    }
}