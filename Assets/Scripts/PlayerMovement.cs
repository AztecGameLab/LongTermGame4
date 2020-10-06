using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public Rigidbody rb;
    public CharacterController charController;
    public float moveSpeed = 10f;
    public float gravity = -9.81f;
    public float jumpSpeed = 100f;

    private int touchingGround = 0;

    Vector3 velocity;
    


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        //Movement
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        charController.Move(move * moveSpeed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;

        charController.Move(velocity * Time.deltaTime);


        //Jump
        if (Input.GetKeyDown (KeyCode.Space) && touchingGround > 0)
        {
            rb.AddForce(Vector3.up * jumpSpeed);
        }

        Debug.Log(touchingGround);

    }
    
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            touchingGround = touchingGround + 1;
            Debug.Log("Touching ground");
        }
    }

    void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            touchingGround = touchingGround - 1;
            Debug.Log("Not touching ground");
        }
    }




}
