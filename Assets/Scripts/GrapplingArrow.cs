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
    private bool destroyLine = false;
    private bool isDestroyed = false;
   
    // Start is called before the first frame update
    void Start()
    {
        arrowRB = GetComponent<Rigidbody>();
        stopPull = false;
        isPulling = false;
        LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>(); //Creates the LineRenderer component and sets defaults
        lineRenderer.material = new Material(Shader.Find("Sprites/Default")); //ADD YOUR MATERIALS HERE @ARTISTS
        lineRenderer.widthMultiplier = 0.2f;
        lineRenderer.positionCount = 2;
        
    }
    void Update()
    {
        //Debug.Log("is Pulling (arrow): " + isPulling);
        if (!isDestroyed && Mathf.Abs(player.transform.position.x - this.transform.position.x) > 1) //If arrow has been shot and rendere not destroyed, update line vertices
        {
            LineRenderer line = GetComponent<LineRenderer>();
            var points = new Vector3[2];
            points[0] = player.transform.position;
            points[1] = this.transform.position;
            line.SetPositions(points);
            if (destroyLine)
            {
                isDestroyed = true;
                Destroy(line);
            }
            
        }
        
    }
    void OnCollisionEnter(Collision collision)
    {
        //if the collided object DOES have a rigid body, then the grapple will work on it
        
        if (collision.rigidbody == null || !collision.rigidbody.GetComponent<IsGrappable>()) //If object is not grappable, destroy line renderer on arrow and return
        {
            destroyLine = true;
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
        LineRenderer line = GetComponent<LineRenderer>();
        var points = new Vector3[2];
        //If the object is above a certain mass, the object will pull the player. Else, the player pulls the object
        if (collision.rigidbody.mass < massThreshold)
        {
            
            while (!stopPull && Vector3.Distance(collision.transform.position, player.transform.position) > pullRadiusThreshold) //Test if we want to stop pulling, if not, continue with lerp
            {
                
                collision.transform.position = Vector3.Lerp(collision.transform.position, player.transform.position, moveSpeed * 2 * Time.deltaTime);
                points[0] = player.transform.position;
                points[1] = collision.transform.position;
                line.SetPositions(points); // update line vertices

                yield return null;
            }
            
            isPulling = false; //Set bool variables back to default
            stopPull = false;
            Destroy(line);
            isDestroyed = true; // destroy line renderer when arrow has no more use
            yield break;
        }
        //When player is being pulled, player movement must temporarily be disabled to function properly
        player.s_playerMovement.enabled = false;
   
        while (!stopPull && Vector3.Distance(player.transform.position, this.transform.position) > pullRadiusThreshold)//Test if we want to stop pulling, if not, continue with lerp
        {
            player.transform.position = Vector3.Lerp(player.transform.position, this.transform.position, moveSpeed * Time.deltaTime);
            points[0] = player.transform.position;
            points[1] = this.transform.position;
            line.SetPositions(points); // update line vertices
            yield return null;
        }
        player.s_playerMovement.enabled = true;
        isPulling = false; //Set bool variables back to default
        stopPull = false;
        Destroy(line);
        isDestroyed = true; // destroy line renderer when arrow has no more use
        //Debug.Log(isPulling);
    }


    // Update is called once per frame
    
}
