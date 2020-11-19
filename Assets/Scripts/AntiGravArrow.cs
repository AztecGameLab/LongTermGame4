using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiGravArrow : MonoBehaviour
{
    [SerializeField] private MultiSound gravSound = default;
    private MultiSoundInstance _gravSound;
    
    private void OnEnable()
    {
        _gravSound = gravSound.GenerateInstance();
    }

    private void OnCollisionEnter(Collision other)
    {
        Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
        if (!rb || rb.isKinematic || other.gameObject.tag == "Player") return;
        
        AntiGrav ag = other.gameObject.GetComponent<AntiGrav>();
        if (ag)
        {
            ag.Deactivate();
        }
        else
        {
            other.gameObject.AddComponent<AntiGrav>().Activate(_gravSound);
        }
    }
}
