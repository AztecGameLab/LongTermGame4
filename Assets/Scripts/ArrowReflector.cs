using System;
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

    //a varibles for boosting the X, Y, and Z
    [SerializeField]
    private float BoostY = 0f;

    [SerializeField]
    private float BoostX = 0f;

    [SerializeField]
    private float BoostZ = 0f;

    //a feild for our inner box collider to calculate the arrows new position
    [SerializeField]
    private Collider InnerBoxCollider = null;

    //scale factor, around half of the arrows width
    private float TransformScaleFactor = .5f;

    //stores velocities of objects before they physical hit the wall
    Dictionary<int, Vector3> Velocities = new Dictionary<int, Vector3>();

    //cashes velocity when an object enters the feild
    private void OnTriggerEnter(Collider other)
    {
        //stores the velocity in the velocity dictonary
        if (other.gameObject.tag.ToUpper().Equals("ARROW"))
        {
            Velocities.Add(other.gameObject.GetInstanceID(), other.attachedRigidbody.velocity);
        }
    }

    //removes velocities from the dictonary
    private void OnTriggerExit(Collider other)
    {
        //removes the velocity from the leaving object from the dictonary
        if (other.gameObject.tag.ToUpper().Equals("ARROW"))
        {
            Velocities.Remove(other.gameObject.GetInstanceID());
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag.ToUpper().Equals("ARROW"))
        {
            //defines varibles to edit collision object
            var rigidBody = other.rigidbody;
            var velocity = rigidBody.velocity;
            var normal = other.GetContact(0).normal;
            var point = other.GetContact(0).point;

            //Uses Vector3.Reflect() with the normal and the stored velocity to get the reflected velocity, multiplys by the energy multiplyer
            var result = Vector3.Reflect(Velocities[other.gameObject.GetInstanceID()], normal) * EnergyMultiplyer;

            //adds a boost amount to the X, Y, and Z
            result.x += BoostX;
            result.y += BoostY;
            result.z += BoostZ;

            //points arrow at reflection
            rigidBody.transform.forward = result;

            //transforms the arrow to make reflection folow line of reflection
            rigidBody.transform.position = InnerBoxCollider.ClosestPointOnBounds(point) + result.normalized * TransformScaleFactor;

            //sets the calculated velocity to the rigidbody
            rigidBody.velocity = result;
        }
    }
}