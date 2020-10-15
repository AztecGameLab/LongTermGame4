using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class ArrowTriggeredIndicator : MonoBehaviour
{

    private void Start()
    {
        GetComponent<Renderer>().material.color = Color.red;
    }
    public void ChangeColor()
    {
        GetComponent<Renderer>().material.color = Color.green;
    }

    public void Revert()
    {
        GetComponent<Renderer>().material.color = Color.red;
    }

  
}
