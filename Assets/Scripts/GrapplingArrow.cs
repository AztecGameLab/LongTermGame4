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
    public bool isPulling;
    public bool stopPull = false;
    // Start is called before the first frame update
    void Start()
    {
        arrowRB = GetComponent<Rigidbody>();
        stopPull = false;
        isPulling = false;
    }
    void Update()
    {
        Debug.Log("is Pulling (arrow): " + isPulling);
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
            Destroy(GetComponent<Rigidbody>());
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
                
                collision.transform.position = Vector3.Lerp(collision.transform.position, player.transform.position, moveSpeed * Time.deltaTime);
                
                yield return null;
            }
            player.s_playerMovement.enabled = true;
            isPulling = false; //Set bool variables back to default
            stopPull = false;
            yield break;
        }
        //When player is being pulled, player movement must temporarily be disabled to function properly
        player.s_playerMovement.enabled = false;
   
        while (!stopPull && Vector3.Distance(player.transform.position, collision.transform.position) > pullRadiusThreshold)//Test if we want to stop pulling, if not, continue with lerp
        {
            player.transform.position = Vector3.Lerp(player.transform.position, collision.transform.position, moveSpeed * Time.deltaTime);
            yield return null;
        }
        player.s_playerMovement.enabled = true;
        isPulling = false; //Set bool variables back to default
        stopPull = false;
        Debug.Log(isPulling);
    }


    // Update is called once per frame
    
}
