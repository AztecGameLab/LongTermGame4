using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorActivator : MonoBehaviour
{
    public bool StartOpen;
    public float animationTime;
    public float cameraShakeAmount;
    bool isOpen;
    NewSlidingDoor[] slidingDoors;

    private void Awake()
    {
        slidingDoors = GetComponentsInChildren<NewSlidingDoor>();
        isOpen = StartOpen;
        foreach (var door in slidingDoors)
        {
            door.SetDoor(isOpen);
            door.animationTime = animationTime;
        }
    }


    public void OpenDoor()
    {
        isOpen = true;
        SetDoor();
    }

    public void CloseDoor()
    {
        isOpen = false;
        SetDoor();
    }

    public void ToggleDoor()
    {
        isOpen = !isOpen;
        SetDoor();
    }

    void SetDoor()
    {
        if (cameraShakeAmount != 0)
        {
            CameraFX.instance.AddTrauma(cameraShakeAmount);
            CameraFX.instance.SetFrozen(true, animationTime);
        }
        foreach (var door in slidingDoors)
            door.SetDoor(isOpen);
    }

    private void OnValidate()
    {
        var tempDoors = GetComponentsInChildren<NewSlidingDoor>();

        foreach (var door in tempDoors)
            if (StartOpen)
                door.transform.localPosition = door.openPosition;
            else
                door.transform.localPosition = door.closedPosition;
    }
}
