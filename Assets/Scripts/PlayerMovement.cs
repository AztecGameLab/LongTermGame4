using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public float moveSpeed = 5f;
    public float rotateVerticleSpeed = 5f;
    public float rotateHorizontalSpeed = 5f;
    public float jumpSpeed = 8f;
    public Rigidbody body;
    public Camera camera;

    private bool ground = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Looking
        Vector3 cameraAngle = camera.transform.rotation.eulerAngles;
        float rotateVertical = Input.GetAxis("Mouse Y") * rotateVerticleSpeed;
        float angleRotateX = cameraAngle.x - rotateVertical;
        if (angleRotateX > 180)
        {
            angleRotateX = angleRotateX - 360;
        }
        angleRotateX = Mathf.Clamp(angleRotateX, -60, 60);
        rotateVertical = cameraAngle.x - angleRotateX;
        camera.transform.Rotate(-rotateVertical, 0f, 0f);

        float rotateHorizontal = Input.GetAxis("Mouse X") * rotateHorizontalSpeed;
        transform.Rotate(0f, rotateHorizontal, 0f);

        //Movement
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(x, 0f, z) * 10;
        transform.Translate(movement * Time.deltaTime);

        //Jump
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (ground == true)
            {
                body.AddForce(Vector3.up * jumpSpeed * 30);
                ground = false;
            }

        }
        Debug.Log(ground);
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            ground = true;
        }
    }

    void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            ground = false;
        }
    }

}
