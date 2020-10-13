using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingArrow : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    void OnCollisionEnter(Collision collision)
    {
        var player = GameObject.Find("Player");
        Vector3 playerPosition = player.transform.position;
        Vector3 arrowPosition = this.transform.position;
        foreach (ContactPoint contact in collision.contacts)
        {
            Vector3 collisionPosition = collision.transform.position;
            if(collision.rigidbody != null)
            {
                Debug.Log(collision.rigidbody.mass);
                if (collision.rigidbody.mass >4)
                {
                    Debug.Log("You go to it");
                }
                else
                {
                    Debug.Log("It go to you");
                }
            }
     
            
            Debug.Log(arrowPosition);
            Debug.Log(playerPosition);
            Debug.Log(collisionPosition);
            Debug.DrawRay(contact.point, contact.normal, Color.white);
        }
        if (collision.relativeVelocity.magnitude > 2)
            Debug.Log("relativeVelocity.magnitude > 2");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
