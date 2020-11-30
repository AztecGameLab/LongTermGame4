using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class EventTriggeredIndicator : MonoBehaviour
{
    private Renderer ren;
    private void Start()
    {
        ren = GetComponent<Renderer>();
        ren.material.color = Color.red;
    }
    public void ChangeColor()
    {
        ren.material.color = Color.green;
    }

    public void Revert()
    {
        ren.material.color = Color.red;
    }

}
    
