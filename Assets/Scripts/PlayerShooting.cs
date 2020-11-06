using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    
    public GameObject arrowPrefab;
    
    public float minForce = 0f;
    public float maxForce = 50f; 
    public float timeToGetToMaxForce = 2.5f;

    public float shootingStrength = 0f;

    public float timer = 0f;
    bool isPulling;
    bool disableShoot;
    GameObject arrowObject;

    // Start is called before the first frame update
    void Start()
    {
        disableShoot = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (arrowObject != null) //if an arrow is currently in the scene, store the isPulling variable
        {
            isPulling = arrowObject.GetComponent<GrapplingArrow>().isPulling;
        }
        if (arrowObject != null && isPulling && arrowPrefab.GetComponent<GrapplingArrow>() && (Input.GetMouseButton(0))) //if arrow isPulling and player left clicks and arrow is grappling arrow, stop pulling
        {
            //Debug.Log("Entered");
            disableShoot = true;
            arrowObject.GetComponent<GrapplingArrow>().stopPull = true;
            return;
            

        }
        else
        {
            
            if (!disableShoot && Input.GetMouseButton(0))
            {
                disableShoot = false;
                timer += Time.deltaTime;
                timer = Mathf.Clamp(timer, 0, timeToGetToMaxForce);
                shootingStrength = Mathf.Lerp(minForce, maxForce, timer / timeToGetToMaxForce);
            }
            else if (!disableShoot && Input.GetMouseButtonUp(0))
            {
                arrowObject = Instantiate(arrowPrefab) as GameObject;
                arrowObject.transform.position = transform.position + transform.forward;
                arrowObject.transform.forward = transform.forward;
                arrowObject.GetComponent<Rigidbody>().AddForce(transform.forward * shootingStrength, ForceMode.Impulse);

                timer = minForce;
                shootingStrength = minForce;
                
            }
            
        }
        
        

        //Debug.Log(shootingStrength);

    }
}
