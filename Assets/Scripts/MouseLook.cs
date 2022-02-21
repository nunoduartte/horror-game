using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    float xRotation = 0;
    public Transform playerBody;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * 100f * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * 100f * Time.deltaTime;

        this.xRotation -= mouseY;
        this.xRotation = Mathf.Clamp(this.xRotation, -90, 90);

        this.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        this.playerBody.Rotate(Vector3.up * mouseX);
    }
}
