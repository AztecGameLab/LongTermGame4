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
            timer = Mathf.Clamp(timer, 0, timeToGetToMaxForce);
            shootingStrength = Mathf.Lerp(minForce, maxForce, timer / timeToGetToMaxForce);
        }
        else if (Input.GetMouseButtonUp(0))
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
