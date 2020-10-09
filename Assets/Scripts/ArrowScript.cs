using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowScript : MonoBehaviour
{
    Rigidbody arrowRB;

    private void Start()
    {
        arrowRB = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        /*
         * Not sure what would be the most efficient could be done in one of multible ways
         * arrowRB.sleep(); but if arrow is hit by another arrow they both fall
         * 
         * OR
         *  
         * arrowRB.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ |
         *                      RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
         *  Like with the implementation bellow arrows stack with each other. 
         */
        arrowRB.isKinematic = true;
        arrowRB.velocity = Vector3.zero;
    }
}
