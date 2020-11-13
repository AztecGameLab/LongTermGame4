using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiGrav : MonoBehaviour
{
    private Rigidbody _rb;
    private AudioManager _audioManager;
    private Sound _gravitySound;
    
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _audioManager = AudioManager.Instance();
    }

    private void FixedUpdate()
    {
        _rb.AddForce(new Vector3(0, 9.81f, 0), ForceMode.Acceleration);
    }
    
    public void Activate(Sound gravitySound)
    {
        _gravitySound = gravitySound;
        _audioManager.PlaySound(_gravitySound);
        _rb.useGravity = false;
    }

    public void Deactivate()
    {
        _audioManager.StopSound(_gravitySound);
        _rb.useGravity = true;
        Destroy(this);
    }
}
