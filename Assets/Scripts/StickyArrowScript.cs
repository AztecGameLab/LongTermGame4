using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyArrowScript : MonoBehaviour
{
    Rigidbody arrowRB;
    Quaternion arrowRotation;

    private void Start()
    {
        arrowRB = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        ArrowReflector script = collision.gameObject.GetComponent<ArrowReflector>();

        //To keep players from stacking arrows oddly, and to test if the arrow should bounce, should not stick to player
        if (!collision.gameObject.CompareTag("arrow") && script == null &&  !collision.gameObject.CompareTag("Player"))
        {
            //changing collision detection mode to avoid warning from unity
            arrowRB.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            arrowRB.isKinematic = true;
            //Make sure the arrow is pointing in the right dirrection using last known rotation before collision.
            gameObject.transform.rotation = arrowRotation;
            //Object sticks to where it first made contact, sinks in just enough to be embedded. 
            gameObject.transform.position = collision.GetContact(0).point + transform.forward * -.4f;
            //Checks if object is a movable object, will set as parrent as to move with object. 
            if (collision.rigidbody != null)
            {
                //Still a bug here, in that the arrow will rotate after having it's parent set. Still researching ways around this
                //This only works with objects that have a scale value of (1,1,1)
                gameObject.transform.parent = collision.gameObject.transform;
                //To avoid objects being trigured repeatedly by the arrow you can use the following
                //If you want the arrows to still interact physically with the world.
                Destroy(arrowRB);
                //could use the following if you don't mind arrows phasing through solid objects
                //gameObject.GetComponent<Collider>().enabled = false;
            }
        }
        
    }
    private void LateUpdate()
    {
        //grab the last rotation before collision's rotation
        arrowRotation = gameObject.transform.rotation;
    }
}
