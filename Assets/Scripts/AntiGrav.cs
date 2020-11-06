using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiGrav : MonoBehaviour
{
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    private void FixedUpdate()
    {
        rb.AddForce(new Vector3(0, 9.81f, 0), ForceMode.Acceleration);
    }

    private void OnDestroy()
    {
        rb.useGravity = true;
    }
}
