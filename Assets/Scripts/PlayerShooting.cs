using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    
    public GameObject arrowPrefab;
    
    public static float minForce = 0f;
    public static float maxForce = 20f;
    public static float timer = 0f;
    public static float timeToGetToMaxForce = 2.5f;

    private float shootingStrength = minForce;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButton(0))
        {
            timer += Time.deltaTime;
            timer = Mathf.Clamp(timer, minForce, timeToGetToMaxForce);
            shootingStrength = maxForce * timer;
        }
        else if (!Input.GetMouseButtonDown(0) && shootingStrength > .1f)
        {
            GameObject arrowObject = Instantiate(arrowPrefab) as GameObject;
            arrowObject.transform.position = transform.position + transform.forward;
            arrowObject.transform.forward = transform.forward;
            arrowObject.GetComponent<Rigidbody>().AddForce(transform.forward * shootingStrength, ForceMode.Impulse);
            timer = minForce;
            shootingStrength = minForce;
        }

        //Debug.Log(shootingStrength);
       
    }
}
