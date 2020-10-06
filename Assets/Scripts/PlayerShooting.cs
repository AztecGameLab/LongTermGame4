using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    
    public GameObject arrowPrefab;
    public float shootingStrength = 0f;
    public float maxShootingStrength = 3000f;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButton(0))
        {
            shootingStrength += 20f;
        }
        else if (!Input.GetMouseButtonDown(0) && shootingStrength > .1f)
        {
            GameObject arrowObject = Instantiate(arrowPrefab) as GameObject;
            arrowObject.transform.position = transform.position + transform.forward;
            arrowObject.transform.forward = transform.forward;
            arrowObject.GetComponent<Rigidbody>().AddForce(transform.forward * shootingStrength);
            shootingStrength = 0;
        }

        if (shootingStrength >= maxShootingStrength)
        {
            shootingStrength = maxShootingStrength;
        }
    }
}
