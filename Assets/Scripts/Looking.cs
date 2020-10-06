using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Looking : MonoBehaviour
{

    public float mouseSense = 100f;
    public Transform playerBody;
    float rotateUpDown = 0f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }


    // Update is called once per frame
    void Update()
    {

        float xMouse = Input.GetAxis("Mouse X") * mouseSense * Time.deltaTime;
        float yMouse = Input.GetAxis("Mouse Y") * mouseSense * Time.deltaTime;

        playerBody.Rotate(Vector3.up * xMouse);

        rotateUpDown -= yMouse;

        rotateUpDown = Mathf.Clamp(rotateUpDown, -90f, 90f);


        transform.localRotation * Quaternion.Euler(rotateUpDown, 0f, 0f);

    }
}
