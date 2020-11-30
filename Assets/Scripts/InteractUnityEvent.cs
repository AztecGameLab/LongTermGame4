using UnityEngine;
using UnityEngine.Events;

public class InteractUnityEvent : Interactable
{
    [SerializeField, Tooltip("This event is called when the player toggles this object on")] 
    private UnityEvent toggleOn = default;

    [SerializeField, Tooltip("The event is called when the player toggles this object off")]
    private UnityEvent toggleOff = default;

    [SerializeField, Tooltip("Does this object start toggled on or off?")]
    private bool toggled = false;
    
    protected override void OnInteract(Transform userTransform)
    {
        if (!toggled)
        {
            toggleOn.Invoke();
        }
        else
        {
            toggleOff.Invoke();
        }

        toggled = !toggled;
    }
}