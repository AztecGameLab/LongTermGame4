using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class arrowTest : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private bool _active;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (_active)
            transform.forward = _rigidbody.velocity.normalized;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.GetComponent<PlayerManager>() == null)
            _active = false;
    }
}
