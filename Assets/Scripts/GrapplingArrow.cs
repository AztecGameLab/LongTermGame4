using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingArrow : MonoBehaviour
{
    public static GrapplingArrow currentArrow;
    PlayerManager player = PlayerManager.instance;
    public float moveSpeed = 1;
    public float massThreshold = 4;
    public float pullRadiusThreshold = 4;
    Rigidbody arrowRB;
    Rigidbody playerRigid;
    public bool isPulling;
    public bool stopPull = false;
    private bool destroyLine = false;
    private bool isDestroyed = false;
    public Material ArrowRopeMaterial;
    LineRenderer line;

    // Start is called before the first frame update
    void Start()
    {
        currentArrow = this;
        arrowRB = GetComponent<Rigidbody>();
        playerRigid = player.GetComponent<Rigidbody>();
        stopPull = false;
        isPulling = false;
        LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>(); //Creates the LineRenderer component and sets defaults
        lineRenderer.material = ArrowRopeMaterial == null ? new Material(Shader.Find("Sprites/Default")) : ArrowRopeMaterial; //ADD YOUR MATERIALS HERE @ARTISTS
        lineRenderer.textureMode = LineTextureMode.Tile;
        lineRenderer.widthMultiplier = 0.05f;
        lineRenderer.positionCount = 2;
        var points = new Vector3[2];
        points[0] = player.transform.position;
        points[1] = this.transform.position;
        lineRenderer.SetPositions(points);

    }
    void Update()
    {
        //Debug.Log("is Pulling (arrow): " + isPulling);
        
        if (!isDestroyed) //If arrow has been shot and rendere not destroyed, update line vertices
        {
            line = GetComponent<LineRenderer>();
            var points = new Vector3[2];
            points[0] = player.transform.position;
            points[1] = this.transform.position;
            line.SetPositions(points);
            if (destroyLine || Input.GetMouseButton(0))
            {
                isDestroyed = true;
                Destroy(line);
            }
            
        }
        
    }
    bool collided;
    void OnCollisionEnter(Collision collision)
    {
        if(collided)//so it can only collide once
            return;

        //if the collided object DOES have a rigid body, then the grapple will work on it

        if (collision.rigidbody == null || !collision.rigidbody.GetComponent<IsGrappable>()) //If object is not grappable, destroy line renderer on arrow and return
        {
            destroyLine = true;
            Destroy(line);
            Destroy(this);
            return;
            
        }
        else if (!collision.gameObject.CompareTag("arrow"))//To keep players from stacking arrows oddly and from grabbing other arrows
        {
            collided = true;
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

            collision.rigidbody.velocity = Vector3.zero;
            collision.rigidbody.useGravity = false;
            while (currentArrow == this && !stopPull && Vector3.Distance(collision.transform.position, player.transform.position) > pullRadiusThreshold) //Test if we want to stop pulling, if not, continue with lerp
            {
                
                collision.transform.position = Vector3.Lerp(collision.transform.position, player.transform.position, moveSpeed * 2 * Time.deltaTime);
                points[0] = player.transform.position;
                points[1] = transform.position;
                if(line)
                    line.SetPositions(points); // update line vertices

                yield return null;
            }
            
            isPulling = false; //Set bool variables back to default
            stopPull = false;
            collision.rigidbody.useGravity = true;
            Destroy(line);
            isDestroyed = true; // destroy line renderer when arrow has no more use
            yield break;
        }
        //When player is being pulled, player movement must temporarily be disabled to function properly
        playerRigid.useGravity = false;


        while (currentArrow == this && !stopPull && Vector3.Distance(player.transform.position, this.transform.position) > pullRadiusThreshold)//Test if we want to stop pulling, if not, continue with lerp
        {
            player.transform.position = Vector3.Lerp(player.transform.position, this.transform.position, moveSpeed * Time.deltaTime);
            points[0] = player.transform.position;
            points[1] = this.transform.position;
            if(line)
                line.SetPositions(points); // update line vertices
            yield return null;
        }
        playerRigid.useGravity = true;
        isPulling = false; //Set bool variables back to default
        stopPull = false;
        Destroy(line);
        isDestroyed = true; // destroy line renderer when arrow has no more use
        //Debug.Log(isPulling);

        currentArrow = null;
    }

    


    // Update is called once per frame
    
}
