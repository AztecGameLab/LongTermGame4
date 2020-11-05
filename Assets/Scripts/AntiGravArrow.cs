using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiGravArrow : MonoBehaviour
{
    void Start()
    {

    }

    void Update()
    {

    }

    private void OnCollisionEnter(Collision other)
    {
        Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
        if (rb && !rb.isKinematic)
        {
            AntiGrav ag = other.gameObject.GetComponent<AntiGrav>();
            if (ag)
            {
                Destroy(ag);
            }
            else
            {
                other.gameObject.AddComponent<AntiGrav>();
            }
        }
    }
}
