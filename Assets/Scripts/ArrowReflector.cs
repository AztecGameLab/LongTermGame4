using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowReflector : MonoBehaviour
{
    /*
     * Simple script that 'refelcts' an object after it collides with the surface.
     * Controllable through an EnergyMultiplyer to change the force, and a BoostHeight to add a value to the Y velocity
     */

    //a multiplyer that affects the resulting velocity
    //  negitive values are more realistic while positive add energy
    [SerializeField]
    private float EnergyMultiplyer = -.9f;

    //a boost to hight to correct for drop when reaching the target, adds directly to the Y of the velocity
    [SerializeField]
    private float BoostHeight = 0f;

    private void OnCollisionEnter(Collision collision)
    {
        //defines varibles to edit collision object
        var rigidBody = collision.rigidbody;
        var velocity = rigidBody.velocity;

        //adds the velocity and normalized venocity to get a good approxamation of the reflection angle,
        //then multiplies by the velocites magnatude and energy multipyer to get resulting force
        var result = (rigidBody.velocity.normalized - collision.GetContact(0).normal) * velocity.magnitude * EnergyMultiplyer;

        //adds a boost amount to the Y for ease of shot lineup
        if (BoostHeight != 0)
        {
            result.y += BoostHeight;
        }

        //sets the calculated velocity to the rigidbody
        rigidBody.velocity = result;
    }
}
//Author: James Maron