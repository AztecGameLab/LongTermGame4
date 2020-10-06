using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public CharacterController charController;
    public float moveSpeed = 10f;
    public float gravity = -9.81f;

    Vector3 velocity;
    private bool isGrounded;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        charController.Move(move * moveSpeed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;

        charController.Move(velocity * Time.deltaTime);
    }
}
