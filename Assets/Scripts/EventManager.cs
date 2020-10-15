
using UnityEngine;
using System;

public class EventManager : MonoBehaviour
{
    public static EventManager current;

    private void Awake()
    {
        current = this;
    }

    public event Action onArrowTriggerEnter;
    public event Action onArrowTriggerExit;

    public void ArrowTriggerEnter()
    {
        print("arrow entered");
        if (onArrowTriggerEnter != null)
        {
            onArrowTriggerEnter();
        }
    }

    public void ArrowTriggerExit()
    {
        if(onArrowTriggerExit != null)
        {
            onArrowTriggerExit();
        }
    }
}
