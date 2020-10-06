using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Looking : MonoBehaviour
{

    public float mouseSenseH = 300f;
    public float mouseSenseV = 300f;
    public Transform playerBody;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }


    // Update is called once per frame
    void Update()
    {

        //look left and right
        float xMouse = Input.GetAxis("Mouse X") * mouseSenseH * Time.deltaTime;
        playerBody.Rotate(Vector3.up * xMouse);

        //look up and down
        float yMouse = Input.GetAxis("Mouse Y") * mouseSenseV * Time.deltaTime;
        Vector3 cameraAngle = transform.rotation.eulerAngles;
        float angleX = cameraAngle.x - yMouse;


        if (angleX > 180)
        {
            angleX = angleX - 360;
        }

        angleX = Mathf.Clamp(angleX, -80f, 80f);
        yMouse = cameraAngle.x - angleX;

        transform.Rotate(-yMouse, 0f, 0f);

    }
}
