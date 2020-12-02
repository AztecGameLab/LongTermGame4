using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCamera : MonoBehaviour
{
    public float angle;
    public float speed;


    void Update()
    {
        transform.eulerAngles = new Vector3(0, angle * Mathf.Sin(Time.time * speed), 0);
    }
}
