﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public float moveSpeed = 5f;
    public float gravity = 10f;
    public float jumpSpeed = 5f;

    private CharacterController charController;
    

    Vector3 velocity;
    


    // Start is called before the first frame update
    void Start()
    {
        charController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {

        //Movement
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        charController.Move(move * moveSpeed * Time.deltaTime);

        velocity.y -= gravity * Time.deltaTime;

        charController.Move(velocity * Time.deltaTime);


        //Jump
        
        if (charController.isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                velocity.y = jumpSpeed;
            }
            
        }
       
    }

}