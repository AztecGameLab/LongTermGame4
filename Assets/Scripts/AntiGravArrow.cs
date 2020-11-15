using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiGravArrow : MonoBehaviour
{
    [SerializeField] private MultiSound gravSound;

    private void OnCollisionEnter(Collision other)
    {
        Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
        if (!rb || rb.isKinematic) return;
        
        AntiGrav ag = other.gameObject.GetComponent<AntiGrav>();
        if (ag)
        {
            ag.Deactivate();
        }
        else
        {
            other.gameObject.AddComponent<AntiGrav>().Activate(gravSound);
        }
        Destroy(gameObject);
    }
}
