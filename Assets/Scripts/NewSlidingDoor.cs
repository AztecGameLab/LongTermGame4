using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewSlidingDoor : MonoBehaviour
{
    public Vector3 closedPosition;
    public Vector3 openPosition;
    public bool AnimateOnStart;

    bool isOpen;
    float t;
    public float animationTime = 1;

    private void Start()
    {
        if (!AnimateOnStart)
            t = isOpen ? 1 : 0;
        else
            t = isOpen ? 0 : 1;

    }

    private void Update()
    {
        t += Time.deltaTime / animationTime * (isOpen ? 1 : -1);
        t = Mathf.Clamp01(t);
        transform.localPosition = Vector3.Lerp(closedPosition, openPosition, t);
    }

    public void SetDoor(bool open) => this.isOpen = open;

}
