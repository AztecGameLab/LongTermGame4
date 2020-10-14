using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingArrow : MonoBehaviour
{
    PlayerManager player = PlayerManager.instance;
    public float moveSpeed = 1;
    public int massThreshold = 4;
    Rigidbody arrowRB;
    // Start is called before the first frame update
    void Start()
    {
        arrowRB = GetComponent<Rigidbody>();
    }
    void OnCollisionEnter(Collision collision)
    {
        


        //Get the position of the collided object
        Vector3 collisionPosition = collision.transform.position;

        //if the collided object DOES have a rigid body, then the grapple will work on it
        if (collision.rigidbody == null)
        {
            return;
        }
        if (!collision.gameObject.CompareTag("arrow"))//To keep players from stacking arrows oddly
        {
            arrowRB.isKinematic = true;
        }
        this.transform.parent = collision.transform;
        StartCoroutine(MoveObject(collision));

        //Debug.Log(collision.rigidbody.mass);
        
       

        
    }

    IEnumerator MoveObject(Collision collision)
    {
        //If the object is above a certain mass, the object will pull the player. Else, the player pulls the object
        if (collision.rigidbody.mass < massThreshold)
        {
            while (Vector3.Distance(this.transform.position, player.transform.position) > 2)
            {
                collision.transform.position = Vector3.Lerp(collision.transform.position, player.transform.position, moveSpeed * Time.deltaTime);
                yield return null;
            }
            yield break;
        }
        //When player is being pulled, player movement must temporarily be disabled to function properly
        player.s_playerMovement.enabled = false;
   
        while (Vector3.Distance(player.transform.position, this.transform.position) > 2)
        {
            player.transform.position = Vector3.Lerp(player.transform.position, this.transform.position, moveSpeed * Time.deltaTime);
            yield return null;
        }
        player.s_playerMovement.enabled = true;
    }


    // Update is called once per frame
    void Update()
    {
        //Debug.Log(Vector3.Distance(this.transform.position, player.transform.position));
    }
}
