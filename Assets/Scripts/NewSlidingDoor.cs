using System;
using UnityEngine;

public class NewSlidingDoor : MonoBehaviour
{
    public EventHandler FinishMoving;
    
    public Vector3 closedPosition;
    public Vector3 openPosition;
    public bool AnimateOnStart;

    bool isOpen;
    float t;
    public float animationTime = 1;

    public bool IsMoving => isOpen ? t != 1 : t != 0;
    
    private void Start()
    {
        if (!AnimateOnStart)
            t = isOpen ? 1 : 0;
        else
            t = isOpen ? 0 : 1;
    }

    private void Update()
    {
        var wasMoving = IsMoving;
        
        t += Time.deltaTime / animationTime * (isOpen ? 1 : -1);
        t = Mathf.Clamp01(t);

        // Debug.Log(name + " " + t + " " + wasMoving + " " + IsMoving);
        
        if (wasMoving == true && IsMoving == false)
        {
            FinishMoving.Invoke(this, EventArgs.Empty);
        }
        
        transform.localPosition = Vector3.Lerp(closedPosition, openPosition, t);
    }

    public void SetDoor(bool open) => this.isOpen = open;

}
