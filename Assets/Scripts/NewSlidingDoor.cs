using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewSlidingDoor : MonoBehaviour
{
    public Vector3 closedPosition;
    public Vector3 openPosition;

    bool isOpen;
    float t;
    public float animationTime = 1;

    private void Start()
    {
        t = isOpen ? 1 : 0;
    }

    private void Update()
    {
        t += Time.deltaTime / animationTime * (isOpen ? 1 : -1);
        t = Mathf.Clamp01(t);
        transform.localPosition = Vector3.Lerp(closedPosition, openPosition, t);
    }

    public void SetDoor(bool open) => this.isOpen = open;

}
