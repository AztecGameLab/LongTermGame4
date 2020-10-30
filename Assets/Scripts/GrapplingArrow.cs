using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingArrow : MonoBehaviour
{
    PlayerManager player = PlayerManager.instance;
    public float moveSpeed = 1;
    public float massThreshold = 4;
    public float pullRadiusThreshold = 4;
    Rigidbody arrowRB;
    bool isPulling = false;
    bool stopPull = false;
    // Start is called before the first frame update
    void Start()
    {
        arrowRB = GetComponent<Rigidbody>();
    }
    void OnCollisionEnter(Collision collision)
    {
        //if the collided object DOES have a rigid body, then the grapple will work on it
        if (collision.rigidbody == null)
        {
            return;
        }
        if (!collision.gameObject.CompareTag("arrow"))//To keep players from stacking arrows oddly and from grabbing other arrows
        {
            arrowRB.isKinematic = true;
            this.transform.parent = collision.transform; 
            isPulling = true;
            StartCoroutine(MoveObject(collision));
        
        }
        

        //Debug.Log(collision.rigidbody.mass);
        
       

        
    }

    IEnumerator MoveObject(Collision collision)
    {
        //If the object is above a certain mass, the object will pull the player. Else, the player pulls the object
        if (collision.rigidbody.mass < massThreshold)
        {
            while (!stopPull && Vector3.Distance(collision.transform.position, player.transform.position) > pullRadiusThreshold) //Test if we want to stop pulling, if not, continue with lerp
            {
                if (Input.GetMouseButton(0)) //if player clicks left mouse again (mid pull) set stopPull to true. else, continue with lerp
                {
                    stopPull = true;
                }
                else
                {
                    collision.transform.position = Vector3.Lerp(collision.transform.position, player.transform.position, moveSpeed * Time.deltaTime);
                }
                yield return null;
            }
            yield break;
        }
        //When player is being pulled, player movement must temporarily be disabled to function properly
        player.s_playerMovement.enabled = false;
   
        while (!stopPull && Vector3.Distance(player.transform.position, collision.transform.position) > pullRadiusThreshold)//Test if we want to stop pulling, if not, continue with lerp
        {
            if (Input.GetMouseButton(0))//if player clicks left mouse again (mid pull) set stopPull to true. else, continue with lerp
            {
                stopPull = true;
            }
            else
            {
                player.transform.position = Vector3.Lerp(player.transform.position, collision.transform.position, moveSpeed * Time.deltaTime);
            }
            
            yield return null;
        }
        player.s_playerMovement.enabled = true;
        isPulling = false; //Set bool variables back to default
        stopPull = false;
    }


    // Update is called once per frame
    void Update()
    {
        //Debug.Log(Vector3.Distance(this.transform.position, player.transform.position));
    }
}
