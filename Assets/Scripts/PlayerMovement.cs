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

    float inputX;
    float inputZ;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        body.velocity = new Vector3(0, body.velocity.y, 0);//This is kains very dumb line but it works so shhhhh

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

        //Movement Input
        inputX = Input.GetAxis("Horizontal");
        inputZ = Input.GetAxis("Vertical");


        //Jump
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (ground)
            {
                body.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
            }

        }
        //Debug.Log(ground);
    }

    private void FixedUpdate()
    {
        //Movement
        body.position += (transform.forward * inputZ + transform.right * inputX) * moveSpeed * Time.deltaTime;

        //ground check
        RaycastHit hit;
        ground = Physics.SphereCast(transform.position, 0.35f, Vector3.down, out hit, 0.1f);
    }

    // void OnCollisionEnter(Collision other)
    // {
    //     if (other.gameObject.CompareTag("Ground"))
    //     {
    //         ground = true;
    //     }
    // }

    // void OnCollisionExit(Collision other)
    // {
    //     if (other.gameObject.CompareTag("Ground"))
    //     {
    //         ground = false;
    //     }
    // }

}
