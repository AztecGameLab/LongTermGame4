using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiGravArrow : MonoBehaviour
{
    private AudioManager audioManager;
    public Sound gravSound;
    private void Awake()
    {
        audioManager = AudioManager.Instance();
    }

    void Update()
    {

    }

    private void OnCollisionEnter(Collision other)
    {
        Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
        if (rb && !rb.isKinematic && other.gameObject.tag != "Player")
        {
            AntiGrav ag = other.gameObject.GetComponent<AntiGrav>();
            if (ag)
            {
                audioManager.StopSound(gravSound, other.gameObject);
                Destroy(ag);
            }
            else
            {
                audioManager.PlaySound(gravSound, other.gameObject);
                other.gameObject.AddComponent<AntiGrav>();
            }
        }
    }
}
