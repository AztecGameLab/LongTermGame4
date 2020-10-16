using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowRotation : MonoBehaviour
{
    private Rigidbody arrowRB;

    // Start is called before the first frame update
    void Start()
    {
        arrowRB = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (arrowRB.velocity.x > 1 || arrowRB.velocity.y > 1 || arrowRB.velocity.z > 1)
            arrowRB.gameObject.transform.rotation = Quaternion.LookRotation(arrowRB.velocity, Vector3.up);
    }

}
