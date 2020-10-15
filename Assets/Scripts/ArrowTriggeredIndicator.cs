using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class ArrowTriggeredIndicator : MonoBehaviour
{

    private void Start()
    {
        GetComponent<Renderer>().material.color = Color.red;

        EventManager.current.onArrowTriggerEnter += ChangeColor;
        EventManager.current.onArrowTriggerExit += Revert;
    }
    private void ChangeColor()
    {
        GetComponent<Renderer>().material.color = Color.green;
    }

    private void Revert()
    {
        GetComponent<Renderer>().material.color = Color.red;
    }

    private void OnDestroy()
    {
        EventManager.current.onArrowTriggerEnter -= ChangeColor;
        EventManager.current.onArrowTriggerExit -= Revert;
    }
}
