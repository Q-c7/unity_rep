using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour
{
    public float sensitivityY = 2.0f, sensitivityX = 2.0f;
    private float yaw = 0.0f, pitch = 25.0f;
    private Vector3 transfer;
    public float speed = 2.0f;
    private readonly Vector3 cameraInitialPosition = new Vector3(8.7f, 10.8f, -14.7f);
    void Start()
    {
        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;
    }

    void FixedUpdate()
    {
        yaw += sensitivityX * Input.GetAxis("Mouse X");
        pitch -= sensitivityY * Input.GetAxis("Mouse Y");
        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
        // перемещение камеры
        transfer = transform.forward * Input.GetAxis("Vertical");
        transfer += transform.right * Input.GetAxis("Horizontal");
        transform.position += transfer * speed * Time.deltaTime;
        if(Input.GetButtonUp("Fire2"))
        {
            ResetCamera();
        }
    }

    private void ResetCamera()
    {
        yaw = 0;
        pitch = 25.0f;
        transform.position = cameraInitialPosition;
    }
}