using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public GameObject arrowPrefab;
    public float speed = 100f;
    private GameObject[] arrowTracker;

    // Start is called before the first frame update
    void Start()
    {
     
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject arrowObject = Instantiate(arrowPrefab) as GameObject;
            arrowObject.transform.position = transform.position + transform.forward;
            arrowObject.transform.forward = transform.forward;
            arrowObject.GetComponent<Rigidbody>().AddForce(transform.forward * speed);
            arrowTracker = GameObject.FindGameObjectsWithTag("arrow");
        }


        foreach (GameObject arrow in arrowTracker)
        {
            if(arrow.GetComponent<Rigidbody>().velocity.x > 0.1)
                arrow.transform.rotation = Quaternion.LookRotation(arrow.GetComponent<Rigidbody>().velocity, Vector3.up);
        }
    }
}
