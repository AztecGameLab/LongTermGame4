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

    //stores velocities of objects before they physical hit the wall
    Dictionary<int, Vector3> Velocities = new Dictionary<int, Vector3>();

    //cashes velocity when an object enters the feild
    private void OnTriggerEnter(Collider other)
    {
        //stores the velocity in the velocity dictonary
        if (other.gameObject.tag != "Player")
        {
            Velocities.Add(other.gameObject.GetInstanceID(), other.attachedRigidbody.velocity);
        }
    }

    //removes velocities from the dictonary
    private void OnTriggerExit(Collider other)
    {
        //removes the velocity from the leaving object from the dictonary
        if (other.gameObject.tag != "Player")
        {
            Velocities.Remove(other.gameObject.GetInstanceID());
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag != "Player")
        {
            //defines varibles to edit collision object
            var rigidBody = other.rigidbody;
            var velocity = rigidBody.velocity;

            //Uses Vector3.Reflect() with the normal and the stored velocity to get the reflected velocity, multiplys by the energy multiplyer
            var result = Vector3.Reflect(Velocities[other.gameObject.GetInstanceID()], other.GetContact(0).normal) * EnergyMultiplyer;

            var point = other.GetContact(0).point;

            Debug.DrawLine(point, point + other.GetContact(0).normal, Color.red);
            Debug.DrawLine(point, point + -Velocities[other.gameObject.GetInstanceID()], Color.green);
            Debug.DrawLine(point, point + result, Color.blue);

            Debug.Break();

            //adds a boost amount to the X, Y, and Z
            result.x += BoostX;
            result.y += BoostY;
            result.z += BoostZ;

            //points arrow at reflection
            rigidBody.transform.forward = result;

            rigidBody.transform.Translate(Vector3.Cross(other.GetContact(0).point - rigidBody.position, other.GetContact(0).normal).normalized, rigidBody.transform);

            //sets the calculated velocity to the rigidbody
            rigidBody.velocity = result;
        }
    }
}