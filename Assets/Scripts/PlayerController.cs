using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float rotateSpeedH = 100;
    public float rotateSpeedV = 100;
    public Transform playerObject;
    float rotateX;



    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Horizontal look:
        float rotateHorizontal = Input.GetAxis("Mouse X") * rotateSpeedH * Time.deltaTime;
        playerObject.Rotate(Vector3.up * rotateHorizontal);

        //Vertical look:
        float rotateVertical = Input.GetAxis("Mouse Y") * rotateSpeedV * Time.deltaTime;

        rotateX -= rotateVertical;

        transform.localRotation = Quaternion.Euler(rotateX, 0f, 0f);
        Camera.main.transform.Rotate(-rotateVertical, 0f, 0f);


    }
}
